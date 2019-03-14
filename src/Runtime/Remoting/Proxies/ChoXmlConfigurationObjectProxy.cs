namespace Cinchoo.Core.Runtime.Remoting.Proxies
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cinchoo.Core.Properties;
	using Cinchoo.Core.Services;
	using Cinchoo.Core.Attributes;
	using Cinchoo.Core.Exceptions;
	using System.Globalization;
	using System.Runtime.Remoting.Activation;
	using System.Runtime.Remoting.Messaging;
	using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	public class ChoXmlConfigurationObjectProxy : ChoRealProxy
	{
		#region Constructors

		public ChoXmlConfigurationObjectProxy(Type type)
			: base(type)
		{
		}

		#endregion Constructors

		public override object DoObjectInitialize(object target)
		{
			base.DoObjectInitialize(target);

			Type configObjType = target.GetType();
			ChoBaseConfigurationSectionAttribute configurationElement = ChoType.GetAttribute(configObjType, typeof(ChoBaseConfigurationSectionAttribute)) as ChoBaseConfigurationSectionAttribute;
			if (configurationElement == null || configurationElement.GetMe(configObjType) == null)
				throw new ChoConfigurationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1001, configObjType.Name));

			return configurationElement.GetMe(configObjType).Construct(target);
		}
	}
}
