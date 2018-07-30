using System;

namespace DockerPlayground.EventStore.Infrastructure
{
    public static class EventStoreUri
    {

        public static Uri FromConfig(IEventStoreConfiguration config)
        {
            var builder = new UriBuilder
            {
                Scheme = "tcp",
                UserName = "admin",
                Password = "changeit",
                Host = config.Host,
                Port = config.Port
            };

            return builder.Uri;
        }
    }
}