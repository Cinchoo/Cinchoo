namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Linq;
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
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Configuration.Install;
    using Microsoft.Win32;

    #endregion NameSpaces

    public sealed class ChoService : ServiceBase
    {
        #region Instance Data Members (Private)

        private ChoApplicationHost _host;
        private const string RegRunSubKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{0}";

        #endregion Instance Data Members (Private)

        public ChoService(ChoApplicationHost host)
        {
            ChoGuard.ArgumentNotNull(host, "Host");
            _host = host;
            _host.Service = this;
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

        internal static void Initialize(ChoApplicationHost host)
        {
            if (!ChoApplicationHost.IsApplicationHostUsed)
                return;

            if (!Environment.UserInteractive)
            {
                //Debugger.Break();
                ChoService service = new ChoService(host);
                
                service.CanHandlePowerEvent = ChoServiceInstallerSettings.Me.CanHandlePowerEvent;
                service.CanHandleSessionChangeEvent = ChoServiceInstallerSettings.Me.CanHandleSessionChangeEvent;
                service.CanPauseAndContinue = ChoServiceInstallerSettings.Me.CanPauseAndContinue;
                service.CanShutdown = ChoServiceInstallerSettings.Me.CanShutdown;
                service.CanStop = ChoServiceInstallerSettings.Me.CanStop;
                service.AutoLog = ChoServiceInstallerSettings.Me.AutoLog;
                //service.ExitCode = ChoServiceInstallerSettings.Me.ExitCode;

                //Windows Service Mode
                ServiceBase[] ServicesToRun = new ServiceBase[]
				{
					service
				};
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                //Parse command line arguments, install, 
                //try
                //{
                    ChoFrameworkCmdLineArgs frameworkCmdLineArgs = new ChoFrameworkCmdLineArgs();
                    frameworkCmdLineArgs.Init();
                    ChoServiceCommandLineArgs serviceCmdLineArgs = new ChoServiceCommandLineArgs();
                    serviceCmdLineArgs.Init();

                    ServiceController sc = new ServiceController(ChoServiceCommandLineArgs.GetServiceName(), Environment.MachineName);
                    if (serviceCmdLineArgs.InstallService)
                    {
                        ChoManagedInstallerClass.InstallService();

                        //Save the command line parameters
                        //if (serviceCmdLineArgs.ServiceParams != null)
                        //{
                        //    try
                        //    {
                        //        ChoRegistryKey _rkAppRun = new ChoRegistryKey(String.Format(RegRunSubKey.FormatString(ChoApplication.Host.ServiceName), true));
                        //        _rkAppRun.SetValue("ImagePath", "{0} {1}".FormatString(ChoAssembly.GetEntryAssembly().Location, ChoServiceCommandLineArgs.GetServiceParams().Replace("'", @"""")));
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        System.Diagnostics.Trace.TraceError(ex.ToString());
                        //    }
                        //}
                    }
                    else if (serviceCmdLineArgs.UninstallService)
                    {
                        ChoManagedInstallerClass.UninstallService();
                    }
                    else if (serviceCmdLineArgs.StartService)
                    {
                        if (!ChoWindowsIdentity.IsAdministrator())
                        {
                            ChoApplication.RestartAsAdmin();
                            return;
                        }
                        
                        if (serviceCmdLineArgs.ServiceParams == null ||
                            serviceCmdLineArgs.ServiceParams.Length == 0)
                        {
                            //ChoEnvironment.CommandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
                        }
                        else
                        {
                            string commandLineArgs = null;
                            if (serviceCmdLineArgs.ServiceParams.StartsWith("\"")
                                && serviceCmdLineArgs.ServiceParams.EndsWith("\""))
                                commandLineArgs = serviceCmdLineArgs.ServiceParams.Substring(1, serviceCmdLineArgs.ServiceParams.Length - 2);
                            else
                                commandLineArgs = serviceCmdLineArgs.ServiceParams;

                            ChoEnvironment.CommandLineArgs = commandLineArgs.SplitNTrim(' ');
                        }

                        sc.Start(ChoEnvironment.CommandLineArgs);

                        if (ChoServiceInstallerSettings.Me.Timeout > 0)
                            sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(ChoServiceInstallerSettings.Me.Timeout));
                    }
                    else if (serviceCmdLineArgs.StopService)
                    {
                        if (!ChoWindowsIdentity.IsAdministrator())
                        {
                            ChoApplication.RestartAsAdmin();
                            return;
                        }
                        sc.Stop();
                    }
                    else if (serviceCmdLineArgs.PauseService)
                    {
                        if (!ChoWindowsIdentity.IsAdministrator())
                        {
                            ChoApplication.RestartAsAdmin();
                            return;
                        }
                        sc.Pause();
                    }
                    else if (serviceCmdLineArgs.ContinueService)
                    {
                        if (!ChoWindowsIdentity.IsAdministrator())
                        {
                            ChoApplication.RestartAsAdmin();
                            return;
                        }
                        sc.Continue();
                    }
                    else if (serviceCmdLineArgs.ExecuteCommand != Int32.MinValue)
                    {
                        if (!ChoWindowsIdentity.IsAdministrator())
                        {
                            ChoApplication.RestartAsAdmin();
                            return;
                        }
                        sc.ExecuteCommand(serviceCmdLineArgs.ExecuteCommand);
                    }
                    else
                    {
                        //ChoApplicationMode? applicationMode = ChoFrameworkCmdLineArgs.GetApplicationMode();
                        //if (applicationMode != null)
                        //    ChoApplication.ApplicationMode = applicationMode.Value;

                        //ChoProfile.WriteLine(ChoApplication.ApplicationMode.ToString());
                        if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                        {
                            ChoApplication.Host.OnStartService(ChoEnvironment.CommandLineArgs);
                        }
                        else if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
                        {
                            if (ChoConsoleSettings.Me.ConsoleMode != uint.MinValue && ChoWindowsManager.ConsoleWindowHandle != IntPtr.Zero)
                                ChoKernel32.SetConsoleMode(ChoWindowsManager.ConsoleWindowHandle, (uint)ChoConsoleSettings.Me.ConsoleMode);

                            ChoApplicationHost.RegisterConsoleControlHandler();
                            ChoApplication.Host.OnStartService(ChoEnvironment.CommandLineArgs);
                            //ChoApplication.Host.OnStopService();
                        }
                        else if (ChoApplication.ApplicationMode == ChoApplicationMode.Web)
                            ChoApplication.Host.OnStartService(ChoEnvironment.CommandLineArgs);
                    }
                //}
                //catch (ChoFatalApplicationException)
                //{
                //    throw;
                //}
                //catch (ChoCommandLineArgException argEx)
                //{
                //    ChoApplication.DisplayMsg(argEx.Message);
                //    throw;
                //}
                //catch (ChoCommandLineArgUsageException usageEx)
                //{
                //    ChoApplication.DisplayMsg(usageEx.Message);
                //    throw;
                //}
                //catch (Exception ex)
                //{
                //    ChoApplication.DisplayMsg(ex.Message, ex);
                //    throw;
                //}
            }
        }
    }
}
