using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooleanValueSample
{
    /// <summary>
    /// Possible command line arguments
    ///     Sample4 /n:Raj /e
    ///     Sample4 /n:Tom /e-
    /// </summary>
    [ChoCommandLineArgObject(ApplicationName = "Hello world", Copyright = "Copyright 2014 Cinchoo Inc.")]
    public class HelloWorldCmdLineParams : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("n", IsRequired = true, Description = "Name of the person.", Order = 1)]
        public string Name;

        [ChoCommandLineArg("e", Description = "IsEnabled.", Order = 2)]
        public bool IsEnabled
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
