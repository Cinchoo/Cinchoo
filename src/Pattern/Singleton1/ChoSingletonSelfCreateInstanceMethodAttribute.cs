namespace Cinchoo.Core.Pattern.Singleton
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoSingletonSelfCreateInstanceMethodAttribute : Attribute
    {
    }
}
