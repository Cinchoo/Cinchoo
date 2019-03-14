using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Configuration
{
    public class ChoAppFrxSettings : ConfigurationSection
    {
        #region Shared Data Members (Internal)

        internal static readonly ChoAppFrxSettings _defaultInstance = new ChoAppFrxSettings();

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
                this["appEnvironment"] = value;
            }
        }

        [ConfigurationProperty("sharedEnvironmentConfgiFilePath")]
        public string SharedEnvironmentConfgiFilePath
        {
            get
            {
                return (string)this["sharedEnvironmentConfgiFilePath"];
            }
            set
            {
                this["sharedEnvironmentConfgiFilePath"] = value;
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
            }
        }

        #endregion Instance Data Members (Public)

        #region Shared Members (Public)

        public static ChoAppFrxSettings Me
        {
            get
            {
                ChoAppFrxSettings _me = (ChoAppFrxSettings)ConfigurationManager.GetSection("appFrxSettings");
                return _me == null ? _defaultInstance : _me;
            }
        }

        public static ChoAppFrxSettings RefreshSection()
        {
            ConfigurationManager.RefreshSection("appFrxSettings");
            return Me;
        }

        #endregion Shared Members (Public)

    }
}
