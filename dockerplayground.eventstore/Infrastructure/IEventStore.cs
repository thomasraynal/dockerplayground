using EventStore.ClientAPI;

namespace DockerPlayground.EventStore.Infrastructure
{
    public interface IEventStore
    {
        IEventStoreConnection Connection { get; }
    }
}