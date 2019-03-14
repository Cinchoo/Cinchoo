namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Text;

    #endregion

    // Summary:
    //     Acts as a base class for deriving a validation class so that a value of an
    //     object can be verified.
    public abstract class ChoValidatorBase : ConfigurationValidatorBase
    {
        private bool _negated = false;
        public bool Negated
        {
            get { return _negated; }
            set { _negated = value; }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        private string _negatedErrorMessage;
        public string NegatedErrorMessage
        {
            get { return _negatedErrorMessage; }
            set { _negatedErrorMessage = value; }
        }

        // Summary:
        //     Initializes an instance of the System.Configuration.ConfigurationValidatorBase
        //     class.
        protected ChoValidatorBase() { }

        protected string GetErrorMsg()
        {
            return _negated ? _negatedErrorMessage : _errorMessage;
        }
    }
}
