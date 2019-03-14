#region NameSpaces

using System;
using System.IO;
using System.Text;

using Cinchoo.Core.Runtime.Serialization;

#endregion NameSpaces

namespace Cinchoo.Core.IO
{
	public class LiteBinaryWriter : IDisposable
	{
		#region Instance Members

		private LiteSerializationContext _serializationContext;
		private BinaryWriter _writer;

		#endregion

		#region Constructors

		public LiteBinaryWriter(Stream stream) 
			: this(stream, Encoding.Default, new LiteSerializationContext()) 
		{
		}

		public LiteBinaryWriter(Stream stream, Encoding encoding) 
			: this(stream, encoding, new LiteSerializationContext()) 
		{
		}

		public LiteBinaryWriter(Stream stream, Encoding encoding, LiteSerializationContext serializationContext) 
		{
			_serializationContext = serializationContext;
			_writer = new BinaryWriter(stream, encoding);
		}

		#endregion

		#region Finalizer

		~LiteBinaryWriter()
		{
			Dispose(false);
		}

		#endregion

		#region Instance Properties (Internal)

		internal LiteSerializationContext SerializationContext
		{
			get { return _serializationContext; }
		}

		internal Stream BaseStream
		{
			get { return _writer.BaseStream; }
		}

		#endregion

		#region Instance Members (Public)

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public void WriteObject(object obj)
		{
			ILiteSerializer serializer = _serializationContext.SerializerFactory.GetSerializerForObject(obj);
			_writer.Write(serializer.Handle);
			serializer.Write(this, obj);
		}

		public void WriteObjectAs(Type type, object obj)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			ILiteSerializer serializer = _serializationContext.SerializerFactory.GetSerializerForType(type);
			serializer.Write(this, obj);
		}

		#endregion

		#region /	WriteXXX functions	/

		public void Write(bool value) { _writer.Write(value); }
		public void Write(byte value) { _writer.Write(value); }
		public void Write(char ch) { _writer.Write(ch); }
		public void Write(short value) { _writer.Write(value); }
		public void Write(int value) { _writer.Write(value); }
		public void Write(long value) { _writer.Write(value); }
		public void Write(decimal value) { _writer.Write(value); }
		public void Write(float value) { _writer.Write(value); }
		public void Write(double value) { _writer.Write(value); }
		public void Write(DateTime value) { _writer.Write(value.Ticks); }
		public void Write(Guid value) { _writer.Write(value.ToByteArray()); }
		public void Write(byte[] value) { _writer.Write(value); }
		public void Write(char[] value) { _writer.Write(value); }
		public void Write(string value) { _writer.Write(value); }
		public void Write(byte[] value, int index, int count) { _writer.Write(value, index, count); }
		public void Write(char[] value, int index, int count) { _writer.Write(value, index, count); }
        [CLSCompliant(false)]
		public void Write(sbyte value) { _writer.Write(value); }
        [CLSCompliant(false)]
		public void Write(ushort value) { _writer.Write(value); }
        [CLSCompliant(false)]
		public void Write(uint value) { _writer.Write(value); }
        [CLSCompliant(false)]
		public void Write(ulong value) { _writer.Write(value); }

		#endregion

		#region IDisposable overrides

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
