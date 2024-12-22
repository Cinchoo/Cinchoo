using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertUsingCallback
{
    /// <summary>
    /// Possible command line arguments
    ///     Sample4 /n:Raj /s:INFINITE
    ///     Sample4 /n:Tom /s:1000
    /// </summary>
    [ChoCommandLineArgObject(ApplicationName = "Hello world", Copyright = "Copyright 2014 Cinchoo Inc.")]
    public class HelloWorldCmdLineParams : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("n", IsRequired = true, Description = "Name of the person.", Order = 1)]
        public string Name;

        [ChoCommandLineArg("s", ShortName = "<int | INFINITE>", Description = "Sleep period.", DefaultValue = "-1")]
        public int Sleep
        {
            get;
            set;
        }

        protected override bool OnBeforeCommandLineArgLoaded(string memberName, ref string value, object defaultValue, object fallbackValue)
        {
            if (memberName == "Sleep")
            {
                if (value == null)
                    Sleep = 0;
                else
                {
                    if (String.Compare(value.ToString(), "INFINITE", true) == 0)
                        Sleep = -1;
                    else
                    {
                        int timeout = 0;
                        int.TryParse(value.ToString(), out timeout);
                        Sleep = timeout;
                    }
                }
                return true;
            }
            else
                return base.OnBeforeCommandLineArgLoaded(memberName, ref value, defaultValue, fallbackValue);
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
