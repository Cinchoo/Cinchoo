#region NameSpaces

using System;
using System.Runtime.Serialization;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSerializationException : ChoApplicationException
	{
		public LiteSerializationException() : base()
		{
		}

		public LiteSerializationException(string message) : base(message)
		{
		}

		public LiteSerializationException(string message, Exception e) : base(message, e)
		{
		}

		protected LiteSerializationException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}
	}
}
