#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteDateTimeSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteDateTimeSerializer() : base(typeof(DateTime))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadDateTime();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((DateTime)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
