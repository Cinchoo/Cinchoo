namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    #endregion NameSpaces

    public class ChoValidationAttributeSurrogateValidator : IChoSurrogateValidator
    {
        #region Instance Data Members (Private)

        private readonly ValidationAttribute _validationAttribute;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoValidationAttributeSurrogateValidator(ValidationAttribute validationAttribute)
        {
            ChoGuard.ArgumentNotNull(validationAttribute, "ValidationAttribute");
            _validationAttribute = validationAttribute;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public void Validate(MemberInfo memberInfo, object value)
        {
            ValidationResult result = _validationAttribute.GetValidationResult(value, new ValidationContext(value, null, null));
            if (result == ValidationResult.Success)
                return;
            else
                throw new ValidationException(result.ErrorMessage);
        }

        public void Validate(MemberInfo memberInfo, object value, ChoValidationResults validationResults)
        {
            try
            {
                ValidationResult result = _validationAttribute.GetValidationResult(value, null);
                if (result != ValidationResult.Success)
                    validationResults.AddResult(result);
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

        #endregion
    }
}
