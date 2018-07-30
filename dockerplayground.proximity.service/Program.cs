using DockerPlayground.EventStore;
using DockerPlayground.Shared.Dao;
using Microsoft.AspNetCore.Hosting;
using System;

namespace DockerPlayground.Proximity.Service
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
