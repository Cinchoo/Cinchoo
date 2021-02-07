namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Resources;
	using System.Reflection;
	using System.Globalization;
	using System.Collections.Generic;

	using Cinchoo.Core.Text;
	using Cinchoo.Core;
	using Cinchoo.Core.Properties;
	using System.Security.Permissions;
	using System.Runtime.Remoting.Proxies;
	using Cinchoo.Core.Pattern;

	#endregion NameSpaces

	public class ChoConfigurationManagementFactory
	{
        //public static T CreateInstance<T>() where T : class
        //{
        //    Type configObjType = typeof(T);

        //    if (typeof(ContextBoundObject).IsAssignableFrom(configObjType))
        //        return ChoSingleton<T>.GetInstance(SingletonTypeValidationRules.AllowAll);
        //    else
        //    {
        //        if (!ChoObjectManagementFactory.IsCached(configObjType))
        //        {
        //            //Load config file and assign values to object memebers
        //            ChoConfigurationSectionAttribute configurationElement = ChoType.GetAttribute(configObjType, typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
        //            if (configurationElement == null || configurationElement.GetMe(configObjType) == null)
        //                throw new ChoConfigurationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1001, configObjType.Name));

        //            return (T)configurationElement.GetMe(configObjType).Construct(configObjType);
        //        }
        //        else
        //            return (T)ChoObjectManagementFactory.GetObject(configObjType);
        //    }

        //}
	}
}
