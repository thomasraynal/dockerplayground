using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    [Event(Name = ServiceConstants.PositionRecordCreatedEventType)]
    public class PositionRecordCreatedEvent : EventBase
    {
        public PositionRecordCreatedEvent(Guid memberId, float latitude, float longitude)
        {
            MemberId = memberId;
            Longitude = longitude;
            Latitude = latitude;
        }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Guid MemberId { get; }

    }
}
