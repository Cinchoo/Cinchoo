namespace eSquare.Core.Configuration
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

    using eSquare.Core.IO;
    using eSquare.Core.Properties;

    #endregion NameSpaces

    public static class ChoConfigFilesMapper
    {
        #region Shared Data Members

        private static StringDictionary _files = new StringDictionary();

        #endregion

        #region Shared Members (Public)

        public static string GetFullPath(string name, string path)
        {
            if (path == null || path.Trim().Length == 0) return path;

            path = path.Trim();

            if (_files[name] != null)
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_files[name]), path);
            else
                return ChoPath.GetFullPath(path);
        }

        public static bool Contains(string name)
        {
            return _files.ContainsKey(name);
        }

        public static void Add(string name, string location)
        {
            if (Contains(name)) return;

            _files.Add(name, location);
        }

        #endregion
    }

    public sealed class ChoXmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        #region Constructors

        public ChoXmlSerializerSectionHandler()
        {
        }

        #endregion Constructors

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

                path = ChoPath.GetFullPath(path);
                if (!File.Exists(path))
                    throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1002, path));

                XmlDocument document = new XmlDocument();
                document.Load(path);

                section = document.DocumentElement;
                navigator = section.CreateNavigator();
            }

            string typename = (string)navigator.Evaluate("string(@type)");

            if (typename == null || typename.Trim().Length == 0)
                throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1003, section.Name));

            Type type = Type.GetType(typename);
            if (type == null)
                type = ChoType.GetType(typename);

            if (type == null)
                throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1004, typename));

            ChoConfigFilesMapper.Add(section.Name, path);

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
