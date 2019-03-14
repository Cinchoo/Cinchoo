namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.ServiceProcess;
    using System.Configuration.Install;
    using System.ComponentModel;
    using System.ServiceProcess;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Shell;
    using Cinchoo.Core.Win32;
    using System.Diagnostics;
    using Cinchoo.Core.Threading.Tasks;
    using System.Windows.Forms;
    using Cinchoo.Core.Forms;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public class ChoApplicationHost : Installer
    {
        #region Shared Data Members (Private)

        private static ConsoleCtrlMessageHandler _consoleCtrlHandler;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ChoIdleTask _idleTask;
        internal static ChoApplicationContext ApplicationContext;

        #endregion Shared Data Members (Private)

        #region Events

        [CLSCompliant(false)]
        public static event ConsoleCtrlMessageHandler ConsoleCtrlMessageReceived;

        #endregion Events

        #region Shared Data Members (Internal)

        internal static bool IsApplicationHostUsed = false;

        #endregion Shared Data Members (Internal)

        #region Instance Data Members (Internal)

        internal string ServiceName;
        internal string[] Args;

        #endregion Instance Data Members (Internal)

        #region Constructors

        public ChoApplicationHost()
		{
            ChoApplication.ApplyFrxParamsOverrides += ((sender, e) =>
            {
                ApplyFrxParamsOverrides(e.GlobalApplicationSettings, e.MetaDataFilePathSettings);

                using (ChoCommandLineArgParser parser = new ChoCommandLineArgParser())
                {
                    parser.Parse();
                    foreach (KeyValuePair<string, string> keyValuePair in parser)
                    {
                        if (keyValuePair.Key.ToUpper() == "I")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.InstallService".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.InstallService".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.InstallService".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                        else if (keyValuePair.Key.ToUpper() == "U")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.UninstallService".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.UninstallService".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.UninstallService".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                        else if (keyValuePair.Key.ToUpper() == "S")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.StartService".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.StartService".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.StartService".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                        else if (keyValuePair.Key.ToUpper() == "T")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.StopService".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.StopService".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.StopService".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                        else if (keyValuePair.Key.ToUpper() == "P")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.PauseService".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.PauseService".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.PauseService".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                        else if (keyValuePair.Key.ToUpper() == "C")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.ContinueService".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.ContinueService".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.ContinueService".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                        else if (keyValuePair.Key.ToUpper() == "E")
                        {
                            ChoApplication.ServiceInstallation = true;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.HideWindow = false;
                            e.GlobalApplicationSettings.ApplicationBehaviourSettings.SingleInstanceApp = false;
                            e.GlobalApplicationSettings.DoAppendProcessIdToLogDir = false;
                            e.GlobalApplicationSettings.ApplicationName = "{0}.ExecuteCommand".FormatString(e.GlobalApplicationSettings.ApplicationNameWithoutExtension);
                            e.GlobalApplicationSettings.EventLogSourceName = "{0}.ExecuteCommand".FormatString(e.GlobalApplicationSettings.EventLogSourceName);
                            e.GlobalApplicationSettings.LogSettings.LogFileName = "{0}.ExecuteCommand".FormatString(e.GlobalApplicationSettings.LogSettings.LogFileName);
                            break;
                        }
                    }
                }
            });

            ChoApplication.FatalApplicationException += FatalApplicationException;

            //ChoApplication.Initialize();
            ChoGlobalApplicationSettings x = ChoGlobalApplicationSettings.Me;
            ApplyServiceInstallParamsOverrides(ChoServiceProcessInstallerSettings.Me, ChoServiceInstallerSettings.Me);
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();

            ChoApplication.WriteToEventLog(ChoServiceProcessInstallerSettings.Me.ToString());
            processInstaller.Account = ChoServiceProcessInstallerSettings.Me.Account;
            processInstaller.Username = ChoServiceProcessInstallerSettings.Me.UserName;
            processInstaller.Password = ChoServiceProcessInstallerSettings.Me.Password;

            ChoApplication.WriteToEventLog(ChoServiceInstallerSettings.Me.ToString());
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceInstaller.DisplayName = ChoServiceInstallerSettings.Me.DisplayName.IsNullOrWhiteSpace() ? ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension : ChoServiceInstallerSettings.Me.DisplayName;
            ServiceName = serviceInstaller.ServiceName = ChoServiceInstallerSettings.Me.ServiceName.IsNullOrWhiteSpace() ? ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension : ChoServiceInstallerSettings.Me.ServiceName;
            serviceInstaller.StartType = ChoServiceInstallerSettings.Me.ServiceStartMode;
            serviceInstaller.DelayedAutoStart = ChoServiceInstallerSettings.Me.DelayedAutoStart;
            serviceInstaller.Description = ChoServiceInstallerSettings.Me.Description;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }

        #endregion Constructors

        #region Service Related Events Handlers

        protected virtual void OnStart(string[] args)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Service)
            {
                if (_idleTask != null)
                    _idleTask.Stop();

                if (this is IChoWinFormApp)
                {
                    ApplicationContext = new ChoApplicationContext(this as IChoWinFormApp);
                }
                else
                {
                    ApplicationContext = new ChoApplicationContext(null);
                }

                if (ApplicationContext != null)
                {
                    //ApplicationContext.Visible = true;
                    Application.Run(ApplicationContext);
                }
                OnStop();
            }
            else
            {
                _idleTask = new ChoIdleTask();
                _idleTask.Start();
            }
        }

        protected virtual void OnStop()
        {
            if (_idleTask != null)
            {
                _idleTask.Stop();
                _idleTask = null;
            }
            else
            {
            }
            ChoAppDomain.Exit();
        }

        protected virtual void OnContinue()
        {
        }

        protected virtual void OnCustomCommand(int Command)
        {
        }

        protected virtual void OnPause()
        {
        }

        protected virtual bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return true;
        }

        protected virtual void OnSessionChange(SessionChangeDescription changeDescription)
        {
        }

        protected virtual void OnShutdown()
        {
            try
            {
                OnStop();
            }
            catch (Exception ex)
            {
                ChoApplication.WriteToEventLog(ex.ToString());
            }
        }

        #endregion Service Related Events Handlers

        #region Instance Members (Protected)

        protected virtual void ApplyServiceInstallParamsOverrides(ChoServiceProcessInstallerSettings serviceProcessInstallerSettings, ChoServiceInstallerSettings serviceInstallerSettings)
        {
        }

        protected virtual void ApplyFrxParamsOverrides(ChoGlobalApplicationSettings globalApplicationSettings, ChoMetaDataFilePathSettings metaDataFilePathSettings)
        {
        }

        #endregion Instance Members (Protected)

        #region Internal Members

        internal void OnStartService(string[] args)
        {
            if (ChoApplication.ApplicationMode == ChoApplicationMode.Service)
            {
                ChoQueuedExecutionService.GetService("Local").Enqueue(() =>
                {
                    OnStart(args);
                });
            }
            else
                OnStart(args);
        }

        internal void OnStopService()
        {
            OnStop();
        }

        internal void OnContinueService()
        {
            OnContinue();
        }

        internal void OnCustomCommandService(int Command)
        {
            OnCustomCommand(Command);
        }

        internal void OnPauseService()
        {
            OnPause();
        }

        internal bool OnPowerEventService(PowerBroadcastStatus powerStatus)
        {
            return OnPowerEvent(powerStatus);
        }

        internal void OnSessionChangeService(SessionChangeDescription changeDescription)
        {
            OnSessionChange(changeDescription);
        }

        internal void OnShutdownService()
        {
            OnShutdown();
        }

        internal static void RegisterConsoleControlHandler()
        {
            _consoleCtrlHandler = new ConsoleCtrlMessageHandler(ConsoleCtrlHandler);
            GC.KeepAlive(_consoleCtrlHandler);
            ChoKernel32.SetConsoleCtrlHandler(_consoleCtrlHandler, true);
        }

        // A private static handler function in the MyApp class.
        internal static bool ConsoleCtrlHandler(CtrlTypes ctrlType)
        {
            ConsoleCtrlMessageHandler consoleCtrlMessageHandler = ChoApplicationHost.ConsoleCtrlMessageReceived;

            if (consoleCtrlMessageHandler != null)
                return consoleCtrlMessageHandler(ctrlType);
            else
                return OnConsoleCtrlMessage(ctrlType);
        }

        private void FatalApplicationException(object sender, ChoFatalErrorEventArgs e)
        {
            try
            {
                ChoApplication.WriteToEventLog(e.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            catch
            {
                //TODO: Write to system log
            }
        }

        private static bool OnConsoleCtrlMessage(CtrlTypes ctrlType)
        {
            String message = "This message should never be seen!";

            // A switch to handle the event type.
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    message = "A CTRL_C_EVENT was raised by the user.";
                    break;
                case CtrlTypes.CTRL_BREAK_EVENT:
                    message = "A CTRL_BREAK_EVENT was raised by the user.";
                    break;
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    message = "A CTRL_CLOSE_EVENT was raised by the user.";
                    break;
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                    message = "A CTRL_LOGOFF_EVENT was raised by the user.";
                    break;
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    message = "A CTRL_SHUTDOWN_EVENT was raised by the user.";
                    break;
            }

            throw new ChoConsoleCtrlException(message);
        }

        #endregion Internal Members
    }
}
