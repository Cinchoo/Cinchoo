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
            foreach (IChoSurrogateValidator validator in Validators)
            {
                validator.Validate(memberInfo, value);
                break;
            }
        }

        #endregion Instance Members (Public)
    }
}
