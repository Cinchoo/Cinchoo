namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using System.ServiceProcess;
    using Cinchoo.Core.Text;
    using Cinchoo.Core.Configuration;
    using System.IO;
    using Cinchoo.Core;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Shell;
    using Cinchoo.Core.Xml.Serialization;

    #endregion NameSpaces

    public class ChoServiceInstallerRecoverySettings
    {
        [XmlAttribute("resetFailedCounterAfter")]
        public int ResetFailedCounterAfter = 0;

        [XmlElement("rebootMessage")]
        public ChoCDATA RebootMessage = new ChoCDATA();

        [XmlElement("command")]
        public ChoCDATA Command = new ChoCDATA();

        [XmlElement("actions")]
        public ChoCDATA Actions = new ChoCDATA();

        public ChoServiceInstallerRecoverySettings RestartService(int delayInMin)
        {
            AddToRecoveryActions("restart", delayInMin);
            return this;
        }
        public ChoServiceInstallerRecoverySettings RunProgram(int delayInMin, string exePath, params string[] parameters)
        {
            AddToRecoveryActions("run", delayInMin);
            Command = new ChoCDATA("{0} {1}".FormatString(exePath, String.Join(" ", parameters)));

            return this;
        }
        public ChoServiceInstallerRecoverySettings RebootSystem(int delayInMin, string broadcastMsg)
        {
            AddToRecoveryActions("reboot", delayInMin);
            RebootMessage = new ChoCDATA(broadcastMsg);

            return this;
        }
        public ChoServiceInstallerRecoverySettings ResetFailCountAfter(int seconds)
        {
            if (seconds >= 0)
                ResetFailedCounterAfter = seconds;
            return this;
        }

        private void AddToRecoveryActions(string action, int delayInMins)
        {
            if (delayInMins < 0)
                delayInMins = 0;

            if (Actions == null || Actions.Value.IsNullOrWhiteSpace())
                Actions = new ChoCDATA("{0}/{1}".FormatString(action, delayInMins * 60 * 1000));
            else
                Actions = new ChoCDATA("{0}/{1}/{2}".FormatString(Actions.Value, action, delayInMins * 60 * 1000));
        }

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("Reset: {0}", ResetFailedCounterAfter);
            msg.AppendFormatLine("RebootMsg: {0}", RebootMessage.GetValue());
            msg.AppendFormatLine("Command: {0}", Command.GetValue());
            msg.AppendFormatLine("Actions: {0}", Actions.GetValue());

            return msg.ToString();
        }
    }

    public enum ChoServiceStartMode
    {
        DelayedAutomatic = 1,
        // Summary:
        //     Indicates that the service is to be started (or was started) by the operating
        //     system, at system start-up. If an automatically started service depends on
        //     a manually started service, the manually started service is also started
        //     automatically at system startup.
        Automatic = 2,
        //
        // Summary:
        //     Indicates that the service is started only manually, by a user (using the
        //     Service Control Manager) or by an application.
        Manual = 3,
        //
        // Summary:
        //     Indicates that the service is disabled, so that it cannot be started by a
        //     user or application.
        Disabled = 4,
    }

    [XmlRoot("serviceInstallerSettings")]
    public class ChoServiceInstallerSettings
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoServiceInstallerSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlElement("description")]
        public string Description = String.Empty;

        [XmlAttribute("serviceName")]
        public string ServiceName = String.Empty;

        [XmlElement("parameters")]
        public ChoCDATA ServiceParams = new ChoCDATA();

        [XmlAttribute("serviceStartMode")]
        public ChoServiceStartMode ServiceStartMode = ChoServiceStartMode.Automatic;

        [XmlAttribute("depends")]
        public string Depends = String.Empty;

        [XmlAttribute("account")]
        public ServiceAccount Account = ServiceAccount.LocalSystem;

        //[XmlAttribute("allowInteractWithDesktop")]
        //public bool AllowInteractWithDesktop = false;

        [XmlAttribute("userName")]
        public string UserName = String.Empty;

        [XmlAttribute("password")]
        public string Password = String.Empty;

        [XmlAttribute("timeoutInTicks")]
        public int Timeout = -1;

        [XmlAttribute("canHandlePowerEvent")]
        public bool CanHandlePowerEvent = false;

        [XmlAttribute("canHandleSessionChangeEvent")]
        public bool CanHandleSessionChangeEvent = false;

        [XmlAttribute("canPauseAndContinue")]
        public bool CanPauseAndContinue = false;

        [XmlAttribute("canShutdown")]
        public bool CanShutdown = true;

        [XmlAttribute("canStop")]
        public bool CanStop = true;

        [XmlAttribute("autoLog")]
        public bool AutoLog = false;

        [XmlAttribute("exitCode")]
        public int ExitCode = 0;

        [XmlElement("recoverySettings")]
        public ChoServiceInstallerRecoverySettings RecoverySettings = new ChoServiceInstallerRecoverySettings();

        [XmlIgnore]
        public string InstanceName;

        [XmlIgnore]
        public string DisplayName;

        private Action _beforeInstall;
        private Action _afterInstall;
        private Action _beforeUninstall;
        private Action _afterUninstall;

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("Description: {0}", Description);
            msg.AppendFormatLine("ServiceName: {0}", ServiceName);
            msg.AppendFormatLine("ServiceStartMode: {0}", ServiceStartMode);
            msg.AppendFormatLine("Depends: {0}", Depends);
            msg.AppendFormatLine("Account: {0}", Account);
            msg.AppendFormatLine("UserName: {0}", UserName);
            msg.AppendFormatLine("Timeout: {0}", Timeout);

            msg.AppendFormatLine("CanHandlePowerEvent: {0}", CanHandlePowerEvent);
            msg.AppendFormatLine("CanHandleSessionChangeEvent: {0}", CanHandleSessionChangeEvent);
            msg.AppendFormatLine("CanPauseAndContinue: {0}", CanPauseAndContinue);
            msg.AppendFormatLine("CanShutdown: {0}", CanShutdown);
            msg.AppendFormatLine("CanStop: {0}", CanStop);
            msg.AppendFormatLine("AutoLog: {0}", AutoLog);
            msg.AppendFormatLine("ExitCode: {0}", ExitCode);

            msg.AppendFormatLine(RecoverySettings.ToString().Indent());

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoServiceInstallerSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoServiceInstallerSettings>();

                        if (_instance.Timeout < -1)
                            _instance.Timeout = -1;

                        ChoApplication.RaiseAfterAppFrxSettingsLoaded(_instance);
                    }
                }

                return _instance;
            }
        }

        #endregion Factory Methods

        #region Fluent API Methods

        public ChoServiceInstallerSettings RunAs(string userName, string password)
        {
            Account = ServiceAccount.User;
            UserName = userName;
            Password = password;
            return this;
        }
        public ChoServiceInstallerSettings RunAsLocalSystem()
        {
            Account = ServiceAccount.LocalSystem;
            return this;
        }
        public ChoServiceInstallerSettings RunAsLocalService()
        {
            Account = ServiceAccount.LocalService;
            return this;
        }
        public ChoServiceInstallerSettings RunAsNetworkService()
        {
            Account = ServiceAccount.NetworkService;
            return this;
        }
        public ChoServiceInstallerSettings RunAsPrompt()
        {
            Account = ServiceAccount.User;
            UserName = null;
            Password = null;
            return this;
        }
        internal string GetUserName()
        {
            if (this.Account == ServiceAccount.NetworkService)
                return @"NT AUTHORITY\NetworkService";
            else if (this.Account == ServiceAccount.LocalSystem)
                return "LocalSystem";
            else if (this.Account == ServiceAccount.LocalService)
                return @"NT AUTHORITY\LocalService";
            else
            {
                if (!UserName.IsNullOrWhiteSpace())
                    return ChoEnvironment.ToDomainUserName(UserName);
                else
                {
                    ChoConsole.Write("Enter UserId: ");
                    return ChoEnvironment.ToDomainUserName(ChoConsole.ReadLine());
                }
            }
        }

        public ChoServiceInstallerSettings SetServiceArguments(string args)
        {
            ServiceParams = new ChoCDATA(args);
            return this;
        }

        internal string GetPassword()
        {
            if (this.Account == ServiceAccount.NetworkService)
                return String.Empty;
            else if (this.Account == ServiceAccount.LocalSystem)
                return String.Empty;
            else if (this.Account == ServiceAccount.LocalService)
                return String.Empty;
            else
            {
                if (!UserName.IsNullOrWhiteSpace())
                    return Password;
                else
                {
                    ChoConsole.Write("Enter Password: ");
                    return ChoConsole.ReadPassword();
                }
            }
        }

        internal ChoServiceInstallerSettings BeforeInstall(Action action)
        {
            _beforeInstall = action;
            return this;
        }
        internal ChoServiceInstallerSettings AfterInstall(Action action)
        {
            _afterInstall = action;
            return this;
        }
        internal ChoServiceInstallerSettings BeforeUninstall(Action action)
        {
            _beforeUninstall = action;
            return this;
        }
        internal ChoServiceInstallerSettings AfterUninstall(Action action)
        {
            _afterUninstall = action;
            return this;
        }

        internal void RaiseBeforeInstall()
        {
            if (_beforeInstall != null)
                _beforeInstall();
        }

        internal void RaiseAfterInstall()
        {
            if (_afterInstall != null)
                _afterInstall();
        }

        internal void RaiseBeforeUninstall()
        {
            if (_beforeUninstall != null)
                _beforeUninstall();
        }

        internal void RaiseAfterUninstall()
        {
            if (_afterUninstall != null)
                _afterUninstall();
        }

        public ChoServiceInstallerSettings DependsOn(params string[] serviceNames)
        {
            if (Depends.IsNullOrWhiteSpace())
                Depends = String.Join("/", serviceNames);
            else
                Depends = "{0}/{1}".FormatString(Depends, String.Join("/", serviceNames));
            return this;
        }

        #endregion Fluent API Methods
    }
}
