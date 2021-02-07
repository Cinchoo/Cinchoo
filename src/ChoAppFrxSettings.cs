using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using Cinchoo.Core.Configuration;

namespace Cinchoo.Core
{
    public class ChoAppFrxSettings : ConfigurationSection
    {
        #region Shared Data Members (Internal)

        private const string SECTION_NAME = "appFrxSettings";
        private static readonly object _padLock = new object();
        private static readonly Lazy<ChoAppFrxSettings> _defaultInstance = new Lazy<ChoAppFrxSettings>(() =>
        {
            ChoAppFrxSettings instance = new ChoAppFrxSettings();
            instance.AppEnvironment = String.Empty;
            instance.AppFrxFilePath = String.Empty;
            instance.ApplicationHostType = String.Empty;
            instance.SharedEnvironmentConfgiFilePath = String.Empty;
            instance.DoNotShowEnvSelectionWnd = false;
            instance.DisableFrxConfig = false;
            instance.DisableAppConfig = false;
            instance.DisableWriteAppConfig = false;
            instance.DisableMetaDataConfig = false;

            return instance;
        });
        private static ChoAppFrxSettings _instance = null;
        private static bool _isInitialized;

        #endregion Shared Data Members (Internal)

        #region Instance Data Members (Public)

        [ConfigurationProperty("appEnvironment")]
        public string AppEnvironment
        {
            get
            {
                return (string)this["appEnvironment"];
            }
            set
            {
                if ((string)this["appEnvironment"] != value)
                {
                    this["appEnvironment"] = value;

                    if (_isInitialized)
                        ChoEnvironmentSettings.CheckNChangeEnvironment();
                }
            }
        }

        [ConfigurationProperty("sharedEnvironmentConfigFilePath")]
        public string SharedEnvironmentConfgiFilePath
        {
            get
            {
                return (string)this["sharedEnvironmentConfigFilePath"];
            }
            set
            {
                this["sharedEnvironmentConfigFilePath"] = value;
                //if ((string)this["sharedEnvironmentConfigFilePath"] != value)
                //{
                //    this["sharedEnvironmentConfigFilePath"] = value;
                //    //if (_isInitialized)
                //    //    ChoEnvironmentSettings.CheckNEnvironmentChanged();
                //}
            }
        }

        [ConfigurationProperty("appFrxFilePath")]
        public string AppFrxFilePath
        {
            get
            {
                return (string)this["appFrxFilePath"];
            }
            set
            {
                this["appFrxFilePath"] = value;
                //if ((string)this["appFrxFilePath"] != value)
                //{
                //    this["appFrxFilePath"] = value;
                //    //if (_isInitialized)
                //    //    ChoEnvironmentSettings.CheckNEnvironmentChanged();
                //}
            }
        }

        [ConfigurationProperty("doNotShowEnvSelectionWnd")]
        public bool DoNotShowEnvSelectionWnd
        {
            get
            {
                return (bool)this["doNotShowEnvSelectionWnd"];
            }
            set
            {
                this["doNotShowEnvSelectionWnd"] = value;
            }
        }

        [ConfigurationProperty("applicationHostType")]
        public string ApplicationHostType
        {
            get
            {
                return (string)this["applicationHostType"];
            }
            set
            {
                this["applicationHostType"] = value;
            }
        }

        [ConfigurationProperty("disableFrxConfig")]
        public bool DisableFrxConfig
        {
            get
            {
                return (bool)this["disableFrxConfig"];
            }
            set
            {
                this["disableFrxConfig"] = value;
            }
        }

        [ConfigurationProperty("disableAppConfig")]
        public bool DisableAppConfig
        {
            get
            {
                return (bool)this["disableAppConfig"];
            }
            set
            {
                this["disableAppConfig"] = value;
            }
        }

        [ConfigurationProperty("disableWriteAppConfig")]
        public bool DisableWriteAppConfig
        {
            get
            {
                return (bool)this["disableWriteAppConfig"];
            }
            set
            {
                this["disableWriteAppConfig"] = value;
            }
        }

        [ConfigurationProperty("disableMetaDataConfig")]
        public bool DisableMetaDataConfig
        {
            get
            {
                return (bool)this["disableMetaDataConfig"];
            }
            set
            {
                this["disableMetaDataConfig"] = value;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        #endregion Instance Data Members (Public)

        #region Shared Members (Public)

        public static ChoAppFrxSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = (ChoAppFrxSettings)ChoConfigurationManager.GetSection(SECTION_NAME);
                        if (_instance == null)
                        {
                            _instance = _defaultInstance.Value;
                            _instance.Save(SECTION_NAME);
                        }
                        _isInitialized = true;
                        ChoApplication.RaiseAfterAppFrxSettingsLoaded(_instance);
                    }
                }

                return _instance == null ? _defaultInstance.Value : _instance; 
            }
        }

        //public static ChoAppFrxSettings RefreshSection()
        //{
        //    lock (_padLock)
        //    {
        //        _instance = null;
        //        //_isInitialized = false;
        //        ConfigurationManager.RefreshSection(SECTION_NAME);
        //    }
        //    return Me;
        //}

        #endregion Shared Members (Public)
    }
}
