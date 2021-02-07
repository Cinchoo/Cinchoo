using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoPingReplyEventArgs : EventArgs
    {
        public readonly bool IsSuccess;
        public readonly Exception Exception;

        internal ChoPingReplyEventArgs(bool isSuccess, Exception ex)
        {
            IsSuccess = isSuccess;
            Exception = ex;
        }
    }

    public class ChoPingAppServer : ChoDisposableObject
    {
        private readonly int _timeout = 5000;
        private readonly int _delay = 1000;
        private readonly int _port = 0;
        private readonly TcpClient _pingServer;
        private readonly byte[] _buffer;
        private IPAddress _ipAddress;
        private IAsyncResult _result;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public event EventHandler<ChoPingReplyEventArgs> PingCompleted;

        public ChoPingAppServer(string ipAddress, int portNo, int timeout = Int32.MinValue, int delay = Int32.MinValue)
            : this(IPAddress.Parse(ipAddress), portNo, timeout, delay)
        {
        }

        public ChoPingAppServer(IPAddress ipAddress, int portNo, int timeout = Int32.MinValue, int delay = Int32.MinValue)
        {
            ChoGuard.ArgumentNotNullOrEmpty(ipAddress, "IPAddress");

            if (timeout != Int32.MinValue && timeout <= 0)
                throw new ArgumentException("Timeout should be >= 0");

            if (delay != Int32.MinValue && delay <= 0)
                throw new ArgumentException("Delay should be >= 0");

            if (portNo <= 0)
                throw new ArgumentException("Invalid {0} port number passed.".FormatString(portNo));
            _port = portNo;
            if (timeout != Int32.MinValue)
                _timeout = timeout; 
            if (delay != Int32.MinValue)
                _delay = delay;

            _ipAddress = ipAddress;
            _buffer = Encoding.ASCII.GetBytes("Ping_{0}".FormatString(ChoRandom.NextRandom()));
            _result = _pingServer.BeginConnect(_ipAddress, _port, TcpAsyncCallback, null);
        }

        public void Start()
        {
            CancellationToken token = _cts.Token;
            Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        if (token.CanBeCanceled)
                            break;

                        TcpClient pingServer = new TcpClient(new IPEndPoint(_ipAddress, _port));


                    }
                }, token);
        }

        void TcpAsyncCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            if (client == null || client.Client == null)
                return;

            EventHandler<ChoPingReplyEventArgs> pingCompleted = PingCompleted;
            try
            {
                client.EndConnect(ar);
                if (pingCompleted != null)
                    pingCompleted(this, new ChoPingReplyEventArgs(true, null));
            }
            catch (Exception ex)
            {
                if (pingCompleted != null)
                    pingCompleted(this, new ChoPingReplyEventArgs(false, ex));
            }

            Thread.Sleep(_delay);
            _result = client.BeginConnect(_ipAddress, _port, TcpAsyncCallback, null);
        }

        protected override void Dispose(bool finalize)
        {
            _cts.Cancel();
            Thread.Sleep(1000);
            _pingServer.Close();
        }
    }
}
