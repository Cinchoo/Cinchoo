using Cinchoo.Core.Configuration;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public enum ChoValidApplicationMode { Console, Windows }
    public enum ChoShellExtensionActionMode { Register, Unregister };

    [ChoCommandLineArgObject("#help, #?, #h")]
    public sealed class ChoFrameworkCmdLineArgs : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("#AM", Description = "Application mode.", Order = -1)]
        public ChoValidApplicationMode? ApplicationMode
        {
            get;
            set;
        }

        [ChoCommandLineArg("#DP", Description = "Display available properties.")]
        public bool DisplayAvailProperties
        {
            get;
            set;
        }

        [ChoCommandLineArg("#DPF", Description = "Display available object parsers/formatters.")]
        public bool DisplayAvailTypeParsersNFormatters
        {
            get;
            set;
        }

        [ChoCommandLineArg("#COH", Description = "Display configuration object help (Pass config object full name).")]
        public string ConfigObjectTypeName
        {
            get;
            set;
        }

        [ChoCommandLineArg("#SE", Description = "Shell extension action mode.")]
        public ChoShellExtensionActionMode? ShellExtensionActionMode
        {
            get;
            set;
        }

        internal static ChoApplicationMode? GetApplicationMode()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();

                ChoValidApplicationMode validApplicationMode;
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "#AM")
                    {
                        if (Enum.TryParse<ChoValidApplicationMode>(keyValuePair.Value, out validApplicationMode))
                        {
                            return (ChoApplicationMode)Enum.Parse(typeof(ChoApplicationMode), validApplicationMode.ToString());
                        }
                        break;
                    }
                }
            }

            return null;
        }

        internal static ChoShellExtensionActionMode? GetShellExtensionActionMode()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();

                string mode = null;
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "#SE")
                    {
                        mode = keyValuePair.Value;
                        break;
                    }
                }

                ChoShellExtensionActionMode seMode;
                if (!mode.IsNullOrWhiteSpace())
                {
                    if (Enum.TryParse<ChoShellExtensionActionMode>(mode, out seMode))
                        return seMode;
                }

                foreach (string pos in parser.PosArgs)
                {
                    if (!pos.IsNullOrWhiteSpace())
                    {
                        if (Enum.TryParse<ChoShellExtensionActionMode>(pos, out seMode))
                            return seMode;
                    }
                }

                return null;
            }
        }

        protected override void OnAfterCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
            ChoShellExtensionActionMode? sem = GetShellExtensionActionMode();

            if (DisplayAvailProperties || DisplayAvailTypeParsersNFormatters)
            {
                if (DisplayAvailProperties)
                    ChoApplication.DisplayMsg(ChoPropertyManagerSettings.Me.GetHelpText(), null, ConsoleColor.Yellow);
                if (DisplayAvailTypeParsersNFormatters)
                    ChoApplication.DisplayMsg(ChoTypesManager.GetHelpText(), null, ConsoleColor.Green);
                if (!ConfigObjectTypeName.IsNullOrWhiteSpace())
                    ChoApplication.DisplayMsg(ChoConfigurationManager.GetHelpText(ChoType.GetType(ConfigObjectTypeName)), null, ConsoleColor.Green);
                Environment.Exit(0);
            }
            else if (!ConfigObjectTypeName.IsNullOrWhiteSpace())
            {
                ChoApplication.DisplayMsg(ChoConfigurationManager.GetHelpText(ChoType.GetType(ConfigObjectTypeName)), null, ConsoleColor.Green);
                Environment.Exit(0);
            }
            else if (sem != null)
            {
                if (sem.Value == ChoShellExtensionActionMode.Register)
                {
                    ChoShellExtension.Register();
                    ChoTrace.WriteLine("Shell Extensions registered successfully.");

                    ChoShellFileAssociation.Register();
                    ChoTrace.WriteLine("File Associations registered successfully.");

                    Environment.Exit(0);
                }
                else if (sem.Value == ChoShellExtensionActionMode.Unregister)
                {
                    ChoShellExtension.Unregister();
                    ChoTrace.WriteLine("Shell Extensions unregistered successfully.");

                    ChoShellFileAssociation.Unregister();
                    ChoTrace.WriteLine("File Associations unregistered successfully.");
                    
                    Environment.Exit(0);
                }
            }

            if (ChoShellExtension.ExecuteShellExtensionMethodIfAnySpecified(commandLineArgs))
            {
                ChoTrace.WriteLine("Shell Extension ran successfully.");
                Environment.Exit(0);
            }

            base.OnAfterCommandLineArgObjectLoaded(commandLineArgs);
        }
    }
}
