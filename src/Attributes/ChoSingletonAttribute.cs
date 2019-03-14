namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Runtime.Remoting.Proxies;
	using Cinchoo.Core.Services;
	using Cinchoo.Core.Runtime.Remoting;
	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.IO;
	using Cinchoo.Core.Diagnostics;

	#endregion NameSpaces

	public sealed class ChoSingletonLoggableObject : ChoLoggableObject
	{
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ChoSingletonAttribute : ChoLoggableProxyAttribute
	{
		#region Shared Data Members (Private)

		private readonly static ChoDictionaryService<Type, RealProxy> _dictService = new ChoDictionaryService<Type, RealProxy>(typeof(ChoSingletonAttribute).Name);

		#endregion Shared Data Members (Private)

		#region Instance Data Members (Private)

		private ChoSingletonLoggableObject _singletonElement;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoSingletonAttribute()
		{
		}

		#endregion

        #region Instance Members (Public)

        public ChoLoggableObject GetMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (_singletonElement == null)
			{
				lock (SyncRoot)
				{
					if (_singletonElement == null)
					{
                        _singletonElement = new ChoSingletonLoggableObject();
						_singletonElement.LogDirectory = LogDirectory;
						_singletonElement.LogFileName = LogFileName.IsNullOrEmpty() ? ChoPath.AddExtension(type.FullName, ChoReservedFileExt.Log) : LogFileName;
						_singletonElement.LogCondition = LogCondition;
						_singletonElement.LogTimeStampFormat = LogTimeStampFormat;
					}
				}
			}

			return _singletonElement;
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
                    RealProxy proxy = new ChoSingletonProxy(base.CreateInstance(configObjType), configObjType);
					_dictService.SetValue(configObjType, proxy);
					return (MarshalByRefObject)proxy.GetTransparentProxy();
				}
			}
		}

		#endregion ChoConfigurationElementAttribute Overrides
	}
}
