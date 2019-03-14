namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ChoIgnoreCompareAttribute : Attribute
    {
    }
}
