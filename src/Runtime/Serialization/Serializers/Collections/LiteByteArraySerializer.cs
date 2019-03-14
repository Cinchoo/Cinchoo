#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteByteArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteByteArraySerializer() : base(typeof(Byte[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Byte[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Byte[] array = (Byte[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadByte();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Byte[] array = (Byte[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
