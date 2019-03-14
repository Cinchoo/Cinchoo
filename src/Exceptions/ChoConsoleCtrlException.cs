namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoConsoleCtrlException : ChoApplicationException
    {
        public ChoConsoleCtrlException()
            : base()
        {
        }

        public ChoConsoleCtrlException(string message)
            : base(message)
        {
        }

        public ChoConsoleCtrlException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoConsoleCtrlException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
