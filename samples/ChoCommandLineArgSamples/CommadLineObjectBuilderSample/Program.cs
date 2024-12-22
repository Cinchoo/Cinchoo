using Cinchoo.Core;
using Cinchoo.Core.Configuration;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommadLineObjectBuilderSample
{
    [ChoCommandLineArgObject]
    public class STARTCmdLineArgObject : ChoCommandLineArgObject
    {
        [ChoPositionalCommandLineArg(1, "service")]
        public string ServiceName;
    }
    
    [ChoCommandLineArgObject(DoNotShowUsageDetail = true)]
    public class STOPCmdLineArgObject : ChoCommandLineArgObject
    {
        [ChoPositionalCommandLineArg(1, "service", IsRequired = true)]
        public string ServiceName;
    }

    public enum Action { ADD, DEL };

    [ChoCommandLineArgObject(DoNotShowUsageDetail = true)]
    public class COMPUTERCmdLineArgObject : ChoCommandLineArgObject
    {
        [ChoPositionalCommandLineArg(1, "\\\\computername", IsRequired = true, Order = 0)]
        public string ComputerName;

        [ChoCommandLineArg("action", IsRequired = true)]
        public Action Action;
    }

    [ChoCommandLineArgBuilder]
    public class NetCmdBuilder : ChoCommandLineArgBuilder
    {
        [ChoCommandLineArgBuilderCommand("START", typeof(STARTCmdLineArgObject), Order = 0)]
        [ChoCommandLineArgBuilderCommand("STOP", typeof(STOPCmdLineArgObject), Order = 1)]
        [ChoCommandLineArgBuilderCommand("COMPUTER", typeof(COMPUTERCmdLineArgObject), Order = -1)]
        public NetCmdBuilder()
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            NetCmdBuilder netCmdBuilder = new NetCmdBuilder();
            Console.WriteLine(netCmdBuilder.CommandLineArgObject.ToString());
        }
    }
}
