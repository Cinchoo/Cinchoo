using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Cinchoo.Core.Runtime.Serialization;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoTcpNullServiceProvider : ChoTcpServiceProvider
    {
        public static readonly ChoTcpNullServiceProvider Default = new ChoTcpNullServiceProvider();
        private ChoNetMessageType? _messageType = null;

        public override object Clone()
        {
            return new ChoTcpNullServiceProvider();
        }

        public override void OnAcceptConnection(ChoConnectionState state)
        {
        }

        public override void OnReceiveData(ChoConnectionState state)
        {
        }

        public override void OnDropConnection(ChoConnectionState state)
        {
        }

        public override byte[] Marshal(byte[] buffer, out int length)
        {
            length = buffer != null ? buffer.Length : 0;
            return buffer;
        }
    }

    public abstract class ChoTcpServiceProvider : ICloneable
    {
        /// <SUMMARY>
        /// Provides a new instance of the object.
        /// </SUMMARY>
        public abstract object Clone();

        /// <SUMMARY>
        /// Gets executed when the server accepts a new connection.
        /// </SUMMARY>
        public abstract void OnAcceptConnection(ChoConnectionState state);

        /// <SUMMARY>
        /// Gets executed when the server detects incoming data.
        /// This method is called only if
        /// OnAcceptConnection has already finished.
        /// </SUMMARY>
        public abstract void OnReceiveData(ChoConnectionState state);

        /// <SUMMARY>
        /// Gets executed when the server needs to shutdown the connection.
        /// </SUMMARY>
        public abstract void OnDropConnection(ChoConnectionState state);

        public abstract byte[] Marshal(byte[] buffer, out int length);
    }

    /// <SUMMARY>
    /// This class holds useful information
    /// for keeping track of each client connected
    /// to the server, and provides the means
    /// for sending/receiving data to the remote
    /// host.
    /// </SUMMARY>
    public class ChoConnectionState
    {
        internal Socket Connection;
        internal ChoTcpServer Server;
        internal ChoTcpClient Client;
        internal ChoTcpServiceProvider Provider;
        internal byte[] Buffer;
        internal IChoObjectSerializer Serializer;

        /// <SUMMARY>
        /// Tells you the IP Address of the remote host.
        /// </SUMMARY>
        public EndPoint RemoteEndPoint
        {
            get { return Connection.RemoteEndPoint; }
        }

        /// <SUMMARY>
        /// Returns the number of bytes waiting to be read.
        /// </SUMMARY>
        public int AvailableData
        {
            get { return Connection.Available; }
        }

        /// <SUMMARY>
        /// Tells you if the socket is connected.
        /// </SUMMARY>
        public bool Connected
        {
            get { return Connection.Connected; }
        }

        /// <SUMMARY>
        /// Reads data on the socket, returns
        /// the number of bytes read.
        /// </SUMMARY>
        public int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                if (Connection.Available > 0)
                    return Connection.Receive(buffer, offset,
                           count, SocketFlags.None);
                else return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <SUMMARY>
        /// Sends Data to the remote host.
        /// </SUMMARY>
        public bool Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) return true;

            try
            {
                buffer = Provider.Marshal(buffer, out count);
                Connection.Send(buffer, offset, count, SocketFlags.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Send(object packet)
        {
            if (!Connected)
                return false;

            byte[] buffer = Serializer.Serialize(packet) as byte[];
            return Write(buffer, 0, buffer.Length);
        }
        
        public void InvokeObjectReceived(byte[] payload)
        {
            if (payload == null) return;

            if (Server != null)
                Server.OnObjectRecieved(this, Server.Serializer.Deserialize(payload));
            else if (Client != null)
                Client.OnObjectRecieved(this, Client.Serializer.Deserialize(payload));
        }

        /// <SUMMARY>
        /// Ends connection with the remote host.
        /// </SUMMARY>
        public void EndConnection()
        {
            if (Server != null)
                Server.DropConnection(this);
        }
    }

}
