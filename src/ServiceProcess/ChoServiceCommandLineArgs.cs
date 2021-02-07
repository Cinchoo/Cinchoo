namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Shell;
    using System.IO;

    #endregion NameSpaces

    [ChoCommandLineArgObject("@help, @?, @h")]
    //[ChoIgnorePrintHeader]
    public sealed class ChoServiceCommandLineArgs : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("@SN", Description = "Service Name.")]
        public string ServiceName
        {
            get;
            set;
        }

        [ChoCommandLineArg("@DN", Description = "Display Name.")]
        public string DisplayName
        {
            get;
            set;
        }

        [ChoCommandLineArg("@IN", Description = "Instance Name.")]
        public string InstanceName
        {
            get;
            set;
        }

        [ChoCommandLineArg("@SD", Description = "Service Description.")]
        public string ServiceDescription
        {
            get;
            set;
        }

        [ChoCommandLineArg("@I", Description = "Install Service.")]
        public bool InstallService
        {
            get;
            set;
        }

        [ChoCommandLineArg("@U", Description = "Uninstall Service.")]
        public bool UninstallService
        {
            get;
            set;
        }

        [ChoCommandLineArg("@S", Description = "Start Service.")]
        public bool StartService
        {
            get;
            set;
        }

        [ChoCommandLineArg("@T", Description = "Stop Service.")]
        public bool StopService
        {
            get;
            set;
        }

        [ChoCommandLineArg("@P", Description = "Pause Service.")]
        public bool PauseService
        {
            get;
            set;
        }

        [ChoCommandLineArg("@C", Description = "Continue Service.")]
        public bool ContinueService
        {
            get;
            set;
        }

        [ChoCommandLineArg("@E", DefaultValue = "~~System.Int32.MinValue~~", Description = "Execute Command.")]
        public int ExecuteCommand
        {
            get;
            set;
        }

        [ChoCommandLineArg("@SP", Description = "Command Line Parameters.")]
        public string ServiceParams
        {
            get;
            set;
        }

        protected override bool OnCommandLineArgLoadError(string memberName, object value, Exception ex)
        {
            if (memberName == "@E")
            {
                ExecuteCommand = Int32.MinValue;
                return true;
            }

            return base.OnCommandLineArgLoadError(memberName, value, ex);
        }

        internal static bool HasServiceParams()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@SN")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@DN")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@IN")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@SD")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@I")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@U")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@S")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@T")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@P")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@C")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@E")
                        return true;
                    if (keyValuePair.Key.ToUpper() == "@SP")
                        return true;
                }
                return false;
            }
        }

        private static string GetLogFileName(string serviceName, string logFileName)
        {
            if (!serviceName.IsNullOrWhiteSpace())
                return serviceName;
            else if (!logFileName.IsNullOrWhiteSpace())
                return Path.Combine(Path.GetDirectoryName(logFileName), Path.GetFileNameWithoutExtension(logFileName));
            else
                return logFileName;
        }

        internal static string[] GetServiceNameCmdLineArgArray()
        {
            return new string[] { "/@SN:{0}".FormatString(GetServiceName()) };
        }

        public static string GetServiceName()
        {
            string instanceName = GetInstanceName();
            return instanceName.IsNullOrWhiteSpace() ? GetServiceNameInternal() : "{0}${1}".FormatString(GetServiceNameInternal(), instanceName);
        }

        private static string GetServiceNameInternal()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@SN")
                    {
                        if (!keyValuePair.Value.IsNullOrWhiteSpace())
                            return keyValuePair.Value;
                        else
                            break;
                    }
                }
                return ChoServiceInstallerSettings.Me.ServiceName.IsNullOrWhiteSpace() ? ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension : ChoServiceInstallerSettings.Me.ServiceName;
            }
        }

        private static string GetInstanceName()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@IN")
                    {
                        if (!keyValuePair.Value.IsNullOrWhiteSpace())
                            return keyValuePair.Value;
                        else
                            break;
                    }
                }
                return null;
            }
        }

        public static string GetDisplayName()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@DN")
                    {
                        if (!keyValuePair.Value.IsNullOrWhiteSpace())
                            return keyValuePair.Value;
                        else
                            break;
                    }
                }
                return GetServiceName();
            }
        }

        public static string GetServiceDescription()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@SD")
                    {
                        if (!keyValuePair.Value.IsNullOrWhiteSpace())
                            return keyValuePair.Value;
                        else
                            break;
                    }
                }
                return ChoServiceInstallerSettings.Me.Description.IsNullOrWhiteSpace() ? null : ChoServiceInstallerSettings.Me.Description;
            }
        }

        public static string GetServiceParams()
        {
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser(false))
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@SP")
                    {
                        if (!keyValuePair.Value.IsNullOrWhiteSpace())
                        {
                            return keyValuePair.Value;
                        }
                        else
                            break;
                    }
                }
                return ChoServiceInstallerSettings.Me.ServiceParams == null || ChoServiceInstallerSettings.Me.ServiceParams.Value.IsNullOrWhiteSpace() ? 
                    String.Empty : ChoServiceInstallerSettings.Me.ServiceParams.Value;
            }
        }

        internal static void OverrideFrxParams(ChoGlobalApplicationSettings gas)
        {
            if (gas == null) return;

            string serviceName = null;
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@SN")
                    {
                        if (!keyValuePair.Value.IsNullOrWhiteSpace())
                            serviceName = gas.LogSettings.LogFolder = keyValuePair.Value.Trim();
                    }
                }
            }

            string command = null;
            using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
            {
                parser.Parse();
                foreach (KeyValuePair<string, string> keyValuePair in parser)
                {
                    if (keyValuePair.Key.ToUpper() == "@I")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "InstallService";
                        break;
                    }
                    else if (keyValuePair.Key.ToUpper() == "@U")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "UninstallService";
                        break;
                    }
                    else if (keyValuePair.Key.ToUpper() == "@S")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "StartService";
                        break;
                    }
                    else if (keyValuePair.Key.ToUpper() == "@T")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "StopService";
                        break;
                    }
                    else if (keyValuePair.Key.ToUpper() == "@P")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "PauseService";
                        break;
                    }
                    else if (keyValuePair.Key.ToUpper() == "@C")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "ContinueService";
                        break;
                    }
                    else if (keyValuePair.Key.ToUpper() == "@E")
                    {
                        ChoApplication.ServiceInstallation = true;
                        command = "ExecuteCommand";
                        break;
                    }
                }
                if (ChoApplication.ServiceInstallation)
                {
                    gas.ApplicationBehaviourSettings.HideWindow = false;
                    gas.ApplicationBehaviourSettings.SingleInstanceApp = true;
                    gas.ApplicationName = "{0}.{1}".FormatString(gas.ApplicationNameWithoutExtension, command);
                    gas.EventLogSourceName = "{0}.{1}".FormatString(gas.EventLogSourceName, command);
                    gas.LogSettings.LogFileName = "{0}.{1}".FormatString(GetLogFileName(serviceName, gas.LogSettings.LogFileName), command);
                    gas.LogSettings.LogFolder = serviceName.IsNullOrWhiteSpace() ? command : Path.Combine(command, serviceName);
                    gas.LogSettings.DoAppendProcessIdToLogDir = false;
                }
            }
        }
    }
}
