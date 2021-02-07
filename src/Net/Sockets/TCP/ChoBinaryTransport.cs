using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Cinchoo.Core.Diagnostics;

namespace Cinchoo.Core.Net.Sockets
{
    /// <summary>
    /// Transports objects over a binary socket.
    /// </summary>
    /// <remarks>
    /// Sends stuff over the wire. Nothing is queued, all calls to Send are discarded if the 
    /// socket is not connected. The transport will try to reconnect every five second.
    /// </remarks>
    public class ChoBinaryTransport<T> where T : new()
    {
        private readonly BinaryFormatter _formatter = new BinaryFormatter();
        private readonly byte[] _inbuffer = new byte[65535];
        private readonly MemoryStream _instream = new MemoryStream();
        private int _bytesRemaining;
        private EndPoint _endPoint;
        private NetworkStream _networkStream;
        private Action<int> _receiveHandler;
        private Timer _reconnectTimer;
        private Socket _socket;
        private byte[] _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTransport{T}"/> class.
        /// </summary>
        /// <param name="socket">Connected socket.</param>
        public ChoBinaryTransport(Socket socket)
        {
            Init(socket);
            _endPoint = socket.RemoteEndPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTransport{T}"/> class.
        /// </summary>
        /// <param name="endPoint">Endpoint to connect to.</param>
        public ChoBinaryTransport(EndPoint endPoint)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(endPoint);
            _endPoint = endPoint;
            Init(_socket);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTransport{T}"/> class.
        /// </summary>
        /// <param name="socket">Connected socket.</param>
        public ChoBinaryTransport(Socket socket, X509Certificate2 certificate)
        {
            _endPoint = socket.RemoteEndPoint;
            Init(socket, certificate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTransport{T}"/> class.
        /// </summary>
        /// <param name="endPoint">Endpoint to connect to.</param>
        public ChoBinaryTransport(EndPoint endPoint, X509Certificate2 certificate)
        {
            _endPoint = endPoint;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Init(_socket, certificate);
        }

        /// <summary>
        /// Gets or sets number of millseconds between reconnect attempts.
        /// </summary>
        /// <value>Default is 5000</value>
        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Gets or sets transport version.
        /// </summary>
        protected int Version
        {
            get { return BitConverter.ToInt32(_version, 0); }
            set { _version = BitConverter.GetBytes(value); }
        }

        private void BeginReceive(int offset, int length, AsyncCallback callback)
        {
            try
            {
                _socket.BeginReceive(_inbuffer, offset, length, SocketFlags.None, callback, null);
            }
            catch (Exception err)
            {
                ChoTrace.Info("Got disconnected when receiving.", err);
                HandleDisconnected();
                return;
            }
        }

        /// <summary>
        /// Create a new packet
        /// </summary>
        /// <param name="value">Object to wrap</param>
        /// <returns>Created packet.</returns>
        protected virtual T CreateType(object value)
        {
            return (T)typeof(T).GetConstructor(new[] { typeof(object) }).Invoke(new[] { value });
        }

        private void Disconnect()
        {
            _reconnectTimer.Dispose();
            _reconnectTimer = null;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket = null;
            Disconnected(this, EventArgs.Empty);
        }

        private void HandleDisconnected()
        {
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
            }
        }

        private void Init(Socket socket, X509Certificate2 certificate)
        {
            Init(socket);
        }

        private void Init(Socket socket)
        {
            ReconnectInterval = 5000;
            Version = 1;
            _socket = socket;
            _receiveHandler = ProcessHeader;
            _networkStream = new NetworkStream(socket);
            _reconnectTimer = new Timer(OnTryToReconnect, null, Timeout.Infinite, Timeout.Infinite);
            if (_endPoint == null)
                _endPoint = socket.RemoteEndPoint;
            Connected(this, EventArgs.Empty);
        }

        private void OnObject(T packet)
        {
            //ObjectReceived(this, new ObjectEventArgs<T>(packet));
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int bytesRead = _socket.EndReceive(ar);
                if (bytesRead != 0)
                {
                    _receiveHandler(bytesRead);
                    return;
                }
            }
            catch (Exception err)
            {
                ChoTrace.Info("EndReceive failed.", err);
            }


            HandleDisconnected();
            return;
        }

        private void OnTryToReconnect(object state)
        {
            try
            {
                _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(_endPoint);
                ChoTrace.Info("Reconnected successfully.");
                Init(socket);
            }
            catch (Exception err)
            {
                ChoTrace.Debug("Failed to reconnect.", err);
                _reconnectTimer.Change(Timeout.Infinite, 5000);
            }
        }

        private void ProcessBody(int bytesRead)
        {
            if (bytesRead >= _bytesRemaining)
            {
                _instream.Write(_inbuffer, 0, _bytesRemaining);
                _bytesRemaining = 0;
                _instream.Flush();
                _instream.Position = 0;
                var packet = (T)_formatter.Deserialize(_instream);
                OnObject(packet);

                _receiveHandler = ProcessHeader;
                BeginReceive(0, 8, OnReceive);
            }
            else
            {
                _bytesRemaining -= bytesRead;
                _instream.Write(_inbuffer, 0, bytesRead);
                BeginReceive(0, _bytesRemaining, OnReceive);
            }
        }

        private void ProcessHeader(int bytesRead)
        {
            if (bytesRead < 8)
            {
                BeginReceive(bytesRead, 8 - bytesRead, OnReceive);
                return;
            }

            int version = BitConverter.ToInt32(_inbuffer, 0);
            _bytesRemaining = BitConverter.ToInt32(_inbuffer, 4);

            _receiveHandler = ProcessBody;
            BeginReceive(0, _bytesRemaining, OnReceive);
        }

        /// <summary>
        /// Send an object.
        /// </summary>
        /// <param name="value">Object to send</param>
        /// <remarks>
        /// Must be marked as serializable and connection must be open.
        /// </remarks>
        public void Send(object value)
        {
            Send(CreateType(value));
        }

        #region IObjectTransport<T> Members

        /// <summary>
        /// Gets or sets if channel is connected to the other end.
        /// </summary>
        public bool IsConnected
        {
            get { return _socket != null && _socket.Connected; }
        }

        /// <summary>
        /// Send an object.
        /// </summary>
        /// <param name="value">Object to send</param>
        /// <remarks>
        /// Must be marked as serializable and connection must be open.
        /// </remarks>
        public void Send(T value)
        {
            if (!_socket.Connected)
                return;

            var stream = new MemoryStream();
            _formatter.Serialize(stream, value);
            byte[] body = stream.GetBuffer();

            var header = new byte[8];
            byte[] streamLength = BitConverter.GetBytes((Int32)stream.Length);
            Buffer.BlockCopy(_version, 0, header, 0, _version.Length);
            Buffer.BlockCopy(streamLength, 0, header, _version.Length, streamLength.Length);

            _socket.Send(header);
            _socket.Send(body, (int)stream.Length, SocketFlags.None);
        }


        /// <summary>
        /// Start waiting for incoming packets.
        /// </summary>
        public void Start(Socket socket)
        {
            if (_socket != null)
            {
                ChoTrace.Warn("Already got a socket, disposing it.");
                Disconnect();
                _socket.Close();
                _socket = null;
            }

            Init(socket);
            _socket.BeginReceive(_inbuffer, 0, 8, SocketFlags.None, OnReceive, null);
        }

        /// <summary>
        /// Received a packet from remote end point.
        /// </summary>
        public event EventHandler<EventArgs> ObjectReceived = delegate { };

        /// <summary>
        /// Client side disconnected.
        /// </summary>
        public event EventHandler Disconnected = delegate { };

        public event EventHandler Connected = delegate { };

        #endregion

        #region IStartable Members

        /// <summary>
        /// Start waiting for incoming packets.
        /// </summary>
        public void Start()
        {
            _socket.BeginReceive(_inbuffer, 0, 8, SocketFlags.None, OnReceive, null);
        }

        /// <summary>
        /// Stop component
        /// </summary>
        public void Stop()
        {
            Disconnect();
        }

        #endregion
    }

    /// <summary>
    /// Creates a new packet used for transport.
    /// </summary>
    public delegate T CreatePacketHandler<T>(object value);
}
