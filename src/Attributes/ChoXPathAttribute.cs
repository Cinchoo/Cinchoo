namespace Cinchoo.Core.Attributes
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoXPathAttribute : Attribute
    {
        public readonly string XPath;

        public ChoXPathAttribute(string xpath)
        {
            ChoGuard.ArgumentNotNullOrEmpty(xpath, "XPath");
            XPath = xpath;
        }
    }
}
