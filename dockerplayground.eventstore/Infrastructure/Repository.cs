using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using DockerPlayground.Shared.Dao;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DockerPlayground.EventStore.Infrastructure
{
    //https://github.com/AdaptiveConsulting/ReactiveTraderCloud/blob/ba2c0fc04275786d98a4491d8828ae643be56f01/src/server/Adaptive.ReactiveTrader.EventStore/Domain/Repository.cs
    public class EventStoreRepository : IEventStoreRepository
    {
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.None};
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<EventStoreRepository> _logger;

        private Type GetEventType(String name)
        {

            _logger.LogInformation("GetEventType");

            var events = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "DockerPlayground.Shared.dll"))
                .DefinedTypes
                .Where(type =>!type.IsAbstract && type.ImplementedInterfaces.Contains(typeof(IEvent)));

            return events.First(@event => (@event.GetCustomAttributes(typeof(EventAttribute), true).First() as EventAttribute).Name == name);

        }

        public EventStoreRepository(IEventStoreConnection eventStoreConnection, ILoggerFactory loggerFactory)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = loggerFactory.CreateLogger<EventStoreRepository>();
        }

        public async Task<TAggregate> GetById<TAggregate>(object id) where TAggregate : IAggregate, new() 
        {

            var aggregate = new TAggregate();

            var streamName = $"{id}";
       
            var eventNumber = 0l;
            StreamEventsSlice currentSlice;
            do
            {
                currentSlice = await _eventStoreConnection.ReadStreamEventsForwardAsync(streamName, eventNumber, ReadPageSize, false);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                {
                    return default(TAggregate);
                }

                if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                {
                    return default(TAggregate);
                }

                eventNumber = currentSlice.NextEventNumber;

                foreach (var resolvedEvent in currentSlice.Events)
                {
                    var payload = DeserializeEvent(resolvedEvent.Event);
                    aggregate.ApplyEvent(payload);
                }
            } while (!currentSlice.IsEndOfStream);

            return aggregate;
        }

        public async Task<long> SaveAsync(AggregateBase aggregate, params KeyValuePair<string, string>[] extraHeaders)
        {
            var streamName = aggregate.Identifier.ToString();
            var pendingEvents = aggregate.GetPendingEvents();
            var originalVersion = aggregate.Version - pendingEvents.Count;

            try
            {
                WriteResult result;

                var commitHeaders = CreateCommitHeaders(aggregate, extraHeaders);
                var eventsToSave = pendingEvents.Select(x => ToEventData(Guid.NewGuid(), x, commitHeaders));

                var eventBatches = GetEventBatches(eventsToSave);

                if (eventBatches.Count == 1)
                {
                    result = await _eventStoreConnection.AppendToStreamAsync(streamName, originalVersion, eventBatches[0]);
                }
                else
                {
                    // If we have more events to save than can be done in one batch according to the WritePageSize, then we need to save them in a transaction to ensure atomicity
                    using (var transaction = await _eventStoreConnection.StartTransactionAsync(streamName, originalVersion))
                    {
                        foreach (var batch in eventBatches)
                        {
                            await transaction.WriteAsync(batch);
                        }

                        result = await transaction.CommitAsync();
                    }
                }

                aggregate.ClearPendingEvents();

                return result.NextExpectedVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to write events for stream: {streamName}.", ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
            }

            return originalVersion + 1;
        }

        private IEvent DeserializeEvent(RecordedEvent evt)
        {
            var targetType = GetEventType(evt.EventType);
            var json = Encoding.UTF8.GetString(evt.Data);
            return JsonConvert.DeserializeObject(json, targetType) as IEvent;
        }
        
        private IList<IList<EventData>> GetEventBatches(IEnumerable<EventData> events)
        {
            return events.Batch(WritePageSize).Select(x => (IList<EventData>) x.ToList()).ToList();
        }

        private static IDictionary<string, string> CreateCommitHeaders(AggregateBase aggregate, KeyValuePair<string, string>[] extraHeaders)
        {
            var commitId = Guid.NewGuid();

            var commitHeaders = new Dictionary<string, string>
            {
                {MetadataKeys.CommitIdHeader, commitId.ToString()},
                {MetadataKeys.AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName},
                {MetadataKeys.UserIdentityHeader, Thread.CurrentThread.Name}, // TODO - was Thread.CurrentPrincipal?.Identity?.Name
                {MetadataKeys.ServerNameHeader, "DefaultServerNameHEader"}, // TODO - was Environment.MachineName
                {MetadataKeys.ServerClockHeader, DateTime.UtcNow.ToString("o")}
            };

            foreach (var extraHeader in extraHeaders)
            {
                commitHeaders[extraHeader.Key] = extraHeader.Value;
            }

            return commitHeaders;
        }

        private static EventData ToEventData<TEvent>(Guid eventId, TEvent evnt, IDictionary<string, string> headers)
            where TEvent: IEvent
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));

            var eventHeaders = new Dictionary<string, string>(headers)
            {
                {MetadataKeys.EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
            };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = evnt.Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }
    }
}