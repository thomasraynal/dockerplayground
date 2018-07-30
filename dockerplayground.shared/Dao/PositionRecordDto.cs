using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class PositionRecordDto
    {
        public PositionRecordDto(float latitude, float longitude, Guid memberId)
        {
            Latitude = latitude;
            Longitude = longitude;
            MemberId = memberId;

            Id = Guid.NewGuid();
            Timestamp = DateTime.Now.Ticks;
        }

        public Guid Id { get; internal set; }
        public float Latitude { get; internal set; }
        public float Longitude { get; internal set; }
        public long Timestamp { get; internal set; }
        public Guid MemberId { get; internal set; }

        public override string ToString()
        {
            return $"[{MemberId}] -  {Latitude}/{Longitude}";
        }

    }
}
