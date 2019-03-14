namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoKeyValuePropertyReplacer : IChoPropertyReplacer
    {
        bool ContainsProperty(string propertyName);
        string ReplaceProperty(string propertyName, string format);
        string GetPropertyDescription(string propertyName);
    }
}
