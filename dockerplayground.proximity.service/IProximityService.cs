using System;
using System.Collections.Generic;

namespace DockerPlayground.Proximity.Service
{
    public interface IProximityService
    {
        IDictionary<Guid, List<ProximityEvent>> Proximities { get; }
        IEnumerable<ProximityEvent> GetState();
        IEnumerable<ProximityEvent> GetStateForMember(Guid memberId);
        void Start();
    }
}