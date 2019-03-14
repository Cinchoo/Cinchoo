#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSByteSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteSByteSerializer() : base(typeof(SByte))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadSByte();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((SByte)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
