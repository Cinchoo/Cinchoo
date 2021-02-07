using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace System.Xml.Linq
{
    public static class XElementEx
    {
        public static T ToObject<T>(this XElement node)
        {
            if (node == null)
                throw new ArgumentNullException("XmlNode");

            //XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(T) }).GetNValue(0);
            return (T)serializer.Deserialize(node.CreateReader());
        }

        public static object ToObject(this XElement node, Type type)
        {
            return ToObject(node, type, null);
        }

        public static object ToObject(this XElement node, Type type, XmlAttributeOverrides overrides)
        {
            if (node == null)
                throw new ArgumentNullException("XmlNode");

            if (type == null)
                throw new ArgumentException("Type");

            XmlSerializer serializer = overrides != null ? new XmlSerializer(type, overrides) : XmlSerializer.FromTypes(new[] { type }).GetNValue(0);
            return serializer.Deserialize(node.CreateReader());
        }

        public static bool IsSkipped(this XElement xmlNode)
        {
            XPathNavigator navigator = xmlNode.CreateNavigator();

            return String.Compare(Boolean.TrueString, (string)navigator.Evaluate("string(@skip)"), true) == 0;
        }

        public static string GetNodeName(this XElement xmlNode)
        {
            //XPathNavigator navigator = xmlNode.CreateNavigator();

            //string nodeName = (string)navigator.Evaluate("string(@name)");
            //return nodeName.IsNullOrWhiteSpace() ? xmlNode.Name.LocalName : nodeName;
            XAttribute attr = xmlNode.Attribute("name");
            return attr != null && !attr.Value.IsNullOrWhiteSpace() ? attr.Value : xmlNode.Name.LocalName;
        }

        public static string GetName(this XElement xmlNode)
        {
            XAttribute attr = xmlNode.Attribute("name");
            return attr != null ? attr.Value : null;
        }
    }
}
