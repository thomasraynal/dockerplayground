using DockerPlayground.Shared.Dao;
using System.Collections.Generic;

namespace DockerPlayground.EventStore.Infrastructure
{
    public interface IAggregate
    {
        int Version { get; }
        object Identifier { get; }
        void ApplyEvent(IEvent @event);
        ICollection<IEvent> GetPendingEvents();
        void ClearPendingEvents();
    }
}