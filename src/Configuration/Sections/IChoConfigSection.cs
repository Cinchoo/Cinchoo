namespace Cinchoo.Core.Configuration.Sections
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Cinchoo.Core.Configuration.Storage;
    using System.Xml;
	using Cinchoo.Core.Configuration.MetaData;

    #endregion NameSpaces

    public interface IChoConfigSection
    {
        object ConfigData
        {
            get;
        }

		object ConfigObject
        {
            get;
            set;
        }

        object this[string key]
        {
            get;
        }

		bool HasConfigData
		{
			get;
		}

		ChoBaseConfigurationMetaDataInfo MetaDataInfo
		{
			get;
		}

		object PersistableState
		{
			get;
		}

		object SyncRoot
		{
			get;
		}

        void Persist(string configSectionFullPath);
        void StartWatching();
        void StopWatching();
        void RestartWatching();
        void ResetWatching();
        void SetWatcher(ChoConfigurationChangedEventHandler configurationChangedEventHandler);
    }
}
