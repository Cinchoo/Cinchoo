using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldSample
{
    /// <summary>
    /// Command line argument object accepts values in the below format
    /// 
    ///     HelloWorldSample /name:Tom /msg:"Good Morning"
    ///     HelloWorldSample /n:Tom /msg:"Good Morning"
    /// </summary>
    [ChoCommandLineArgObject(ApplicationName = "Hello world", Copyright = "Copyright 2014 Cinchoo Inc.")]
    public class HelloWorldCmdLineParams : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("name", Aliases = "n", IsRequired = true, Description = "Name of the person.")]
        public string Name
        {
            get;
            set;
        }
        [ChoCommandLineArg("msg", DefaultValue = "Good Morning", Description = "Greeting message.")]
        public string Message
        {
            get;
            set;
        }

        public override string ToString()
        {
            return "{0}! {1}.".FormatString(Message, Name);
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
