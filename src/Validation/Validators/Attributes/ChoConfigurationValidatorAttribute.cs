namespace eSquare.Core.Attributes.Validators
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Configuration;
    using System.Collections.Generic;

    using eSquare.Core.Resources;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ChoValidatorAttribute : ChoMemberAttribute
    {
        private Type _validator;

        protected ChoValidatorAttribute()
        {
        }

        public ChoValidatorAttribute(Type validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");
            if (!typeof(ChoValidatorBase).IsAssignableFrom(validator))
                throw new ArgumentException("Validator object should be type of ChoValidatorBase.");
            
            _validator = validator;
        }

        private bool _ignoreValidateType;
        public bool IgnoreValidateType
        {
            get { return _ignoreValidateType; }
            set { _ignoreValidateType = value; }
        }

        #region IChoBeforeMemberCallAttribute Members

        // Summary:
        //     Gets the validator attribute instance.
        //
        // Returns:
        //     The current System.Configuration.ConfigurationValidatorBase.
        public virtual object ValidatorInstance 
        { 
            get 
            {
                return (ChoValidatorBase)ChoType.CreateInstanceWithReflectionPermission(_validator);
            } 
        }

        public Type ValidatorType
        {
            get { return _validator; }
        }

        public override void Validate(object value, bool silent)
        {
            if (ValidatorInstance is ChoValidatorBase)
            {
                ChoValidatorBase validatorInstance = ValidatorInstance as ChoValidatorBase;
                if (validatorInstance != null &&
                    !silent
                    && (value != null && validatorInstance.CanValidate(value.GetType())))
                    validatorInstance.Validate(value);
            }
            else if (ValidatorInstance is ConfigurationValidatorBase)
            {
                ConfigurationValidatorBase validatorInstance = ValidatorInstance as ConfigurationValidatorBase;
                if (validatorInstance != null &&
                    !silent
                    && (value != null && validatorInstance.CanValidate(value.GetType())))
                    validatorInstance.Validate(value);
            }
        }

        #endregion
    }
}
