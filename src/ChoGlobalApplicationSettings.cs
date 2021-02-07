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
    using System.Drawing;

    #endregion

    public enum ChoTrayAppTurnOnMode { OnMinimize, OnClose, OnMinimizeOrClose }

    public class ChoTrayApplicationFontSettings : IChoMergeable<ChoTrayApplicationFontSettings>
    {
        [XmlAttribute("fontName")]
        public string FontName = "Helvetica";

        [XmlAttribute("fontSize")]
        public int FontSize = 8;

        [XmlElement("fontColor")]
        public Color FontColor = Color.Black;

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Tray Application Font Settings");

            msg.AppendFormatLine("FontName: {0}", FontName);
            msg.AppendFormatLine("FontSize: {0}", FontSize);
            msg.AppendFormatLine("FontColor: {0}", FontColor);

            return msg.ToString();
        }

        #endregion Object Overrides

        public void Merge(ChoTrayApplicationFontSettings source)
        {
            if (source == null) return;

            if (source.FontName.IsNullOrWhiteSpace())
                FontName = source.FontName;

            FontSize = source.FontSize;
            FontColor = source.FontColor;
        }

        public void Merge(object source)
        {
            Merge(source as ChoTrayApplicationFontSettings);
        }
    }

    public class ChoTrayApplicationContextMenuSettings : IChoMergeable<ChoTrayApplicationContextMenuSettings>
    {
        [XmlAttribute("displayShowMainWndMenuItem")]
        public bool DisplayShowMainWndMenuItem = false;

        [XmlAttribute("displayAlwaysOnTopMenuItem")]
        public bool DisplayAlwaysOnTopMenuItem = true;

        [XmlAttribute("displayRunAtSystemsStartupMenuItem")]
        public bool DisplayRunAtSystemsStartupMenuItem = true;

        [XmlAttribute("displayShowInTaskbarMenuItem")]
        public bool DisplayShowInTaskbarMenuItem = true;

        [XmlAttribute("displayAboutMenuItem")]
        public bool DisplayAboutMenuItem = true;

        [XmlAttribute("displayHelpMenuItem")]
        public bool DisplayHelpMenuItem = true;

        [XmlAttribute("displayExitMenuItem")]
        public bool DisplayExitMenuItem = true;

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Tray Application Context Menu Settings");

            msg.AppendFormatLine("DisplayAlwaysOnTopMenuItem: {0}", DisplayAlwaysOnTopMenuItem);
            msg.AppendFormatLine("DisplayRunAtSystemsStartupMenuItem: {0}", DisplayRunAtSystemsStartupMenuItem);
            msg.AppendFormatLine("DisplayShowInTaskbarMenuItem: {0}", DisplayShowInTaskbarMenuItem);
            msg.AppendFormatLine("DisplayAboutMenuItem: {0}", DisplayAboutMenuItem);
            msg.AppendFormatLine("DisplayHelpMenuItem: {0}", DisplayHelpMenuItem);
            msg.AppendFormatLine("DisplayExitMenuItem: {0}", DisplayExitMenuItem);

            return msg.ToString();
        }

        #endregion Object Overrides

        public void Merge(ChoTrayApplicationContextMenuSettings source)
        {
            if (source == null) return;

            if (!source.DisplayAlwaysOnTopMenuItem)
                DisplayAlwaysOnTopMenuItem = source.DisplayAlwaysOnTopMenuItem;

            if (!source.DisplayRunAtSystemsStartupMenuItem)
                DisplayRunAtSystemsStartupMenuItem = source.DisplayRunAtSystemsStartupMenuItem;

            if (!source.DisplayShowInTaskbarMenuItem)
                DisplayShowInTaskbarMenuItem = source.DisplayShowInTaskbarMenuItem;

            if (!source.DisplayAboutMenuItem)
                DisplayAboutMenuItem = source.DisplayAboutMenuItem;

            if (!source.DisplayHelpMenuItem)
                DisplayHelpMenuItem = source.DisplayHelpMenuItem;

            if (!source.DisplayExitMenuItem)
                DisplayExitMenuItem = source.DisplayExitMenuItem;
        }

        public void Merge(object source)
        {
            Merge(source as ChoTrayApplicationContextMenuSettings);
        }
    }

    public class ChoTrayApplicationBehaviourSettings : IChoMergeable<ChoTrayApplicationBehaviourSettings>
    {
        [XmlAttribute("turnOn")]
        public bool TurnOn = false;

        [XmlAttribute("showInTaskbar")]
        public bool ShowInTaskbar = true;

        [XmlAttribute("hideMainWindowAtStartup")]
        public bool HideMainWindowAtStartup = false;

        [XmlAttribute("tooltipText")]
        public string TooltipText = String.Empty;

        [XmlAttribute("balloonTipText")]
        public string BalloonTipText = String.Empty;

        [XmlAttribute("hideTrayIconWhenMainWindowShown")]
        public bool HideTrayIconWhenMainWindowShown = false;

        [XmlAttribute("disableDefaultDoubleClickEvent")]
        public bool DisableDefaultDoubleClickEvent = false;

        [XmlAttribute("disableDefaultClickEvent")]
        public bool DisableDefaultClickEvent = false;

        [XmlAttribute("turnOnMode")]
        public ChoTrayAppTurnOnMode TurnOnMode = ChoTrayAppTurnOnMode.OnMinimize;

        [XmlElement("trayIcon")]
        public string TrayIcon = String.Empty;

        [XmlElement("fontSettings")]
        public ChoTrayApplicationFontSettings FontSettings = new ChoTrayApplicationFontSettings();

        [XmlElement("contextMenuSettings")]
        public ChoTrayApplicationContextMenuSettings ContextMenuSettings = new ChoTrayApplicationContextMenuSettings();

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Tray Application Behaviour Settings");

            msg.AppendFormatLine("TurnOn: {0}", TurnOn);
            msg.AppendFormatLine("HideMainWindowAtStartup: {0}", HideMainWindowAtStartup);
            msg.AppendFormatLine("HideTrayIconWhenMainWindowShown: {0}", HideTrayIconWhenMainWindowShown);
            msg.AppendFormatLine("TooltipText: {0}", TooltipText);
            msg.AppendFormatLine("BalloonTipText: {0}", BalloonTipText);
            msg.AppendFormatLine("TurnOnMode: {0}", TurnOnMode);
            msg.AppendFormatLine("ShowInTaskbar: {0}", ShowInTaskbar);
            msg.AppendFormatLine("TrayIcon: {0}", TrayIcon);
            if (FontSettings != null)
            {
                msg.AppendFormatLine(FontSettings.ToString().Indent());
            }
            if (ContextMenuSettings != null)
            {
                msg.AppendFormatLine(ContextMenuSettings.ToString().Indent());
            }

            return msg.ToString();
        }

        #endregion Object Overrides

        public void Merge(ChoTrayApplicationBehaviourSettings source)
        {
            if (source == null) return;

            if (source.TurnOn)
                TurnOn = source.TurnOn;

            if (!source.ShowInTaskbar)
                ShowInTaskbar = source.ShowInTaskbar;

            if (source.HideMainWindowAtStartup)
                HideMainWindowAtStartup = source.HideMainWindowAtStartup;

            if (!source.TooltipText.IsNullOrWhiteSpace())
                TooltipText = source.TooltipText;

            if (source.HideTrayIconWhenMainWindowShown)
                HideTrayIconWhenMainWindowShown = source.HideTrayIconWhenMainWindowShown;

            if (source.TurnOnMode != ChoTrayAppTurnOnMode.OnMinimize)
                TurnOnMode = source.TurnOnMode;

            if (!source.TrayIcon.IsNullOrWhiteSpace())
                TrayIcon = source.TrayIcon;

            if (source.FontSettings != null)
                FontSettings.Merge(source.FontSettings);

            if (source.ContextMenuSettings != null)
                ContextMenuSettings.Merge(source.ContextMenuSettings);
        }

        public void Merge(object source)
        {
            Merge(source as ChoTrayApplicationBehaviourSettings);
        }
    }

    public class ChoApplicationBehaviourSettings : IChoMergeable<ChoApplicationBehaviourSettings>
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
        public bool SingleInstanceApp = true;

        [XmlAttribute("activateFirstInstance")]
        public bool ActivateFirstInstance = false;

        [XmlAttribute("showEnvironmentSelectionWnd")]
        public bool ShowEnvironmentSelectionWnd = true;

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
            msg.AppendFormatLine("ShowEnvironmentSelectionWnd: {0}", ShowEnvironmentSelectionWnd);

            return msg.ToString();
        }

        #endregion Object Overrides

        public void Merge(ChoApplicationBehaviourSettings source)
        {
            if (source == null) return;

            if (source.HideWindow)
                HideWindow = source.HideWindow;
            if (source.BringWindowToTop)
                BringWindowToTop = source.BringWindowToTop;
            if (source.AlwaysOnTop)
                AlwaysOnTop = source.AlwaysOnTop;
            if (source.RunAtStartup)
                RunAtStartup = source.RunAtStartup;
            if (source.RunOnceAtStartup)
                RunOnceAtStartup = source.RunOnceAtStartup;
            if (!source.SingleInstanceApp)
                SingleInstanceApp = source.SingleInstanceApp;
            if (source.ActivateFirstInstance)
                ActivateFirstInstance = source.ActivateFirstInstance;
            if (!source.ShowEnvironmentSelectionWnd)
                ShowEnvironmentSelectionWnd = source.ShowEnvironmentSelectionWnd;
        }

        public void Merge(object source)
        {
            Merge(source as ChoApplicationBehaviourSettings);
        }
    }

    public class ChoLogSettings : IChoMergeable<ChoLogSettings>
    {
        //[XmlAttribute("traceLevel")]
        internal TraceLevel TraceLevel
        {
            get { return ChoTraceSwitch.Switch.Level; }
            //set 
            //{
            //    if (ChoTraceSwitch.Switch.Level == value) return;
            //    ChoTraceSwitch.Switch = new TraceSwitch("ChoSwitch", "Cinchoo Trace Switch", value.ToString());
            //}
        }

        [XmlElement("logFolder")]
        public string LogFolder;

        [XmlElement("logFilename")]
        public string LogFileName = String.Empty;

        [XmlElement("logTimeStampFormat")]
        public string LogTimeStampFormat = String.Empty;

        [XmlElement("doAppendProcessIdToLogDir")]
        public ChoNullable<bool> DoAppendProcessIdToLogDir;

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Log Settings");

            msg.AppendFormatLine("TraceLevel: {0}", TraceLevel);
            msg.AppendFormatLine("LogFolder: {0}", LogFolder);
            msg.AppendFormatLine("LogFileName: {0}", LogFileName);
            msg.AppendFormatLine("LogTimeStampFormat: {0}", LogTimeStampFormat);
            msg.AppendFormatLine("DoAppendProcessIdToLogDir: {0}", DoAppendProcessIdToLogDir);

            return msg.ToString();
        }

        #endregion Object Overrides

        public void Merge(ChoLogSettings source)
        {
            if (source == null) return;

            if (!source.LogFolder.IsNullOrWhiteSpace())
                LogFolder = source.LogFolder;
            if (!source.LogFileName.IsNullOrWhiteSpace())
                LogFileName = source.LogFileName;
            if (!source.LogTimeStampFormat.IsNullOrWhiteSpace())
                LogTimeStampFormat = source.LogTimeStampFormat;
            //if (source.DoAppendProcessIdToLogDir != null)
                DoAppendProcessIdToLogDir = source.DoAppendProcessIdToLogDir;
        }

        public void Merge(object source)
        {
            Merge(source as ChoLogSettings);
        }
    }

    [XmlRoot("globalApplicationSettings")]
    public class ChoGlobalApplicationSettings : IChoInitializable /*, IChoObjectChangeWatcheable */, IChoMergeable<ChoGlobalApplicationSettings>
    {
        internal readonly static ChoGlobalApplicationSettings Default = new ChoGlobalApplicationSettings();
        internal static string AppFrxConfigFilePath = String.Empty;

        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoGlobalApplicationSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlElement("behaviourSettings")]
        public ChoApplicationBehaviourSettings ApplicationBehaviourSettings = new ChoApplicationBehaviourSettings();

        [XmlElement("trayApplicationBehaviourSettings")]
        public ChoTrayApplicationBehaviourSettings TrayApplicationBehaviourSettings = new ChoTrayApplicationBehaviourSettings();

        [XmlElement("logSettings")]
        public ChoLogSettings LogSettings = new ChoLogSettings();

        [XmlAttribute("applicationId")]
        public string ApplicationName = String.Empty;

        [XmlAttribute("eventLogSourceName")]
        public string EventLogSourceName = String.Empty;

        //[XmlAttribute("appConfigFilePath")]
        [XmlIgnore]
        internal string AppConfigFilePath = String.Empty;

        //[XmlAttribute("turnOnConsoleOutput")]
        //public bool TurnOnConsoleOutput = false;

        [XmlIgnore]
        internal string ApplicationConfigFilePath = String.Empty;

        [XmlIgnore]
        public string ApplicationNameWithoutExtension
        {
            get { return Path.GetFileNameWithoutExtension(ApplicationName); }
        }

        [XmlIgnore]
        public string HostName
        {
            get;
            private set;
        }

        [XmlIgnore]
        public readonly List<string> IPAddresses = new List<string>();

        [XmlIgnore]
        internal string ApplicationConfigDirectory;

        [XmlIgnore]
        public string ApplicationLogDirectory;

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
            ////msg.AppendFormatLine("TurnOnConsoleOutput: {0}", TurnOnConsoleOutput);
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
                        //Update overriables

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
            #region Get ApplicationName

            if (ApplicationName.IsNullOrWhiteSpace())
            {
                try
                {
                    ApplicationName = System.IO.Path.GetFileName(ChoAssembly.GetEntryAssemblyLocation());
                }
                catch (System.Security.SecurityException ex)
                {
                    ChoApplication.Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
                }
            }

            #endregion Get ApplicationName

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

            if (ApplicationBehaviourSettings == null)
                ApplicationBehaviourSettings = new ChoApplicationBehaviourSettings();

            if (TrayApplicationBehaviourSettings == null)
                TrayApplicationBehaviourSettings = new ChoTrayApplicationBehaviourSettings();

            if (LogSettings == null)
                LogSettings = new ChoLogSettings();

            ChoApplication.RaiseGlobalApplicationSettingsOverrides(this);
            Merge(ChoGlobalApplicationSettings.Default);

            PostInitialize();
        }

        internal void PostInitialize()
        {
            if (ApplicationName.IsNullOrEmpty())
                ChoEnvironment.Exit(101, "Missing ApplicationName.");

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
                ChoApplication.Trace(ChoTraceSwitch.Switch.TraceError, "Invalid LogTimeStampFormat '{0}' configured.".FormatString(LogSettings.LogTimeStampFormat));
                ChoApplication.Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
                LogSettings.LogTimeStampFormat = "yyyy-MM-dd hh:mm:ss.fffffff";
            }

            try
            {
                string sharedEnvConfigDir = null;

                if (!AppFrxConfigFilePath.IsNullOrEmpty())
                    sharedEnvConfigDir = Path.GetDirectoryName(ChoPath.GetFullPath(AppFrxConfigFilePath));

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
                        ApplicationConfigFilePath = AppConfigFilePath;
                }

                ApplicationConfigDirectory = Path.GetDirectoryName(ChoPath.GetFullPath(ApplicationConfigFilePath));
                AppFrxConfigFilePath = Path.Combine(ApplicationConfigDirectory, ChoReservedFileName.CoreFrxConfigFileName);
            }
            catch (System.Security.SecurityException ex)
            {
                // This security exception will occur if the caller does not have 
                // some undefined set of SecurityPermission flags.
                ChoApplication.Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
            }

            if (!LogSettings.LogFolder.IsNullOrWhiteSpace())
                ApplicationLogDirectory = ChoString.ExpandProperties(LogSettings.LogFolder, ChoEnvironmentVariablePropertyReplacer.Instance);

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

        public void Merge(ChoGlobalApplicationSettings source)
        {
            if (source == null) return;

            if (source.ApplicationBehaviourSettings != null)
                ApplicationBehaviourSettings.Merge(source.ApplicationBehaviourSettings);

            if (source.TrayApplicationBehaviourSettings != null)
                TrayApplicationBehaviourSettings.Merge(source.TrayApplicationBehaviourSettings);

            if (source.LogSettings != null)
                LogSettings.Merge(source.LogSettings);

            if (!source.ApplicationName.IsNullOrWhiteSpace())
                ApplicationName = source.ApplicationName;

            if (!source.EventLogSourceName.IsNullOrWhiteSpace())
                EventLogSourceName = source.EventLogSourceName;

            if (!source.AppConfigFilePath.IsNullOrWhiteSpace())
                AppConfigFilePath = source.AppConfigFilePath;

            //if (!source.TurnOnConsoleOutput)
            //    TurnOnConsoleOutput = source.TurnOnConsoleOutput;

            if (!source.ApplicationConfigFilePath.IsNullOrWhiteSpace())
                ApplicationConfigFilePath = source.ApplicationConfigFilePath;

            if (!source.ApplicationConfigDirectory.IsNullOrWhiteSpace())
                ApplicationConfigDirectory = source.ApplicationConfigDirectory;

            if (!source.ApplicationLogDirectory.IsNullOrWhiteSpace())
                ApplicationLogDirectory = source.ApplicationLogDirectory;
        }

        public void Merge(object source)
        {
            Merge(source as ChoGlobalApplicationSettings);
        }
    }
}
