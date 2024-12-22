using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core;
using Cinchoo.Core.Shell;
using System.Windows.Forms;

namespace ShellExtensionSample
{
    [ChoShellExtension]
    public class HelloWorldShellExt
    {
        [ChoShellExtensionContextMenu("*")]
        public static void HelloWorld(string[] args)
        {
            MessageBox.Show("Test");
        }
    }

    [ChoApplicationHost]
    public class AppHost : ChoApplicationHost
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            ChoApplication.Run(args);
        }
    }
}
