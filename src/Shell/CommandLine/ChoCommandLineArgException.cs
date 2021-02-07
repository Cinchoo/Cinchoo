namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    [ChoDoNotShowStackTrace]
    public class ChoCommandLineArgException : ApplicationException
    {
        public ChoCommandLineArgException()
            : base()
        {
        }

        public ChoCommandLineArgException(string message, string usageMessage)
            : base("{1}{0}{0}{2}".FormatString(Environment.NewLine, message, usageMessage))
        {
            ErrorMessage = message;
            UsageMessage = usageMessage;
        }

        public ChoCommandLineArgException(string message, string usageMessage, Exception e)
            : base("{1}{0}{0}{2}".FormatString(Environment.NewLine, message, usageMessage), e)
        {
            ErrorMessage = message;
            UsageMessage = usageMessage;
        }

        protected ChoCommandLineArgException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        public string UsageMessage
        {
            get;
            private set;
        }
    }
}
