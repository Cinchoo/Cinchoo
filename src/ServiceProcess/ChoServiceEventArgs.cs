namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ServiceProcess;

    #endregion NameSpaces

    public class ChoServiceStartEventArgs : EventArgs
    {
        public readonly string[] Args;

        public ChoServiceStartEventArgs(string[] args)
        {
            Args = args;
        }
    }

    public class ChoServiceCustomCommandEventArgs : EventArgs
    {
        public readonly int Command;

        public ChoServiceCustomCommandEventArgs(int command)
        {
            Command = command;
        }
    }

    public class ChoServicePowerEventEventArgs : EventArgs
    {
        public readonly PowerBroadcastStatus PowerStatus;

        public ChoServicePowerEventEventArgs(PowerBroadcastStatus powerStatus)
        {
            PowerStatus = powerStatus;
        }
    }

    public class ChoServiceSessionChangeEventArgs : EventArgs
    {
        public readonly SessionChangeDescription ChangeDescription;

        public ChoServiceSessionChangeEventArgs(SessionChangeDescription changeDescription)
        {
            ChangeDescription = changeDescription;
        }
    }
}
