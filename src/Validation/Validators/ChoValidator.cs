namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion

    // Summary:
    //     Acts as a base class for deriving a validation class so that a value of an
    //     object can be verified.
    public abstract class ChoValidator
    {
        private bool _negated = false;
        public bool Negated
        {
            get { return _negated; }
            set { _negated = value; }
        }

        private bool _forceValidate = false;
        public bool ForceValidate
        {
            get { return _forceValidate; }
            set { _forceValidate = value; }
        }

        // Summary:
        //     Initializes an instance of the System.Configuration.ConfigurationValidatorBase
        //     class.
        //protected ChoValidatorBase() { }
        protected ChoValidator(bool negated)
        {
            Negated = negated;
        }

        protected abstract string GetErrMsg();

        // Summary:
        //     Determines whether an object can be validated based on type.
        //
        // Parameters:
        //   type:
        //     The object type.
        //
        // Returns:
        //     true if the type parameter value matches the expected type; otherwise, false.
        public virtual bool CanValidate(Type type)
        {
            return true;
        }

        //
        // Summary:
        //     Determines whether the value of an object is valid.
        //
        // Parameters:
        //   value:
        //     The object value.
        public abstract void Validate(object value);
        public virtual void Validate(object value, ChoValidationResults validationResults)
        {
            ChoGuard.ArgumentNotNull(validationResults, "ValidationResults");

            try
            {
                Validate(value);
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                validationResults.AddResult(new ChoValidationResult(ex.Message));
            }
        }
    }


    // Summary:
    //     Acts as a base class for deriving a validation class so that a value of an
    //     object can be verified.
    public abstract class ChoValidatorGeneric<T> : ChoValidator
    {
        protected ChoValidatorGeneric(bool negated) : base(negated)
        {
        }

        public abstract void Validate(T objectToValidate);

        public override void Validate(object value)
        {
            Validate((T)value);
        }
    }
}
