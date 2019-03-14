#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteDecimalSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteDecimalSerializer() : base(typeof(decimal))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadDecimal();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Decimal)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
