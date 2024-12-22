namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
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
    using Cinchoo.Core.Windows.Forms;
    using Cinchoo.Core.Services;
    using System.Windows;
    using Cinchoo.Core.Compiler;
    using System.Threading;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoApplicationHostAttribute : Attribute
    {
    }

    //NOTE: Must have ApplicationHost defined in the entry assembly
    //Can't be abstract, because designer complains
    public class ChoApplicationHost //: Installer
    {
        #region Shared Data Members (Private)

        private static ConsoleCtrlMessageHandler _consoleCtrlHandler;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ChoIdleTask _idleTask = null;
        internal static ChoApplicationContext ApplicationContext;
        internal Exception _unhandledException = null;
        public static ChoApplicationHost Instance;

        #endregion Shared Data Members (Private)

        #region Events

        [CLSCompliant(false)]
        public static event ConsoleCtrlMessageHandler ConsoleCtrlMessageReceived;

        #endregion Events

        #region Shared Data Members (Internal)

        internal static bool IsApplicationHostUsed = false;

        #endregion Shared Data Members (Internal)

        #region Instance Data Members (Internal)

        internal ChoService Service;
        internal System.Windows.Application WinApp = null;

        #endregion Instance Data Members (Internal)

        #region Constructors

        static ChoApplicationHost()
        {
        }

        public ChoApplicationHost()
		{
            Instance = this;
            //if (ChoApplication.ApplicationMode == ChoApplicationMode.NA)
            //if (!ChoApplication.IsInitialized)
            //{
                if (!ChoApplication.InternalEventsSubscriped)
                {
                    ChoApplication.InternalEventsSubscriped = true;
                    ChoApplication.ApplyGlobalApplicationSettingsOverridesInternal += ((sender, e) =>
                        {
                            ChoServiceCommandLineArgs.OverrideFrxParams(e.Value);
                            ApplyGlobalApplicationSettingsOverrides(e.Value);
                        }
                    );
                    ChoApplication.AfterAppFrxSettingsLoadedInternal += ((sender, e) =>
                    {
                        if (e.Value is ChoServiceInstallerSettings)
                        {
                            ChoServiceInstallerSettings o = (ChoServiceInstallerSettings)e.Value;
                            o.BeforeInstall(() => BeforeInstall());
                            o.AfterInstall(() => AfterInstall());
                            o.BeforeUninstall(() => BeforeUninstall());
                            o.AfterUninstall(() => AfterUninstall());
                        }

                        AfterAppFrxSettingsLoaded(e.Value);
                    }
                    );
                    ChoApplication.ApplyMetaDataFilePathSettingsOverridesInternal += ((sender, e) =>
                    {
                        ApplyMetaDataFilePathSettingsOverrides(e.Value);
                    }
                    );
                    ChoApplication.AfterNotifyIconConstructedInternal += ((sender, e) =>
                    {
                        AfterNotifyIconConstructed(e.Value);
                    }
                    );
                }
                //ChoApplication.ApplyFrxParamsOverridesInternal += ((sender, e) =>
                //{
                //    ApplyFrxParamsOverrides(e.GlobalApplicationSettings, e.MetaDataFilePathSettings);
                    
                //    if (_initialized) return;
                //    _initialized = true;

                //    ChoServiceCommandLineArgs.OverrideFrxParams(e);
                //});

                if (Environment.UserInteractive)
                {
                    if (this.GetMainWindowObject() != null)
                    {
                        if (ChoApplication.ApplicationMode == ChoApplicationMode.NA)
                        {
                            if (GetMainWindowObject() is Window
                                || GetMainWindowObject() is Form)
                            {
                                ChoApplication.ApplicationMode = ChoApplicationMode.Windows;
                                if (GetMainWindowObject() is Window)
                                    ChoApplication.WindowsAppType = ChoWindowsAppType.WPF;
                                else
                                    ChoApplication.WindowsAppType = ChoWindowsAppType.WinForms;
                            }
                            else
                                ChoApplication.ApplicationMode = ChoApplicationMode.Console;
                        }
                        //if (!(this is IChoWindowApp))
                        //    throw new ChoApplicationException("ApplicationHost must derive from IChoWindowApp for Windows application.");
                    }

                    //ChoApplicationMode? applicationMode = ChoFrameworkCmdLineArgs.GetApplicationMode();
                    //if (applicationMode != null)
                    //    ChoApplication.ApplicationMode = applicationMode.Value;

                    if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                    {
                        //if (ChoApplication.WindowsAppType == ChoWindowsAppType.NA)
                        //    ChoApplication.ApplicationMode = ChoApplicationMode.Console;
                    }
                }


                ChoApplication.FatalApplicationException += FatalApplicationException;

                //if (Environment.UserInteractive)
                //{
                //    if (!ChoServiceCommandLineArgs.HasServiceParams())
                //    {
                //        if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                //        {
                //            if (!ChoAppFrxSettings.Me.DoNotShowEnvSelectionWnd)
                //            {
                //                ChoChooseEnvironmentFrm.Show();
                //                //frm1 = new ChoChooseEnvironmentFrm();
                //                //if (frm1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                //                //    Environment.Exit(-101);
                //            }
                //        }
                //    }
                //}
            //}
            //else
            //    ApplyFrxParamsOverrides(ChoGlobalApplicationSettings.Me, ChoMetaDataFilePathSettings.Me);

            //ChoApplication.Initialize();
            ChoGlobalApplicationSettings x = ChoGlobalApplicationSettings.Me;
            ChoApplication.WriteToEventLog(ChoServiceInstallerSettings.Me.ToString());
        }

        #endregion Constructors

        #region Service Related Events Handlers

        internal void TerminateService(Exception ex)
        {
            _unhandledException = ex;

            if (Service != null)
                Service.Stop();
        }

        protected virtual void ApplyServiceParametersOverrides(ChoServiceInstallerSettings obj)
        {
        }

        protected virtual void ApplyGlobalApplicationSettingsOverrides(ChoGlobalApplicationSettings obj)
        {
        }

        protected virtual void AfterAppFrxSettingsLoaded(object obj)
        {
        }

        protected virtual void ApplyMetaDataFilePathSettingsOverrides(ChoMetaDataFilePathSettings obj)
        {
        }

        protected virtual void AfterNotifyIconConstructed(ChoNotifyIcon ni)
        {
        }

        public void RequestAdditionalTime(int milliseconds)
        {
            if (Service != null)
                Service.RequestAdditionalTime(milliseconds);
        }

        protected virtual void OnStart(string[] args)
        {
            //if (ChoApplication.ServiceInstallation) return;

            //if (ChoApplication.ApplicationMode != ChoApplicationMode.Service)
            //{
            //    if (_idleTask != null)
            //        _idleTask.Stop();

            //    if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
            //    {
            //        ApplicationContext = new ChoApplicationContext(this);
            //        System.Windows.Forms.Application.Run(ApplicationContext);

            //        OnStop();
            //    }
                //else
                //{
                //    if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                //    {
                //        if (ChoApplication.WindowsAppType == ChoWindowsAppType.WinForms)
                //        {
                //            ApplicationContext = new ChoApplicationContext(this);
                //            System.Windows.Forms.Application.Run(ApplicationContext);
                //        }
                //        else if (ChoApplication.WindowsAppType == ChoWindowsAppType.WPF)
                //        {
                //            System.Windows.Application app = ApplicationObject as System.Windows.Application;
                //            if (app == null)
                //            {
                //                ApplicationContext = new ChoApplicationContext(this);
                //                System.Windows.Forms.Application.Run(ApplicationContext);
                //            }
                //            else
                //            {
                //                app.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);
                //                app.Run(MainWindowObject as Window);
                //            }
                //        }
                //    }
                //}

                //if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows
                //    || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                //{
                //    //if (this.MainWindow == null)
                //    //    throw new ChoApplicationException("Missing main window. Windows application must have main window specified.");

                //    //app = SystemApp as System.Windows.Application;
                //    ApplicationContext = new ChoApplicationContext(this);
                //    System.Windows.Forms.Application.Run(ApplicationContext);

                //    OnStop();
                //}
                //else
                //{
                //    ApplicationContext = new ChoApplicationContext(null);
                //}

                //if (ApplicationContext != null)
                //{
                //    //ApplicationContext.Visible = true;
                //    if (ChoApplication.WindowsAppType == ChoWindowsAppType.WPF && app != null)
                //    {
                //        //if (app == null)
                //        //    throw new ChoApplicationException("Missing System.Windows.Application object.");

                //        if (System.Windows.Application.Current != null)
                //            System.Windows.Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);

                //        app.Run(MainWindow as Window);
                //    }
                //    else if (ChoApplication.WindowsAppType == ChoWindowsAppType.WinForms
                //        || ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                //        System.Windows.Forms.Application.Run(ApplicationContext);

                //    OnStop();
                //}
            //}
            //else
            //{
                //_idleTask = new ChoIdleTask();
                //_idleTask.Start();
            //}
        }

        protected virtual void OnStop()
        {
            //if (ChoApplication.ServiceInstallation) return;

            //if (_idleTask != null)
            //{
            //    //_idleTask.Stop();
            //    //_idleTask = null;
            //}
            //else
            //{
            //}

            //if (_unhandledException != null)
            //    throw _unhandledException;

            //ChoConsole.PauseLine();
            //ChoAppDomain.Exit();
        }

        protected virtual void BeforeInstall()
        {
        }

        protected virtual void AfterInstall()
        {
        }

        protected virtual void BeforeUninstall()
        {
        }

        protected virtual void AfterUninstall()
        {
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
                OnStopService();
            }
            catch (Exception ex)
            {
                ChoApplication.WriteToEventLog(ex.ToString());
            }
        }

        #endregion Service Related Events Handlers

        #region Internal Members

        internal void OnStartService(string[] args)
        {
            if (args == null || args.Length == 0)
                ChoEnvironment.CommandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
            else
                ChoEnvironment.CommandLineArgs = args;

            if (ChoApplication.ApplicationMode == ChoApplicationMode.Service)
            {
                if (ChoServiceInstallerSettings.Me.Timeout <= 0)
                {
                    ChoQueuedExecutionService.GetService("Local").Enqueue(() =>
                    {
                        OnStart(ChoEnvironment.CommandLineArgs);
                    },
                    (result) =>
                    {
                        result.EndInvoke();
                    },
                    null);
                }
                else
                {
                    OnStart(ChoEnvironment.CommandLineArgs);
                }
            }
            else
            {
                OnStart(ChoEnvironment.CommandLineArgs);
                PostStart();
            }
        
        }

        private void PostStart()
        {
            if (ChoApplication.ServiceInstallation) return;

            if (ChoApplication.ApplicationMode != ChoApplicationMode.Service)
            {
                if (_idleTask != null)
                    _idleTask.Stop();

                if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                {
                    WinApp = ApplicationObject as System.Windows.Application;
                    //if (WinApp == null)
                    //    WinApp = ChoWPFDefaultApplication.Default;

                    if (WinApp != null)
                        WinApp.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);

                    if (ChoApplication.WindowsAppType == ChoWindowsAppType.NA)
                    {
                        if (GetMainWindowObject() is Window)
                            ChoApplication.WindowsAppType = ChoWindowsAppType.WPF;
                        else if (GetMainWindowObject() is Form)
                            ChoApplication.WindowsAppType = ChoWindowsAppType.WinForms;
                    }

                    if (ChoApplication.WindowsAppType == ChoWindowsAppType.WinForms)
                    {
                        ApplicationContext = new ChoApplicationContext(this);
                        ApplicationContext.Run();
                        //System.Windows.Forms.Application.Run(ApplicationContext);
                    }
                    else if (ChoApplication.WindowsAppType == ChoWindowsAppType.WPF)
                    {
                        ApplicationContext = new ChoApplicationContext(this);
                        ApplicationContext.Run();
                        
                        //if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                        //    WinApp.Run(GetMainWindowObject() as Window);
                        //else
                        //{
                        //    ApplicationContext = new ChoApplicationContext(this);
                        //    ApplicationContext.Run();
                        //    System.Windows.Forms.Application.Run(ApplicationContext);
                        //}
                    }
                    else
                    {
                        if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                        {
                            ApplicationContext = new ChoApplicationContext(this);
                            ApplicationContext.Run();
                            //ApplicationContext = new ChoApplicationContext(this);
                            //System.Windows.Forms.Application.Run(ApplicationContext);
                        }
                    }
                }
                else
                {
                    if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                    {
                        ApplicationContext = new ChoApplicationContext(this);
                        if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                            System.Windows.Forms.Application.Run(ApplicationContext);
                    }
                }
            }

            //if (ChoApplication.ApplicationMode != ChoApplicationMode.Service)
            //{
            //    if (_idleTask != null)
            //        _idleTask.Stop();

            //    if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
            //    {
            //        ApplicationContext = new ChoApplicationContext(this);
            //        System.Windows.Forms.Application.Run(ApplicationContext);

            //        OnStop();
            //    }
                //else
                //{
                //    if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                //    {
                //        if (ChoApplication.WindowsAppType == ChoWindowsAppType.WinForms)
                //        {
                //            ApplicationContext = new ChoApplicationContext(this);
                //            System.Windows.Forms.Application.Run(ApplicationContext);
                //        }
                //        else if (ChoApplication.WindowsAppType == ChoWindowsAppType.WPF)
                //        {
                //            System.Windows.Application app = ApplicationObject as System.Windows.Application;
                //            if (app == null)
                //            {
                //                ApplicationContext = new ChoApplicationContext(this);
                //                System.Windows.Forms.Application.Run(ApplicationContext);
                //            }
                //            else
                //            {
                //                app.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);
                //                app.Run(MainWindowObject as Window);
                //            }
                //        }
                //    }
                //}

                //if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows
                //    || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                //{
                //    //if (this.MainWindow == null)
                //    //    throw new ChoApplicationException("Missing main window. Windows application must have main window specified.");

                //    //app = SystemApp as System.Windows.Application;
                //    ApplicationContext = new ChoApplicationContext(this);
                //    System.Windows.Forms.Application.Run(ApplicationContext);

                //    OnStop();
                //}
                //else
                //{
                //    ApplicationContext = new ChoApplicationContext(null);
                //}

                //if (ApplicationContext != null)
                //{
                //    //ApplicationContext.Visible = true;
                //    if (ChoApplication.WindowsAppType == ChoWindowsAppType.WPF && app != null)
                //    {
                //        //if (app == null)
                //        //    throw new ChoApplicationException("Missing System.Windows.Application object.");

                //        if (System.Windows.Application.Current != null)
                //            System.Windows.Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);

                //        app.Run(MainWindow as Window);
                //    }
                //    else if (ChoApplication.WindowsAppType == ChoWindowsAppType.WinForms
                //        || ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                //        System.Windows.Forms.Application.Run(ApplicationContext);

                //    OnStop();
                //}
            //}
            //else
            //{
            //    //_idleTask = new ChoIdleTask();
            //    //_idleTask.Start();
            //}
        }

        internal void OnStopService()
        {
            OnStop();
            PostStop();
        }

        private void PostStop()
        {
            //if (ChoApplication.ServiceInstallation) return;

            if (_idleTask != null)
            {
                //_idleTask.Stop();
                //_idleTask = null;
            }
            else
            {
            }

            if (_unhandledException != null)
                throw _unhandledException;

            //ChoConsole.PauseLine();
            ChoAppDomain.Exit();
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
            if (!ChoConsoleSettings.Me.DisableConsoleCtrlHandler)
            {
                _consoleCtrlHandler = new ConsoleCtrlMessageHandler(ConsoleCtrlHandler);
                GC.KeepAlive(_consoleCtrlHandler);
                ChoKernel32.SetConsoleCtrlHandler(_consoleCtrlHandler, true);
            }
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
            ChoFramework.Shutdown();

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

        #region Instance Members (Public)

        protected virtual void OnWindowMinimize(ChoNotifyIcon notifyIcon)
        {
        }

        internal void OnWindowMinimizeInternal(ChoNotifyIcon notifyIcon)
        {
            OnWindowMinimize(notifyIcon);
        }

        public virtual void ShowMessage(string message)
        {
            if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
            {
                Console.WriteLine(message);
            }
            else if (ChoApplication.ApplicationMode == ChoApplicationMode.Service)
            {
                Trace.WriteLine(message);
            }
            else if (ChoApplication.ApplicationMode == ChoApplicationMode.Web)
            {
                Trace.WriteLine(message);
            }
            else if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
            {
                if (ChoApplication.WindowsAppType == ChoWindowsAppType.WinForms)
                {
                    System.Windows.Forms.MessageBox.Show(message, ChoApplication.AppDomainName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ChoApplication.WindowsAppType == ChoWindowsAppType.WPF)
                {
                    System.Windows.MessageBox.Show(message, ChoApplication.AppDomainName, System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                else
                    Trace.WriteLine(message);
            }
        }

        public void TraceMessage(string message)
        {
            TraceMessage(true, message);
        }

        public virtual void TraceMessage(bool condition, string message)
        {
            if (!condition) return;

            if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
            {
                Console.WriteLine(message);
                Trace.WriteLine(message);
            }
            else if (ChoApplication.ApplicationMode == ChoApplicationMode.Service)
            {
                Trace.WriteLine(message);
            }
            else if (ChoApplication.ApplicationMode == ChoApplicationMode.Web)
            {
                Trace.WriteLine(message);
            }
            else if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
            {
                Trace.WriteLine(message);
            }
        }

        #endregion

        private object _mainWindowObject = null;
        private bool _isMainWindowObjectInitialized = false;

        internal object GetMainWindowObject()
        {
            if (_isMainWindowObjectInitialized) return _mainWindowObject;

            _isMainWindowObjectInitialized = true;
            if (ChoFrameworkCmdLineArgs.GetApplicationMode() != null
                && ChoFrameworkCmdLineArgs.GetApplicationMode().Value == ChoApplicationMode.Console)
            //if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
            {
            }
            else
                _mainWindowObject = MainWindowObject;

            return _mainWindowObject;
        }

        public virtual object MainWindowObject
        {
            get { return null; }
        }

        public virtual object ApplicationObject
        {
            get { return null; }
        }

        public bool IsWindowApp
        {
            get
            {
                return GetMainWindowObject() is Form || GetMainWindowObject() is Window;
            }
        }

        public virtual void OnTrayAppAboutMenuClicked(object sender, EventArgs e)
        {
        }

        public virtual void OnTrayAppHelpMenuClicked(object sender, EventArgs e)
        {
        }

        public virtual void OnTrayAppExitMenuClicked(object sender, EventArgs e)
        {
            if (ApplicationContext != null)
                ApplicationContext.ExitThread();
        }

        public virtual void OnTrayAppOpenMenuClicked(object sender, EventArgs e)
        {
            if (ApplicationContext != null)
                ApplicationContext.ShowMenuItemClicked();
        }
    }
}
