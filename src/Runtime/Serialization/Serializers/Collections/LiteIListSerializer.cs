#region NameSpaces

using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteIListSerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteIListSerializer(Type type) : base(type)
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			int length = reader.ReadInt32();
			IList list = (IList)obj;
			for (int i = 0; i < length; i++)
				list.Add(reader.ReadObject());
			return list;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			IList list = (IList)obj;
			writer.Write(list.Count);
			for (int i = 0; i < list.Count; i++)
				writer.WriteObject(list[i]);
		}

		#endregion
	}
}
