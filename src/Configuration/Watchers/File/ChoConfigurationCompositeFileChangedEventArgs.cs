namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Properties;
    using System.Diagnostics;

    #endregion NameSpaces

    [Serializable]
    public class ChoConfigurationCompositeFileChangedEventArgs : ChoConfigurationChangedEventArgs
    {
        private readonly string _configurationFile;
        private readonly string[] _configurationIncludeFiles;
        private readonly string[] _modifiedIncludeFiles;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the configuration file, the section name, the old value, and the new value of the changes.</para>
        /// </summary>
        /// <param name="configurationFile"><para>The configuration file where the change occured.</para></param>
        /// <param name="configurationSectionName"><para>The section name of the changes.</para></param>
        public ChoConfigurationCompositeFileChangedEventArgs(string configurationSectionName, string configurationFile, string[] configurationIncludeFiles, string[] modifiedIncludeFiles, DateTime lastUpdatedTimeStamp)
            : base(configurationSectionName, lastUpdatedTimeStamp)
        {
            _configurationFile = configurationFile;
            _configurationIncludeFiles = configurationIncludeFiles;
            _modifiedIncludeFiles = modifiedIncludeFiles;
        }

        /// <summary>
        /// <para>Gets the configuration file of the data that is changing.</para>
        /// </summary>
        /// <value>
        /// <para>The configuration file of the data that is changing.</para>
        /// </value>
        public string ConfigurationFile
        {
            get { return _configurationFile; }
        }

        public string[] ConfigurationIncludeFiles
        {
            get { return _configurationIncludeFiles; }
        }

        public string[] ModifiedIncludeFiles
        {
            get { return _modifiedIncludeFiles; }
        }

        #region Object Overrides

        public override string ToString()
        {
            return String.Format("SectionName: {0}, Type: {1}, ConfigFile: {2}, IncludeFiles: {3}, ModifiedFiles: {4}",
                SectionName, typeof(ChoConfigurationFileChangedEventArgs).FullName, ConfigurationFile, ChoString.Join(ConfigurationIncludeFiles), ChoString.Join(ModifiedIncludeFiles));
        }

        #endregion Object Overrides
    }
}
