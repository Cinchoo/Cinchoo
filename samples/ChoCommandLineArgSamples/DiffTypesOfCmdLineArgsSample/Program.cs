using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffTypesOfCmdLineArgsSample
{
    [ChoCommandLineArgObject(ApplicationName = "Hello world", Copyright = "Copyright 2014 Cinchoo Inc.")]
    public class HelloWorldCmdLineParams : ChoCommandLineArgObject
    {
        [ChoPositionalCommandLineArg(1, "Pos1", Description = "First positional argument.", Order = 1, NoOfTabsSwitchDescFormatSeparator = 2)]
        public string PosArg1;

        [ChoPositionalCommandLineArg(2, "Pos2", Description = "Sescond positional argument.", Order = 2, NoOfTabsSwitchDescFormatSeparator = 2)]
        public string PosArg2;

        [ChoCommandLineArg("name", Aliases = "n, m", IsRequired = true, Description = "Name of the person.", Order = 3, NoOfTabsSwitchDescFormatSeparator = 1)]
        public string Name;

        [ChoCommandLineArg("msg", DefaultValue = "Good Morning", Description = "Greeting message.", Order = 4, NoOfTabsSwitchDescFormatSeparator = 2)]
        public string Message
        {
            get;
            set;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            HelloWorldCmdLineParams helloWorldCmdLineParams = new HelloWorldCmdLineParams();
            Console.WriteLine(helloWorldCmdLineParams.ToString());
        }
    }
}
