using DockerPlayground.EventStore.Connection;
using DockerPlayground.EventStore.Infrastructure;
using DockerPlayground.Shared.Dao;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.EventStore
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseEventStore(this IWebHostBuilder hostBuilder)
        {
            var configuration = new EventStoreConfiguration(ServiceConstants.EventStoreUrl);
            var eventStoreConnection = new ExternalEventStore(EventStoreUri.FromConfig(configuration)).Connection;
            eventStoreConnection.ConnectAsync().Wait();

            var monitor = new ConnectionStatusMonitor(eventStoreConnection);
            var eventStoreStream = monitor.GetEventStoreConnectedStream(eventStoreConnection);

            hostBuilder.ConfigureServices((services) =>
            {
                services.Add(new ServiceDescriptor(typeof(IEventStoreConnection), eventStoreConnection));
                services.Add(new ServiceDescriptor(typeof(IObservable<IConnected<IEventStoreConnection>>), eventStoreStream));
            });

            return hostBuilder;

        }
    }
}
