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
        private readonly List<UdpClient> _sendClients = new List<UdpClient>();

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoUDPServer(string multicastAddress, int port, ChoUDPMessageType messageType = ChoUDPMessageType.Binary, Encoding encoding = null)
            : this(IPAddress.Parse(multicastAddress), port)
        {
        }

        public ChoUDPServer(IPAddress multicastAddress, int port, ChoUDPMessageType messageType = ChoUDPMessageType.Binary, Encoding encoding = null)
            //: this(new IPEndPoint(ipAddress, port), messageType, encoding)
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            
            foreach (IPAddress localAddress in ChoUDPClient.LocalAddresses)
            {
                client.JoinMulticastGroup(multicastAddress, localAddress);
                _sendClients.Add(new UdpClient(new IPEndPoint(localAddress, port)));
            }
            listener.Client.Bind(new IPEndPoint(localAddress, port));


            _serverEndPoint = new IPEndPoint(multicastAddress, port);
            _messageType = messageType;
            if (encoding != null)
                _encoding = encoding;
        }

        //public ChoUDPServer(IPEndPoint serverEndPoint, ChoUDPMessageType messageType = ChoUDPMessageType.Binary, Encoding encoding = null)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(serverEndPoint, "EndPoint");

        //    _serverEndPoint = serverEndPoint;
        //    _messageType = messageType;
        //    if (encoding != null)
        //        _encoding = encoding;
        //}

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

            int length = value.Length;

            foreach (UdpClient client in _sendClients)
                client.Send(value, length, _serverEndPoint);

            return length;
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
