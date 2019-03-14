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
        string ErrMsg
        {
            get;
            set;
        }
        IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get;
        }

        Hashtable ToHashtable();
        void Persist(ChoConfigSectionObjectMap configSectionObjectMap);

        void StartWatching();
        void StopWatching();
    }
}
