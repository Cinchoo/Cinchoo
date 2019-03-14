#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteByteSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteByteSerializer() : base(typeof(Byte))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadByte();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Byte)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
