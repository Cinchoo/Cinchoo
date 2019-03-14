#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSByteArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteSByteArraySerializer() : base(typeof(SByte[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new SByte[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			SByte[] array = (SByte[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadSByte();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			SByte[] array = (SByte[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
