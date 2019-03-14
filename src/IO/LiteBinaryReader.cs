using System;
using System.IO;
using System.Text;
using Cinchoo.Core.Runtime.Serialization;

namespace Cinchoo.Core.IO
{
	public class LiteBinaryReader : IDisposable
	{
		#region Instance Data Members (Private)

		private LiteSerializationContext _serializationContext;
		private BinaryReader _reader;

		#endregion

		#region Constructors

		public LiteBinaryReader(Stream stream) 
			: this(stream, Encoding.Default, new LiteSerializationContext()) 
		{
		}

		public LiteBinaryReader(Stream stream, Encoding encoding) 
			: this(stream, encoding, new LiteSerializationContext()) 
		{
		}

		public LiteBinaryReader(Stream stream, Encoding encoding, LiteSerializationContext serializationContext) 
		{
			_serializationContext = serializationContext;
			_reader = new BinaryReader(stream, encoding);
		}

		#endregion

		#region Finalizer

		~LiteBinaryReader()
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
			get { return _reader.BaseStream; }
		}

		#endregion

		#region Instance Members (Public)

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public object ReadObject()
		{
			// read type handle
			short handle = _reader.ReadInt16();
			// Find an appropriate surrogate by handle
			return _serializationContext.SerializerFactory.GetSerializerForHandle(handle).Read(this);
		}

		public object ReadObjectAs(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _serializationContext.SerializerFactory.GetSerializerForType(type).Read(this);
		}

		#endregion

		#region /	ReadXXX functions	/

		public bool ReadBoolean() { return _reader.ReadBoolean(); }
		public byte ReadByte() { return _reader.ReadByte(); }
		public byte[] ReadBytes(int count) { return _reader.ReadBytes(count); }
		public char ReadChar() { return _reader.ReadChar(); }
		public char[] ReadChars(int count) { return _reader.ReadChars(count); }
		public decimal ReadDecimal() { return _reader.ReadDecimal(); }
		public float ReadSingle() { return _reader.ReadSingle(); }
		public double ReadDouble() { return _reader.ReadDouble(); }
		public short ReadInt16() { return _reader.ReadInt16(); }
		public int ReadInt32() { return _reader.ReadInt32(); }
		public long ReadInt64() { return _reader.ReadInt64(); }
		public string ReadString() { return _reader.ReadString(); }
		public DateTime ReadDateTime() { return new DateTime(_reader.ReadInt64()); }
		public Guid ReadGuid() { return new Guid(_reader.ReadBytes(16)); }
		public int Read(byte[] buffer, int index, int count) { return _reader.Read(buffer, index, count); }
		public int Read(char[] buffer, int index, int count) { return _reader.Read(buffer, index, count); }
        [CLSCompliant(false)]
		public sbyte ReadSByte() { return _reader.ReadSByte(); }
        [CLSCompliant(false)]
		public ushort ReadUInt16() { return _reader.ReadUInt16(); }
        [CLSCompliant(false)]
		public uint ReadUInt32() { return _reader.ReadUInt32(); }
        [CLSCompliant(false)]
		public ulong ReadUInt64() { return _reader.ReadUInt64(); }

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
