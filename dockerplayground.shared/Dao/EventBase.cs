using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public abstract class EventBase : IEvent
    {
        public string Name => (this.GetType().GetCustomAttributes(typeof(EventAttribute), true).First() as EventAttribute).Name;
    }
}
