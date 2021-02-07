namespace Cinchoo.Core.Diagnostics
{
	#region Namespaces

    using Cinchoo.Core.Configuration;
    using System;
    using System.Configuration;

    #endregion

    public class ChoTraceSettings : ConfigurationSection
    {
        #region Shared Data Members (Internal)

        private const string SECTION_NAME = "traceSettings";
        private static readonly object _padLock = new object();
        private static readonly Lazy<ChoTraceSettings> _defaultInstance = new Lazy<ChoTraceSettings>(() =>
        {
            ChoTraceSettings instance = new ChoTraceSettings();
            instance.IndentProfiling = true;

            return instance;
        });
        private static ChoTraceSettings _instance = null;

        #endregion Shared Data Members (Internal)

        #region Instance Data Members (Public)

        [ConfigurationProperty("indentProfiling")]
        public bool IndentProfiling
        {
            get
            {
                return (bool)this["indentProfiling"];
            }
            set
            {
                this["indentProfiling"] = value;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        #endregion Instance Data Members (Public)

        #region Shared Members (Public)

        public static ChoTraceSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = (ChoTraceSettings)ChoConfigurationManager.GetSection(SECTION_NAME);
                        if (_instance == null)
                            _instance = ChoConfigurationManager.GetSection(SECTION_NAME, _defaultInstance.Value);
                    }
                }

                return _instance == null ? _defaultInstance.Value : _instance;
            }
        }

        #endregion IChoMergeable Overrides
    }
}
