using System;

namespace DockerPlayground.Shared.Dao
{
    public class LocationRecord
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Guid MemberId { get; set; }
        
        public override string ToString()
        {
            return $"{Latitude}/{Longitude}";
        }

        public override int GetHashCode()
        {
            return MemberId.GetHashCode() ^ Longitude.GetHashCode() ^ Latitude.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LocationRecord && GetHashCode() == obj.GetHashCode();
        }
    }
}
