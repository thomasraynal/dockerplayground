using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public abstract class ServiceBase<TService>
    {
        protected readonly TService _client;

        public ServiceBase(String uri)
        {
            _client = RestService.For<TService>(uri);
        }
    }
}
