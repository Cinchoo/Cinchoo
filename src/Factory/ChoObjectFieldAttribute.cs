namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoObjectFieldAttribute : ChoObjectMemberAttribute
    {
    }
}
