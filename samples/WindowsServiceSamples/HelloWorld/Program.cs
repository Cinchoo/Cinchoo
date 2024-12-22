using Cinchoo.Core;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.ServiceProcess;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    [ChoApplicationHost]
    public class HelloWorldAppHost : ChoApplicationHost
    {
        protected override void OnStart(string[] args)
        {
            ChoProfile.WriteLine("Hello world!");

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
