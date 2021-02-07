namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    public class ChoApplicationPropertyReplacer : IChoKeyValuePropertyReplacer
    {
        public static readonly ChoApplicationPropertyReplacer Instance = new ChoApplicationPropertyReplacer();

        #region Instance Data Members (Private)

        private readonly Dictionary<string, string> _availPropeties = new Dictionary<string,string>()
            {
                { "APP_DOMAIN_NAME", "Application domain name." },
                { "APP_ENVIRONMENT", "Application environment name." },
                { "APP_BASE_DIR", "Base working application directory." },
                { "APP_CONFIG_DIR", "Application configuration directory." },
                { "APP_LOG_DIR", "Application log directory." },
                { "APP_MODE", "Application run mode." },
                { "ENTRY_ASSEMBLY_FILE_NAME", "Entry assembly file name." },
                { "ENTRY_ASSEMBLY_LOCATION", "Entry assembly file path." },
            };

        #endregion Instance Data Members (Private)

        #region IChoPropertyReplacer Members

        public bool ContainsProperty(string propertyName, object context)
        {
            return _availPropeties.ContainsKey(propertyName);
        }

        public string ReplaceProperty(string propertyName, string format, object context)
        {
            if (String.IsNullOrEmpty(propertyName))
                return propertyName;

            switch (propertyName)
            {
                case "APP_DOMAIN_NAME":
                    return ChoObject.Format(ChoApplication.AppDomainName, format);
                case "APP_ENVIRONMENT":
                    return ChoObject.Format(ChoApplication.AppEnvironment, format);
                case "APP_BASE_DIR":
                    return ChoObject.Format(ChoApplication.ApplicationBaseDirectory, format);
                case "APP_CONFIG_DIR":
                    return ChoObject.Format(ChoApplication.ApplicationConfigDirectory, format);
                case "APP_LOG_DIR":
                    return ChoObject.Format(ChoApplication.ApplicationLogDirectory, format);
                case "APP_MODE":
                    return ChoObject.Format(ChoApplication.ApplicationMode, format);
                case "ENTRY_ASSEMBLY_FILE_NAME":
                    return ChoObject.Format(ChoApplication.EntryAssemblyFileName, format);
                case "ENTRY_ASSEMBLY_LOCATION":
                    return ChoObject.Format(ChoApplication.EntryAssemblyLocation, format);
            }

            return propertyName;
        }

        #endregion

        #region IChoPropertyReplacer Members

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get
            {
                foreach (KeyValuePair<string, string> keyValue in _availPropeties)
                    yield return keyValue;
            }
        }

        public string Name
        {
            get { return this.GetType().FullName; }
        }

        public string GetPropertyDescription(string propertyName)
        {
            if (_availPropeties.ContainsKey(propertyName))
                return _availPropeties[propertyName];
            else
                return null;
        }

        #endregion
    }
}
