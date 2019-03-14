namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Xml;
    using System.IO;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Xml;
    using Cinchoo.Core.Attributes;

    #endregion NameSpaces

    public class ChoXmlFileConfigurationManager
    {
        #region Shared Data Members (Private)

        private XmlDocument _xmlDocument;
        private string _configFilePath;

        #endregion Shared Data Members (Private)

        #region Constructors

        protected ChoXmlFileConfigurationManager()
        {
        }

        public ChoXmlFileConfigurationManager(string configFilePath)
        {
            ChoGuard.ArgumentNotNullOrEmpty(configFilePath, "ConfigFilePath");

            _configFilePath = configFilePath;

            try
            {
                if (File.Exists(_configFilePath))
                {
                    _xmlDocument = (new ChoXmlDocument(_configFilePath)).XmlDocument;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion Constructors

        #region Instance Members (Public)

        public T ToObject<T>()
        {
            if (XmlDocument != null)
                return XmlDocument.ToObject<T>();

            return default(T);
        }

        #endregion Instance Members (Public)

        #region Instance Properties (Public)

        public string ConfigFilePath
        {
            get { return _configFilePath; }
        }

        public XmlDocument XmlDocument
        {
            get { return _xmlDocument != null ? _xmlDocument : null; }
        }

        #endregion Instance Properties (Public)
    }

}
