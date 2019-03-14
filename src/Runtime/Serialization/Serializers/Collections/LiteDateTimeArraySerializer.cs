#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteDateTimeArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteDateTimeArraySerializer() : base(typeof(DateTime[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new DateTime[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			DateTime[] array = (DateTime[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadDateTime();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			DateTime[] array = (DateTime[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
