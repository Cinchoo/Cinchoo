namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

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
