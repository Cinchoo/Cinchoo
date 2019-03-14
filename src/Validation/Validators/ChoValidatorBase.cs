namespace eSquare.Core.Validation.Validators
{
    #region NameSpaces

    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Text;
    using eSquare.Core.Exceptions;

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

        // Summary:
        //     Initializes an instance of the System.Configuration.ConfigurationValidatorBase
        //     class.
        //protected ChoValidatorBase() { }
        protected ChoValidatorBase(bool negated)
        {
            Negated = negated;
        }

        protected abstract string GetErrMsg();
    }


    // Summary:
    //     Acts as a base class for deriving a validation class so that a value of an
    //     object can be verified.
    public abstract class ChoValidatorBase<T> : ChoValidatorBase
    {
        protected ChoValidatorBase(bool negated) : base(negated)
        {
        }

        public abstract void Validate(T objectToValidate);

        public override void Validate(object value)
        {
            Validate((T)value);
        }
    }
}
