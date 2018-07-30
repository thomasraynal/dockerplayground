using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public interface IConnected<out T>
    {
        T Value { get; }
        bool IsConnected { get; }
    }
}
