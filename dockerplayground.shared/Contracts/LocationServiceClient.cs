using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DockerPlayground.Shared.Dao
{
    public class LocationServiceClient : ServiceBase<ILocationClient> , ILocationClient
    {
        public LocationServiceClient() : base(ServiceConstants.LocationServiceUrl)
        {
        }

        public async Task<LocationRecord> AddLocation(Guid memberId, [Body] LocationRecord location)
        {
            return await _client.AddLocation(memberId, location);
        }

        public async Task<IEnumerable<LocationRecord>> GetAllLocations()
        {
            return await _client.GetAllLocations();
        }

        public async Task<LocationRecord> GetLatestForMember(Guid memberId)
        {
            return await _client.GetLatestForMember(memberId);
        }

        public async Task<IEnumerable<LocationRecord>> GetLocationForMember(Guid memberId)
        {
            return await _client.GetLocationForMember(memberId);
        }
    }
}
