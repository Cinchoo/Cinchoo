namespace Cinchoo.Core.Xml
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using Cinchoo.Core.Attributes;
    using System.Xml.Serialization;

    #endregion NameSpaces

    public static class ChoXmlNodeEx
    {
        public static void SaveAsChild(this XmlNode node, object target)
        {
            if (target == null)
                return;

            Type type = target.GetType();

            string elementName = type.Name;
            XmlNode elementNode = null;

            ChoXPathAttribute xpathAttribute = type.GetCustomAttribute<ChoXPathAttribute>();
            if (xpathAttribute == null)
            {
                XmlRootAttribute rootAttribute = type.GetCustomAttribute<XmlRootAttribute>();
                elementName = rootAttribute != null ? rootAttribute.ElementName : type.Name;
                elementNode = node.SelectSingleNode("/{0}".FormatString(elementName));
            }
            else
                elementNode = node.SelectSingleNode(xpathAttribute.XPath);

            XmlNode configNode = node.MakeXPath(elementName);
            if (configNode != null)
            {
                string configXml = target.ToNullNSXml();
                if (configXml.IsNullOrEmpty())
                    return;

                ChoXmlDocument.SetOuterXml(configNode, configXml);
            }
        }
    }
}
