#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteObjectArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteObjectArraySerializer() : base(typeof(object[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new object[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			object[] array = (object[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadObject();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			object[] array = (object[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.WriteObject(array[i]);
		}

		#endregion
	}
}
