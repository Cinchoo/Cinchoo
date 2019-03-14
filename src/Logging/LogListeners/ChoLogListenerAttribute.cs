namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoLogListenerAttribute : ChoObjectNameableAttribute
    {
        public ChoLogListenerAttribute(string name)
            : base(name)
        {
        }
    }
}
