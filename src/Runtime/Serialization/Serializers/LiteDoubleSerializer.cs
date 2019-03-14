#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteDoubleSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteDoubleSerializer() : base(typeof(double))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadDouble();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Double)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
