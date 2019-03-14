namespace System.Collections.Specialized
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public static class ChoNameValueCollectionEx
    {
        public static string ToXml(this NameValueCollection nameValueCollection, string rootElementName)
        {
            if (nameValueCollection == null)
                return null;
            if (rootElementName.IsNullOrEmpty())
                throw new ArgumentException("RootElementName");

            StringBuilder xmlString = new StringBuilder(String.Format("<{0}>", rootElementName));
            foreach (string key in nameValueCollection.AllKeys)
                xmlString.AppendFormat("<add key=\"{0}\" value=\"{1}\" />", key, nameValueCollection[key]);
            xmlString.Append(String.Format("</{0}>", rootElementName));

            return xmlString.ToString();
        }

        public static string ToSingleTagXml(this NameValueCollection nameValueCollection, string rootElementName)
        {
            if (nameValueCollection == null)
                return null;
            if (rootElementName.IsNullOrEmpty())
                throw new ArgumentException("RootElementName");

            StringBuilder xmlString = new StringBuilder(String.Format("<{0} ", rootElementName));
            foreach (string key in nameValueCollection.AllKeys)
                xmlString.AppendFormat("{0}=\"{1}\" ", key, nameValueCollection[key]);
            xmlString.Append("/>");

            return xmlString.ToString();
        }
    }
}
