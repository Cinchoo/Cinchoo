namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Reflection;

    #endregion NameSpaces

    public class ChoAndCompositeValidator : ChoCompositeValidator
    {
        #region Constructors

        public ChoAndCompositeValidator(params IChoSurrogateValidator[] validators)
            : base(validators)
        {
        }

        #endregion Constructors

        public override void Validate(MemberInfo memberInfo, object value)
        {
            foreach (IChoSurrogateValidator validator in Validators)
            {
                validator.Validate(memberInfo, value);
            }
        }
    }
}
