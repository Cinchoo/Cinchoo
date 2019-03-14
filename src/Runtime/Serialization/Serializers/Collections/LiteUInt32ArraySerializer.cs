#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteUInt32ArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteUInt32ArraySerializer() : base(typeof(UInt32[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new UInt32[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			UInt32[] array = (UInt32[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadUInt32();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			UInt32[] array = (UInt32[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
