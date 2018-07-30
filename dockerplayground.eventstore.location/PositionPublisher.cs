using DockerPlayground.EventStore.Infrastructure;
using DockerPlayground.Shared.Dao;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;


namespace DockerPlayground.Location.EventStore
{
    public class PositionPublisher : IPositionPublisher, IDisposable
    {
        public PositionPublisher(IMemberClient memberService, IEventStoreRepository repository, IObservable<IConnected<IEventStoreConnection>> eventStoreStream, IEventStoreConnection eventStore, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory.CreateLogger<PositionPublisher>();

            _members = memberService.GetAllMembers().Result.ToList(); 

            _loggerFactory.LogInformation($"Found {_members.Count()} member(s)");

            _eventStoreStream = eventStoreStream;
            _eventStore = eventStore;
            _repository = repository;
        }

        private readonly ILogger _loggerFactory;
        private readonly List<Member> _members;
        private readonly IEventStoreConnection _eventStore;
        private readonly IObservable<IConnected<IEventStoreConnection>> _eventStoreStream;
        private CompositeDisposable _dispose;

        private readonly IEventStoreRepository _repository;

        private async Task<PositionRecord> CreateOrUpdatePositionRecord()
        {
   
            var location = GpsUtilities.GetLocation();
            var memberId = _members.Random().Id;
            var record = await _repository.GetById<PositionRecord>(memberId);

            if (null== record)
            {
                record = new PositionRecord(memberId, location.Latitude, location.Longitude);
                _loggerFactory.LogInformation($"New Position Record [{memberId}] [{location.Longitude}/{location.Latitude}]");
            }
            else
            {
                record.Update(new PositionRecordUpdatedEvent(record.MemberId, location.Latitude, location.Longitude));
                _loggerFactory.LogInformation($"Update Position Record [{memberId}] [{location.Longitude}/{location.Latitude}]");
            }

            return record;
        }

        public void StartStream()
        {
            var rand = new Random();

            _dispose = new CompositeDisposable();

            var locationGenerator = Observable
              .Interval(TimeSpan.FromMilliseconds(1000))
              .Subscribe(async _ =>
              {
                  var record = await CreateOrUpdatePositionRecord();
                  await _repository.SaveAsync(record);
              });

            _dispose.Add(locationGenerator);

        }

        public void EndStream()
        {
            Dispose();
        }

        public void Dispose()
        {
            _dispose.Dispose();
        }
    }
}
