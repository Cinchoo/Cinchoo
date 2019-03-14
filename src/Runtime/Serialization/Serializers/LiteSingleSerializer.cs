#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSingleSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteSingleSerializer() : base(typeof(Single))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadSingle();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Single)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
