namespace eSquare.Core.Diagnostics.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using eSquare.Core.Attributes;

    #endregion NameSpaces
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoLogEntrySelfInitizaliableAttribute : Attribute
    {
    }
}
