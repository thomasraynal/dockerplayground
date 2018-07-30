using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DockerPlayground.Shared.Dao
{
    public interface ILocationClient
    {

        [Post("/locations/{memberId}")]
        Task<LocationRecord> AddLocation(Guid memberId, [Body] LocationRecord location);

        [Get("/locations/{memberId}")]
        Task<IEnumerable<LocationRecord>> GetLocationForMember(Guid memberId);

        [Get("/locations/{memberId}/latest")]
        Task<LocationRecord> GetLatestForMember(Guid memberId);

        [Get("/locations")]
        Task<IEnumerable<LocationRecord>> GetAllLocations();
    }
}
