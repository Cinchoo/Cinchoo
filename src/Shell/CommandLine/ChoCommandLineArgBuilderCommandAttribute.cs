using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Shell
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
    public class ChoCommandLineArgBuilderCommandAttribute : ChoProxyAttribute
    {
        public string Command
        {
            get;
            private set;
        }

        public Type CommandType
        {
            get;
            private set;
        }

        public int Order
        {
            get;
            set;
        }

        public ChoCommandLineArgBuilderCommandAttribute(string command, Type commandType)
        {
            ChoGuard.ArgumentNotNullOrEmpty(command, "Command");
            ChoGuard.ArgumentNotNullOrEmpty(commandType, "CommandType");

            if (!commandType.IsSubclassOf(typeof(ChoCommandLineArgObject)))
                throw new ArgumentException("CommandType is not of ChoCommandLineArgObject type.");

            Command = command;
            CommandType = commandType;
        }
    }
}
