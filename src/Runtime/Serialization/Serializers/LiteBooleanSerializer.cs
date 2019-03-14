#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteBooleanSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteBooleanSerializer() : base(typeof(Boolean))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadBoolean();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Boolean)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
