#region NameSpaces

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization.Formatters.Binary
{
	public class LiteBinaryFormatter : IFormatter
	{
		#region Shared Data Members (Private)

		private static LiteBinaryFormatter _default;

		#endregion

		#region Instance Data Members (Private)

		private LiteSerializerFactory _serializerFactory;
		private LiteSerializationContext _serializationContext;

		#endregion

		#region Constructors

		public LiteBinaryFormatter() : this(LiteSerializerFactory.Default)
		{
		}

		public LiteBinaryFormatter(LiteSerializerFactory serializerFactory)
		{
			if (serializerFactory == null)
				throw new ArgumentNullException("serializerFactory");

			_serializerFactory = serializerFactory;
			_serializationContext = new LiteSerializationContext(_serializerFactory);
		}

		#endregion

		#region Shared Properties (Public)

		public static LiteBinaryFormatter Default
		{
			get 
			{ 
				if (_default == null) _default = new LiteBinaryFormatter();
				return _default;
			}
		}

		#endregion

		#region Instance Properties

		public LiteSerializerFactory SerializerFactory
		{
			get { return _serializerFactory; }
		}

		#endregion

		#region IFormatter Members

		public void Serialize(Stream stream, object obj)
		{
			using (LiteBinaryWriter writer = new LiteBinaryWriter(stream, Encoding.Default, _serializationContext))
			{
				writer.WriteObject(obj);
			}
		}

		public void SerializeAs(Stream stream, object obj, Type type)
		{
			using (LiteBinaryWriter writer = new LiteBinaryWriter(stream, Encoding.Default, _serializationContext))
			{
				writer.WriteObjectAs(type, obj);
			}
		}

		public SerializationBinder Binder
		{
			get { return null; }
			set { }
		}

		public StreamingContext Context
		{
			get { return new StreamingContext (); }
			set { }
		}

		public ISurrogateSelector SurrogateSelector
		{
			get { return null; }
			set { }
		}

		public object Deserialize(Stream stream)
		{
			using (LiteBinaryReader reader = new LiteBinaryReader(stream, Encoding.Default, _serializationContext))
			{
				return reader.ReadObject();
			}
		}

		public object DeserializeAs(Stream stream, Type type)
		{
			using (LiteBinaryReader reader = new LiteBinaryReader(stream, Encoding.Default, _serializationContext))
			{
				return reader.ReadObjectAs(type);
			}
		}

		#endregion

		#region Other Instance Members (Public)

		public byte[] ToByteArray(object obj)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		public object FromByteArray(byte[] buffer)
		{
			if (buffer == null) return null;

			using (MemoryStream stream = new MemoryStream(buffer))
			{
				return Deserialize(stream);
			}
		}

		#endregion
	}
}
