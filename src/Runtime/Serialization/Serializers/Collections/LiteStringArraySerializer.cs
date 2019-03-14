#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteStringArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteStringArraySerializer() : base(typeof(String[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new String[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			String[] array = (String[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadString();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			String[] array = (String[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
