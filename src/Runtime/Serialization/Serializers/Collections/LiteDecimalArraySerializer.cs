#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteDecimalArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteDecimalArraySerializer() : base(typeof(Decimal[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Decimal[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Decimal[] array = (Decimal[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadDecimal();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Decimal[] array = (Decimal[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
