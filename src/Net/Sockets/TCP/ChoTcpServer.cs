using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using Cinchoo.Core.Runtime.Serialization;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoTcpServer
    {
        private int _port;
        private Socket _listener;
        private ChoTcpServiceProvider _provider;
        private List<ChoConnectionState> _connections;
        private int _maxConnections = 100; // TO BE Parameterized

        private AsyncCallback ConnectionReady;
        private WaitCallback AcceptConnection;
        private AsyncCallback ReceivedDataReady;

        public readonly IChoObjectSerializer Serializer = new ChoBinaryObjectSerializer();

        private readonly object _padLock = new object();
        public Action<ChoConnectionState, object> OnObjectRecieved;

        /// <SUMMARY>
        /// Initializes server. To start accepting
        /// connections call Start method.
        /// </SUMMARY>
        public ChoTcpServer(int port, ChoTcpServiceProvider provider = null, IChoObjectSerializer serializer = null)
        {
            _provider = provider == null ? ChoTcpServiceProviderImpl.Instance : provider;

            _port = port;
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _connections = new List<ChoConnectionState>();
            ConnectionReady = new AsyncCallback(ConnectionReady_Handler);
            AcceptConnection = new WaitCallback(AcceptConnection_Handler);
            ReceivedDataReady = new AsyncCallback(ReceivedDataReady_Handler);
            if (serializer != null)
                Serializer = serializer;
        }


        /// <SUMMARY>
        /// Start accepting connections.
        /// A false return value tell you that the port is not available.
        /// </SUMMARY>
        public bool Start()
        {
            try
            {
                _listener.Bind(new IPEndPoint(IPAddress.Any, _port));
                _listener.Listen(100);
                _listener.BeginAccept(ConnectionReady, null);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <SUMMARY>
        /// Callback function: A new connection is waiting.
        /// </SUMMARY>
        private void ConnectionReady_Handler(IAsyncResult ar)
        {
            lock (_padLock)
            {
                if (_listener == null) return;
                Socket conn = _listener.EndAccept(ar);
                if (_connections.Count >= _maxConnections)
                {
                    //Max number of connections reached.
                    string msg = "SE001: Server busy";
                    conn.Send(Encoding.UTF8.GetBytes(msg), 0,
                              msg.Length, SocketFlags.None);
                    conn.Shutdown(SocketShutdown.Both);
                    conn.Close();
                }
                else
                {
                    //Start servicing a new connection
                    ChoConnectionState st = new ChoConnectionState();
                    st.Connection = conn;
                    st.Server = this;
                    st.Serializer = Serializer;
                    st.Provider = (ChoTcpServiceProvider)_provider.Clone();
                    st.Buffer = new byte[4];
                    _connections.Add(st);
                    //Queue the rest of the job to be executed latter
                    ThreadPool.QueueUserWorkItem(AcceptConnection, st);
                }
                //Resume the listening callback loop
                _listener.BeginAccept(ConnectionReady, null);
            }
        }


        /// <SUMMARY>
        /// Executes OnAcceptConnection method from the service provider.
        /// </SUMMARY>
        private void AcceptConnection_Handler(object state)
        {
            ChoConnectionState st = state as ChoConnectionState;
            try { st.Provider.OnAcceptConnection(st); }
            catch
            {
                //report error in provider... Probably to the EventLog
            }

            try
            {
                //Starts the ReceiveData callback loop
                if (st.Connection.Connected)
                    st.Connection.BeginReceive(st.Buffer, 0, 0, SocketFlags.None,
                      ReceivedDataReady, st);
            }
            catch
            {
                DropConnection(st);
            }
        }


        /// <SUMMARY>
        /// Executes OnReceiveData method from the service provider.
        /// </SUMMARY>
        private void ReceivedDataReady_Handler(IAsyncResult ar)
        {
            ChoConnectionState st = ar.AsyncState as ChoConnectionState;
            try
            {
                st.Connection.EndReceive(ar);
                //Im considering the following condition as a signal that the
                //remote host droped the connection.
                if (st.Connection.Available == 0) DropConnection(st);
                else
                {
                    try
                    {
                        st.Provider.OnReceiveData(st);
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
                        catch (Exception)
                        {
                            DropConnection(st);
                        }
                    }
                }
            }
            catch 
            {
                try
                {
                    DropConnection(st);
                }
                catch { }
            }
        }

        public void Broadcast(object packet)
        {
            try
            {
                byte[] buffer = Serializer.Serialize(packet) as byte[];
                Broadcast(buffer);
            }
            catch
            {
            }
        }

        public void Broadcast(byte[] buffer)
        {
            lock (_padLock)
            {
                List<ChoConnectionState> unreachClients = new List<ChoConnectionState>();
                foreach (ChoConnectionState st in _connections)
                {
                    if (!st.Connected)
                    {
                        unreachClients.Add(st);
                        continue;
                    }

                    try 
                    {
                        st.Write(buffer, 0, buffer.Length); 
                    }
                    catch
                    {
                        //some error in the provider
                        unreachClients.Add(st);
                    }
                }
                foreach (ChoConnectionState st in unreachClients)
                    DropConnection(st);
            }
        }

        /// <SUMMARY>
        /// Shutdown the server
        /// </SUMMARY>
        public void Stop()
        {
            lock (_padLock)
            {
                if (_listener != null)
                {
                    _listener.Close();
                    _listener = null;
                }
                //Close all active connections
                foreach (ChoConnectionState st in _connections)
                {
                    try { st.Provider.OnDropConnection(st); }
                    catch
                    {
                        //some error in the provider
                    }
                    st.Connection.Shutdown(SocketShutdown.Both);
                    st.Connection.Close();
                }
                _connections.Clear();
            }
        }


        /// <SUMMARY>
        /// Removes a connection from the list
        /// </SUMMARY>
        internal void DropConnection(ChoConnectionState st)
        {
            lock (_padLock)
            {
                if (st.Connection != null && st.Connection.Connected)
                {
                    st.Connection.Shutdown(SocketShutdown.Both);
                    st.Connection.Close();
                }
                if (_connections.Contains(st))
                    _connections.Remove(st);
            }
        }

        public int MaxConnections
        {
            get
            {
                return _maxConnections;
            }
            set
            {
                _maxConnections = value;
            }
        }


        public int CurrentConnections
        {
            get
            {
                lock (_padLock) { return _connections.Count; }
            }
        }
    }
}
