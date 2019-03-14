#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteInt32Serializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteInt32Serializer() : base(typeof(Int32))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadInt32();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Int32)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
