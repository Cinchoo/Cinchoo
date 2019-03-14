#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteStringSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteStringSerializer() : base(typeof(string))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadString();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((String)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
