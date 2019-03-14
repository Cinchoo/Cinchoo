namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;
    
    #endregion NameSpaces

    [Serializable]
    public class ChoTimerServiceException : ChoApplicationException
    {
        public ChoTimerServiceException()
            : base()
        {
        }

        public ChoTimerServiceException(string message)
            : base(message)
        {
        }

        public ChoTimerServiceException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoTimerServiceException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
