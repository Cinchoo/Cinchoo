namespace Cinchoo.Core.Attributes
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoStringToObjectConverter : Attribute
    {
    }
}
