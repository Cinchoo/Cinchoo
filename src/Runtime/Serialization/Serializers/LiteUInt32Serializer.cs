#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteUInt32Serializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteUInt32Serializer() : base(typeof(UInt32))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadUInt32();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((UInt32)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
