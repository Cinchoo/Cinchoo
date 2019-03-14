#region NameSpaces

using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteDictionarySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteDictionarySerializer(Type type) : base(type)
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			int length = reader.ReadInt32();
			IDictionary dict = (IDictionary)obj;
			for (int i = 0; i < length; i++)
			{
				object key = reader.ReadObject();
				object value = reader.ReadObject();
				dict.Add(key, value);
			}
			return dict;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			IDictionary dict = (IDictionary)obj;
			writer.Write(dict.Count);
			for (IDictionaryEnumerator i = dict.GetEnumerator(); i.MoveNext(); )
			{
				writer.WriteObject(i.Key);
				writer.WriteObject(i.Value);
			}
		}

		#endregion
	}
}
