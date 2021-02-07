namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Runtime.Remoting.Proxies;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Runtime.Remoting;
    using Cinchoo.Core.Services;
    using System.Reflection;
    using System.Text;
    using System.IO;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoCommandLineArgObjectAttribute : ChoProxyAttribute
    {
		#region Shared Data Members (Private)

        private static bool _isHeaderPrinted = false;
        private static object _padLock = new object();
        protected readonly static ChoDictionaryService<Type, RealProxy> _dictService = new ChoDictionaryService<Type, RealProxy>(typeof(ChoSingletonAttribute).Name);

		#endregion Shared Data Members (Private)

		#region Instance Data Members (Private)

        private ChoCommandLineArgObjectDirector _loggableElement;
        internal bool Silent = false;
        public string UsageSwitch = null;

		#endregion Instance Data Members (Private)

		#region Constructors

        static ChoCommandLineArgObjectAttribute()
        {
        }

        public ChoCommandLineArgObjectAttribute()
        {
        }

		internal ChoCommandLineArgObjectAttribute(bool usageAvail = true)
		{
            Silent = !usageAvail;
		}

        internal ChoCommandLineArgObjectAttribute(string usageSwitch = null)
        {
            UsageSwitch = usageSwitch;
        }

		#endregion

        #region Instance Properties (Public)

        public string ApplicationName
        {
            get;
            set;
        }

        public string Copyright
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string AdditionalInfo
        {
            get;
            set;
        }

        public bool ShowUsageIfEmpty
        {
            get { return ShowUsageIfEmptyInternal == null ? false : ShowUsageIfEmptyInternal.Value; }
            set { ShowUsageIfEmptyInternal = value; }
        }

        internal bool? ShowUsageIfEmptyInternal
        {
            get;
            set;
        }

        public bool DisplayDefaultValue 
        {
            get { return DisplayDefaultValueInternal == null ? false : DisplayDefaultValueInternal.Value; }
            set { DisplayDefaultValueInternal = value; }
        }

        internal bool? DisplayDefaultValueInternal { get; set; }

        public bool DoNotShowUsageDetail
        {
            get;
            set;
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Public)

        internal bool GetShowUsageIfEmpty()
        {
            ChoCommandLineParserSettings commandLineParserSettings = ChoCommandLineParserSettings.Me;
            bool showUsageIfEmpty = false;

            if (ShowUsageIfEmptyInternal != null)
                showUsageIfEmpty = ShowUsageIfEmptyInternal.Value;
            else
                showUsageIfEmpty = commandLineParserSettings.ShowUsageIfEmpty;

            return showUsageIfEmpty;
        }

        internal bool GetDisplayDefaultValue()
        {
            ChoCommandLineParserSettings commandLineParserSettings = ChoCommandLineParserSettings.Me;
            bool displayDefaultValue = false;

            if (DisplayDefaultValueInternal != null)
                displayDefaultValue = DisplayDefaultValueInternal.Value;
            else
                displayDefaultValue = commandLineParserSettings.DisplayDefaultValue;

            return displayDefaultValue;
        }

        internal ChoCommandLineArgObjectDirector GetMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (_loggableElement == null)
			{
				lock (SyncRoot)
				{
					if (_loggableElement == null)
					{
                        _loggableElement = new ChoCommandLineArgObjectDirector();
					}
				}
			}

			return _loggableElement;
		}

        #endregion Instance Members (Public)

        #region ChoConfigurationElementAttribute Overrides

        public override MarshalByRefObject CreateInstance(Type cmdLineObjType)
		{
			if (_dictService.ContainsKey(cmdLineObjType))
				return (MarshalByRefObject)_dictService.GetValue(cmdLineObjType).GetTransparentProxy();

			lock (_dictService.SyncRoot)
			{
				if (_dictService.ContainsKey(cmdLineObjType))
					return (MarshalByRefObject)_dictService.GetValue(cmdLineObjType).GetTransparentProxy();
				else
				{
                    //ChoCmdLineArgMetaDataManager.LoadFromConfig(cmdLineObjType, this);

                    RealProxy proxy = new ChoCommandLineArgsObjectProxy(base.CreateInstance(cmdLineObjType), cmdLineObjType);
					_dictService.SetValue(cmdLineObjType, proxy);
                    //if (ChoType.GetAttribute<ChoIgnorePrintHeaderAttribute>(cmdLineObjType) == null)
                        PrintHeader();
					return (MarshalByRefObject)proxy.GetTransparentProxy();
				}
			}
		}

		#endregion ChoConfigurationElementAttribute Overrides

        #region Instance Members (Private)

        protected void PrintHeader()
        {
            if (ChoCommandLineParserSettings.Me.DoNotShowHeader)
                return;

            if (Silent)
                return;

            lock (_padLock)
            {
                if (_isHeaderPrinted) return;
                _isHeaderPrinted = true;
            }

            string applicationName = ApplicationName;
            if (applicationName.IsNullOrWhiteSpace())
            {
                applicationName = ChoAssembly.GetAssemblyTitle();
                if (applicationName.IsNullOrWhiteSpace())
                {
                    applicationName = ChoApplication.EntryAssemblyFileName;
                    //if (Assembly.GetEntryAssembly() != null && !Assembly.GetEntryAssembly().FullName.IsNullOrWhiteSpace()
                    //    && Assembly.GetEntryAssembly().FullName.IndexOf(',') > 0)
                    //    applicationName = Assembly.GetEntryAssembly().FullName.SplitNTrim(',')[0]; // EntryAssemblyFileName; //ChoGlobalApplicationSettings.Me.ApplicationName;
                }
            }
            if (applicationName.IsNullOrWhiteSpace())
                applicationName = "Unknown";

            string version = Version;
            
            if (version.IsNullOrWhiteSpace())
                version = ChoAssembly.GetEntryAssembly().GetName().Version.ToString();

            Console.WriteLine("{0} [Version {1}]".FormatString(applicationName, version));

            string copyright = Copyright;
            if (copyright.IsNullOrWhiteSpace())
                copyright = ChoAssembly.GetAssemblyCopyright();

            if (!copyright.IsNullOrWhiteSpace())
                Console.WriteLine(copyright);

            string description = Description;
            if (description.IsNullOrWhiteSpace())
                description = ChoAssembly.GetAssemblyDescription();

            if (!description.IsNullOrWhiteSpace())
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(description);
            }

            if (!AdditionalInfo.IsNullOrWhiteSpace())
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(AdditionalInfo);
            }

            Console.WriteLine();
        }

        #endregion Instance Members (Private)
    }
}
