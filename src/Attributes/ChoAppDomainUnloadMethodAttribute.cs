namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoAppDomainUnloadMethodAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private string _description;
        public string Description
        {
            get { return _description; }
        }

        #endregion Instance Data Members (Private)

        public ChoAppDomainUnloadMethodAttribute(string description)
        {
            ChoGuard.ArgumentNotNull(description, "Description");
            _description = description;
        }
    }
}
