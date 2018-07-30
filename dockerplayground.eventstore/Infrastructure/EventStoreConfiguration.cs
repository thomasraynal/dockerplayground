using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.EventStore.Infrastructure
{
    public class EventStoreConfiguration : IEventStoreConfiguration
    {
        public EventStoreConfiguration(String url)
        {
            var uri = new Uri(url);
            Host = uri.Host;
            Port = uri.Port;
        }

        public EventStoreConfiguration(String host, int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get; }
        public int Port { get; }
    }
}
