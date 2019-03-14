namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Security;
    using System.Threading;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Security.Permissions;

    //using Cinchoo.Core.Win32;
    using Cinchoo.Core.Types;
    using Cinchoo.Core.Diagnostics;
    using System.IO;
    using Cinchoo.Core.Win32;
	using Cinchoo.Core.Pattern.Singleton;
	using Cinchoo.Core.Exceptions;
	using Cinchoo.Core.IO;
    using System.Configuration;
    using Cinchoo.Core.Text;
    using Cinchoo.Core.Reflection;

    #endregion NameSpaces

	[ChoSingleton]
	[ChoTypeFormatter("Application Info")]
	public class ChoApplicationInfo //: ChoSingletonObject
    {
        #region Instance Data Members (Public)

        public string AppEnvironment
        {
            get;
            set;
        }

        public string SharedEnvironmentConfigFilePath
        {
            get;
            set;
        }

        private string _appDomainName;
		public string AppDomainName
		{
            get { return _appDomainName; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "AppDomainName");
                _appDomainName = value;
            }
		}

		public int ProcessId
		{
			get;
			private set;
		}

        private string _processFilePath;
        public string ProcessFilePath
        {
            get { return _processFilePath; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ProcessFilePath");
                _processFilePath = value;
            }
        }

        public bool UnmanagedCodePermissionAvailable
		{
			get;
			private set;
		}

        private string _applicationName;
        public string ApplicationName
        {
            get { return _applicationName; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ApplicationName");
                _applicationName = value;
            }
        }

        private string _applicationNameWithoutExtension;
        public string ApplicationNameWithoutExtension
        {
            get { return _applicationNameWithoutExtension; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ApplicationNameWithoutExtension");
                _applicationNameWithoutExtension = value;
            }
        }

        private string _entryAssemblyLocation;
        public string EntryAssemblyLocation
        {
            get { return _entryAssemblyLocation; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "EntryAssemblyLocation");
                _entryAssemblyLocation = value;
            }
        }

        private string _entryAssemblyFileName;
        public string EntryAssemblyFileName
        {
            get { return _entryAssemblyFileName; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "EntryAssemblyFileName");
                _entryAssemblyFileName = value;
            }
        }

        private string _applicationConfigFilePath;
        public string ApplicationConfigFilePath
        {
            get { return _applicationConfigFilePath; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ApplicationConfigFilePath");
                _applicationConfigFilePath = value;
            }
        }

        private string _applicationBaseDirectory;
        public string ApplicationBaseDirectory
        {
            get { return _applicationBaseDirectory; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ApplicationBaseDirectory");
                _applicationBaseDirectory = value;
            }
        }

        private string _applicationLogDirectory;
        public string ApplicationLogDirectory
        {
            get { return _applicationLogDirectory; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ApplicationLogDirectory");
                _applicationLogDirectory = value;
            }
        }

        private string _hostName;
        public string HostName
        {
            get { return _hostName; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "HostName");
                _hostName = value;
            }
        }

        private string _eventLogSourceName;
        public string EventLogSourceName
        {
            get { return _eventLogSourceName; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "EventLogSourceName");
                _eventLogSourceName = value;
            }
        }

        #endregion Shared Data Members (Public)

        private bool _initialized = false;
        private readonly object _padLock = new object();

        static ChoApplicationInfo()
        {
            _instance = new ChoApplicationInfo();
        }

		private ChoApplicationInfo()
		{
		}

        #region Shared Members (Private)

		//[ChoSingletonInstanceInitializer]
		public void Initialize()
		{
            if (_initialized)
                return;

            lock (_padLock)
            {
                if (_initialized)
                    return;
                
                _initialized = true;

                try
                {
                    EntryAssemblyLocation = ChoAssembly.GetEntryAssembly().Location;
                    EntryAssemblyFileName = System.IO.Path.GetFileName(EntryAssemblyLocation);
                }
                catch (System.Security.SecurityException ex)
                {
                    // This security exception will occur if the caller does not have 
                    // some undefined set of SecurityPermission flags.
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine(ex.ToString());
                }

                try
                {
                    AppEnvironment = ConfigurationManager.AppSettings["appEnvironment"];
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

                try
                {
                    SharedEnvironmentConfigFilePath = ConfigurationManager.AppSettings["sharedEnvironmentConfigFilePath"];
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

                try
                {
                    if (ConfigurationManager.AppSettings["appConfigPath"].IsNullOrWhiteSpace())
                        ApplicationConfigFilePath = System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    else
                        ApplicationConfigFilePath = ConfigurationManager.AppSettings["appConfigPath"].Trim();
                }
                catch (System.Security.SecurityException ex)
                {
                    // This security exception will occur if the caller does not have 
                    // some undefined set of SecurityPermission flags.
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine(ex.ToString());
                }

                try
                {
                    ApplicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }
                catch (System.Security.SecurityException ex)
                {
                    // This security exception will occur if the caller does not have 
                    // some undefined set of SecurityPermission flags.
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine(ex.ToString());
                }

                #region Check for Unmanaged Code Permission Available

                // check whether the unmanaged code permission is available to avoid three potential stack walks
                SecurityPermission unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                // avoid a stack walk by checking for the permission on the current assembly. this is safe because there are no
                // stack walk modifiers before the call.
                if (SecurityManager.IsGranted(unmanagedCodePermission))
                {
                    try
                    {
                        unmanagedCodePermission.Demand();
                        UnmanagedCodePermissionAvailable = true;
                    }
                    catch (SecurityException e)
                    {
                        if (ChoTrace.ChoSwitch.TraceError)
                            Trace.WriteLine(ChoApplicationException.ToString(e));
                    }
                }

                #endregion Check for Unmanaged Code Permission Available

                EventLogSourceName = ChoApplicationSettings.State.EventLogSourceName;

                #region GetApplicationName

                try
                {
                    ApplicationName = ChoApplicationSettings.State.ApplicationId;
                }
                catch (System.Security.SecurityException ex)
                {
                    // This security exception will occur if the caller does not have 
                    // some undefined set of SecurityPermission flags.
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine(ex.ToString());

                    try
                    {
                        ApplicationName = System.IO.Path.GetFileName(EntryAssemblyLocation);
                    }
                    catch (System.Security.SecurityException e)
                    {
                        ChoTrace.Error(ChoApplicationException.ToString(e));
                    }
                }

                ApplicationNameWithoutExtension = Path.GetFileNameWithoutExtension(ApplicationName);
                if (!ChoApplicationSettings.State.LogFolder.IsNullOrWhiteSpace())
                    ApplicationLogDirectory = ChoApplicationSettings.State.LogFolder;
                else if (ChoApplicationSettings.State.UseApplicationDataFolderAsLogFolder)
                    ApplicationLogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationNameWithoutExtension, ChoReservedDirectoryName.Logs);
                else
                    ApplicationLogDirectory = Path.Combine(ApplicationBaseDirectory, ChoReservedDirectoryName.Logs);

                #endregion GetApplicationName

                #region Get AppDomainName

                try
                {
                    AppDomainName = AppDomain.CurrentDomain.FriendlyName;
                }
                catch (Exception ex)
                {
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine(ex.ToString());
                }

                #endregion Get AppDomainName

                #region Get ProcessId, ProcessName

                if (UnmanagedCodePermissionAvailable)
                {
                    try
                    {
                        ProcessId = ChoKernel32Core.GetCurrentProcessId();
                    }
                    catch (Exception ex)
                    {
                        if (ChoTrace.ChoSwitch.TraceError)
                            Trace.WriteLine(ex.ToString());
                    }

                    try
                    {
                        ProcessFilePath = GetProcessName();
                    }
                    catch (Exception ex)
                    {
                        if (ChoTrace.ChoSwitch.TraceError)
                            Trace.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine("Failed to retrieve value due to unmanaged code permission denied.");
                }

                #endregion Get ProcessId, ProcessName

                #region Get HostName

                // Get the DNS host name of the current machine
                try
                {
                    // Lookup the host name
                    HostName = System.Net.Dns.GetHostName();
                }
                catch (System.Net.Sockets.SocketException)
                {
                }
                catch (System.Security.SecurityException)
                {
                    // We may get a security exception looking up the hostname
                    // You must have Unrestricted DnsPermission to access resource
                }

                // Get the NETBIOS machine name of the current machine
                if (HostName.IsNullOrWhiteSpace())
                {
                    try
                    {
                        HostName = Environment.MachineName;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (System.Security.SecurityException)
                    {
                        // We may get a security exception looking up the machine name
                        // You must have Unrestricted EnvironmentPermission to access resource
                    }
                }

                #endregion Get HostName

                ApplyFrxParamsOverrides.Raise(this, null);
            }
        }

        private string GetProcessName()
        {
            StringBuilder buffer = new StringBuilder(1024);
            int length = ChoKernel32Core.GetModuleFileName(ChoKernel32Core.GetModuleHandle(null), buffer, buffer.Capacity);
            return buffer.ToString();
        }

        #endregion Shared Members (Private)

        #region Shared Members (Public)

        public int GetThreadId()
        {
            int threadId = 0;
            if (UnmanagedCodePermissionAvailable)
            {
                try
                {
					threadId = ChoKernel32Core.GetCurrentThreadId();
                }
                catch (Exception e)
                {
                    if (ChoTrace.ChoSwitch.TraceError)
                        Trace.WriteLine(ChoApplicationException.ToString(e));
                }
            }
            else
            {
                if (ChoTrace.ChoSwitch.TraceError)
                    Trace.WriteLine("Failed to retrieve value due to unmanaged code permission denied.");
            }

            return threadId;
        }

        public string GetThreadName()
        {
            string threadName = null;
            try
            {
                threadName = Thread.CurrentThread.Name;
            }
            catch (Exception e)
            {
                if (ChoTrace.ChoSwitch.TraceError)
                    Trace.WriteLine(ChoApplicationException.ToString(e));
            }
			if (String.IsNullOrEmpty(threadName))
				threadName = String.Format("ChoThread {0}", GetThreadId());

            return threadName;
        }

        #endregion Shared Members (Public)
		
		#region Shared Members (Public)

        private static readonly ChoApplicationInfo _instance;

		public static ChoApplicationInfo Me
		{
            get 
            {
                _instance.Initialize();
                return _instance; 
            }
		}

		#endregion Shared Members (Public)

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("AppEnvironment: {0}", AppEnvironment);
            msg.AppendFormatLine("SharedEnvironmentConfigFilePath: {0}", SharedEnvironmentConfigFilePath);
            msg.AppendFormatLine("AppDomainName: {0}", AppDomainName);
            msg.AppendFormatLine("ProcessId: {0}", ProcessId);
            msg.AppendFormatLine("ProcessFilePath: {0}", ProcessFilePath);
            msg.AppendFormatLine("UnmanagedCodePermissionAvailable: {0}", UnmanagedCodePermissionAvailable);
            msg.AppendFormatLine("ApplicationName: {0}", ApplicationName);
            msg.AppendFormatLine("ApplicationNameWithoutExtension: {0}", ApplicationNameWithoutExtension);
            msg.AppendFormatLine("EntryAssemblyLocation: {0}", EntryAssemblyLocation);
            msg.AppendFormatLine("EntryAssemblyFileName: {0}", EntryAssemblyFileName);
            msg.AppendFormatLine("ApplicationConfigFilePath: {0}", ApplicationConfigFilePath);
            msg.AppendFormatLine("ApplicationBaseDirectory: {0}", ApplicationBaseDirectory);
            msg.AppendFormatLine("ApplicationLogDirectory: {0}", ApplicationLogDirectory);
            msg.AppendFormatLine("HostName: {0}", HostName);

            return msg.ToString();
        }
	}
}
