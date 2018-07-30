using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Proximity.Service
{
    public class ProximityEvent
    {
        public ProximityEvent(Guid memberOne, Guid memberTwo, double distance)
        {
            MemberOne = memberOne;
            MemberTwo = memberTwo;
            Distance = distance;
        }

        public Guid MemberOne { get; private set; }
        public Guid MemberTwo { get; private set; }
        public double Distance { get; private set; }

        public bool HasMember(Guid memberId)
        {
            return MemberOne == memberId || MemberTwo == memberId;
        }
    }
}
