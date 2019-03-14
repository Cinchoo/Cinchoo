namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Web;
    using System.Xml.XPath;
    using System.Globalization;
    using System.Configuration;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Cinchoo.Core.IO;
    using Cinchoo.Core.Properties;
    using Cinchoo.Core.Xml.Serialization;
    using Cinchoo.Core.Configuration.Sections;

    #endregion NameSpaces

    public sealed class ChoFileXmlSerializerSectionHandler : ChoConfigurationSectionHandler
    {
        public override ChoConfigSection GetSection(XmlNode section)
        {
            return new ChoXmlSerializerSection(section);
        }
    }
}
