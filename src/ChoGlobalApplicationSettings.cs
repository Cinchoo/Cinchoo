namespace Cinchoo.Core
{
	#region Namespaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Text;
    using System.IO;
    using System.Diagnostics;
    using System.Net;
    using System.Collections.Generic;
    using System.Configuration;

	#endregion

    public enum ChoTrayAppTurnOnMode { OnMinimize, OnClose, OnMinimizeOrClose }

    public class ChoTrayApplicationBehaviourSettings
    {
        [XmlAttribute("turnOn")]
        public bool TurnOn = false;

        [XmlAttribute("showInTaskbar")]
        public bool ShowInTaskbar = true;

        [XmlAttribute("hideMainWindowAtStartup")]
        public bool HideMainWindowAtStartup = false;

        [XmlAttribute("tooltipText")]
        public string TooltipText;

        [XmlAttribute("balloonTipText")]
        public string BalloonTipText;

        [XmlAttribute("hideTrayIconWhenMainWindowShown")]
        public bool HideTrayIconWhenMainWindowShown = false;

        [XmlAttribute("trayAppTurnOnMode")]
        public ChoTrayAppTurnOnMode TrayAppTurnOnMode = ChoTrayAppTurnOnMode.OnMinimize;

        [XmlElement("trayIcon")]
        public string TrayIcon;

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Tray Application Behaviour Settings");

            msg.AppendFormatLine("TurnOn: {0}", TurnOn);
            msg.AppendFormatLine("HideMainWindowAtStartup: {0}", HideMainWindowAtStartup);
            msg.AppendFormatLine("HideTrayIconWhenMainWindowShown: {0}", HideTrayIconWhenMainWindowShown);
            msg.AppendFormatLine("TooltipText: {0}", TooltipText);
            msg.AppendFormatLine("BalloonTipText: {0}", BalloonTipText);
            msg.AppendFormatLine("TrayAppTurnOnMode: {0}", TrayAppTurnOnMode);
            msg.AppendFormatLine("ShowInTaskbar: {0}", ShowInTaskbar);
            msg.AppendFormatLine("TrayIcon: {0}", TrayIcon);

            return msg.ToString();
        }

        #endregion Object Overrides
    }

    public class ChoApplicationBehaviourSettings
    {
        [XmlAttribute("hideWindow")]
        public bool HideWindow = false;

        [XmlAttribute("bringWindowToTop")]
        public bool BringWindowToTop = false;

        [XmlAttribute("alwaysOnTop")]
        public bool AlwaysOnTop = false;

        [XmlAttribute("runAtStartup")]
        public bool RunAtStartup = false;

        [XmlAttribute("runOnceAtStartup")]
        public bool RunOnceAtStartup = false;

        [XmlAttribute("singleInstanceApp")]
        public bool SingleInstanceApp = false;

        [XmlAttribute("activateFirstInstance")]
        public bool ActivateFirstInstance = false;

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Application Behaviour Settings");

            msg.AppendFormatLine("HideWindow: {0}", HideWindow);
            msg.AppendFormatLine("BringWindowToTop: {0}", BringWindowToTop);
            msg.AppendFormatLine("AlwaysOnTop: {0}", AlwaysOnTop);
            msg.AppendFormatLine("RunAtStartup: {0}", RunAtStartup);
            msg.AppendFormatLine("RunOnceAtStartup: {0}", RunOnceAtStartup);
            msg.AppendFormatLine("SingleInstanceApp: {0}", SingleInstanceApp);
            msg.AppendFormatLine("ActivateFirstInstance: {0}", ActivateFirstInstance);

            return msg.ToString();
        }

        #endregion Object Overrides
    }

    public class ChoLogSettings
    {
        [XmlAttribute("traceLevel")]
        public int TraceLevel = 4;

        [XmlElement("logFolder")]
        public string LogFolder = String.Empty;

        [XmlElement("logFilename")]
        public string LogFileName = String.Empty;

        [XmlElement("logTimeStampFormat")]
        public string LogTimeStampFormat = String.Empty;

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Log Settings");

            msg.AppendFormatLine("TraceLevel: {0}", TraceLevel);
            msg.AppendFormatLine("LogFolder: {0}", LogFolder);
            msg.AppendFormatLine("LogFileName: {0}", LogFileName);
            msg.AppendFormatLine("LogTimeStampFormat: {0}", LogTimeStampFormat);

            return msg.ToString();
        }

        #endregion Object Overrides
    }

    [XmlRoot("globalApplicationSettings")]
    public class ChoGlobalApplicationSettings : IChoInitializable, IChoObjectChangeWatcheable
    {
        internal static string AppFrxConfigFilePath = String.Empty;

        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoGlobalApplicationSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlElement("behaviourSettings")]
        public ChoApplicationBehaviourSettings ApplicationBehaviourSettings;

        [XmlElement("trayApplicationBehaviourSettings")]
        public ChoTrayApplicationBehaviourSettings TrayApplicationBehaviourSettings;

        [XmlElement("logSettings")]
        public ChoLogSettings LogSettings;

        [XmlAttribute("applicationId")]
        public string ApplicationName = String.Empty;

        [XmlAttribute("eventLogSourceName")]
        public string EventLogSourceName = String.Empty;

        [XmlAttribute("appConfigFilePath")]
        public string AppConfigFilePath = String.Empty;

        [XmlAttribute("turnOnConsoleOutput")]
        public bool TurnOnConsoleOutput = false;

        [XmlIgnore]
        internal string ApplicationConfigFilePath = String.Empty;

        [XmlIgnore]
        public string ApplicationNameWithoutExtension;

        [XmlIgnore]
        public string HostName = String.Empty;

        [XmlIgnore]
        public List<string> IPAddresses = new List<string>();

        [XmlIgnore]
        internal bool DoAppendProcessIdToLogDir;

        [XmlIgnore]
        internal string ApplicationConfigDirectory;

        [XmlIgnore]
        internal string ApplicationLogDirectory;

        #endregion

        #region Constructors

        public ChoGlobalApplicationSettings()
        {
        }

        #endregion Constructors

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("ApplicationId: {0}", ApplicationName);
            msg.AppendFormatLine("EventLogSourceName: {0}", EventLogSourceName);
            msg.AppendFormatLine("TurnOnConsoleOutput: {0}", TurnOnConsoleOutput);
            msg.AppendFormatLine("ApplicationConfigFilePath: {0}", ApplicationConfigFilePath);

            msg.AppendFormatLine(String.Empty);

            msg.AppendFormatLine("ApplicationNameWithoutExtension: {0}", ApplicationNameWithoutExtension);
            msg.AppendFormatLine("ApplicationLogDirectory: {0}", ApplicationLogDirectory);
            msg.AppendFormatLine("HostName: {0}", HostName);

            msg.AppendFormatLine(String.Empty);

            if (ApplicationBehaviourSettings != null)
                msg.AppendFormatLine(ApplicationBehaviourSettings.ToString());

            if (TrayApplicationBehaviourSettings != null)
                msg.AppendFormatLine(TrayApplicationBehaviourSettings.ToString());
            
            if (LogSettings != null)
                msg.AppendFormatLine(LogSettings.ToString());

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoGlobalApplicationSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoGlobalApplicationSettings>();
                    }
                }

                return _instance;
            }
        }

        #endregion Factory Methods

        #region Shared Members (Internal)

        public void Initialize()
        {
            if (ApplicationBehaviourSettings == null)
                ApplicationBehaviourSettings = new ChoApplicationBehaviourSettings();

            if (TrayApplicationBehaviourSettings == null)
                TrayApplicationBehaviourSettings = new ChoTrayApplicationBehaviourSettings();

            if (LogSettings == null)
                LogSettings = new ChoLogSettings();

            if (ApplicationName.IsNullOrWhiteSpace())
            {
                try
                {
                    ApplicationName = System.IO.Path.GetFileName(ChoAssembly.GetEntryAssemblyLocation());
                }
                catch (System.Security.SecurityException ex)
                {
                    ChoApplication.Trace(ChoTrace.ChoSwitch.TraceError, ex.ToString());
                }
            }

            if (ApplicationName.IsNullOrEmpty())
            {
                ChoApplication.OnFatalApplicationException(101, "Missing ApplicationName.");
            }

            ApplicationNameWithoutExtension = Path.GetFileNameWithoutExtension(ApplicationName);

            if (EventLogSourceName.IsNullOrWhiteSpace())
                EventLogSourceName = System.IO.Path.GetFileName(ChoAssembly.GetEntryAssemblyLocation());
            if (LogSettings.LogTimeStampFormat.IsNullOrWhiteSpace())
                LogSettings.LogTimeStampFormat = "yyyy-MM-dd hh:mm:ss.fffffff";

            if (LogSettings.LogFileName.IsNullOrWhiteSpace())
                LogSettings.LogFileName = ChoPath.ChangeExtension(ApplicationName, ChoReservedFileExt.Log);

            LogSettings.LogFileName = ChoPath.CleanFileName(LogSettings.LogFileName);
            if (Path.IsPathRooted(LogSettings.LogFileName))
                LogSettings.LogFileName = Path.GetFileName(LogSettings.LogFileName);

            try
            {
                DateTime.Now.ToString(LogSettings.LogTimeStampFormat);
            }
            catch (Exception ex)
            {
                ChoApplication.Trace(ChoTrace.ChoSwitch.TraceError, "Invalid LogTimeStampFormat '{0}' configured.".FormatString(LogSettings.LogTimeStampFormat));
                ChoApplication.Trace(ChoTrace.ChoSwitch.TraceError, ex.ToString());
                LogSettings.LogTimeStampFormat = "yyyy-MM-dd hh:mm:ss.fffffff";
            }

            try 
            {
                string sharedEnvConfigDir = null;

                if (!AppFrxConfigFilePath.IsNullOrEmpty())
                    sharedEnvConfigDir = Path.GetDirectoryName(AppFrxConfigFilePath);

                if (AppConfigFilePath.IsNullOrWhiteSpace())
                {
                    if (sharedEnvConfigDir.IsNullOrWhiteSpace())
                        ApplicationConfigFilePath = ChoPath.GetFullPath(Path.Combine(ChoReservedDirectoryName.Config, ChoPath.AddExtension(ChoPath.CleanFileName(ApplicationName), ChoReservedFileExt.Xml)));
                    else
                        ApplicationConfigFilePath = Path.Combine(sharedEnvConfigDir, ChoPath.AddExtension(ChoPath.CleanFileName(ApplicationName), ChoReservedFileExt.Xml));
                }
                else
                {
                    if (!Path.IsPathRooted(AppConfigFilePath))
                    {
                        if (sharedEnvConfigDir.IsNullOrWhiteSpace())
                            ApplicationConfigFilePath = ChoPath.GetFullPath(Path.Combine(ChoReservedDirectoryName.Config, AppConfigFilePath));
                        else
                            ApplicationConfigFilePath = Path.Combine(sharedEnvConfigDir, AppConfigFilePath);
                    }
                    else
                        ApplicationConfigFilePath = AppFrxConfigFilePath;
                }

                ApplicationConfigDirectory = Path.GetDirectoryName(ApplicationConfigFilePath);
            }
            catch (System.Security.SecurityException ex)
            {
                // This security exception will occur if the caller does not have 
                // some undefined set of SecurityPermission flags.
                ChoApplication.Trace(ChoTrace.ChoSwitch.TraceError, ex.ToString());
            }

            #region Get HostName

            // Get the DNS host name of the current machine
            try
            {
                // Lookup the host name
                if (HostName.IsNullOrWhiteSpace())
                    HostName = System.Net.Dns.GetHostName();
            }
            catch (System.Net.Sockets.SocketException)
            {
            }
            catch (System.Security.SecurityException)
            {
                // We may get a security exception looking up the hostname
                // You must have Unrestricted DnsPermission to access resource
            }

            // Get the NETBIOS machine name of the current machine
            if (HostName.IsNullOrWhiteSpace())
            {
                try
                {
                    HostName = Environment.MachineName;
                }
                catch (InvalidOperationException)
                {
                }
                catch (System.Security.SecurityException)
                {
                    // We may get a security exception looking up the machine name
                    // You must have Unrestricted EnvironmentPermission to access resource
                }
            }

            #endregion Get HostName

            #region Get IpAddresses

            try
            {
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                for (int i = 0; i < localIPs.Length; i++)
                    IPAddresses.Add(localIPs[i].ToString());
            }
            catch (System.Net.Sockets.SocketException)
            {
            }
            catch (System.Security.SecurityException)
            {
                // We may get a security exception looking up the hostname
                // You must have Unrestricted DnsPermission to access resource
            }

            #endregion Get IpAddresses

            if (LogSettings.TraceLevel < 0)
                LogSettings.TraceLevel = 4;

            if (!LogSettings.LogFolder.IsNullOrWhiteSpace())
                ApplicationLogDirectory = ChoString.ExpandProperties(LogSettings.LogFolder, ChoEnvironmentVariablePropertyReplacer.Instance);
            //else
            //    ApplicationLogDirectory = Path.Combine(Path.GetDirectoryName(ChoGlobalApplicationSettings.SharedEnvConfigPath), ChoReservedDirectoryName.Logs);

            if (ApplicationLogDirectory.IsNullOrWhiteSpace())
            {
                if (ChoApplication.AppEnvironment.IsNullOrWhiteSpace())
                    ApplicationLogDirectory = Path.Combine(ChoPath.AssemblyBaseDirectory, ChoReservedDirectoryName.Logs);
                else
                    ApplicationLogDirectory = Path.Combine(ChoPath.AssemblyBaseDirectory, ChoReservedDirectoryName.Logs, ChoApplication.AppEnvironment);
            }
            if (!Path.IsPathRooted(ApplicationLogDirectory))
                ApplicationLogDirectory = Path.Combine(ChoPath.AssemblyBaseDirectory, ApplicationLogDirectory);

            if (ChoApplication.ApplicationMode == ChoApplicationMode.Service)
                TrayApplicationBehaviourSettings.TurnOn = false;
        }

        #endregion Shared Members (Internal)

        #region IChoConfigChangeWatcheable Overrides

        public void OnObjectChanged(object sender, EventArgs e)
        {
            _instance = sender as ChoGlobalApplicationSettings;
            ChoApplication.Refresh();

            ChoEventManager.GetValue(GetType()).OnObjectChanged(sender, e);
        }

        public event EventHandler ObjectChanged
        {
            add
            {
                ChoEventManager.GetValue(GetType()).ObjectChanged += value;
            }
            remove
            {
                ChoEventManager.GetValue(GetType()).ObjectChanged -= value;
            }
        }

        #endregion IChoConfigChangeWatcheable Overrides
    }
}
