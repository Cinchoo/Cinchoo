namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoThreadAbortException : ChoApplicationException
    {
        public ChoThreadAbortException()
            : base()
        {
        }

        public ChoThreadAbortException(string message)
            : base(message)
        {
        }

        public ChoThreadAbortException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoThreadAbortException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
