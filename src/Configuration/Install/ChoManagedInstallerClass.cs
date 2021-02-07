using Cinchoo.Core.ServiceProcess;
using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Cinchoo.Core.Configuration.Install
{
    public static class ChoManagedInstallerClass
    {
        private static string _systemsDirPath = Environment.SystemDirectory;


        public static void InstallService(string exePath = null, string serviceName = null)
        {
            if (!ChoWindowsIdentity.IsAdministrator())
            {
                ChoApplication.RestartAsAdmin();
                return;
            }

            ChoServiceInstallerSettings.Me.RaiseBeforeInstall();

            serviceName = serviceName.IsNullOrWhiteSpace() ? ChoServiceCommandLineArgs.GetServiceName() : serviceName;
            serviceName = serviceName.Replace(" ", "_");

            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(_systemsDirPath, "sc.exe");
            //if (ChoServiceInstallerSettings.Me.Account == ServiceAccount.User)
            proc.StartInfo.Arguments = "create {0} binpath= \"{1}\" obj= \"{2}\" password= \"{3}\" displayname= \"{4}\"".FormatString(
                    serviceName,
                    "{0} {1}".FormatString(exePath.IsNullOrWhiteSpace() ? ChoApplication.EntryAssemblyLocation : exePath, ChoServiceCommandLineArgs.GetServiceParams()),
                    //exePath.IsNullOrWhiteSpace() ? ChoApplication.EntryAssemblyLocation : exePath,
                    ChoServiceInstallerSettings.Me.GetUserName(), ChoServiceInstallerSettings.Me.GetPassword(),
                    ChoServiceCommandLineArgs.GetDisplayName());
            proc.StartInfo.Verb = "runas";

            //else
            //    proc.StartInfo.Arguments = "create {0} binpath= \"{1}\" obj= {2}".FormatString(
            //        serviceName.IsNullOrWhiteSpace() ? ChoServiceCommandLineArgs.GetServiceName() : serviceName,
            //        exePath.IsNullOrWhiteSpace() ? ChoApplication.EntryAssemblyLocation : exePath,
            //        ChoServiceInstallerSettings.Me.GetUserName());

            if (!ChoServiceInstallerSettings.Me.Depends.IsNullOrWhiteSpace())
                proc.StartInfo.Arguments = "{0} depend= \"{1}\"".FormatString(proc.StartInfo.Arguments, ChoServiceInstallerSettings.Me.Depends);

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //  ---check result---
            if (proc.ExitCode != 0)
                throw new ChoApplicationException("SC.exe create failed with error code: {0}. {1}".FormatString(proc.ExitCode, outputResult));
            else
                Console.WriteLine(outputResult);

            ConfigService(serviceName);
            AddHelpTextIfAvailable(serviceName);
            AddRecoveryIfAvailable(serviceName);

            ChoServiceInstallerSettings.Me.RaiseAfterInstall();
        }

        private static void AddRecoveryIfAvailable(string serviceName = null)
        {
            if (ChoServiceInstallerSettings.Me.RecoverySettings == null
                || ChoServiceInstallerSettings.Me.RecoverySettings.Actions.IsNullOrWhiteSpace())
                return;

            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(_systemsDirPath, "sc.exe");
            proc.StartInfo.Arguments = "failure {0} reset= {1} reboot= \"{2}\" command= \"{3}\" actions= \"{4}\"".FormatString(
                    serviceName.IsNullOrWhiteSpace() ? ChoServiceCommandLineArgs.GetServiceName() : serviceName,
                    ChoServiceInstallerSettings.Me.RecoverySettings.ResetFailedCounterAfter,
                    ChoServiceInstallerSettings.Me.RecoverySettings.RebootMessage.GetValue(),
                    ChoServiceInstallerSettings.Me.RecoverySettings.Command.GetValue(),
                    ChoServiceInstallerSettings.Me.RecoverySettings.Actions.GetValue());
            proc.StartInfo.Verb = "runas";

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //  ---check result---
            if (proc.ExitCode != 0)
                throw new ChoApplicationException("SC.exe failure failed with error code: {0}. {1}".FormatString(proc.ExitCode, outputResult));
            else
                Console.WriteLine(outputResult);
        }

        private static void ConfigService(string serviceName = null)
        {
            string startMode = null;

            switch (ChoServiceInstallerSettings.Me.ServiceStartMode)
            {
                case ChoServiceStartMode.DelayedAutomatic:
                    startMode = "delayed-auto";
                    break;
                case ChoServiceStartMode.Automatic:
                    startMode = "auto";
                    break;
                case ChoServiceStartMode.Disabled:
                    startMode = "disabled";
                    break;
                case ChoServiceStartMode.Manual:
                    break;
            }

            if (startMode == null) return;

            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(_systemsDirPath, "sc.exe");
            proc.StartInfo.Arguments = "config {0} start= \"{1}\"".FormatString(
                serviceName.IsNullOrWhiteSpace() ? ChoServiceCommandLineArgs.GetServiceName() : serviceName,
                startMode);
            proc.StartInfo.Verb = "runas";

            //if (ChoServiceInstallerSettings.Me.AllowInteractWithDesktop)
            //{
            //    if (ChoServiceInstallerSettings.Me.Account == ServiceAccount.LocalSystem)
            //    {
            //        proc.StartInfo.Arguments += " type= own type= interact";
            //    }
            //}

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //  ---check result---
            if (proc.ExitCode != 0)
                throw new ChoApplicationException("SC.exe config failed with error code: {0}. {1}".FormatString(proc.ExitCode, outputResult));
            else
                Console.WriteLine(outputResult);
        }

        private static void AddHelpTextIfAvailable(string serviceName = null)
        {
            if (ChoServiceCommandLineArgs.GetServiceDescription().IsNullOrWhiteSpace())
                return;

            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(_systemsDirPath, "sc.exe");
            proc.StartInfo.Arguments = "description {0} \"{1}\"".FormatString(
                serviceName.IsNullOrWhiteSpace() ? ChoServiceCommandLineArgs.GetServiceName() : serviceName,
                ChoServiceCommandLineArgs.GetServiceDescription());
            proc.StartInfo.Verb = "runas";

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //  ---check result---
            if (proc.ExitCode != 0)
                throw new ChoApplicationException("SC.exe description failed with error code: {0}. {1}".FormatString(proc.ExitCode, outputResult));
            else
                Console.WriteLine(outputResult);
        }

        public static void UninstallService(string serviceName = null)
        {
            if (!ChoWindowsIdentity.IsAdministrator())
            {
                ChoApplication.RestartAsAdmin();
                return;
            }

            ChoServiceInstallerSettings.Me.RaiseBeforeUninstall();

            serviceName = serviceName.IsNullOrWhiteSpace() ? ChoServiceCommandLineArgs.GetServiceName() : serviceName;
            serviceName = serviceName.Replace(" ", "_");

            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(_systemsDirPath, "sc.exe");
            proc.StartInfo.Arguments = "delete {0}".FormatString(serviceName);
            proc.StartInfo.Verb = "runas";

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //  ---check result---
            if (proc.ExitCode != 0)
                throw new ChoApplicationException("SC.exe delete failed with error code: {0}. {1}".FormatString(proc.ExitCode, outputResult));
            else
                Console.WriteLine(outputResult);

            ChoServiceInstallerSettings.Me.RaiseAfterUninstall();
        }
    }
}
