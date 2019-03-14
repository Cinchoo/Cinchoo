namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Xml;
    using System.Xml.XPath;

    #endregion NameSpaces

    internal static class ChoConfigStorageManager
    {
        #region Constants

        private const string CONFIG_STORAGE_TOKEN = "__storage__";

        #endregion Constants

        public static IChoConfigStorage GetDefaultConfigStorage()
        {
            return ChoConfigStorageManagerSettings.Me.GetDefaultConfigStorage();
        }

        public static IChoConfigStorage GetConfigStorage(string configStorageName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(configStorageName, "Config Storage Name");

            return null;
        }

        public static IChoConfigStorage GetConfigStorage(XmlNode node)
        {
            if (node != null)
            {
                XPathNavigator navigator = node.CreateNavigator();

                string configStorageName = (string)navigator.Evaluate(String.Format("string(@{0})", CONFIG_STORAGE_TOKEN));

                if (!String.IsNullOrEmpty(configStorageName))
                    return ChoConfigStorageManagerSettings.Me.GetConfigStorage(configStorageName);
            }

            return null;
        }
    }
}
