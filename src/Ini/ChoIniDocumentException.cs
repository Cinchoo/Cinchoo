namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;
    
    #endregion NameSpaces

    [Serializable]
    public class ChoIniDocumentException : ChoApplicationException
    {
        public ChoIniDocumentException()
            : base()
        {
        }

        public ChoIniDocumentException(string message)
            : base(message)
        {
        }

        public ChoIniDocumentException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoIniDocumentException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
