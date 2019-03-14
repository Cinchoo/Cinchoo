namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoConsoleException : ApplicationException
    {
        public ChoConsoleException()
            : base()
        {
        }

        public ChoConsoleException(string message)
            : base(message)
        {
        }

        public ChoConsoleException(string message, Exception e)
            : base(message, e)
        {
        }

        protected ChoConsoleException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
