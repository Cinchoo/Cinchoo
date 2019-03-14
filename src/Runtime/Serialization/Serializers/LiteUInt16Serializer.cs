#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteUInt16Serializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteUInt16Serializer() : base(typeof(UInt16))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadUInt16();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((UInt16)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
