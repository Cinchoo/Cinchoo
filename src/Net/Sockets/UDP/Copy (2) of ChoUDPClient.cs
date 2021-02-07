using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Cinchoo.Core.Diagnostics;
using System.Diagnostics;
using System.Net.NetworkInformation;

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

        public ChoUDPClient(int port, Encoding encoding = null, string multicastAddress = null, int TTL = 50)
        {
            if (port <= 0)
                throw new ArgumentException("Invalid port [{0}] number.".FormatString(port));

            if (encoding != null)
                _encoding = encoding;

            foreach (IPAddress localAddress in LocalAddresses)
            {
                var listener = new UdpClient(AddressFamily.InterNetwork);
                listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.Client.Bind(new IPEndPoint(localAddress, port));

                if (!multicastAddress.IsNullOrWhiteSpace())
                {
                    if (TTL < 0)
                        throw new ArgumentException("TTL must be > 0 [{0}].".FormatString(port));
                
                    listener.JoinMulticastGroup(IPAddress.Parse(multicastAddress), localAddress);
                }
                //_groupEP = new IPEndPoint(IPAddress.Any, 0);
                _recieveResult = listener.BeginReceive(ReceiveCallback, 
                    new object[]
                    {
                        listener, new IPEndPoint(localAddress, ((IPEndPoint)listener.Client.LocalEndPoint).Port)
                    });
            }
        }

        #endregion Constructors

        internal static IEnumerable<IPAddress> LocalAddresses
        {
            get
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if ((!networkInterface.Supports(NetworkInterfaceComponent.IPv4)) ||
                        (networkInterface.OperationalStatus != OperationalStatus.Up))
                    {
                        continue;
                    }

                    IPInterfaceProperties adapterProperties = networkInterface.GetIPProperties();
                    UnicastIPAddressInformationCollection unicastIPAddresses = adapterProperties.UnicastAddresses;

                    foreach (UnicastIPAddressInformation unicastIPAddress in unicastIPAddresses)
                    {
                        if (unicastIPAddress.Address.AddressFamily != AddressFamily.InterNetwork)
                        {
                            continue;
                        }

                        yield return unicastIPAddress.Address;
                    }
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint ep = null;
            var args = (object[])ar.AsyncState;
            var session = (UdpClient)args[0];
            var local = (IPEndPoint)args[1];
            
            try
            {

                Byte[] payload = session.EndReceive(ar, ref ep);

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
                _recieveResult = session.BeginReceive(ReceiveCallback, args);
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
