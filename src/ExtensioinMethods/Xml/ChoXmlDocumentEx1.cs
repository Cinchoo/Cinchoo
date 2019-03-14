namespace System.Xml
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoXmlDocumentEx
    {
        #region Constants

        internal const string PathToken = "__path__";
        internal const string IncludeToken = "include";

        #endregion Constants
        
        #region Embedded Include File Loading Members

        #region ExpandIncludes Overloads

        public static string[] ExpandIncludes(this XmlDocument xmlDocument, string baseDirectory)
        {
            List<string> parentFileList = new List<string>();

            return ExpandIncludes(xmlDocument, baseDirectory, parentFileList);
        }

        #endregion ExpandIncludes Overloads

        #region ExpandIncludes Helper methods (Private)

        private static string[] ExpandIncludes(XmlNode section, string baseDirectory, List<string> parentFileList)
        {
            List<string> includeFileList = new List<string>();
            if (section.SelectNodes(String.Format("//{1}[@{0}]", PathToken, IncludeToken)).Count > 0)
            {
                if (!String.IsNullOrEmpty(baseDirectory) && !parentFileList.Contains(baseDirectory) && Path.HasExtension(baseDirectory))
                    parentFileList.Add(baseDirectory);

                if (!String.IsNullOrEmpty(baseDirectory) && Path.HasExtension(baseDirectory))
                    baseDirectory = Path.GetDirectoryName(baseDirectory);

                XmlDocument ownerDocument = section is XmlDocument ? section as XmlDocument : section.OwnerDocument;

                foreach (XmlElement includeElement in section.SelectNodes(String.Format("//{1}[@{0}]", PathToken, IncludeToken)))
                {
                    if (includeElement == null) continue;

                    string includeFileName = includeElement.GetAttribute(PathToken);

                    bool isFirstChild = true;
                    XmlNode lastChild = null;
                    XmlNode parentNode = includeElement.ParentNode;
                    foreach (XmlNode newNode in ExpandIncludeNode(ownerDocument, baseDirectory, parentFileList, includeFileList, includeElement))
                    {
                        if (isFirstChild)
                        {
                            isFirstChild = false;
                            lastChild = newNode;

                            parentNode.ReplaceChild(newNode, includeElement);
                            parentNode.InsertBefore(ownerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - BEGIN INCLUDE {0}", includeFileName))), newNode);
                        }
                        else
                        {
                            parentNode.InsertAfter(newNode, lastChild);
                            lastChild = newNode;
                        }

                        if (newNode.NodeType == XmlNodeType.Element)
                            ExpandIncludes(newNode, baseDirectory, parentFileList);
                    }

                    if (lastChild.Name != IncludeToken)
                        parentNode.InsertAfter(ownerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - END INCLUDE {0}", includeFileName))), lastChild);
                    else
                        parentNode.AppendChild(ownerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - END INCLUDE {0}", includeFileName))));
                }
            }
            return includeFileList.ToArray();
        }

        private static IEnumerable ExpandIncludeNode(XmlDocument ownerDocment, string baseDirectory, List<string> parentFileList, List<string> includeFileList, 
            XmlElement element)
        {
            string includeFileName = element.GetAttribute(PathToken);
            if (String.IsNullOrEmpty(includeFileName)) return new XmlNode[] { };
            if (!String.IsNullOrEmpty(baseDirectory) && !Path.IsPathRooted(includeFileName))
                includeFileName = Path.Combine(baseDirectory, includeFileName);

            if (!String.IsNullOrEmpty(baseDirectory) && IsCircularFileExists(parentFileList, includeFileName))
                throw new XmlException(String.Format("Circular reference encountered on the {0} file.", baseDirectory));

            if (!includeFileList.Contains(includeFileName))
                includeFileList.Add(includeFileName);

            XmlDocumentFragment fragment = ownerDocment.CreateDocumentFragment();

            try
            {
                XmlDocument includeDoc = new XmlDocument();
                includeDoc.Load(includeFileName);

                foreach (XmlAttribute attr in element.Attributes)
                {
                    if (attr.Name != PathToken)
                        includeDoc.DocumentElement.SetAttribute(attr.Name, attr.Value);
                }

                fragment.InnerXml = includeDoc.InnerXml;

                return new XmlNode[] { fragment.SelectSingleNode("/*") };
            }
            catch (XmlException)
            {
                try
                {
                    using (StreamReader sr = File.OpenText(includeFileName))
                        fragment.InnerXml = sr.ReadToEnd();

                    return fragment.SelectNodes("/*|/comment()");
                }
                catch (Exception innerEx)
                {
                    throw new XmlException(String.Format("{0}: Error loading xml file.", includeFileName), innerEx);
                }
            }
            catch (Exception ex)
            {
                throw new XmlException(String.Format("{0}: Error loading xml file.", includeFileName), ex);
            }
        }

        private static bool IsCircularFileExists(List<string> parentFileList, string xmlFilePath)
        {
            return parentFileList.Exists(delegate(string item) { return String.Compare(item, xmlFilePath, true) == 0; });
        }

        #endregion ExpandIncludes Helper methods (Private)

        #endregion Embedded Include File Loading Members
    }
}
