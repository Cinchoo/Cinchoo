namespace eSquare.Core.Attributes
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using eSquare.Core.Properties;
    using eSquare.Core.Exceptions;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Configuration;
    using eSquare.Core.Collections.Specialized;
    using eSquare.Core.Configuration.Sections;

    #endregion NameSpaces

    #region ChoXmlSerializationConfigurationElementMapAttribute Class

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ChoXmlSerializationConfigurationElementMapAttribute : ChoConfigurationElementMapAttribute
    {
        #region Instance Data Members (Private)

        private IChoConfigSectionable _configSection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="ChoXmlSerializationConfigurationElementMapAttribute"/> class with the configuration element name.
        /// </summary>
        /// <param name="configElementName">The <see cref="string"/> configuration element name.</param>
        public ChoXmlSerializationConfigurationElementMapAttribute(string configElementName)
        {
            ConfigElementName = configElementName;
        }

        #endregion

        #region IChoConfigurationElementMap Members

        public override void GetConfig(bool refresh, ChoConfigSectionObjectMap configSectionObjectMap)
        {
            _configSection = ChoConfigurationManager.GetConfig(ConfigElementName, refresh) as IChoConfigSectionable;
            if (_configSection != null)
                _configSection.Init(configSectionObjectMap);
        }

        public override void PersistConfig(ChoConfigSectionObjectMap configSectionObjectMap)
        {
            if (_configSection == null) return;

            _configSection.Persist(configSectionObjectMap);
        }

        #endregion
    }

    #endregion ChoXmlSerializationConfigurationElementMapAttribute Class
}
