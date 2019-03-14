namespace Cinchoo.Core.Xml
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoXmlDocumentException : ChoApplicationException
    {
        public ChoXmlDocumentException()
            : base()
        {
        }

        public ChoXmlDocumentException(string message)
            : base(message)
        {
        }

        public ChoXmlDocumentException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoXmlDocumentException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
