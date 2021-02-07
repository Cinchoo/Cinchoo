namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoValidationException : ChoApplicationException
    {
        #region Instance Data Members (Private)

        ChoValidationResults _validationResults;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoValidationException()
            : base()
        {
        }

        public ChoValidationException(string message)
            : base(message)
        {
        }

        public ChoValidationException(string message, Exception e)
            : base(message, e)
        {
        }
        
        public ChoValidationException(string message, ChoValidationResults validationResults)
            : this(message, validationResults, null)
        {
        }

        public ChoValidationException(string message, ChoValidationResults validationResults, Exception e)
            : base(message, e)
        {
            _validationResults = validationResults;
        }

		protected ChoValidationException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public ChoValidationResults ValidationResults
        {
            get { return _validationResults; }
        }

        #endregion Instance Properties (Public)

        public override string ToString()
        {
            return "{0}{1}{2}".FormatString(Message, Environment.NewLine, ValidationResults != null && ValidationResults.Count > 0 ? ValidationResults.ToString() : String.Empty);
        }
    }
}
