namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoSurrogateValidator
    {
        void Validate(MemberInfo memberInfo, object value);
        void Validate(MemberInfo memberInfo, object value, ChoValidationResults validationResults);
    }
}
