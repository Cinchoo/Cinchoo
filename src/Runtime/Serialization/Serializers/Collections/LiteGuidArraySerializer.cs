#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteGuidArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteGuidArraySerializer() : base(typeof(Guid[]))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return new Guid[length];
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Guid[] array = (Guid[])obj;
			for (int i = 0; i < array.Length; i++)
				array[i] = reader.ReadGuid();
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Guid[] array = (Guid[])obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.Write(array[i]);
		}

		#endregion
	}
}
