namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using Cinchoo.Core.Configuration;
    using System.Xml.Serialization;
using Cinchoo.Core.Text;

    #endregion NameSpaces

    public abstract class ChoFrxSettings
    {
        public EventHandler FrxSettingsChanged;

        protected void OnFrxSettingsChanged()
        {
            FrxSettingsChanged.Raise(this, null);
        }
    }

    [XmlRoot("consoleSettings")]
    public class ChoConsoleSettings
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoConsoleSettings _instance;

        #endregion Shared Data Members (Private)
        
        #region Instance Data Members (Public)

        [XmlAttribute("foregroundColor")]
        public ConsoleColor ForegroundColor = Console.ForegroundColor;

        [XmlAttribute("backgroundColor")]
        public ConsoleColor BackgroundColor = Console.BackgroundColor;

        [XmlAttribute("consoleMode")]
        [CLSCompliant(false)]
        public uint ConsoleMode = uint.MinValue;

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
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoConsoleSettings>();
                }

                return _instance;
            }
        }

        #endregion Factory Methods
}
}
