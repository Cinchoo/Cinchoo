using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cinchoo.Core.Net.Sockets
{
    public abstract class ChoTcpObjectServiceProvider : ChoTcpServiceProvider
    {
        private bool _headerFound = false;
        private byte[] _inbuffer;
        //private readonly MemoryStream _instream = new MemoryStream();
        private readonly BinaryFormatter _formatter = new BinaryFormatter();
        private int _msgBodySize = 0;
        private byte[] _version;

        public ChoTcpObjectServiceProvider()
        {
            Version = 1;
        }

        public override void OnAcceptConnection(ChoConnectionState state)
        {
        }

        public override void OnReceiveData(ChoConnectionState state)
        {
            while (state.AvailableData > 0)
            {
                int readBytes = state.Read(state._buffer, 0, state._buffer.Length);

                byte[] buffer = state._buffer.Take(readBytes).ToArray();
                if (_inbuffer == null)
                    _inbuffer = buffer;
                else
                    _inbuffer = ChoByteArrayEx.Combine(_inbuffer, buffer);

                if (!_headerFound)
                {
                    _msgBodySize = ProcessHeader();
                    if (_headerFound)
                        ProcessBody();
                }
                else
                {
                    ProcessBody();
                }
            }
        }

        protected abstract void OnObject(object packet);

        /// <summary>
        /// Send an object.
        /// </summary>
        /// <param name="value">Object to send</param>
        /// <remarks>
        /// Must be marked as serializable and connection must be open.
        /// </remarks>
        public override byte[] ToByteArray(object value)
        {
            var stream = new MemoryStream();
            stream.Position = 0;
            _formatter.Serialize(stream, value);
            byte[] body = stream.GetBuffer();

            var header = new byte[8];
            byte[] streamLength = BitConverter.GetBytes((Int32)stream.Length);
            Buffer.BlockCopy(_version, 0, header, 0, _version.Length);
            Buffer.BlockCopy(streamLength, 0, header, _version.Length, streamLength.Length);

            return ChoByteArrayEx.Combine(header, body.Take((Int32)stream.Length).ToArray());
        }

        private bool ProcessBody()
        {
            if (_inbuffer.Length < _msgBodySize)
                return false;

            MemoryStream _instream = new MemoryStream();
            _instream.Write(_inbuffer, 0, _msgBodySize);
            _inbuffer = ChoByteArrayEx.SubArray(_inbuffer, _msgBodySize);
            _instream.Flush();
            _instream.Position = 0;

            try
            {
                var packet = _formatter.Deserialize(_instream);
                OnObject(packet);
            }
            finally
            {
                _headerFound = false;
            }

            return true;
        }

        private int ProcessHeader()
        {
            if (_inbuffer.Length < 8)
            {
                return -1;
            }

            _headerFound = true;
            int version = BitConverter.ToInt32(_inbuffer, 0);
            int bodySize = BitConverter.ToInt32(_inbuffer, 4);

            _inbuffer = ChoByteArrayEx.SubArray(_inbuffer, 8);
            return bodySize;
        }

        public override void OnDropConnection(ChoConnectionState state)
        {
        }

        /// <summary>
        /// Gets or sets transport version.
        /// </summary>
        protected int Version
        {
            get { return BitConverter.ToInt32(_version, 0); }
            set { _version = BitConverter.GetBytes(value); }
        }
    }
}
