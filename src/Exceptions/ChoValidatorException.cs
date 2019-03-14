namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoValidatorException : ChoApplicationException
    {
        public ChoValidatorException()
            : base()
        {
        }

        public ChoValidatorException(string message)
            : base(message)
        {
        }

        public ChoValidatorException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoValidatorException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
