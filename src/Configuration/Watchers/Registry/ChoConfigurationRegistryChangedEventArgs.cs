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
    public class ChoConfigurationRegistryChangedEventArgs : ChoConfigurationChangedEventArgs
    {
        private readonly string _registryKey;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the configuration file, the section name, the old value, and the new value of the changes.</para>
        /// </summary>
        /// <param name="configurationFile"><para>The configuration file where the change occured.</para></param>
        /// <param name="sectionName"><para>The section name of the changes.</para></param>
		public ChoConfigurationRegistryChangedEventArgs(string sectionName, string registryKey, DateTime lastUpdatedTimeStamp)
            : base(sectionName, lastUpdatedTimeStamp)
        {
			_registryKey = registryKey;
        }

        /// <summary>
        /// <para>Gets the configuration file of the data that is changing.</para>
        /// </summary>
        /// <value>
        /// <para>The configuration file of the data that is changing.</para>
        /// </value>
		public string RegistryKey
        {
			get { return _registryKey; }
        }

        #region Object Overrides

        public override string ToString()
        {
			return String.Format("SectionName: {0}, Type: {1}, RegistryKey: {2}", this.SectionName, typeof(ChoConfigurationRegistryChangedEventArgs).FullName, this.RegistryKey);
        }

        #endregion Object Overrides
    }
}
