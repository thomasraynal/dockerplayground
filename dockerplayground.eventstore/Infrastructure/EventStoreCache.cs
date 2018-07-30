using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EventStore.ClientAPI;
using DockerPlayground.Shared.Dao;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DockerPlayground.EventStore
{
    public class CacheState<TKey, TCacheItem>
    {
        public CacheState()
        {
            State = new ConcurrentDictionary<TKey, TCacheItem>();
            IsStale = true;
        }

        public bool IsStale { get; set; }

        public ConcurrentDictionary<TKey, TCacheItem> State { get; }
    }

    //https://github.com/AdaptiveConsulting/ReactiveTraderCloud/blob/ba2c0fc04275786d98a4491d8828ae643be56f01/src/server/Adaptive.ReactiveTrader.EventStore/EventStoreCache.cs
    public abstract class EventStoreCache<TKey, TCacheItem, TOutput> : IDisposable
    {
        private readonly IConnectableObservable<IConnected<IEventStoreConnection>> _connectionChanged;
        private readonly IScheduler _eventLoopScheduler = new EventLoopScheduler();
        private readonly SerialDisposable _eventsConnection = new SerialDisposable();
        private readonly SerialDisposable _eventsSubscription = new SerialDisposable();

        private readonly CacheState<TKey, TCacheItem> _cacheState = new CacheState<TKey, TCacheItem>();

        private readonly BehaviorSubject<CacheState<TKey, TCacheItem>> _cacheStatedUpdates =
            new BehaviorSubject<CacheState<TKey, TCacheItem>>(new CacheState<TKey, TCacheItem>());

      
        private IConnectableObservable<RecordedEvent> _events = Observable.Never<RecordedEvent>().Publish();
        private bool _isCaughtUp;

        protected EventStoreCache(IObservable<IConnected<IEventStoreConnection>> eventStoreConnectionStream, ILoggerFactory loggerFactory)
        {

            _logger = loggerFactory.CreateLogger<EventStoreCache<TKey, TCacheItem, TOutput>>();

            Disposables = new CompositeDisposable(_eventsConnection, _eventsSubscription);

            _connectionChanged = eventStoreConnectionStream.ObserveOn(_eventLoopScheduler)
                                                           .Publish();

            Disposables.Add(_connectionChanged.Connect());

            Disposables.Add(_connectionChanged.Subscribe(x =>
            {
                if (x.IsConnected)
                {

                    _logger.LogInformation("Connected to Event Store");
                    
                    Initialize(x.Value);
                }
                else
                {
                    _logger.LogInformation("Disconnected to Event Store");

                    if (!_cacheState.IsStale)
                    {
                        _cacheState.IsStale = true;
                        _cacheStatedUpdates.OnNext(_cacheState);
                    }
                }
            }));
        }

        private readonly ILogger<EventStoreCache<TKey, TCacheItem, TOutput>> _logger;

        private CompositeDisposable Disposables { get; }

        public virtual void Dispose()
        {
            Disposables.Dispose();
        }

        protected abstract bool IsMatchingEventType(string eventType);
        protected abstract void UpdateCacheState(IDictionary<TKey, TCacheItem> currentCacheState, RecordedEvent evt);
        protected abstract bool IsValidUpdate(TOutput update);
        protected abstract TOutput CreateResponseFromCacheState(CacheState<TKey, TCacheItem> container);
        protected abstract TOutput MapSingleEventToUpdateDto(IDictionary<TKey, TCacheItem> currentCacheState, RecordedEvent evt);
        protected abstract TOutput GetDisconnectedStaleUpdate();

        public CacheState<TKey, TCacheItem> CacheState
        {
            get
            {
                return _cacheState;
            }
        }

        public IObservable<TOutput> GetOutputStream()
        {
            return GetOutputStreamImpl().SubscribeOn(_eventLoopScheduler)
                                        .TakeUntil(_connectionChanged.Where(x => x.IsConnected))
                                        .Repeat();
        }

        private void Initialize(IEventStoreConnection connection)
        {

            _logger.LogInformation("Initializing Cache");

            _cacheState.IsStale = true;
            _cacheState.State.Clear();
            _isCaughtUp = false;

            _events = GetAllEvents(connection).Where(x => IsMatchingEventType(x.EventType))
                                              .SubscribeOn(_eventLoopScheduler)
                                              .Publish();

            _eventsSubscription.Disposable = _events.Subscribe(evt =>
            {
                UpdateCacheState(_cacheState.State, evt);

                if (_isCaughtUp)
                {
                    _cacheStatedUpdates.OnNext(_cacheState);
                }
            });
            _eventsConnection.Disposable = _events.Connect();
        }

        private IObservable<TOutput> GetOutputStreamImpl()
        {
            return Observable.Create<TOutput>(obs =>
            {
 
                _logger.LogInformation("Got stream request from client");

                var sotw = _cacheStatedUpdates.TakeUntilInclusive(x => !x.IsStale)
                                                  .Select(CreateResponseFromCacheState);

                return sotw.Concat(_events.Select(evt => MapSingleEventToUpdateDto(_cacheState.State, evt)))
                           .Merge(_connectionChanged.Where(x => !x.IsConnected).Select(_ => GetDisconnectedStaleUpdate()))
                           .Where(IsValidUpdate)
                           .Subscribe(obs);
            });
        }

        private IObservable<RecordedEvent> GetAllEvents(IEventStoreConnection connection)
        {
            return Observable.Create<RecordedEvent>(o =>
            {
    
                _logger.LogInformation("Getting events from Event Store");

                Action<EventStoreCatchUpSubscription, ResolvedEvent> onEvent =
                    (_, e) =>
                    {
                        _eventLoopScheduler.Schedule(() =>
                        {
                            o.OnNext(e.Event);
                        });
                    };

                Action<EventStoreCatchUpSubscription> onCaughtUp = evt =>
                {
                    _eventLoopScheduler.Schedule(() =>
                    {
     
                        _logger.LogInformation("Caught up to live events. Publishing State");

                        _isCaughtUp = true;
                        _cacheState.IsStale = false;
                        _cacheStatedUpdates.OnNext(_cacheState);
                    });
                };

                var subscription = connection.SubscribeToAllFrom(null, CatchUpSubscriptionSettings.Default, onEvent, onCaughtUp);

                _logger.LogInformation($"Subscribed to Event Store.");

                return Disposable.Create(() =>
                {
                    _logger.LogInformation($"Stopping Event Store subscription");

                    subscription.Stop();
                });
            });
        }
    }
}