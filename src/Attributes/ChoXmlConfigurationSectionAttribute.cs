namespace Cinchoo.Core.Attributes
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Reflection;
	using System.Collections;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	using Cinchoo.Core.Properties;
	using Cinchoo.Core.Exceptions;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.Collections.Specialized;
	using Cinchoo.Core.Configuration.Sections;
	using System.Runtime.Remoting.Proxies;
	using Cinchoo.Core.Runtime.Remoting.Proxies;
	using Cinchoo.Core.Services;
	using Cinchoo.Core.IO;

	#endregion NameSpaces

	#region ChoXmlConfigurationSectionAttribute Class

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoXmlConfigurationSectionAttribute : ChoBaseConfigurationSectionAttribute
	{
		#region Instance Data Members (Private)

		private ChoXmlConfigurationElement _xmlConfigurationElement;

		#endregion Instance Data Members (Private)

		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoXmlConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoXmlConfigurationSectionAttribute(string configElementPath)
		{
			ConfigElementPath = configElementPath;
		}

		#endregion

		#region ChoConfigurationElementAttribute Overrides

		public override ChoBaseConfigurationElement GetMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (_xmlConfigurationElement == null)
			{
				lock (_padLock)
				{
					if (_xmlConfigurationElement == null)
					{
						_xmlConfigurationElement = new ChoXmlConfigurationElement(ConfigElementPath, BindingMode, TraceOutputDirectory, TraceOutputFileName.IsNullOrEmpty() ? ChoPath.AddExtension(type.FullName, ChoReservedFileExt.Log) : TraceOutputFileName);
						_xmlConfigurationElement.DefaultConfigSectionHandlerType = ConfigSectionHandlerType;
						_xmlConfigurationElement.LogCondition = LogCondition;
						_xmlConfigurationElement.LogTimeStampFormat = LogTimeStampFormat;
					}
				}
			}

			return _xmlConfigurationElement;
		}

		private readonly static ChoDictionaryService<Type, RealProxy> _instanceMsgs = new ChoDictionaryService<Type, RealProxy>("d");

		public override MarshalByRefObject CreateInstance(Type configObjType)
		{
			if (_instanceMsgs.ContainsKey(configObjType))
			{
				return (MarshalByRefObject)_instanceMsgs.GetValue(configObjType).GetTransparentProxy();
			}
			else
			{
				RealProxy proxy = new ChoXmlConfigurationObjectProxy(configObjType);
				_instanceMsgs.SetValue(configObjType, proxy);
				return (MarshalByRefObject)proxy.GetTransparentProxy();
			}
		}

		#endregion ChoConfigurationElementAttribute Overrides
	}

	#endregion ChoXmlConfigurationSectionAttribute Class
}
