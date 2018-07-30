using DockerPlayground.EventStore;
using DockerPlayground.Shared.Dao;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerPlayground.Proximity.Service
{
    public class ProximityService : IProximityService
    {
        private readonly EventStoreCache<Guid, PositionRecord, PositionRecordsDto> _cache;
        private readonly ILogger<ProximityService> _logger;
        private readonly List<ProximityEvent> _proximities;

        private object _gateKeeper = new object();
        private const double proximityThreshold = 10.0;

        public IDictionary<Guid, List<ProximityEvent>> Proximities
        {
            get
            {
                return null;
            }
        }

        public void Start()
        {
            _cache.GetOutputStream().Subscribe(obs =>
            {
                if (obs.PositionRecords.Count > 0)
                {
                    foreach (var record in obs.PositionRecords)
                    {
                        var memberOne = record.MemberId;
                        var memberOnePosition = _cache.CacheState.State[memberOne];

                        lock (_gateKeeper)
                        {

                            _proximities.RemoveAll((proximity) => proximity.HasMember(memberOne));

                            foreach (var member in _cache.CacheState.State)
                            {
                                if (member.Key == record.MemberId) continue;

                                var memberTwo = member.Key;
                                var memberTwoPosition = member.Value;

                                var proximity = GpsUtilities.DistanceBetweenPoints(memberOnePosition, memberTwoPosition);

                                if (proximity < proximityThreshold)
                                {
                                    _proximities.Add(new ProximityEvent(memberOne, memberTwo, proximity));
                                    _logger.LogInformation($"Proximity - [{memberOne}] [{memberTwo}] | {proximity}");
                                }
                            }

                        }
                    }
                }
            });

        }

        public IEnumerable<ProximityEvent> GetState()
        {
            return _proximities;
        }

        public IEnumerable<ProximityEvent> GetStateForMember(Guid memberId)
        {
            return _proximities.Where(proximity => proximity.HasMember(memberId));
        }

        public ProximityService(EventStoreCache<Guid, PositionRecord, PositionRecordsDto> cache, ILoggerFactory loggerFactory)
        {
            _cache = cache;
            _logger = loggerFactory.CreateLogger<ProximityService>();
            _proximities = new List<ProximityEvent>();
        }


    }
}
