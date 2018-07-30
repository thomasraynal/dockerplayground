using DockerPlayground.EventStore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class PositionRecord : AggregateBase
    {
        public Guid MemberId { get; private set; }
        public float Latitude { get;  set; }
        public float Longitude { get;  set; }
        private bool IsCreated { get; set; }

        public override object Identifier => MemberId;

        public PositionRecord() { }

        public PositionRecord(Guid memberId, float initialLatitude, float initialLongitude)
        {
            if (IsCreated)
            {
                return;
            }

            RaiseEvent(new PositionRecordCreatedEvent(memberId, initialLatitude, initialLongitude));
        }

        public void Update(PositionRecordUpdatedEvent @event)
        {
            RaiseEvent(@event);
        }

        public void Apply(PositionRecordCreatedEvent @event)
        {
            MemberId = @event.MemberId;
            Latitude = @event.Latitude;
            Longitude = @event.Longitude;
            IsCreated = true;
        }

        public void Apply(PositionRecordUpdatedEvent @event)
        {
            Latitude = @event.Latitude;
            Longitude = @event.Longitude;
        }
    }
}
