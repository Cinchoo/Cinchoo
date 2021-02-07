namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using Cinchoo.Core.Configuration;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;
    using System.Configuration;

    #endregion NameSpaces

    public class ChoConsoleSettings : ConfigurationSection
    {
        #region Shared Data Members (Internal)

        private const string SECTION_NAME = "consoleSettings";
        private static readonly object _padLock = new object();
        private static readonly Lazy<ChoConsoleSettings> _defaultInstance = new Lazy<ChoConsoleSettings>(() =>
        {
            ChoConsoleSettings instance = new ChoConsoleSettings();
            instance.ForegroundColor = Console.ForegroundColor;
            instance.BackgroundColor = Console.BackgroundColor;
            instance.ConsoleMode = uint.MinValue;

            return instance;
        });
        private static ChoConsoleSettings _instance = null;

        #endregion Shared Data Members (Internal)

        #region Instance Data Members (Public)

        [ConfigurationProperty("foregroundColor")]
        public ConsoleColor ForegroundColor
        {
            get { return (ConsoleColor)this["foregroundColor"]; }
            set { this["foregroundColor"] = value; }
        }

        [ConfigurationProperty("backgroundColor")]
        public ConsoleColor BackgroundColor
        {
            get { return (ConsoleColor)this["backgroundColor"]; }
            set { this["backgroundColor"] = value; }
        }

        [ConfigurationProperty("consoleMode")]
        [CLSCompliant(false)]
        public uint ConsoleMode
        {
            get { return (uint)this["consoleMode"]; }
            set { this["consoleMode"] = value; }
        }

        [ConfigurationProperty("disableConsoleCtrlHandler")]
        public bool DisableConsoleCtrlHandler
        {
            get { return (bool)this["disableConsoleCtrlHandler"]; }
            set { this["disableConsoleCtrlHandler"] = value; }
        }

        #endregion Instance Data Members (Public)

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Console Settings");

            msg.AppendFormatLine("ForegroundColor: {0}", ForegroundColor);
            msg.AppendFormatLine("BackgroundColor: {0}", BackgroundColor);
            msg.AppendFormatLine("ConsoleMode: {0}", ConsoleMode);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoConsoleSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoConfigurationManager.GetSection(SECTION_NAME, _defaultInstance.Value);
                }

                return _instance == null ? _defaultInstance.Value : _instance;
            }
        }

        #endregion Factory Methods
    }
}
