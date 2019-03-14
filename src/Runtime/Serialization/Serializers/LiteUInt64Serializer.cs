#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteUInt64Serializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteUInt64Serializer() : base(typeof(UInt64))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadUInt64();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((UInt64)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
