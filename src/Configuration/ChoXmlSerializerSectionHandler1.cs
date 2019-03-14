namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Web;
    using System.Xml.XPath;
    using System.Configuration;
    using System.Xml.Serialization;
    using System.Collections.Specialized;

    using eSquare.Core.IO;

    #endregion NameSpaces

    public class ChoXmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        #region Constructors

        public ChoXmlSerializerSectionHandler()
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

        #endregion Instance Members (Public)

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            XPathNavigator navigator = section.CreateNavigator();

            string path = (string)navigator.Evaluate("string(@path)");

            if (path != null && path.Length > 0)
            {
                bool envFriendly = false;
                try
                {
                    envFriendly = Boolean.Parse((string)navigator.Evaluate("string(@envFriendly)"));
                }
                catch { }

                if (envFriendly)
                    path = ChoEnvironmentSettings.ToEnvSpecificConfigFile(path);

                path = RITPath.GetFullPath(path);
                if (!File.Exists(path))
                    throw new ApplicationException(String.Format("{0} not exists.", path));

                XmlDocument document = new XmlDocument();
                document.Load(path);

                section = document.DocumentElement;
                navigator = section.CreateNavigator();
            }

            string typename = (string)navigator.Evaluate("string(@type)");

            if (typename == null || typename.Trim().Length == 0)
                throw new ApplicationException(String.Format("Missing type attribute in the '{0}' config section.", section.Name));

            Type type = Type.GetType(typename);
            if (type == null)
                type = new RITType().GetType(typename);

            if (type == null)
                throw new ApplicationException(String.Format("Can't find {0} type.", typename));

            RITConfigFileLocator.Add(section.Name, path);

            XmlSerializer serializer = new XmlSerializer(type);

            try
            {
                return serializer.Deserialize(new XmlNodeReader(section));
            }
            catch
            {
                if (section.Name != type.Name && section.Name.ToUpper() == type.Name.ToUpper())
                {
                    XmlNode newSection = new XmlDocument().CreateElement(type.Name);
                    newSection.InnerXml = section.InnerXml;
                    section = newSection;
                }
                return serializer.Deserialize(new XmlNodeReader(section));
            }
        }

        #endregion
    }
}
