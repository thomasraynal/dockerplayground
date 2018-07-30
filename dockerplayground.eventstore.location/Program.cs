using DockerPlayground.EventStore;
using DockerPlayground.EventStore.Connection;
using DockerPlayground.EventStore.Infrastructure;
using DockerPlayground.Shared.Dao;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace DockerPlayground.Location.EventStore
{
    class Program
    {

        static void Main(string[] args)
        {

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseEventStore()
                .Build();

            host.Run();
            
        }
    }
}
