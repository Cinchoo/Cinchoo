using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdLineArgsFromFile
{
    public enum Shape { Square, Circle, Rectancle }

    /// <summary>
    /// Possible command line arguments
    ///     Sample4 @"C:\SampleCmdLineArg.txt"
    /// </summary>
    [ChoCommandLineArgObject(ApplicationName = "Hello world", Copyright = "Copyright 2014 Cinchoo Inc.")]
    public class HelloWorldCmdLineParams : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("n", IsRequired = true, Description = "Name of the person.", Order = 1)]
        public string Name;

        [ChoCommandLineArg("s", DefaultValue = "Square", Description = "Shape Information.", Order = 2)]
        public Shape Shape
        {
            get;
            set;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            HelloWorldCmdLineParams cmd = new HelloWorldCmdLineParams();
            Console.WriteLine(cmd.ToString());
        }
    }
}
