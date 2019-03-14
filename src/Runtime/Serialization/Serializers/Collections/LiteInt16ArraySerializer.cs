#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteInt16ArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteInt16ArraySerializer() : base(typeof(Int16[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Int16[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Int16[] array = (Int16[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadInt16();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Int16[] array = (Int16[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
