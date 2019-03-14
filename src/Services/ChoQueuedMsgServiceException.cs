namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoQueuedMsgServiceException : ChoApplicationException
    {
        public ChoQueuedMsgServiceException()
            : base()
        {
        }

        public ChoQueuedMsgServiceException(string message)
            : base(message)
        {
        }

        public ChoQueuedMsgServiceException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoQueuedMsgServiceException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
