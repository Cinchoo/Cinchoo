namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoCommandLineArgException : ApplicationException
    {
        public ChoCommandLineArgException()
            : base()
        {
        }

        public ChoCommandLineArgException(string message)
            : base(message)
        {
        }

        public ChoCommandLineArgException(string message, Exception e)
            : base(message, e)
        {
        }

        protected ChoCommandLineArgException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
