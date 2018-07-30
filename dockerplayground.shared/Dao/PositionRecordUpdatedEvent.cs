using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    [Event(Name = ServiceConstants.PositionRecordUpdatedEventType)]
    public class PositionRecordUpdatedEvent : EventBase
    {
        public PositionRecordUpdatedEvent(Guid memberId, float latitude, float longitude)
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
