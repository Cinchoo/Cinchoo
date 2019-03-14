namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using eSquare.Core.Configuration.Storage;

    #endregion NameSpaces

    public interface IChoConfigSectionable
    {
        string ConfigSectionName
        {
            get;
            set;
        }
        string ErrMsg
        {
            get;
            set;
        }
        IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get;
        }

        object this[string key]
        {
            get;
        }
        void Init(ChoConfigSectionObjectMap configSectionObjectMap);
        void Persist(ChoConfigSectionObjectMap configSectionObjectMap);

        void StartWatching();
        void StopWatching();
    }
}
