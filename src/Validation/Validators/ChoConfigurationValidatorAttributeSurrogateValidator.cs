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

    public class ChoConfigurationValidatorAttributeSurrogateValidator : IChoSurrogateValidator
    {
        #region Instance Data Members (Private)

        private readonly ConfigurationValidatorBase _configurationValidatorBase;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoConfigurationValidatorAttributeSurrogateValidator(ConfigurationValidatorAttribute validationAttribute)
        {
            ChoGuard.ArgumentNotNull(validationAttribute, "ValidationAttribute");
            _configurationValidatorBase = validationAttribute.ValidatorInstance;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public void Validate(MemberInfo memberInfo, object value)
        {
            if (_configurationValidatorBase == null)
                return;

            Type memberType = ChoType.GetMemberType(memberInfo);

            if (_configurationValidatorBase.CanValidate(memberType))
                _configurationValidatorBase.Validate(value);
        }

        public void Validate(MemberInfo memberInfo, object value, ChoValidationResults validationResults)
        {
            try
            {
                if (_configurationValidatorBase == null)
                    return;

                Type memberType = ChoType.GetMemberType(memberInfo);

                if (_configurationValidatorBase.CanValidate(memberType))
                    _configurationValidatorBase.Validate(value);
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
