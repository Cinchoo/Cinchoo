#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteInt16Serializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteInt16Serializer() : base(typeof(Int16))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadInt16();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Int16)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
