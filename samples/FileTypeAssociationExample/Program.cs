using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Cinchoo.Core;
using Cinchoo.Core.Shell;

[assembly: ChoShellFileAssociation(".hlw", Description = "Hello World! Document")]

namespace FileTypeAssociationExample
{
    [ChoApplicationHost]
    public class AppHost : ChoApplicationHost
    {
        public override object MainWindowObject
        {
            get
            {
                return new MainForm();
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ChoApplication.Run(args);
        }
    }
}
