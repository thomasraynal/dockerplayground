using System;
using EventStore.ClientAPI;
using DockerPlayground.EventStore.Connection;

namespace DockerPlayground.EventStore.Infrastructure
{
    public class ExternalEventStore : IEventStore
    {
        public ExternalEventStore(Uri uri)
        {
            Connection = EventStoreConnection.Create(EventStoreConnectionSettings.Default, uri);
        }

        public IEventStoreConnection Connection { get; }
    }
}