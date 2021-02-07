namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Reflection;

    #endregion NameSpaces

    public class ChoOrCompositeValidator : ChoCompositeValidator
    {
        #region Constructors

        public ChoOrCompositeValidator(params IChoSurrogateValidator[] validators)
            : base(validators)
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

        public override void Validate(MemberInfo memberInfo, object value)
        {
            ChoValidationResults validationResults = new ChoValidationResults();
            foreach (IChoSurrogateValidator validator in Validators)
            {
                try
                {
                    validator.Validate(memberInfo, value);
                    return;
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

            if (validationResults.Count > 0)
            {
                if (validationResults.Count == 1)
                    throw new ChoValidationException("Failed to validate {0} member. {1}".FormatString(memberInfo.Name, validationResults.ToString()), validationResults);
                else
                    throw new ChoValidationException("Failed to validate {0} member. Must meet one of the following conditions. {2}{1}".FormatString(memberInfo.Name, 
                        validationResults.ToString().Indent(), Environment.NewLine), validationResults);
            }
        }

        #endregion Instance Members (Public)
    }
}
