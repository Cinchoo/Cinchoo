using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoEchoServer : ChoDisposableObject
    {
        private readonly ChoEchoServiceProvider _provider;
        private readonly ChoTcpServer _server;

        public ChoEchoServer(int port)
        {
            _provider = new ChoEchoServiceProvider();
            _server = new ChoTcpServer(port, _provider);
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            if (_server != null)
                _server.Stop();
        }

        protected override void Dispose(bool finalize)
        {
            Stop();
        }
    }

    /// <SUMMARY>
    /// EchoServiceProvider. Just replies messages
    /// received from the clients.
    /// </SUMMARY>
    public class ChoEchoServiceProvider : ChoTcpServiceProvider
    {
        private string _receivedStr;

        public override object Clone()
        {
            return new ChoEchoServiceProvider();
        }

        public override void OnAcceptConnection(ChoConnectionState state)
        {
            _receivedStr = "";
            if (!state.Write(Encoding.UTF8.GetBytes(
                            "Hello World!\r\n"), 0, 14))
                state.EndConnection();
            //if write fails... then close connection
        }


        public override void OnReceiveData(ChoConnectionState state)
        {
            byte[] buffer = new byte[1024];
            while (state.AvailableData > 0)
            {
                int readBytes = state.Read(buffer, 0, 1024);
                if (readBytes > 0)
                {
                    _receivedStr +=
                      Encoding.UTF8.GetString(buffer, 0, readBytes);
                    if (_receivedStr.IndexOf("<EOF>") >= 0)
                    {
                        state.Write(Encoding.UTF8.GetBytes(_receivedStr), 0,
                        _receivedStr.Length);
                        _receivedStr = "";
                    }
                }
                else state.EndConnection();
                //If read fails then close connection
            }
        }


        public override void OnDropConnection(ChoConnectionState state)
        {
            //Nothing to clean here
        }

        public override byte[] Marshal(byte[] buffer, out int length)
        {
            length = buffer != null ? buffer.Length : 0;
            return buffer;
        }
    }
}