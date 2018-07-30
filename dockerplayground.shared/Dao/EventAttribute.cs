using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class EventAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
