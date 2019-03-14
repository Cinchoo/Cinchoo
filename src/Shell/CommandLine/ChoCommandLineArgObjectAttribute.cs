namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Runtime.Remoting.Proxies;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Runtime.Remoting;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoCommandLineArgObjectAttribute : ChoProxyAttribute
    {
		#region Shared Data Members (Private)

		private readonly static ChoDictionaryService<Type, RealProxy> _dictService = new ChoDictionaryService<Type, RealProxy>(typeof(ChoSingletonAttribute).Name);

		#endregion Shared Data Members (Private)

		#region Instance Data Members (Private)

        private ChoCommandLineArgObjectDirector _loggableElement;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoCommandLineArgObjectAttribute()
		{
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

        #endregion Instance Properties (Public)

        #region Instance Members (Public)

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

        public override MarshalByRefObject CreateInstance(Type configObjType)
		{
			if (_dictService.ContainsKey(configObjType))
				return (MarshalByRefObject)_dictService.GetValue(configObjType).GetTransparentProxy();

			lock (_dictService.SyncRoot)
			{
				if (_dictService.ContainsKey(configObjType))
					return (MarshalByRefObject)_dictService.GetValue(configObjType).GetTransparentProxy();
				else
				{
                    RealProxy proxy = new ChoCommandLineArgsObjectProxy(base.CreateInstance(configObjType), configObjType);
					_dictService.SetValue(configObjType, proxy);
                    PrintHeader();
					return (MarshalByRefObject)proxy.GetTransparentProxy();
				}
			}
		}

		#endregion ChoConfigurationElementAttribute Overrides

        #region Instance Members (Private)

        private void PrintHeader()
        {
            string applicationName = ApplicationName;
            if (applicationName.IsNullOrWhiteSpace())
                applicationName = ChoGlobalApplicationSettings.Me.ApplicationName;

            string version = ChoAssembly.GetEntryAssembly().GetName().Version.ToString();

            ChoConsole.WriteLine("{0} [Version {1}]".FormatString(applicationName, version));

            if (!Copyright.IsNullOrWhiteSpace())
                ChoConsole.WriteLine(Copyright);

            ChoConsole.WriteLine();
        }

        #endregion Instance Members (Private)
    }
}
