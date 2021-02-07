using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Cinchoo.Core.Diagnostics;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Threading;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoNetMessageEventArgs : EventArgs
    {
        public readonly ChoNetMessage Message;
        public readonly byte[] RawPayload;

        public ChoNetMessageEventArgs(ChoNetMessage message, byte[] rawMessage)
        {
            Message = message;
            RawPayload = rawMessage;
        }
    }

    [Serializable]
    public class ChoPingNetMessage : ChoNetMessage
    {
        private const string PingMsgToken = "__PING__";

        public ChoPingNetMessage() : base(PingMsgToken)
        {
        }
    }

    [Serializable]
    public class ChoNetMessage : IXmlSerializable, ISerializable
    {
        public string Id;
        public object Payload;
        public DateTime TimeStamp;
        public string IPAddress;
        public string HostName;

        #region Constructors

        public ChoNetMessage() : this(null)
        {
        }

        public ChoNetMessage(object value)
        {
            HostName = Dns.GetHostName();
            IPAddress = ChoIPAddress.GetLocalIPv4();
            Payload = value;
            TimeStamp = DateTime.Now;
        }

        protected ChoNetMessage(SerializationInfo info, StreamingContext context)
        {
            string objTypeText = info.GetString("_valueType");
            if (!objTypeText.IsNullOrWhiteSpace())
            {
                Type objType = Type.GetType(objTypeText);
                Payload = info.GetValue("_value", objType);
            }
            TimeStamp = info.GetDateTime("_timeStamp");
            Id = info.GetString("_id");
            IPAddress = info.GetString("_ipAddress");
            HostName = info.GetString("_hostName");
        }

        #endregion Constructors

        public override string ToString()
        {
            return Payload != null ? ChoObject.ToString(Payload) : null;
        }

        public string ToStringL()
        {
            return ChoObject.ToString(this);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            Boolean isEmptyElement = reader.IsEmptyElement; //Value
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                isEmptyElement = reader.IsEmptyElement; //Value Content
                reader.ReadStartElement();
                if (!isEmptyElement)
                {
                    Payload = reader.ReadOuterXml().ToObjectFromXml();
                    reader.ReadEndElement();
                }
            }

            isEmptyElement = reader.IsEmptyElement; //Timestamp
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                XmlSerializer serializer2 = XmlSerializer.FromTypes(new[] { typeof(DateTime) }).GetNValue(0);
                TimeStamp = (DateTime)serializer2.Deserialize(reader);
                reader.ReadEndElement();
            }
            isEmptyElement = reader.IsEmptyElement; //Id
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                XmlSerializer serializer2 = XmlSerializer.FromTypes(new[] { typeof(String) }).GetNValue(0);
                Id = (String)serializer2.Deserialize(reader);
                reader.ReadEndElement();
            }
            isEmptyElement = reader.IsEmptyElement; //IPAddress
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                XmlSerializer serializer2 = XmlSerializer.FromTypes(new[] { typeof(String) }).GetNValue(0);
                IPAddress = (String)serializer2.Deserialize(reader);
                reader.ReadEndElement();
            }
            isEmptyElement = reader.IsEmptyElement; //HostName
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                XmlSerializer serializer2 = XmlSerializer.FromTypes(new[] { typeof(String) }).GetNValue(0);
                HostName = (String)serializer2.Deserialize(reader);
                reader.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Value");
            if (Payload != null)
                writer.WriteRaw(Payload.ToNullNSXmlWithType());

            writer.WriteEndElement();

            writer.WriteStartElement("TimeStamp");
            writer.WriteRaw(TimeStamp.ToNullNSXml());
            writer.WriteEndElement();
            writer.WriteStartElement("Id");
            writer.WriteRaw(Id.IsNullOrWhiteSpace() ? String.Empty.ToNullNSXml() : Id.ToNullNSXml());
            writer.WriteEndElement();
            writer.WriteStartElement("IPAddress");
            writer.WriteRaw(IPAddress.IsNullOrWhiteSpace() ? String.Empty.ToNullNSXml() : IPAddress.ToNullNSXml());
            writer.WriteEndElement();
            writer.WriteStartElement("HostName");
            writer.WriteRaw(HostName.IsNullOrWhiteSpace() ? String.Empty.ToNullNSXml() : HostName.ToNullNSXml());
            writer.WriteEndElement();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Payload != null)
            {
                info.AddValue("_valueType", Payload.GetTypeName(), typeof(string));
                info.AddValue("_value", Payload, Payload.GetType());
            }
            info.AddValue("_timeStamp", TimeStamp, typeof(DateTime));
            info.AddValue("_id", Id, typeof(string));
            info.AddValue("_ipAddress", IPAddress, typeof(string));
            info.AddValue("_hostName", HostName, typeof(string));
        }
    }

    public enum ChoNetMessageType { Raw, Binary, Xml };

    public class ChoUDPMulticastClient : ChoDisposableObject, IChoNetClient
    {
        #region Instance Data Members (Private)

        private const int PingMsgInterval = 1000;

        private readonly IPAddress _multicastAddress;
        private readonly UdpClient _serverSocket = null;
        private readonly Encoding _encoding = Encoding.ASCII;
        private IPEndPoint _localEP = null;
        private IPEndPoint _remoteEP = null;
        private IAsyncResult _recieveResult = null;
        private ChoNetMessageType? _messageType = null;
        private bool _identifiedMessageType = false;
        private Task _sendPingMsgTask;
        private Task _pingServerTask;

        public EventHandler<ChoEventArgs<bool>> OnServerAvailable;

        public bool IsServer
        {
            get;
            private set;
        }

        private bool _isServerAvailable;
        public bool IsServerAvailable
        {
            get { return _isServerAvailable; }
            private set
            {
                if (_isServerAvailable == value) return;
                _isServerAvailable = value;
                OnServerAvailable.Raise(this, new ChoEventArgs<bool>(value));
            }
        }

        private DateTime _lastPingMsgArrived = DateTime.MinValue;

        #endregion Instance Data Members (Private)

        #region Events

        public event EventHandler<ChoNetMessageEventArgs> MessageReceived;
        public event EventHandler<ChoNetMessageEventArgs> RawMessageReceived;
        public Func<object, byte[]> SerializePayload;

        #endregion Events

        #region Constructors

        public ChoUDPMulticastClient(string multicastAddress, int port, ChoNetMessageType? messageType = null, Encoding encoding = null)
            : this(IPAddress.Parse(multicastAddress), port, messageType, encoding)
        {
        }

        public ChoUDPMulticastClient(IPAddress multicastAddress, int port, ChoNetMessageType? messageType = null, Encoding encoding = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(multicastAddress, "MulticastAddress");

            if (encoding != null)
                _encoding = encoding;
            _messageType = messageType;

            _multicastAddress = multicastAddress;
            _localEP = new IPEndPoint(IPAddress.Any, port);
            _remoteEP = new IPEndPoint(_multicastAddress, port);

            _serverSocket = new UdpClient();
            _serverSocket.DontFragment = true;
            //_serverSocket.MulticastLoopback = false;
            _serverSocket.ExclusiveAddressUse = false;
            _serverSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _serverSocket.Client.Bind(_localEP);
            _serverSocket.JoinMulticastGroup(_multicastAddress);

            _recieveResult = _serverSocket.BeginReceive(ReceiveCallback, _localEP);
            _pingServerTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_lastPingMsgArrived != DateTime.MinValue)
                    {
                        IsServerAvailable = (DateTime.Now.Subtract(_lastPingMsgArrived).TotalMilliseconds <= PingMsgInterval * 2);
                    }

                    Thread.Sleep(PingMsgInterval); //TODO: Parameterize
                }
            });
        }

        #endregion Constructors

        #region Send Overloads

        public int Send(string value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "value");

            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(int value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(bool value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(char value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(double value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(float value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(long value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(short value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(uint value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(ulong value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(ushort value)
        {
            return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(object value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "Value");
            if (value != null && typeof(ChoNetMessage).IsAssignableFrom(value.GetType()))
                return Send(ConvertToBytes(value as ChoNetMessage));
            else
                return Send(ConvertToBytes(new ChoNetMessage(value)));
        }

        public int Send(byte[] value)
        {
            if (value == null) return 0;

            return _serverSocket.Send(value, value.Length, _remoteEP);
        }

        #endregion Send Overloads

        #region Instance Properties (Public)

        public void StartAsServer()
        {
            IsServer = true;
            _sendPingMsgTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Send(new ChoPingNetMessage());
                        Thread.Sleep(PingMsgInterval); //TODO: Parameterize
                    }
                });
        }

        public UdpClient Client
        {
            get { return _serverSocket; }
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Private)

        private byte[] ConvertToBytes(ChoNetMessage obj)
        {
            if (_messageType == ChoNetMessageType.Xml)
                return _encoding.GetBytes(obj.ToNullNSXmlWithType());
            else if (_messageType == ChoNetMessageType.Binary)
                return ChoObject.Serialize(obj);
            else
            {
                if (SerializePayload != null)
                    return SerializePayload(obj.Payload);
                else
                    return null;
            }
        }

        protected virtual bool PreProcessMessage(ChoNetMessage msg)
        {
            if (msg is ChoPingNetMessage)
            {
                if (IsServer)
                    return true;
                else
                {
                    _lastPingMsgArrived = msg.TimeStamp;
                    //IsServerAvailable = true;
                    return true;
                }
            }

            return false;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Byte[] payload = _serverSocket.EndReceive(ar, ref _localEP);

                IdentifyMessageType(payload);
                if (_messageType == null || _messageType.Value == ChoNetMessageType.Raw)
                {
                    EventHandler<ChoNetMessageEventArgs> rawMessageReceived = RawMessageReceived;
                    if (rawMessageReceived != null)
                        rawMessageReceived(this, new ChoNetMessageEventArgs(null, payload));
                }
                else if (_messageType.Value == ChoNetMessageType.Xml)
                {
                    try
                    {
                        ChoNetMessage msg = (ChoNetMessage)_encoding.GetString(payload).ToObjectFromXml(); // ChoObject.XmlDeserialize<ChoNetMessage>(_encoding.GetString(payload));
                        if (!PreProcessMessage(msg))
                        {
                            EventHandler<ChoNetMessageEventArgs> messageReceived = MessageReceived;
                            if (messageReceived != null)
                                messageReceived(this, new ChoNetMessageEventArgs(msg, null));
                        }
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex.ToString());
                    }
                }
                else if (_messageType.Value == ChoNetMessageType.Binary)
                {
                    try
                    {
                        ChoNetMessage msg = (ChoNetMessage)ChoObject.Deserialize(payload);
                        if (!PreProcessMessage(msg))
                        {
                            EventHandler<ChoNetMessageEventArgs> messageReceived = MessageReceived;
                            if (messageReceived != null)
                                messageReceived(this, new ChoNetMessageEventArgs(msg, null));
                        }
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex.ToString());
                    }
                }
            }
            finally
            {
                _recieveResult = _serverSocket.BeginReceive(ReceiveCallback, null);
            }
        }

        private void IdentifyMessageType(byte[] payload)
        {
            if (_identifiedMessageType) return;

            try
            {
                if (_messageType == null)
                {
                    _messageType = ChoNetMessageType.Raw;
                    try
                    {
                        ChoNetMessage scalarObject = (ChoNetMessage)ChoObject.Deserialize(payload);
                        _messageType = ChoNetMessageType.Binary;
                    }
                    catch
                    {
                        try
                        {
                            ChoNetMessage scalarObject = (ChoNetMessage)(ChoNetMessage)_encoding.GetString(payload).ToObjectFromXml(); // ChoObject.XmlDeserialize<ChoNetMessage>(_encoding.GetString(payload));
                            _messageType = ChoNetMessageType.Xml;
                        }
                        catch
                        {
                            Trace.WriteLine("Failed to identity message type. Setting the message format as raw payload mode.");
                        }
                    }
                }
            }
            finally
            {
                _identifiedMessageType = true;
            }
        }

        #endregion Instance Members (Private)

        #region Dispoable Overrides

        protected override void Dispose(bool finalize)
        {
            if (!finalize)
            {
                if (_serverSocket != null)
                {
                    _serverSocket.DropMulticastGroup(_multicastAddress);
                    _serverSocket.Close();
                }
            }
        }

        #endregion Dispoable Overrides
    }
}