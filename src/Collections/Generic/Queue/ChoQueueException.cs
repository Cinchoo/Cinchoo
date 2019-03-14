namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
    using System.Runtime.Serialization;

	#endregion NameSpaces

	[Serializable]
	public class ChoQueueException : ChoApplicationException
	{
		public ChoQueueException()
			: base()
		{
		}

		public ChoQueueException(string message)
			: base(message)
		{
		}

		public ChoQueueException(string message, Exception e)
			: base(message, e)
		{
		}

		protected ChoQueueException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}
	}
}
