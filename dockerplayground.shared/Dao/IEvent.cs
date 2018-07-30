using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{

    public interface IEvent
    {
        String Name { get; }
    }
}
