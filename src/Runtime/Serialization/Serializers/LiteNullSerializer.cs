#region NameSpaces

using System;

using Cinchoo.Core.Runtime.Serialization;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public sealed class LiteNullSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteNullSerializer() : base(typeof(LiteNullSerializer))
		{
		}
	
		#endregion
	}
}
