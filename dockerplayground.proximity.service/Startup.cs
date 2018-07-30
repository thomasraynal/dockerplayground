using DockerPlayground.EventStore;
using DockerPlayground.Location.EventStore;
using DockerPlayground.Shared.Dao;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Proximity.Service
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EventStoreCache<Guid, PositionRecord, PositionRecordsDto>, PositionEventCache>();
            services.AddSingleton<IProximityService, ProximityService>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            var proximityService = app.ApplicationServices.GetService<IProximityService>();
            proximityService.Start();

            app.UseMvc();
        }
    }
}
