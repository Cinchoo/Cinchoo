namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Specialized;

    using eSquare.Core.IO;
    using eSquare.Core.Exceptions;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Collections.Specialized;

    #endregion

    public class ChoFileNameValueSectionHandler : ChoConfigurationSectionHandler
    {
        /// <summary>
        ///		Creates a new configuration handler and adds the specified configuration object to the collection.
        /// </summary>
        /// <param name="parent">Composed from the configuration settings in a corresponding parent configuration section.</param>
        /// <param name="context">Provides access to the virtual path for which the configuration section handler computes configuration values. Normally this parameter is reserved and is null.</param>
        /// <param name="section">The XML node that contains the configuration information to be handled. section provides direct access to the XML contents of the configuration section.</param>
        /// <returns></returns>
        public override object Create(object parent, object context, XmlNode section)
        {
            ChoFileNameValueCollection nameValues = new ChoFileNameValueCollection(section);
            if (nameValues.ErrMsg == null)
                nameValues.Add(LoadConfigSection(section, nameValues.ConfigPath));

            return nameValues;
        }

        /// <summary>
        /// Gets the name of the key attribute tag. This property is overidden by derived classes to change 
        /// the name of the key attribute tag. The default is "key".
        /// </summary>
        protected virtual string KeyAttributeName
        {
            get
            {
                return "key";
            }
        }

        /// <summary>
        /// Gets the name of the value tag. This property may be overidden by derived classes to change
        /// the name of the value tag. The default is "value".
        /// </summary>
        protected virtual string ValueAttributeName
        {
            get
            {
                return "value";
            }
        }

        private NameValueCollection LoadConfigSection(XmlNode section, string configPath)
        {
            if (configPath != null && configPath.Length > 0 && configPath != ChoConfigurationManager.AppConfigFilePath)
            {
                ConfigXmlDocument doc = new ConfigXmlDocument();
                doc.Load(configPath);

                if (doc.DocumentElement.Name != section.Name)
                    throw new ChoConfigurationException(String.Format("Invalid {0} root element found in {1} config file. Expected {2}",
                        doc.DocumentElement.Name, configPath, section.Name), doc.DocumentElement);

                base.ExpandIncludes(doc.DocumentElement, configPath);

                return ChoConfigurationManager.GetNameValueCollection(null, doc.DocumentElement,
                    KeyAttributeName, ValueAttributeName);
            }
            else
                return ChoConfigurationManager.GetNameValueCollection(null, section,
                        KeyAttributeName, ValueAttributeName);
        }

    }
}
