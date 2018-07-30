using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EventStore.ClientAPI;

namespace DockerPlayground.EventStore.Connection
{
    public interface IConnectionStatusMonitor : IDisposable
    {
        IObservable<ConnectionInfo> ConnectionInfoChanged { get; }
    }
    //https://github.com/AdaptiveConsulting/ReactiveTraderCloud/blob/ba2c0fc04275786d98a4491d8828ae643be56f01/src/server/Adaptive.ReactiveTrader.EventStore/Connection/ConnectionStatusMonitor.cs
    public class ConnectionStatusMonitor : IConnectionStatusMonitor
    {
        private readonly IConnectableObservable<ConnectionInfo> _connectionInfoChanged;
        private readonly IDisposable _connection;

        public ConnectionStatusMonitor(IEventStoreConnection connection)
        {
            var connectedChanged = Observable.FromEventPattern<ClientConnectionEventArgs>(h => connection.Connected += h,
                                                                                          h => connection.Connected -= h)
                                             .Select(_ => ConnectionStatus.Connected);

            var disconnectedChanged = Observable.FromEventPattern<ClientConnectionEventArgs>(h => connection.Disconnected += h,
                                                                                             h => connection.Disconnected -= h)
                                                .Select(_ => ConnectionStatus.Disconnected);

            var reconnectingChanged = Observable.FromEventPattern<ClientReconnectingEventArgs>(h => connection.Reconnecting += h,
                                                                                               h => connection.Reconnecting -= h)
                                                .Select(_ => ConnectionStatus.Connecting);

            _connectionInfoChanged = Observable.Merge(connectedChanged, disconnectedChanged, reconnectingChanged)
                                               .Scan(ConnectionInfo.Initial, UpdateConnectionInfo)
                                               .StartWith(ConnectionInfo.Initial)
                                               .Replay(1);

            _connection = _connectionInfoChanged.Connect();
        }

        public IObservable<ConnectionInfo> ConnectionInfoChanged => _connectionInfoChanged;

        public void Dispose()
        {
            _connection.Dispose();
        }

        private static ConnectionInfo UpdateConnectionInfo(ConnectionInfo previousConnectionInfo, ConnectionStatus connectionStatus)
        {
            ConnectionInfo newConnectionInfo;

            if ((previousConnectionInfo.Status == ConnectionStatus.Disconnected ||
                 previousConnectionInfo.Status == ConnectionStatus.Connecting) &&
                connectionStatus == ConnectionStatus.Connected)
            {
                newConnectionInfo = new ConnectionInfo(connectionStatus, previousConnectionInfo.ConnectCount + 1);
            }
            else
            {
                newConnectionInfo = new ConnectionInfo(connectionStatus, previousConnectionInfo.ConnectCount);
            }

            return newConnectionInfo;
        }
    }
}