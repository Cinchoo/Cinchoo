using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Cinchoo.Core.Diagnostics;
using System.Diagnostics;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoUDPMessageEventArgs : EventArgs
    {
        public readonly ChoScalarObject Payload;
        public readonly byte[] RawPayload;

        public ChoUDPMessageEventArgs(ChoScalarObject payload, byte[] rawPayload)
        {
            Payload = payload;
            RawPayload = rawPayload;
        }
    }

    public class ChoUDPClient : ChoDisposableObject
    {
        #region Instance Data Members (Private)

        private readonly UdpClient _listener = null;
        private readonly Encoding _encoding = Encoding.ASCII;
        private IPEndPoint _groupEP = null;
        private IAsyncResult _recieveResult = null;
        private ChoUDPMessageType? _messageType = null;
        private bool _identifiedMessageType = false;

        #endregion Instance Data Members (Private)

        public event EventHandler<ChoUDPMessageEventArgs> MessageReceived;
        public event EventHandler<ChoUDPMessageEventArgs> RawMessageReceived;

        #region Constructors

        public ChoUDPClient(int port, Encoding encoding = null, string multicastAddress = null)
        {
            if (port <= 0)
                throw new ArgumentException("Invalid port [{0}] number.".FormatString(port));

            if (encoding != null)
                _encoding = encoding;

            _listener = new UdpClient();
            _listener.ExclusiveAddressUse = false;
            _groupEP = new IPEndPoint(IPAddress.Any, port);
            _listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listener.ExclusiveAddressUse = false;
            _listener.Client.Bind(_groupEP);

            if (!multicastAddress.IsNullOrWhiteSpace())
                _listener.JoinMulticastGroup(IPAddress.Parse(multicastAddress));

            _recieveResult = _listener.BeginReceive(ReceiveCallback, _groupEP);
        }

        #endregion Constructors

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Byte[] payload = _listener.EndReceive(ar, ref _groupEP);

                EventHandler<ChoUDPMessageEventArgs> rawMessageReceived = RawMessageReceived;
                EventHandler<ChoUDPMessageEventArgs> messageReceived = MessageReceived;
                if (rawMessageReceived != null)
                    rawMessageReceived(this, new ChoUDPMessageEventArgs(null, payload));
                else if (messageReceived != null)
                {
                    IdentifyMessageType(payload);
                    if (_messageType == null)
                    {
                        if (rawMessageReceived != null)
                            rawMessageReceived(this, new ChoUDPMessageEventArgs(null, payload));
                    }
                    else if (_messageType.Value == ChoUDPMessageType.Xml)
                    {
                        try
                        {
                            ChoScalarObject scalarObject = (ChoScalarObject)ChoObject.XmlDeserialize<ChoScalarObject>(_encoding.GetString(payload));
                            MessageReceived(this, new ChoUDPMessageEventArgs(scalarObject, null));
                        }
                        catch (Exception ex)
                        {
                            ChoTrace.Error(ex.ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            ChoScalarObject scalarObject = (ChoScalarObject)ChoObject.Deserialize(payload);
                            MessageReceived(this, new ChoUDPMessageEventArgs(scalarObject, null));
                        }
                        catch (Exception ex)
                        {
                            ChoTrace.Error(ex.ToString());
                        }
                    }
                }
            }
            finally
            {
                _recieveResult = _listener.BeginReceive(ReceiveCallback, null);
            }
        }

        private void IdentifyMessageType(byte[] payload)
        {
            if (_identifiedMessageType) return;

            try
            {
                try
                {
                    ChoScalarObject scalarObject = (ChoScalarObject)ChoObject.Deserialize(payload);
                    _messageType = ChoUDPMessageType.Binary;
                }
                catch
                {
                    try
                    {
                        ChoScalarObject scalarObject = (ChoScalarObject)ChoObject.XmlDeserialize<ChoScalarObject>(_encoding.GetString(payload));
                        _messageType = ChoUDPMessageType.Xml;
                    }
                    catch
                    {
                        Trace.WriteLine("Failed to identity message type. Messages will be discarded.");
                    }
                }
            }
            finally
            {
                _identifiedMessageType = true;
            }
        }

        protected override void Dispose(bool finalize)
        {
            _listener.Close();
        }
    }
}
