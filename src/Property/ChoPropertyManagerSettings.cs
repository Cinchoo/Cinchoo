namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Text;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    [ChoTypeFormatter("Property Manager Settings")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    [ChoConfigurationSection("cinchoo/propertyManagerSettings", Defaultable = false)]
    [XmlRoot("propertyManagerSettings")]
    [ChoBufferProfile("Loaded Property Replacers....", NameFromTypeFullName = typeof(ChoPropertyManagerSettings))]
    public class ChoPropertyManagerSettings : IChoInitializable
    {
        #region Instance Data Members (Public)

        [XmlArray("propertyReplacers")]
        //[XmlArrayItem("propertyDictionaryReplacer", typeof(ChoDictionaryPropertyReplacer))]
        //[XmlArrayItem("propertyKeyValueReplaceHandler", typeof(ChoCustomKeyValuePropertyReplacer))]
        [ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable=false)]
        public ChoObjConfigurable[] PropertyDictionaryReplacers = new ChoObjConfigurable[0];

        [XmlArray("propertyCustomReplaceHandlers")]
        //[XmlArrayItem("propertyCustomReplaceHandler", typeof(ChoCustomPropertyReplacer))]
        [ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable = false)]
        public ChoObjConfigurable[] CustomPropertyReplacers = new ChoObjConfigurable[0];

        #endregion

        #region Instance Data Members (Private)

        private readonly object _padLock = new object();
        private ChoDictionary<string, IChoPropertyReplacer> _propertyReplacers;

        #endregion

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable = false)]
        public IChoPropertyReplacer[] PropertyReplacers
        {
            get 
            {
                if (_propertyReplacers == null)
                    return new IChoPropertyReplacer[] { };
                else
                {
                    lock (_padLock)
                    {
                        return _propertyReplacers.ToValuesArray();
                    }
                }
            }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail Property Replacers", Formatter = typeof(ChoArrayToStringFormatter))]
        [ChoPropertyInfo(Persistable = false)]
        internal string[] PropertyReplacersNames
        {
            get {
                if (_propertyReplacers == null)
                    return new string[] { };
                else
                {
                    lock (_padLock)
                    {
                        return _propertyReplacers.ToKeysArray();
                    }
                }
            }
        }

        #endregion

        #region Shared Properties

        private static readonly object _padLockFactory = new object();
        private static ChoPropertyManagerSettings _instance;

        public static ChoPropertyManagerSettings Me
        {
            get 
            {
                if (_instance != null)
                    return _instance;

                lock (_padLockFactory)
                {
                    if (_instance == null)
                    {
                        _instance = new ChoPropertyManagerSettings();
                        _instance.Initialize();
                        ChoApplication.RaiseAfterAppFrxSettingsLoaded(_instance);
                    }
                }

                return _instance;
            }
        }

        private static ChoDictionary<string, IChoPropertyReplacer> _defaultPropertyReplacers;
        public static ChoDictionary<string, IChoPropertyReplacer> DefaultPropertyReplacers
        {
            get
            {
                if (_defaultPropertyReplacers == null)
                {
                    _defaultPropertyReplacers = new ChoDictionary<string, IChoPropertyReplacer>();
                    
                    ChoGlobalDictionaryPropertyReplacer globalDictionaryPropertyReplacer = new ChoGlobalDictionaryPropertyReplacer();
                    _defaultPropertyReplacers.Add(globalDictionaryPropertyReplacer.Name, globalDictionaryPropertyReplacer);

                    _defaultPropertyReplacers.Add(ChoStaticDictionaryPropertyReplacer.Me.Name, ChoStaticDictionaryPropertyReplacer.Me);

                    ChoEnvironmentVariablePropertyReplacer environmentVariablePropertyReplacer = new ChoEnvironmentVariablePropertyReplacer();
                    _defaultPropertyReplacers.Add(environmentVariablePropertyReplacer.Name, environmentVariablePropertyReplacer);

                    ChoApplicationPropertyReplacer applicationPropertyReplacer = new ChoApplicationPropertyReplacer();
                    _defaultPropertyReplacers.Add(applicationPropertyReplacer.Name, applicationPropertyReplacer);

                    //_defaultPropertyReplacers.Add(typeof(ChoGlobalCustomPropertyReplacer).Name, new ChoGlobalCustomPropertyReplacer());

                    ChoGlobalDynamicMethodInvokeReplacer globalDynamicMethodInvokeReplacer = new ChoGlobalDynamicMethodInvokeReplacer();
                    _defaultPropertyReplacers.Add(globalDynamicMethodInvokeReplacer.Name, globalDynamicMethodInvokeReplacer);

                    ChoGlobalExpressionEvaluatorReplacer globalPropertyEvaluatorReplacer = new ChoGlobalExpressionEvaluatorReplacer();
                    _defaultPropertyReplacers.Add(globalPropertyEvaluatorReplacer.Name, globalPropertyEvaluatorReplacer);
                }

                return _defaultPropertyReplacers;
            }
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize()
        {
            if (_propertyReplacers == null)
                _propertyReplacers = new ChoDictionary<string, IChoPropertyReplacer>(DefaultPropertyReplacers);

            if (_propertyReplacers.Count == DefaultPropertyReplacers.Count)
            {
                if (PropertyDictionaryReplacers != null)
                {
                    foreach (ChoObjConfigurable objConfigurable in PropertyDictionaryReplacers)
                    {
                        if (objConfigurable == null)
                            continue;
                        if (String.IsNullOrEmpty(objConfigurable.Name))
                            continue;
                        if (!(objConfigurable is IChoKeyValuePropertyReplacer))
                            continue;

                        if (_propertyReplacers.ContainsKey(objConfigurable.Name))
                        {
                            ChoProfile.WriteLine(String.Format("Item with {0} key already exists.", objConfigurable.Name));
                            continue;
                        }

                        _propertyReplacers.Add(objConfigurable.Name, objConfigurable as IChoPropertyReplacer);
                    }
                }
                if (CustomPropertyReplacers != null)
                {
                    foreach (ChoObjConfigurable objConfigurable in CustomPropertyReplacers)
                    {
                        if (objConfigurable == null)
                            continue;
                        if (String.IsNullOrEmpty(objConfigurable.Name))
                            continue;
                        if (!(objConfigurable is IChoCustomPropertyReplacer))
                            continue;

                        if (_propertyReplacers.ContainsKey(objConfigurable.Name))
                        {
                            ChoProfile.WriteLine(String.Format("Item with {0} key already exists.", objConfigurable.Name));
                            continue;
                        }

                        _propertyReplacers.Add(objConfigurable.Name, objConfigurable as IChoPropertyReplacer);
                    }
                }
            }
        }

        #endregion

        #region Instance Members (Public)

        public bool Add(IChoPropertyReplacer propertyReplacer)
        {
            ChoGuard.ArgumentNotNull(propertyReplacer, "PropertyReplacer");
            lock (_padLock)
            {
                if (_propertyReplacers.ContainsKey(propertyReplacer.Name))
                    return false;

                _propertyReplacers.Add(propertyReplacer.Name, propertyReplacer);
                return true;
            }
        }

        public bool Remove(IChoPropertyReplacer propertyReplacer)
        {
            ChoGuard.ArgumentNotNull(propertyReplacer, "PropertyReplacer");
            return Remove(propertyReplacer.GetType().Name);
        }

        public bool Remove(string name)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
            lock (_padLock)
            {
                if (!_propertyReplacers.ContainsKey(name))
                    return false;

                _propertyReplacers.Remove(name);
                return true;
            }
        }

        public string GetHelpText()
        {
            StringBuilder msg = new StringBuilder("List of available properties" + Environment.NewLine);

            msg.AppendLine();
            msg.AppendLine("@this - Context info will be replaced.");
            msg.AppendLine();

            foreach (IChoPropertyReplacer propertyReplacer in PropertyReplacers)
            {
                if (propertyReplacer.AvailablePropeties == null) continue;

                ChoStringMsgBuilder msg1 = new ChoStringMsgBuilder(propertyReplacer.Name);

                foreach (KeyValuePair<string, string> keyValue in propertyReplacer.AvailablePropeties)
                {
                    msg1.AppendFormatLine("{0} - {1}", keyValue.Key, keyValue.Value);
                }

                msg.AppendLine(msg1.ToString());
            }

            ChoStringMsgBuilder msg2 = new ChoStringMsgBuilder("Application Propeties");
            foreach (Dictionary<string, string> dict in ChoApplication.GetPropertyHelpTexts())
            {
                if (dict == null) continue;

                foreach (KeyValuePair<string, string> keyValue in dict)
                {
                    msg2.AppendFormatLine("{0} - {1}", keyValue.Key, keyValue.Value);
                }
            }
            msg.AppendLine(msg2.ToString());

            return msg.ToString();
        }

        #endregion Instance Members (Public)
    }
}
