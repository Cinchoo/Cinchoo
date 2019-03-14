namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Xml;
	using System.Text;
	using System.Xml.XPath;
	using System.Globalization;
	using System.Xml.Serialization;
	using System.Collections.Generic;

	using Cinchoo.Core.Properties;
	using System.Text.RegularExpressions;
	using Cinchoo.Core.Xml;
	using System.Diagnostics;

	#endregion NameSpaces

	public static class ChoXmlSerializerConfigHelper
	{
		#region Constants

		private const string ObjectTypeToken = "cinchoo:type";

		#endregion Constants

		public static Type GetType(XmlNode node)
		{
			string typeName;
			return GetType(node, out typeName);
		}

		public static Type GetType(XmlNode node, out string typeName)
		{
			XPathNavigator navigator = node.CreateNavigator();

			typeName = (string)navigator.Evaluate(String.Format("string(@{0})", ObjectTypeToken), ChoXmlDocument.NameSpaceManager);
			Type objectType = null;

			if (typeName == null || typeName.Trim().Length == 0)
				objectType = ChoType.GetTypeFromXmlSectionNode(node);

			if (objectType == null)
				objectType = Type.GetType(typeName);
			if (objectType == null)
				objectType = ChoType.GetType(typeName);

			return objectType;
		}

		public static string GetObjectInnerXmlString(object data)
		{
            string xmlString = data.ToNullNSXml();
			if (xmlString.IsNullOrEmpty())
				return xmlString;
			else
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlString);
				return doc.DocumentElement.InnerXml;
			}
			//Insert the type attribute to the xml
			//xmlString = Regex.Replace(xmlString, @"^\<\w+\s+", String.Format("$&{0}=\"{1}\" ", ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI));
			//xmlString = Regex.Replace(xmlString, @"^\<\w+\s+", String.Format("$&{0}=\"{1}\" {2}=\"{3}\" ", ObjectTypeToken, objectType.AssemblyQualifiedName, ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI));

            //return xmlString;
		}

		public static string GetObjectXmlString(object data)
		{
			string xmlString = data.ToNullNSXml();
			if (xmlString.IsNullOrEmpty()) return xmlString;

			//Insert the type attribute to the xml
			//xmlString = Regex.Replace(xmlString, @"^\<\w+\s+", String.Format("$&{0}=\"{1}\" ", ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI));
			//xmlString = Regex.Replace(xmlString, @"^\<\w+\s+", String.Format("$&{0}=\"{1}\" {2}=\"{3}\" ", ObjectTypeToken, objectType.AssemblyQualifiedName, ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI));

			return xmlString;
		}
	}
}
