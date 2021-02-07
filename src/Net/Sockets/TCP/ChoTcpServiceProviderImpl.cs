using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cinchoo.Core.Diagnostics;
using System.Diagnostics;
using Cinchoo.Core.Runtime.Serialization;

namespace Cinchoo.Core.Net.Sockets
{
    public class ChoTcpServiceProviderImpl : ChoTcpServiceProvider
    {
        public static readonly ChoTcpServiceProviderImpl Instance = new ChoTcpServiceProviderImpl();

        private byte[] _inbuffer;
        private byte[] _version;
        private DateTime _lastPingMsgArrived = DateTime.MinValue;
        private bool _headerFound = false;
        private int _msgBodySize = 0;

        public ChoTcpServiceProviderImpl()
        {
            Version = 1;
        }

        public override object Clone()
        {
            return new ChoTcpServiceProviderImpl();
        }

        public override void OnAcceptConnection(ChoConnectionState state)
        {
        }

        public override void OnReceiveData(ChoConnectionState state)
        {
            while (state.AvailableData > 0)
            {
                int readBytes = state.Read(state.Buffer, 0, state.Buffer.Length);

                byte[] buffer = state.Buffer.Take(readBytes).ToArray();
                if (_inbuffer == null)
                {
                    _inbuffer = buffer;
                    _headerFound = false;
                }
                else
                    _inbuffer = ChoByteArrayEx.Combine(_inbuffer, buffer);

                try
                {
                    if (!_headerFound)
                        _msgBodySize = ProcessHeader(out _headerFound);

                    if (_headerFound)
                        ProcessBody(state, _msgBodySize);
                }
                catch (Exception ex)
                {
                    ChoTrace.Write(ex);
                }
            }
        }

        public override byte[] Marshal(byte[] buffer, out int length)
        {
            var header = new byte[8];
            byte[] streamLength = BitConverter.GetBytes((Int32)buffer.Length);
            Buffer.BlockCopy(_version, 0, header, 0, _version.Length);
            Buffer.BlockCopy(streamLength, 0, header, _version.Length, streamLength.Length);

            byte[] ret = ChoByteArrayEx.Combine(header, buffer.Take((Int32)buffer.Length).ToArray());
            length = ret != null ? ret.Length : 0;
            return ret;
        }

        /// <summary>
        /// Send an object.
        /// </summary>
        /// <param name="value">Object to send</param>
        /// <remarks>
        /// Must be marked as serializable and connection must be open.
        /// </remarks>
        //public override byte[] ToByteArray(object value)
        //{
        //    //var stream = new MemoryStream();
        //    //stream.Position = 0;
        //    //_formatter.Serialize(stream, value);
        //    byte[] body = _serializer.Serialize(value) as byte[]; // stream.GetBuffer();
        //    if (body == null) return null;

        //    var header = new byte[8];
        //    byte[] streamLength = BitConverter.GetBytes((Int32)body.Length);
        //    Buffer.BlockCopy(_version, 0, header, 0, _version.Length);
        //    Buffer.BlockCopy(streamLength, 0, header, _version.Length, streamLength.Length);

        //    return ChoByteArrayEx.Combine(header, body.Take((Int32)body.Length).ToArray());
        //}

        private void InvokeObjectReceived(ChoConnectionState state, byte[] payload)
        {
            if (state == null || payload == null) return;
            state.InvokeObjectReceived(payload);
        }

        private bool ProcessBody(ChoConnectionState state, int msgBodySize)
        {
            if (_inbuffer.Length < msgBodySize)
                return false;

            try
            {
                MemoryStream instream = new MemoryStream();
                instream.Write(_inbuffer, 0, msgBodySize);
                _inbuffer = ChoByteArrayEx.SubArray(_inbuffer, msgBodySize);
                instream.Flush();
                instream.Position = 0;

                InvokeObjectReceived(state, instream.GetBuffer());
            }
            catch (Exception ex)
            {
                ChoTrace.Write(ex);
            }
            finally
            {
                //_inbuffer = null;
                _headerFound = false;
            }

            return true;
        }

        private int ProcessHeader(out bool headerFound)
        {
            headerFound = false;
            if (_inbuffer.Length < 8)
            {
                return -1;
            }

            headerFound = true;
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
