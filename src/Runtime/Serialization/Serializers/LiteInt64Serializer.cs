#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteInt64Serializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteInt64Serializer() : base(typeof(Int64))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadInt64();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Int64)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
