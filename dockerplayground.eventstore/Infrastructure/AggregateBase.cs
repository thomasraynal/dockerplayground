using DockerPlayground.Shared.Dao;
using System.Collections.Generic;

namespace DockerPlayground.EventStore.Infrastructure
{
    public abstract class AggregateBase : IAggregate
    {
        private readonly List<IEvent> _pendingEvents = new List<IEvent>();

        public abstract object Identifier { get; }
        public int Version { get; private set; } = -1;

        void IAggregate.ApplyEvent(IEvent @event)
        {
            ((dynamic) this).Apply((dynamic) @event);
            Version++;
        }

        public ICollection<IEvent> GetPendingEvents()
        {
            return _pendingEvents;
        }

        public void ClearPendingEvents()
        {
            _pendingEvents.Clear();
        }

        protected void RaiseEvent(IEvent @event)
        {
            ((IAggregate) this).ApplyEvent(@event);
            _pendingEvents.Add(@event);
        }
    }
}