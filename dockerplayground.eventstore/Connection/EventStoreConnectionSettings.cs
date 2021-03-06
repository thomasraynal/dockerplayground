using System;
using EventStore.ClientAPI;

namespace DockerPlayground.EventStore.Connection
{
    public static class EventStoreConnectionSettings
    {
        public static readonly ConnectionSettingsBuilder Default = ConnectionSettings.Create()
                                                                                     .SetReconnectionDelayTo(TimeSpan.FromSeconds(1))
                                                                                     .KeepReconnecting();
    }
}