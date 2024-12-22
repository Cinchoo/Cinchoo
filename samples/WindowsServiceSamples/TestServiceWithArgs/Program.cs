using Cinchoo.Core;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.ServiceProcess;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServiceWithArgs
{
    [ChoCommandLineArgObject]
    public class AppCmdLineArgs : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("name")]
        public string Name;

        [ChoCommandLineArg("msg")]
        public string Message;
    }

    [ChoApplicationHost]
    public class HelloWorldAppHost : ChoApplicationHost
    {
        protected override void OnStart(string[] args)
        {
            ChoProfile.WriteLine("Test Service with Arguments!");

            AppCmdLineArgs arg = new AppCmdLineArgs();
            ChoProfile.WriteLine(arg.ToString());

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ChoProfile.WriteLine("OnStop()");
            base.OnStop();
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
