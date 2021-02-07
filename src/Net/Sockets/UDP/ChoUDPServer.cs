using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization;

namespace Cinchoo.Core.Net.Sockets
{
    [Serializable]
    public class ChoScalarObject
    {
        public readonly static ChoScalarObject Default = new ChoScalarObject();

        public object Value;
        public DateTime TimeStamp;

        #region Constructors

        public ChoScalarObject()
        {
        }

        public ChoScalarObject(object value)
        {
            ChoGuard.ArgumentNotNull(value, "value");

            Value = value;
            TimeStamp = DateTime.Now;
        }

        protected ChoScalarObject(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion Constructors

        public override string ToString()
        {
            return Value != null ? Value.ToString() : null;
        }
    }

    public enum ChoUDPMessageType { Binary, Xml };

    public class ChoUDPServer : ChoDisposableObject
    {
        #region Instance Data Members (Private)

        //private readonly Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private readonly UdpClient _serverSocket = new UdpClient();
        private readonly IPEndPoint _serverEndPoint = null;
        private readonly ChoUDPMessageType _messageType = ChoUDPMessageType.Binary;
        private readonly Encoding _encoding = Encoding.ASCII;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoUDPServer(string multicastAddress, int port, ChoUDPMessageType messageType = ChoUDPMessageType.Binary, Encoding encoding = null, int timeToLive = 0)
            : this(IPAddress.Parse(multicastAddress), port)
        {
        }

        public ChoUDPServer(IPAddress multicastAddress, int port, ChoUDPMessageType messageType = ChoUDPMessageType.Binary, Encoding encoding = null, int timeToLive = 0)
            : this(new IPEndPoint(multicastAddress, port), messageType, encoding)
        {

        }

        public ChoUDPServer(IPEndPoint serverEndPoint, ChoUDPMessageType messageType = ChoUDPMessageType.Binary, Encoding encoding = null, int timeToLive = 0)
        {
            ChoGuard.ArgumentNotNullOrEmpty(serverEndPoint, "EndPoint");

            _serverEndPoint = serverEndPoint;
            _messageType = messageType;
            if (encoding != null)
                _encoding = encoding;
            _serverSocket.JoinMulticastGroup(serverEndPoint.Address, timeToLive);
        }

        #endregion Constructors

        #region Send Overloads

        public int Send(string value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "value");

            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(int value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(bool value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(char value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(double value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(float value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(long value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(short value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(uint value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(ulong value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(ushort value)
        {
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(object value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "Value");
            return Send(ConvertToBytes(new ChoScalarObject(value)));
        }

        public int Send(byte[] value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "Value");

            return _serverSocket.Send(value, value.Length, _serverEndPoint);
        }

        #endregion Send Overloads

        private byte[] ConvertToBytes(ChoScalarObject obj)
        {
            if (_messageType == ChoUDPMessageType.Xml)
                return _encoding.GetBytes(ChoObject.XmlSerialize(new ChoScalarObject(obj)));
            else
                return ChoObject.Serialize(new ChoScalarObject(obj));
        }

        protected override void Dispose(bool finalize)
        {
            _serverSocket.Close();
        }
    }
}