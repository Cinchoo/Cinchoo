#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteObjectSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Shared Data Members (Private)

		private static BinaryFormatter _formatter = new BinaryFormatter();

		#endregion

		#region Constructors

		public LiteObjectSerializer(Type type) : base(type)
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			int cookie = reader.ReadInt32();
			object custom = reader.SerializationContext.GetObject(cookie);
			if (custom == null)
			{
				custom = _formatter.Deserialize(reader.BaseStream);
				reader.SerializationContext.CacheObjectForRead(custom);
			}
			return custom;
		}

		public override void Write(LiteBinaryWriter writer, object obj)
		{
			int cookie = writer.SerializationContext.GetCookie(obj);
			if (cookie != LiteSerializationContext.InvalidCookie)
			{
				writer.Write(cookie);
				return;
			}

			cookie = writer.SerializationContext.CacheObjectForWrite(obj);
			writer.Write(cookie);
			_formatter.Serialize(writer.BaseStream, obj);
		}

		#endregion
	}
}
