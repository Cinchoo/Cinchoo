namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    #region ChoWorkItemCancelException Class

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been canceled
    /// </summary>
    [Serializable]
    public class ChoWorkItemCancelException : ApplicationException
    {
        public ChoWorkItemCancelException()
            : base()
        {
        }

        public ChoWorkItemCancelException(string message)
            : base(message)
        {
        }

        public ChoWorkItemCancelException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoWorkItemCancelException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }

    }

    #endregion ChoWorkItemCancelException Class

    #region ChoWorkItemTimeoutException Class

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    [Serializable]
    public class ChoWorkItemTimeoutException : ApplicationException
    {
        public ChoWorkItemTimeoutException()
            : base()
        {
        }

        public ChoWorkItemTimeoutException(string message)
            : base(message)
        {
        }

        public ChoWorkItemTimeoutException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoWorkItemTimeoutException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }

    #endregion ChoWorkItemTimeoutException Class

    #region ChoThreadPoolException Class

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    [Serializable]
    public class ChoThreadPoolException : ApplicationException
    {
        public ChoThreadPoolException()
            : base()
        {
        }

        public ChoThreadPoolException(string message)
            : base(message)
        {
        }

        public ChoThreadPoolException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoThreadPoolException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }

    #endregion ChoThreadPoolException Class
}
