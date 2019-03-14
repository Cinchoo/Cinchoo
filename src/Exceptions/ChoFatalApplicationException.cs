namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoFatalApplicationException : ChoApplicationException
    {
        public ChoFatalApplicationException()
            : base()
        {
        }

        public ChoFatalApplicationException(string message)
            : base(message)
        {
        }

        public ChoFatalApplicationException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoFatalApplicationException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
