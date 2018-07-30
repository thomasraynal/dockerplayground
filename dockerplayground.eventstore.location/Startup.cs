using DockerPlayground.EventStore;
using DockerPlayground.EventStore.Infrastructure;
using DockerPlayground.Shared.Dao;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Location.EventStore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices (IServiceCollection services)
        {
            services.AddSingleton<IPositionPublisher, PositionPublisher>();
            services.AddSingleton<IMemberClient, MemberServiceClient>();
            services.AddSingleton<IEventStoreRepository, EventStoreRepository>();
 
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            var publisher = app.ApplicationServices.GetService<IPositionPublisher>();
            publisher.StartStream();

            app.UseMvc();
        }
    }
}
