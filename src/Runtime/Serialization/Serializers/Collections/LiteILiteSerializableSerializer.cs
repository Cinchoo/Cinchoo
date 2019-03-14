#region NameSpaces

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteILiteSerializableSerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteILiteSerializableSerializer(Type type) : base(type)
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			return FormatterServices.GetUninitializedObject(Type);
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			((ILiteSerializable)obj).Deserialize(reader);
			return obj;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			((ILiteSerializable)obj).Serialize(writer);
		}

		#endregion
	}
}
