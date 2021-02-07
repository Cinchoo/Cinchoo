using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoEchoReplyEventArgs : EventArgs
    {
        public readonly bool IsSuccess;
        public readonly Exception Exception;

        internal ChoEchoReplyEventArgs(bool isSuccess, Exception ex)
        {
            IsSuccess = isSuccess;
            Exception = ex;
        }
    }

    public class ChoEchoClient
    {
        private readonly IPEndPoint _localEP;
        private readonly int _uniqueId = ChoRandom.NextRandom();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private int _sleepInterval = 1000;

        public event EventHandler<ChoEchoReplyEventArgs> EchoCompleted;

        public int SleepInterval
        {
            get { return _sleepInterval; }
            set
            {
                if (value > 0)
                    _sleepInterval = value;
            }
        }

        public ChoEchoClient(string hostname, int port)
        {
            _localEP = new IPEndPoint(IPAddress.Parse(hostname), port);
        }

        public ChoEchoClient(IPEndPoint localEP)
        {
            _localEP = localEP;
        }
        
        public void Start()
        {
            Task.Factory.StartNew(() => StartEcho());
        }

        public void Stop()
        {
            _cts.Cancel();
            Thread.Sleep(SleepInterval);
        }

        private void StartEcho()
        {
            CancellationToken cancellationToken = _cts.Token;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                EventHandler<ChoEchoReplyEventArgs> echoCompleted = EchoCompleted;

                // Data buffer for incoming data.
                byte[] bytes = new byte[1024];

                // Connect to a remote device.
                try
                {
                    // Create a TCP/IP  socket.
                    while (true)
                    {
                        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        if (cancellationToken.IsCancellationRequested)
                            break;

                        // Connect the socket to the remote endpoint. Catch any errors.
                        try
                        {
                            sender.Connect(_localEP);

                            while (true)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                    break;

                                // Encode the data string into a byte array.
                                byte[] msg = Encoding.ASCII.GetBytes("This is a test [{0}]<EOF>".FormatString(_uniqueId));

                                // Send the data through the socket.
                                int bytesSent = sender.Send(msg);

                                // Receive the response from the remote device.
                                int bytesRec = sender.Receive(bytes);

                                if (echoCompleted != null)
                                    echoCompleted(this, new ChoEchoReplyEventArgs(true, null));
                            }

                            // Release the socket.
                            sender.Shutdown(SocketShutdown.Both);
                            sender.Close();
                        }
                        catch (Exception e)
                        {
                            if (echoCompleted != null)
                                echoCompleted(this, new ChoEchoReplyEventArgs(false, e));
                        }

                        Thread.Sleep(SleepInterval);
                    }
                }
                catch (Exception e)
                {
                    if (echoCompleted != null)
                        echoCompleted(this, new ChoEchoReplyEventArgs(false, e));
                }

                Thread.Sleep(SleepInterval);
            }
        }
    }
}
