namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoCommandLineArgUsageException : ApplicationException
    {
        public ChoCommandLineArgUsageException()
            : base()
        {
        }

        public ChoCommandLineArgUsageException(string message)
            : base(message)
        {
        }

        public ChoCommandLineArgUsageException(string message, Exception e)
            : base(message, e)
        {
        }

        protected ChoCommandLineArgUsageException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
