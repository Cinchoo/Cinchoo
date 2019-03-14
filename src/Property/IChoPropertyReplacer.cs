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

    public interface IChoPropertyReplacer
    {
        string Name
        {
            get;
        }
        IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get;
        }
    }
}
