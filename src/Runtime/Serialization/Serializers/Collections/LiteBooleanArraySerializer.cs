#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteBooleanArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteBooleanArraySerializer() : base(typeof(Boolean[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Boolean[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Boolean[] array = (Boolean[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadBoolean();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Boolean[] array = (Boolean[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
