namespace eSquare.Core.Diagnostics.Logging.Formatters
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Attributes;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoLogFormatterAttribute : ChoObjectNameableAttribute
    {
        public ChoLogFormatterAttribute(string name)
            : base(name)
        {
        }
    }
}
