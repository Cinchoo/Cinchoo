using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Runtime.Serialization;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoTcpClient
    {
        private readonly IPEndPoint _localEP;
        private readonly int _uniqueId = ChoRandom.NextRandom();
        private int _sleepInterval = 1000;
        private byte[] _buffer = new byte[1024];
        private ChoTcpServiceProvider _provider;
        private Socket _socket;
        private ChoConnectionState _connection;
        private Timer _reconnectTimer;
        private Timer _pollTimer;

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public readonly IChoObjectSerializer Serializer = new ChoBinaryObjectSerializer();
        public Action<ChoConnectionState, object> OnObjectRecieved;

        public int SleepInterval
        {
            get { return _sleepInterval; }
            set
            {
                if (value > 0)
                    _sleepInterval = value;
            }
        }

        public ChoTcpClient(string hostname, int port, ChoTcpServiceProvider provider = null, IChoObjectSerializer serializer = null)
            : this(new IPEndPoint(IPAddress.Parse(hostname), port), provider, serializer)
        {
        }

        public ChoTcpClient(IPEndPoint localEP, ChoTcpServiceProvider provider = null, IChoObjectSerializer serializer = null)
        {
            _localEP = localEP;
            _provider = provider == null ? ChoTcpServiceProviderImpl.Instance : provider;
            ReconnectInterval = 5000;
            _pollTimer = new Timer(OnSocketDropPoll, null, ReconnectInterval, ReconnectInterval);
            if (serializer != null)
                Serializer = serializer;
        }
        
        public void Start()
        {
            if (_socket != null)
            {
                ChoTrace.Warn("Already got a socket, disconnect first.");
                return;
            }
            _reconnectTimer = new Timer(OnTryToReconnect, null, Timeout.Infinite, Timeout.Infinite);

            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                ChoTrace.Debug("Failed to reconnect.", ex);
                HandleDisconnected();
            }
        }

        private void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_localEP);
            ChoConnectionState st = _connection = new ChoConnectionState();
            st.Connection = _socket;
            st.Client = this;
            st.Serializer = Serializer;
            st.Provider = (ChoTcpServiceProvider)_provider.Clone();
            st.Buffer = new byte[1024];
            if (_socket.Connected)
            {
                RaiseConnected();
                _socket.BeginReceive(_connection.Buffer, 0, 0, SocketFlags.None, ReceivedDataReady, _connection);
            }
        }

        public void Stop()
        {
            if (_reconnectTimer != null)
            {
                _reconnectTimer.Dispose();
                _reconnectTimer = null;
            }
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
                RaiseDisconnected();
            }
        }

        private void RaiseDisconnected()
        {
            EventHandler disconnected = Disconnected;
            if (disconnected != null)
                disconnected(this, null);
        }

        private void RaiseConnected()
        {
            EventHandler connected = Connected;
            if (connected != null)
                connected(this, null);
        }

        private void OnSocketDropPoll(object state)
        {
            _pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (_socket != null && !_socket.IsConnected())
                    HandleDisconnected();
            }
            catch
            {
            }
            _pollTimer.Change(ReconnectInterval, ReconnectInterval);
        }

        private void OnTryToReconnect(object state)
        {
            if (_reconnectTimer == null) return;

            try
            {
                _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                Connect();
                ChoTrace.Info("Reconnected successfully.");
            }
            catch (Exception ex)
            {
                ChoTrace.Debug("Failed to reconnect.", ex);
                //_reconnectTimer.Change(Timeout.Infinite, 5000);
                _reconnectTimer.Change(ReconnectInterval, Timeout.Infinite);
            }
        }

        private void HandleDisconnected()
        {
            //if (_socket != null && _socket.IsConnected())
            //    return;
            if (_reconnectTimer == null) return;

            ChoTrace.Info("Got disconnected, launching reconnect timer.");
            _reconnectTimer.Change(ReconnectInterval, Timeout.Infinite);
            if (_socket != null)
            {
                try
                {
                    _socket.Close();
                }
                catch (Exception err)
                {
                    ChoTrace.Error("Failed to close socket.", err);
                }
                _socket = null;
                RaiseDisconnected();
            }
        }

        public void Send(object packet)
        {
            if (!_connection.Connection.Connected)
                return;

            try
            {
                byte[] buffer = Serializer.Serialize(packet) as byte[];
                Send(buffer);
            }
            catch
            {
                //some error in the provider
                throw;
            }
        }

        public void Send(byte[] buffer)
        {
            if (!_connection.Connection.Connected)
            {
                return;
            }

            try
            {
                _connection.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                //some error in the provider
                throw;
            }
        }

        private void ReceivedDataReady(IAsyncResult ar)
        {
            ChoConnectionState st = ar.AsyncState as ChoConnectionState;
            try
            {
                st.Connection.EndReceive(ar);
                //Im considering the following condition as a signal that the
                //remote host droped the connection.
                if (st.Connection.Available == 0) return;
                else
                {
                    try 
                    { 
                        _provider.OnReceiveData(st);
                    }
                    catch (ChoFatalApplicationException)
                    {
                        throw;
                    }
                    catch
                    {
                        //report error in the provider
                    }
                    //Resume ReceivedData callback loop
                    if (st.Connection.Connected)
                    {
                        try
                        {
                            st.Connection.BeginReceive(st.Buffer, 0, 0, SocketFlags.None,
                              ReceivedDataReady, st);
                        }
                        catch (Exception innerEx)
                        {
                            ChoTrace.Error(innerEx);
                            HandleDisconnected();
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                HandleDisconnected();
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex);
                HandleDisconnected();
            }
        }

        /// <summary>
        /// Gets or sets number of millseconds between reconnect attempts.
        /// </summary>
        /// <value>Default is 5000</value>
        public int ReconnectInterval { get; set; }
    }
}
