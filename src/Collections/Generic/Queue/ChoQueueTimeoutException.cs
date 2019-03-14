namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
    using System.Runtime.Serialization;

	#endregion NameSpaces

	[Serializable]
	public class ChoQueueTimeoutException : ChoApplicationException
	{
		public ChoQueueTimeoutException()
			: base("Queue method timed out on wait.")
		{
		}

		public ChoQueueTimeoutException(string message)
			: base(message)
		{
		}

		public ChoQueueTimeoutException(string message, Exception e)
			: base(message, e)
		{
		}

		protected ChoQueueTimeoutException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}
	}
}
