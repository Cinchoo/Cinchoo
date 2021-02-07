namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;

    #endregion NameSpaces

    [ChoValidationManager]
    public sealed class ChoValidationManager : IChoValidationManager
    {
        #region IChoValidationManager Members

        public bool IsValid(Attribute attribute, out string validatorName)
        {
            validatorName = null;
            bool retValue = attribute is ChoValidatorAttribute 
                || attribute is ConfigurationValidatorAttribute
                || attribute is ValidationAttribute;

            if (attribute is ChoValidatorAttribute)
                validatorName = ((ChoValidatorAttribute)attribute).Name;
            return retValue;
        }

        public IChoSurrogateValidator CreateValidator(Attribute attribute, ValidationScope validationScope, ValidatorSource validatorSource)
        {
            if (attribute == null)
                return null;

            if (attribute is ChoValidatorAttribute)
            {
                object validator = ((ChoValidatorAttribute)attribute).ValidatorInstance;

                ChoGuard.NotNull(validator, "Validator");

                if (validator is ChoValidator)
                    return new ChoSurrogateValidator(validator as ChoValidator);
                else if (validator is ConfigurationValidatorBase)
                    return new ChoConfigurationValidatorBaseSurrogateValidator(validator as ConfigurationValidatorBase);
                else
                    throw new ChoValidationException(String.Format("Validator of type '{0}' is not supported by this manager.", validator.GetType().FullName));
            }
            else if (attribute is ValidationAttribute)
            {
                return new ChoValidationAttributeSurrogateValidator(attribute as ValidationAttribute);
            }
            else if (attribute is ConfigurationValidatorAttribute)
            {
                return new ChoConfigurationValidatorAttributeSurrogateValidator(attribute as ConfigurationValidatorAttribute);
            }

            throw new ChoValidationException(String.Format("Invalid validation attribute '{0}' passed and not supported by this manager.", attribute.GetType().FullName));
        }

        #endregion
    }
}
