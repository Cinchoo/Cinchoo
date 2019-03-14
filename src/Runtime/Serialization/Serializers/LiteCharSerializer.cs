#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteCharSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteCharSerializer() : base(typeof(char))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadChar();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Char)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
