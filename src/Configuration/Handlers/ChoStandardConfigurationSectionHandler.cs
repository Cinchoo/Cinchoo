namespace Cinchoo.Core.Configuration.Handlers
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
    using Cinchoo.Core.Xml;
    using Cinchoo.Core.Exceptions;
using Cinchoo.Core.Configuration.Sections;

    #endregion NameSpaces

    public abstract class ChoStandardConfigurationSectionHandler : IChoStandardConfigurationSectionHandler
    {
        #region IChoConfigurationSectionHandler Members

        /// <summary>
        ///		Creates a new configuration handler and adds the specified configuration object to the collection.
        /// </summary>
        /// <param name="parent">Composed from the configuration settings in a corresponding parent configuration section.</param>
        /// <param name="context">Provides access to the virtual path for which the configuration section handler computes configuration values. Normally this parameter is reserved and is null.</param>
        /// <param name="section">The XML node that contains the configuration information to be handled. section provides direct access to the XML contents of the configuration section.</param>
        /// <returns></returns>
		public abstract ChoConfigSection Create(XmlNode section, XmlNode[] contextNodes);

        #endregion
    }
}
