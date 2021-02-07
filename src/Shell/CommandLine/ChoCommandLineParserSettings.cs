namespace Cinchoo.Core.Shell
{
	#region NameSpaces

    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;
    using System.IO;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;
    using System.Configuration;
    using System;
    using System.Linq;

    #endregion NameSpaces

    public class ChoCommandLineParserSettings : ConfigurationSection
    {
        #region Shared Data Members (Internal)

        private const string SECTION_NAME = "commandLineParserSettings";
        private static readonly object _padLock = new object();
        private static readonly Lazy<ChoCommandLineParserSettings> _defaultInstance = new Lazy<ChoCommandLineParserSettings>(() =>
        {
            ChoCommandLineParserSettings instance = new ChoCommandLineParserSettings();
            instance.SwitchCharsText = "/, -";
            instance.ValueSeparatorsText = ":, =";
            instance.UsageSwitchesText = "?, h, help";
            instance.FileArgSwitchesText = "@";
            instance.IgnoreCase = true;
            instance.ShowUsageIfEmpty = false;
            instance.DisplayDefaultValue = false;
            instance.DoNotShowHeader = false;

            return instance;
        });
        private static ChoCommandLineParserSettings _instance = null;

        #endregion Shared Data Members (Internal)

        #region Constructors

        private ChoCommandLineParserSettings()
        {
            IgnoreCase = true;
        }

        #endregion Constructors

        #region Instance Data Members (Public)

        [ConfigurationProperty("switchChars")]
        public string SwitchCharsText
        {
            get { return (string)this["switchChars"]; }
            set { this["switchChars"] = value; }
        }

        public char[] SwitchChars = new char[] { '/', '-' };

        [ConfigurationProperty("valueSeparators")]
        public string ValueSeparatorsText
        {
            get { return (string)this["valueSeparators"]; }
            set { this["valueSeparators"] = value; }
        }

        public char[] ValueSeparators = new char[] { ':', '=' };

        [ConfigurationProperty("usageSwitches")]
        public string UsageSwitchesText
        {
            get { return (string)this["usageSwitches"]; }
            set { this["usageSwitches"] = value; }
        }

        public string[] UsageSwitches = new string[] { "?", "h", "help" };

        [ConfigurationProperty("fileArgSwitches")]
        public string FileArgSwitchesText
        {
            get { return (string)this["fileArgSwitches"]; }
            set { this["fileArgSwitches"] = value; }
        }

        public char[] FileArgSwitches = new char[] { '@' };

        [ConfigurationProperty("ignoreCase")]
        public bool IgnoreCase
        {
            get { return (bool)this["ignoreCase"]; }
            set { this["ignoreCase"] = value; }
        }

        [ConfigurationProperty("showUsageIfEmpty")]
        public bool ShowUsageIfEmpty
        {
            get { return (bool)this["showUsageIfEmpty"]; }
            set { this["showUsageIfEmpty"] = value; }
        }

        [ConfigurationProperty("displayDefaultValue")]
        public bool DisplayDefaultValue
        {
            get { return (bool)this["displayDefaultValue"]; }
            set { this["displayDefaultValue"] = value; }
        }

        [ConfigurationProperty("doNotShowHeader")]
        public bool DoNotShowHeader
        {
            get { return (bool)this["doNotShowHeader"]; }
            set { this["doNotShowHeader"] = value; }
        }

        #endregion Instance Data Members (Public)

        #region Shared Members (Public)

        public static ChoCommandLineParserSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = ChoConfigurationManager.GetSection(SECTION_NAME, _defaultInstance.Value);

                        char[] singleCharUsageSwitches = null;
                        if (_instance != null)
                        {
                            if (!_instance.SwitchCharsText.IsNullOrWhiteSpace())
                            {
                                _instance.SwitchChars = (from token in _instance.SwitchCharsText.SplitNTrim()
                                               where token != null && token.Length == 1
                                               select token[0]).ToArray();

                                if (_instance.SwitchChars == null || _instance.SwitchChars.Length == 0)
                                    throw new ApplicationException("Missing switch characters.");
                            }
                            if (ContainsInvalidChars(_instance.SwitchChars))
                                throw new ApplicationException("Switch characters contains invalid characters.");

                            if (!_instance.ValueSeparatorsText.IsNullOrWhiteSpace())
                            {
                                _instance.ValueSeparators = (from token in _instance.ValueSeparatorsText.SplitNTrim()
                                                   where token != null && token.Length == 1
                                                   select token[0]).ToArray();

                                if (_instance.ValueSeparators == null || _instance.ValueSeparators.Length == 0)
                                    throw new ApplicationException("Missing value separators.");
                            }
                            if (ContainsInvalidChars(_instance.ValueSeparators))
                                throw new ApplicationException("Value separators contains invalid characters.");

                            //Check if valueseparators found in switchchars
                            char[] duplicates = _instance.SwitchChars.Intersect(_instance.ValueSeparators).ToArray();
                            if (duplicates != null && duplicates.Length > 0)
                                throw new ApplicationException("Value separators conflict with Switch characters.");

                            if (!_instance.UsageSwitchesText.IsNullOrWhiteSpace())
                            {
                                _instance.UsageSwitches = (from token in _instance.UsageSwitchesText.SplitNTrim()
                                                 where token != null
                                                 select token).ToArray();

                                if (_instance.UsageSwitches == null || _instance.UsageSwitches.Length == 0)
                                    throw new ApplicationException("Missing usage switches.");
                            }
                            singleCharUsageSwitches = (from token in _instance.UsageSwitches
                                                       where token != null && token.Length == 1
                                                       select token[0]).ToArray();
                            if (ContainsInvalidChars(singleCharUsageSwitches))
                                throw new ApplicationException("Some usage text characters contains invalid characters.");

                            //Check if usage text chars found in switchchars
                            duplicates = _instance.SwitchChars.Intersect(singleCharUsageSwitches).ToArray();
                            if (duplicates != null && duplicates.Length > 0)
                                throw new ApplicationException("Some usage text characters conflict with Switch characters.");

                            //Check if filearg switches found in switchchars
                            duplicates = _instance.ValueSeparators.Intersect(singleCharUsageSwitches).ToArray();
                            if (duplicates != null && duplicates.Length > 0)
                                throw new ApplicationException("Some usage text characters conflict with Value Separators.");

                            if (!_instance.FileArgSwitchesText.IsNullOrWhiteSpace())
                            {
                                _instance.FileArgSwitches = (from token in _instance.FileArgSwitchesText.SplitNTrim()
                                                             where token != null && token.Length == 1
                                                             select token[0]).ToArray();

                                if (_instance.FileArgSwitches == null || _instance.FileArgSwitches.Length == 0)
                                    throw new ApplicationException("Missing file argument switches.");
                            }
                            if (ContainsInvalidChars(_instance.FileArgSwitches))
                                throw new ApplicationException("File argument switches contains invalid characters.");

                            //Check if filearg switches found in switchchars
                            duplicates = _instance.SwitchChars.Intersect(_instance.FileArgSwitches).ToArray();
                            if (duplicates != null && duplicates.Length > 0)
                                throw new ApplicationException("File argument switches conflict with Switch characters.");
                            //Check if filearg switches found in switchchars
                            duplicates = _instance.ValueSeparators.Intersect(_instance.FileArgSwitches).ToArray();
                            if (duplicates != null && duplicates.Length > 0)
                                throw new ApplicationException("File argument switches conflict with Value Separators.");
                            if (singleCharUsageSwitches != null)
                            {
                                duplicates = singleCharUsageSwitches.Intersect(_instance.FileArgSwitches).ToArray();
                                if (duplicates != null && duplicates.Length > 0)
                                    throw new ApplicationException("File argument switches conflict with usage text characters.");
                            }
                        }
                    }
                }

                return _instance == null ? _defaultInstance.Value : _instance;
            }
        }

        private static bool ContainsInvalidChars(char[] chars)
        {
            foreach (char i in chars)
            {
                if (i == ChoChar.BackSpace
                    || i == ChoChar.NUL
                    || i == ChoChar.VerticalTab
                    || i == ChoChar.Choape
                    || i == ChoChar.BackSpace
                    || i == ChoChar.CarriageReturn
                    || i == ChoChar.LineFeed
                    || i == ChoChar.Formfeed
                    || i == ChoChar.BEL
                    || i == ChoChar.Backslash
                    || i == ChoChar.SingleQuotationMark
                    || i == ChoChar.DoubleQuotationMark)
                    return true;
            }

            return false;
        }

        public static ChoCommandLineParserSettings RefreshSection()
        {
            lock (_padLock)
            {
                _instance = null;
                ConfigurationManager.RefreshSection(SECTION_NAME);
            }
            return Me;
        }

        #endregion Shared Members (Public)
    }
}
