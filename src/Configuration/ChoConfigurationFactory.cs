namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Globalization;
    using System.Collections.Generic;

    using eSquare.Core.Text;
    using eSquare.Core.Attributes;
    using eSquare.Core.Properties;

    #endregion NameSpaces

    public static class ChoConfigurationManagementFactory
    {
        private static Dictionary<Type, Object> _globalConfigurationsCache = new Dictionary<Type, object>();
        private static readonly object _cacheLockObj = new object();

        public static string ToString(object configObject)
        {
            if (configObject == null) return String.Empty;

            string configObjectString = ChoObject.ToString(configObject);

            ChoConfigurationElementMapAttribute configurationElementMap = ChoType.GetAttribute(configObject.GetType(), typeof(ChoConfigurationElementMapAttribute)) as ChoConfigurationElementMapAttribute;
            if (configurationElementMap == null)
                return configObjectString;
            else if (configObjectString == configObject.GetType().FullName)
            {
                ChoStringMsgBuilder msg = new ChoStringMsgBuilder(configurationElementMap.Description);

                MemberInfo[] memberInfos = ChoType.GetMembers(configObject.GetType(), typeof(ChoMemberInfoAttribute));
                if (memberInfos == null || memberInfos.Length == 0)
                    msg.AppendLine(ChoStringMsgBuilder.Empty);
                else
                {
                    string errMsg;
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        errMsg = ChoType.GetAttributeNameParameterValue(configObject.GetType(), memberInfo.Name, typeof(ChoMemberInfoAttribute), "ErrMsg") as string;
                        if (errMsg == null)
                            msg.AppendFormatLine("{0}: {1}", memberInfo.Name, ChoType.GetMemberValue(configObject, memberInfo.Name));
                        else
                            msg.AppendFormatLine("{0}: {1} [ERROR: {2}]", memberInfo.Name, ChoType.GetMemberValue(configObject, memberInfo.Name), errMsg);
                    }
                }
                msg.AppendNewLine();
                return msg.ToString();
            }
            else
            {
                StringBuilder msg = new StringBuilder(configObjectString);

                MemberInfo[] memberInfos = ChoType.GetMembers(configObject.GetType(), typeof(ChoMemberInfoAttribute));
                List<string> errMsgs = new List<string>();
                if (memberInfos != null && memberInfos.Length >= 0)
                {
                    string errMsg;
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        errMsg = ChoType.GetAttributeNameParameterValue(configObject.GetType(), memberInfo.Name, typeof(ChoMemberInfoAttribute), "ErrMsg") as string;
                        if (errMsg != null)
                            errMsgs.Add(String.Format("{0}: {1}", memberInfo.Name, errMsg));
                    }
                }

                if (errMsgs.Count > 0)
                {
                    ChoStringMsgBuilder errReport = new ChoStringMsgBuilder("Following errors produced while construction");

                    foreach (string errMsg in errMsgs)
                        errReport.AppendFormatLine(errMsg);

                    msg.AppendLine(errReport.ToString());
                }

                return msg.ToString();
            }
        }

        public static object CreateInstance(Type configObjType)
        {
            if (configObjType == null)
                throw new ArgumentNullException("configObjType");

            lock (_cacheLockObj)
            {
                if (_globalConfigurationsCache.ContainsKey(configObjType))
                    return _globalConfigurationsCache[configObjType];
                else
                {
                    object configObject = ChoType.CreateInstance(configObjType);
                    object owner = configObject;

                    if (ChoType.IsInterceptableObject(configObjType))
                        configObject = ChoInterceptingProxy.Instance(configObject);

                    //Load config file and assign values to object memebers
                    ChoConfigurationElementMapAttribute configurationElementMap = ChoType.GetAttribute(configObjType, typeof(ChoConfigurationElementMapAttribute)) as ChoConfigurationElementMapAttribute;
                    if (configurationElementMap == null)
                        throw new NullReferenceException(String.Format(CultureInfo.InvariantCulture, Resources.ES1001, configObjType.Name));

                    if (ChoType.IsConfigurableObject(configObjType))
                        ((ChoConfigurableObject)owner).ReadOnly = configurationElementMap.ReadOnly;

                    _globalConfigurationsCache[configObjType] = configObject;

                    configurationElementMap.Construct(configObject);

                    return configObject;
                }
            }
        }
    }
}
