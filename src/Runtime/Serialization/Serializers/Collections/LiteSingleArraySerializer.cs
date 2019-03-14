#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSingleArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteSingleArraySerializer() : base(typeof(Single[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Single[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Single[] array = (Single[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadSingle();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Single[] array = (Single[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
