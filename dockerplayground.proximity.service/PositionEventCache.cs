using DockerPlayground.EventStore;
using DockerPlayground.EventStore.Infrastructure;
using DockerPlayground.Location.EventStore;
using DockerPlayground.Shared.Dao;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerPlayground.Proximity.Service
{
    public class PositionEventCache : EventStoreCache<Guid, PositionRecord, PositionRecordsDto>
    {

        private static readonly ISet<string> PositionEventCacheEventTypes = new HashSet<string>
        {
            ServiceConstants.PositionRecordCreatedEventType,
            ServiceConstants.PositionRecordUpdatedEventType
        };
        private readonly ILogger<PositionEventCache> _logger;

        public PositionEventCache(IObservable<IConnected<IEventStoreConnection>> eventStoreConnectionStream, ILoggerFactory loggerFactory) : base(eventStoreConnectionStream, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PositionEventCache>();
        }

        protected override bool IsMatchingEventType(string eventType)
        {
            return PositionEventCacheEventTypes.Contains(eventType);
        }

        protected override PositionRecordsDto CreateResponseFromCacheState(CacheState<Guid, PositionRecord> container)
        {
            var enabledTestEvents = container.State.Values;

            return new PositionRecordsDto(enabledTestEvents.Select(positionRecord => 
               new PositionRecordDto(positionRecord.Latitude, positionRecord.Longitude, positionRecord.MemberId)).ToList(),
                true,
                container.IsStale);
        }

        protected override void UpdateCacheState(IDictionary<Guid, PositionRecord> currentSotw, RecordedEvent evt)
        {
            switch (evt.EventType)
            {
                case ServiceConstants.PositionRecordCreatedEventType:
                    var createdEvent = evt.GetEvent<PositionRecordCreatedEvent>();
                    currentSotw.Add(createdEvent.MemberId, new PositionRecord(createdEvent.MemberId, createdEvent.Latitude, createdEvent.Longitude));
                    break;
                case ServiceConstants.PositionRecordUpdatedEventType:
                    var updatedEvent = evt.GetEvent<PositionRecordUpdatedEvent>();
                    var current = currentSotw[updatedEvent.MemberId];
                    current.Latitude = updatedEvent.Latitude;
                    current.Longitude = updatedEvent.Longitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported event type");
            }
        }

        protected override PositionRecordsDto MapSingleEventToUpdateDto(IDictionary<Guid, PositionRecord> currentSotw, RecordedEvent evt)
        {
            switch (evt.EventType)
            {
                case ServiceConstants.PositionRecordCreatedEventType:
                    return CreateSingleEventUpdateDto(currentSotw, evt.GetEvent<PositionRecordCreatedEvent>().MemberId, UpdateTypeDto.Added);
 
                case ServiceConstants.PositionRecordUpdatedEventType:
                    return CreateSingleEventUpdateDto(currentSotw, evt.GetEvent<PositionRecordUpdatedEvent>().MemberId, UpdateTypeDto.Updated);
                default:
                    throw new ArgumentOutOfRangeException("Unsupported event type");
            }
        }

        protected override PositionRecordsDto GetDisconnectedStaleUpdate()
        {
            return new PositionRecordsDto(new List<PositionRecordDto>(), false, true);
        }

        protected override bool IsValidUpdate(PositionRecordsDto update)
        {
            return update != PositionRecordsDto.Empty;
        }

        private static PositionRecordsDto CreateSingleEventUpdateDto(IDictionary<Guid, PositionRecord> currentSow,
                                                                         Guid member,
                                                                         UpdateTypeDto updateType)
        {
            var memberPosition = currentSow[member];
            return new PositionRecordsDto(new[]
            {
                new PositionRecordDto(memberPosition.Latitude,memberPosition.Longitude,memberPosition.MemberId)
            },
                false,
                false);
        }
    }
}
