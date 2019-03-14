#region NameSpaces

using System;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteGuidSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteGuidSerializer() : base(typeof(Guid))
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
			return reader.ReadGuid();
		}
		public override void Write(LiteBinaryWriter writer, object obj)
		{
			writer.Write((Guid)obj);
		}

		#endregion LiteSerializer Overrides
	}
}
