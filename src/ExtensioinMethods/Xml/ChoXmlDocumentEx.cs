namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using Cinchoo.Core.Attributes;
    using System.Xml.Serialization;
    using System.Xml;

    #endregion NameSpaces

    public static class ChoXmlDocumentEx
    {
        public static XmlNode MakeXPath(this XmlDocument doc, string xpath)
        {
            return doc.DocumentElement.MakeXPath(doc.DocumentElement as XmlNode, xpath);
        }

        public static T ToObject<T>(this XmlDocument doc)
        {
            Type type = typeof(T);

            string elementName = type.Name;
            XmlNode elementNode = null;

            ChoXPathAttribute xpathAttribute = type.GetCustomAttribute<ChoXPathAttribute>();
            if (xpathAttribute == null)
            {
                XmlRootAttribute rootAttribute = type.GetCustomAttribute<XmlRootAttribute>();
                elementName = rootAttribute != null ? rootAttribute.ElementName : type.Name;
                elementNode = doc.SelectSingleNode("//{0}".FormatString(elementName));
            }
            else
                elementNode = doc.SelectSingleNode(xpathAttribute.XPath);

            if (elementNode != null)
            {
                return elementNode.ToObject<T>();
            }

            return default(T);
        }

        public static bool Exists<T>(this XmlDocument doc)
        {
            Type type = typeof(T);

            string elementName = type.Name;
            XmlNode elementNode = null;

            ChoXPathAttribute xpathAttribute = type.GetCustomAttribute<ChoXPathAttribute>();
            if (xpathAttribute == null)
            {
                XmlRootAttribute rootAttribute = type.GetCustomAttribute<XmlRootAttribute>();
                elementName = rootAttribute != null ? rootAttribute.ElementName : type.Name;
                elementNode = doc.SelectSingleNode("//{0}".FormatString(elementName));
            }
            else
                elementNode = doc.SelectSingleNode(xpathAttribute.XPath);

            return elementNode != null;
        }
    }
}
