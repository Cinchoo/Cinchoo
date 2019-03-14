namespace Cinchoo.Core.Shell.Console
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Attributes;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Converters;

    #endregion NameSpaces

    [ChoConfigurationSection("console/consoleProcessorSettings")]
    public class ChoConsoleProcessorSettings : ChoConfigurableObject
    {
        #region Instance Data Members (Public)

        [ChoConfigurationProperty("foregroundColor")]
        [ChoTypeConverter(typeof(ChoToEnumConverter), Parameter = typeof(ConsoleColor))]
        public ConsoleColor ForegroundColor = Console.ForegroundColor;

        [ChoConfigurationProperty("backgroundColor")]
        [ChoTypeConverter(typeof(ChoToEnumConverter), Parameter = typeof(ConsoleColor))]
        public ConsoleColor BackgroundColor = Console.BackgroundColor;

        #endregion Instance Data Members (Public)
    }
}
