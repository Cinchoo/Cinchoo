namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoConfigurationValidatorBaseSurrogateValidator : IChoSurrogateValidator
    {
        #region Instance Data Members (Private)

        private readonly ConfigurationValidatorBase _validator;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoConfigurationValidatorBaseSurrogateValidator(ConfigurationValidatorBase validator)
        {
            ChoGuard.ArgumentNotNull(validator, "Validator");
            _validator = validator;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public ConfigurationValidatorBase Validator
        {
            get { return _validator; }
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Public)

        public void Validate(MemberInfo memberInfo, object value)
        {
            if (value == null || _validator.CanValidate(value.GetType()))
                _validator.Validate(value);
        }

        public void Validate(MemberInfo memberInfo, object value, ChoValidationResults validationResults)
        {
            try
            {
                Validate(memberInfo, value);
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
