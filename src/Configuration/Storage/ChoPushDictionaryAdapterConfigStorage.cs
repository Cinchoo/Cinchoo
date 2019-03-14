namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System.Collections.Generic;
    using System.Xml;
    using Cinchoo.Core.Services;
    using System.Xml.XPath;
    using Cinchoo.Core.Xml;
    using System;
    using System.Collections;

    #endregion NameSpaces

    public sealed class ChoPushDictionaryAdapterConfigStorage : ChoDictionaryAdapterConfigStorage
    {
        public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get
            {
                //Main app configuration
                List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
                _watchers.Add(base.ConfigurationChangeWatcher);

                if (_sectionInfo != null)
                {
                    IChoConfigurationChangeWatcher configurationChangeWatcher = new ChoConfigurationAdapterChangeWatcher(ConfigSectionName, _sectionInfo.ConfigObjectAdapter);
                    _watchers.Add(configurationChangeWatcher);
                }

                return new ChoConfigurationChangeCompositeWatcher(ConfigSectionName, _watchers.ToArray());
            }
        }
    }
}
