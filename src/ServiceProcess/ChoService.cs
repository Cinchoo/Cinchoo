namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ServiceProcess;
    using Cinchoo.Core.Shell;
    using System.Configuration.Install;
    using Cinchoo.Core.Reflection;
    using System.Reflection;
    using System.ComponentModel;
    using Cinchoo.Core.Win32;
    using System.Diagnostics;

    #endregion NameSpaces

    public sealed class ChoService : ServiceBase
    {
        #region Instance Data Members (Private)

        private ChoApplicationHost _host;

        #endregion Instance Data Members (Private)

        public ChoService()
        {
            try
            {
                //Dicover Service Installer
                Assembly entryAssembly = ChoAssembly.GetEntryAssembly();

                Type runInstallerType = null;
                if (entryAssembly != null)
                {
                    foreach (Type type in entryAssembly.GetTypes())
                    {
                        RunInstallerAttribute runInstallerAttribute = type.GetCustomAttribute<RunInstallerAttribute>();
                        if (runInstallerAttribute == null)
                            continue;

                        if (typeof(ChoApplicationHost).IsAssignableFrom(type))
                        {
                            runInstallerType = type;
                            break;
                        }
                    }
                }


                if (runInstallerType != null)
                    _host = Activator.CreateInstance(runInstallerType) as ChoApplicationHost;
                else
                    ChoApplication.WriteToEventLog("No type found with RunInstallerAttribute in the entry assembly.");
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ChoApplication.WriteToEventLog(ex.ToString());
            }
        }

        #region ServiceBase Overrides

        protected override void OnStart(string[] args)
        {
            ChoApplication.WriteToEventLog("OnStart()...");
            if (_host != null)
                _host.OnStartService(args);
        }

        protected override void OnStop()
        {
            ChoApplication.WriteToEventLog("OnStop()...");
            if (_host != null)
                _host.OnStopService();
        }

        protected override void OnContinue()
        {
            ChoApplication.WriteToEventLog("OnContinue()...");
            if (_host != null)
                _host.OnContinueService();
        }

        protected override void OnCustomCommand(int command)
        {
            ChoApplication.WriteToEventLog("OnCustomCommand()...");
            if (_host != null)
                _host.OnCustomCommandService(command);
        }

        protected override void OnPause()
        {
            ChoApplication.WriteToEventLog("OnPause()...");
            if (_host != null)
                _host.OnPauseService();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            ChoApplication.WriteToEventLog("OnPowerEvent()...");
            if (_host != null)
                return _host.OnPowerEventService(powerStatus);

            return true;
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            ChoApplication.WriteToEventLog("OnSessionChange()...");
            if (_host != null)
                _host.OnSessionChangeService(changeDescription);
        }

        protected override void OnShutdown()
        {
            ChoApplication.WriteToEventLog("OnShutdown()...");
            if (_host != null)
                _host.OnShutdownService();
        }

        #endregion ServiceBase Overrides

        internal static void Initialize()
        {
            if (!ChoApplicationHost.IsApplicationHostUsed)
                return;

            if (!Environment.UserInteractive)
            {
                //Windows Service Mode
                ServiceBase[] ServicesToRun = new ServiceBase[]
				{
					new ChoService()
				};
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                //Parse command line arguments, install, 
                try
                {
                    ChoServiceCommandLineArgs serviceCmdLineArgs = new ChoServiceCommandLineArgs();

                    ServiceController sc = new ServiceController(ChoApplication.Host.ServiceName, Environment.MachineName);
                    if (serviceCmdLineArgs.InstallService)
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { ChoApplication.EntryAssemblyLocation });
                    }
                    else if (serviceCmdLineArgs.UninstallService)
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", ChoApplication.EntryAssemblyLocation });
                    }
                    else if (serviceCmdLineArgs.StartService)
                    {
                        sc.Start();
                    }
                    else if (serviceCmdLineArgs.StopService)
                    {
                        sc.Stop();
                    }
                    else if (serviceCmdLineArgs.PauseService)
                    {
                        sc.Pause();
                    }
                    else if (serviceCmdLineArgs.ContinueService)
                    {
                        sc.Continue();
                    }
                    else if (serviceCmdLineArgs.ExecuteCommand != Int32.MinValue)
                    {
                        sc.ExecuteCommand(serviceCmdLineArgs.ExecuteCommand);
                    }
                    else
                    {
                        //Console mode
                        if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
                        {
                            if (ChoConsoleSettings.Me.ConsoleMode != uint.MinValue && ChoWindowsManager.ConsoleWindowHandle != IntPtr.Zero)
                                ChoKernel32.SetConsoleMode(ChoWindowsManager.ConsoleWindowHandle, ChoConsoleSettings.Me.ConsoleMode);

                            ChoApplicationHost.RegisterConsoleControlHandler();
                        }
                        ChoApplication.Host.OnStartService(ChoApplication.Host.Args);
                    }
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ChoApplication.WriteToEventLog(ex.ToString(), EventLogEntryType.Error);
                }
            }
        }
    }
}
