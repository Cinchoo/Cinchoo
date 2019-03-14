#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteCharArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteCharArraySerializer() : base(typeof(Char[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Char[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Char[] array = (Char[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadChar();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Char[] array = (Char[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
