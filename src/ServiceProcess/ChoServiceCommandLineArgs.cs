namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Shell;

    #endregion NameSpaces

    [ChoCommandLineArgObject]
    public class ChoServiceCommandLineArgs : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("I", DefaultValue = false, FallbackValue = true, Description = "Install Service.")]
        public bool InstallService
        {
            get;
            set;
        }

        [ChoCommandLineArg("U", DefaultValue = false, FallbackValue = true, Description = "Uninstall Service.")]
        public bool UninstallService
        {
            get;
            set;
        }

        [ChoCommandLineArg("S", DefaultValue = false, FallbackValue = true, Description = "Start Service.")]
        public bool StartService
        {
            get;
            set;
        }

        [ChoCommandLineArg("T", DefaultValue = false, FallbackValue = true, Description = "Stop Service.")]
        public bool StopService
        {
            get;
            set;
        }

        [ChoCommandLineArg("P", DefaultValue = false, FallbackValue = true, Description = "Pause Service.")]
        public bool PauseService
        {
            get;
            set;
        }

        [ChoCommandLineArg("C", DefaultValue = false, FallbackValue = true, Description = "Continue Service.")]
        public bool ContinueService
        {
            get;
            set;
        }

        [ChoCommandLineArg("E", DefaultValue = Int32.MinValue, Description = "Execute Command.")]
        public int ExecuteCommand
        {
            get;
            set;
        }
    }
}
