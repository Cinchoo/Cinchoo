using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoPingServer : ChoDisposableObject
    {
        private readonly int _timeout = 5000;
        private readonly int _delay = 1000;
        private readonly Ping _pingServer;
        private readonly byte[] _buffer;
        private readonly PingOptions _options = new PingOptions(64, true);
        private IPAddress _ipAddress;

        public event EventHandler<PingCompletedEventArgs> PingCompleted;

        public ChoPingServer(string ipAddress, int timeout = Int32.MinValue, int delay = Int32.MinValue, PingOptions options = null)
            : this(IPAddress.Parse(ipAddress), timeout, delay, options)
        {
        }

        public ChoPingServer(IPAddress ipAddress, int timeout = Int32.MinValue, int delay = Int32.MinValue, PingOptions options = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(ipAddress, "IPAddress");

            if (timeout != Int32.MinValue && timeout <= 0)
                throw new ArgumentException("Timeout should be >= 0");

            if (delay != Int32.MinValue && delay <= 0)
                throw new ArgumentException("Delay should be >= 0");

            if (timeout != Int32.MinValue)
                _timeout = timeout;
            if (delay != Int32.MinValue)
                _delay = delay;
            if (options != null)
                _options = options;
            _buffer = Encoding.ASCII.GetBytes("Ping_{0}".FormatString(ChoRandom.NextRandom()));
            _pingServer = new Ping();
            _pingServer.PingCompleted += new PingCompletedEventHandler(PingServerPingCompleted);
            _ipAddress = ipAddress;
        }

        public void Start()
        {
            ChoGuard.NotDisposed(this);
            _pingServer.SendAsync(_ipAddress, _timeout, _options);
        }

        private void PingServerPingCompleted(object sender, PingCompletedEventArgs e)
        {
            EventHandler<PingCompletedEventArgs> pingCompleted = PingCompleted;
            if (pingCompleted != null)
                pingCompleted(this, e);

            Thread.Sleep(_delay);
            Start();
        }

        protected override void Dispose(bool finalize)
        {
            _pingServer.Dispose();
        }
    }
}
