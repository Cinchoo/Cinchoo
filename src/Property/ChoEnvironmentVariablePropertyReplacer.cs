﻿namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    public class ChoEnvironmentVariablePropertyReplacer : IChoKeyValuePropertyReplacer
    {
        public static readonly ChoEnvironmentVariablePropertyReplacer Instance = new ChoEnvironmentVariablePropertyReplacer();

        #region Instance Data Members (Private)

        private readonly Dictionary<string, string> _availPropeties = new Dictionary<string,string>()
            {
                { "CURRENT_DIRECTORY", "Fully qualified path of the current working directory." },
                { "MACHINE_NAME", "The NetBIOS name of this local computer." },
                { "OS_VERSION", "Current platform identifier and version number." },
                { "PROCESSOR_COUNT", "Number of processors on the current machine." },
                { "SYSTEM_DIRECTORY", "Fully qualified path of the system directory." },
                { "SYSTEM_PAGE_SIZE", "Application Current Directory" },
                { "TICK_COUNT", "Number of milliseconds elapsed since the system started." },
                { "USER_DOMAIN_NAME", "Network domain name associated with the current user." },
                { "USER_NAME", "User name of the person who is currently logged on to the Windows OS." },
                { "VERSION", "Version numbers of the common language runtime." },
                { "WORKING_SET", "Amount of physical memory mapped to the process context." }
            };

        #endregion Instance Data Members (Private)

        #region IChoPropertyReplacer Members

        public bool ContainsProperty(string propertyName)
        {
            if (_availPropeties.ContainsKey(propertyName))
                return true;
            else
            {
                try
                {
                    if (!propertyName.IsNullOrWhiteSpace())
                    {
                        Environment.SpecialFolder specialFolder;
                        if (Enum.TryParse<Environment.SpecialFolder>(propertyName, out specialFolder))
                            return true;
                        else
                            return !Environment.GetEnvironmentVariable(propertyName).IsNullOrWhiteSpace();
                    }
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string ReplaceProperty(string propertyName, string format)
        {
            if (String.IsNullOrEmpty(propertyName))
                return propertyName;

            switch (propertyName)
            {
                case "CURRENT_DIRECTORY":
                    return ChoObject.Format(Environment.CurrentDirectory, format);
                case "MACHINE_NAME":
                    return ChoObject.Format(Environment.MachineName, format);
                case "OS_VERSION":
                    return ChoObject.Format(Environment.OSVersion, format);
                case "PROCESSOR_COUNT":
                    return ChoObject.Format(Environment.ProcessorCount, format);
                case "SYSTEM_DIRECTORY":
                    return ChoObject.Format(Environment.SystemDirectory, format);
                case "SYSTEM_PAGE_SIZE":
                    return ChoObject.Format(Environment.SystemPageSize, format);
                case "TICK_COUNT":
                    return ChoObject.Format(Environment.TickCount, format);
                case "USER_DOMAIN_NAME":
                    return ChoObject.Format(Environment.UserDomainName, format);
                case "USER_NAME":
                    return ChoObject.Format(Environment.UserName, format);
                case "VERSION":
                    return ChoObject.Format(Environment.Version, format);
                case "WORKING_SET":
                    return ChoObject.Format(Environment.WorkingSet, format);
                default:
                    {
                        Environment.SpecialFolder specialFolder;
                        if (Enum.TryParse<Environment.SpecialFolder>(propertyName, out specialFolder))
                            return ChoObject.Format(Environment.GetFolderPath(specialFolder), format);
                        else
                            return ChoObject.Format(Environment.GetEnvironmentVariable(propertyName), format);
                    }
            }
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
