namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core;

    #endregion NameSpaces

    public abstract class ChoBaseSurrogateValidator : IChoSurrogateValidator
    {
        #region Instance Members (Public)

        public abstract void Validate(MemberInfo memberInfo, object value);
        public virtual void Validate(MemberInfo memberInfo, object value, ChoValidationResults validationResults)
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
