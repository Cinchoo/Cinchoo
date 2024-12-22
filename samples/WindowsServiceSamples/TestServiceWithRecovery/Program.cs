using Cinchoo.Core;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.ServiceProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServiceWithRecovery
{
    [ChoApplicationHost]
    public class TestServiceWithRecoveryAppHost : ChoApplicationHost
    {
        protected override void OnStart(string[] args)
        {
            ChoProfile.WriteLine("Test Service With Recovery Options!");

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ChoProfile.WriteLine("OnStop()");
            base.OnStop();
        }

        protected override void ApplyServiceParametersOverrides(ChoServiceInstallerSettings obj)
        {
            obj.RunAsLocalSystem();
            obj.DependsOn("MySQL55", "MsKeyboardFilter");
            obj.RecoverySettings.ResetFailCountAfter(2);
            obj.RecoverySettings.RestartService(9);
            obj.RecoverySettings.RunProgram(10, "Notepad.exe", @"C:\Test.txt");
            obj.RecoverySettings.RebootSystem(5, "Service down");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ChoApplication.Run(args);
        }
    }
}
