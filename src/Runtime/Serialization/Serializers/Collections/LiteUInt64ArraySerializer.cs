#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteUInt64ArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteUInt64ArraySerializer() : base(typeof(UInt64[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new UInt64[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			UInt64[] array = (UInt64[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadUInt64();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			UInt64[] array = (UInt64[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
