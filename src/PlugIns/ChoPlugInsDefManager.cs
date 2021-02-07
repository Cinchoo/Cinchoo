using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cinchoo.Core;
using System.IO;

namespace Cinchoo.Core
{
    public class ChoPlugInsDefManager
    {
        private const string PLUGIN_TAG_NAME = "plugIns";
        private const string PLUGIN_ROOT_TAG_NAME = "plugInGroups";
        internal const string DEF_PLUGIN_GROUP_NAME = "#DEFAULT#";
        public readonly string PlugInsDefFilePath;
        private Dictionary<string, XElement> _plugInsGroupDict = new Dictionary<string, XElement>();
        public Dictionary<string, XElement> PlugInsGroupDict
        {
            get { return _plugInsGroupDict; }
            private set { _plugInsGroupDict = value; }
        }

        public ChoPlugInsDefManager(string plugInsDefFilePath = null)
        {
            PlugInsDefFilePath = plugInsDefFilePath;
            LoadPlugInsDef();
        }

        private void LoadPlugInsDef()
        {
            if (PlugInsDefFilePath.IsNullOrWhiteSpace() || !File.Exists(PlugInsDefFilePath))
                return;

            Dictionary<string, XElement> plugInsGroupDict = new Dictionary<string, XElement>();
            XDocument doc = XDocument.Load(PlugInsDefFilePath);
            if (doc == null || doc.Root == null) return;
            if (GetNodeName(doc.Root) != PLUGIN_ROOT_TAG_NAME) return;

            StringBuilder defaultPlugInsGroupXml = new StringBuilder();

            foreach (XElement node in doc.Root.Elements())
            {
                if (node.Name.LocalName == PLUGIN_ROOT_TAG_NAME)
                    continue;

                if (node.Name.LocalName == PLUGIN_TAG_NAME)
                {
                    string name = GetName(node);
                    if (name.IsNullOrWhiteSpace()) continue;
                    if (!plugInsGroupDict.ContainsKey(name))
                        plugInsGroupDict.Add(name, node);
                }
                else
                {
                    defaultPlugInsGroupXml.AppendLine(node.ToString());
                }
            }

            if (defaultPlugInsGroupXml.Length > 0)
            {
                XDocument doc1 = XDocument.Parse("<{1}>{0}</{1}>".FormatString(defaultPlugInsGroupXml.ToString(), PLUGIN_TAG_NAME));
                PlugInsGroupDict.Add(DEF_PLUGIN_GROUP_NAME, doc1.Root);
            }

            foreach (string key in plugInsGroupDict.Keys)
                PlugInsGroupDict.Add(key, plugInsGroupDict[key]);
        }

        public bool ContainsPlugInGroup(string plugInGroupName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(plugInGroupName, "PlugInGroupName");

            if (!PlugInsGroupDict.ContainsKey(plugInGroupName))
                return false;
            
            return PlugInsGroupDict[plugInGroupName] != null;
        }

        public void AddOrReplacePlugInGroup(string plugInGroupName, string xml)
        {
            ChoGuard.ArgumentNotNullOrEmpty(plugInGroupName, "PlugInGroupName");

            XDocument doc = XDocument.Parse(xml);
            if (PlugInsGroupDict.ContainsKey(plugInGroupName))
                PlugInsGroupDict[plugInGroupName] = doc.Root;
            else
                PlugInsGroupDict.Add(plugInGroupName, doc.Root);
        }

        public void AddNewPlugInGroup(string plugInGroupName)
        {
            if (plugInGroupName.IsNullOrWhiteSpace()) return;
            if (PlugInsGroupDict.ContainsKey(plugInGroupName)) return;

            Dictionary<string, XElement> _tmpPlugInsGroupDict = null;
            if (plugInGroupName == DEF_PLUGIN_GROUP_NAME)
            {
                _tmpPlugInsGroupDict = _plugInsGroupDict;
                _plugInsGroupDict = new Dictionary<string, XElement>();
            }

            PlugInsGroupDict.Add(plugInGroupName, XDocument.Parse("<{0} name='{1}'></{0}>".FormatString(ChoPlugInsDefManager.PLUGIN_TAG_NAME, plugInGroupName)).Root);
            if (_tmpPlugInsGroupDict != null)
            {
                foreach (string key in _tmpPlugInsGroupDict.Keys)
                    PlugInsGroupDict.Add(key, _tmpPlugInsGroupDict[key]);
            }
        }

        private static string GetName(XElement xmlNode)
        {
            string text = (string)System.Xml.XPath.Extensions.CreateNavigator((XNode)xmlNode).Evaluate("string(@name)");
            if (!text.IsNullOrWhiteSpace())
                return text;
            else
                return null;
        }

        private static string GetNodeName(XElement xmlNode)
        {
            string text = (string)System.Xml.XPath.Extensions.CreateNavigator((XNode)xmlNode).Evaluate("string(@name)");
            if (!text.IsNullOrWhiteSpace())
                return text;
            else
                return xmlNode.Name.LocalName;
        }

        private static void SetName(XElement xmlNode, string name)
        {
            xmlNode.SetAttributeValue("name", name);
        }

        public IEnumerable<ChoPlugInBuilder> GetPlugInBuilders(string plugInGroupName)
        {
            if (!_plugInsGroupDict.ContainsKey(plugInGroupName)) return new ChoPlugInBuilder[] {};

            return ChoPlugInBuilder.Parse(_plugInsGroupDict[plugInGroupName].ToString());
        }

        public string ToXml()
        {
            if (PlugInsGroupDict.Count == 0)
                return "<{0}></{0}>".FormatString(PLUGIN_ROOT_TAG_NAME).IndentXml();

            StringBuilder xml = new StringBuilder();
            foreach (string key in PlugInsGroupDict.Keys)
            {
                if (key == ChoPlugInsDefManager.DEF_PLUGIN_GROUP_NAME) continue;

                SetName(PlugInsGroupDict[key], key);
                xml.AppendLine(PlugInsGroupDict[key].ToString());
            }

            if (PlugInsGroupDict.ContainsKey(DEF_PLUGIN_GROUP_NAME))
            {
                return "<{2}>{0}{1}</{2}>".FormatString(xml.ToString(),
                    PlugInsGroupDict[DEF_PLUGIN_GROUP_NAME] != null ? String.Concat(PlugInsGroupDict[DEF_PLUGIN_GROUP_NAME].Nodes()) : null, PLUGIN_ROOT_TAG_NAME).IndentXml();
            }
            else
            {
                return "<{1}>{0}</{1}>".FormatString(xml.ToString(), PLUGIN_ROOT_TAG_NAME).IndentXml();
            }
        }

        public void Save(string plugInsDefFilePath)
        {
            ChoGuard.ArgumentNotNullOrEmpty(plugInsDefFilePath, "PlugInsDefFilePath");
            File.WriteAllText(plugInsDefFilePath, ToXml());
        }

        public void Save()
        {
            Save(PlugInsDefFilePath);
        }
    }
}