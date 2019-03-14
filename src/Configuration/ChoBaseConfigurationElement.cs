namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using Cinchoo.Core;
    using Cinchoo.Core.Factory;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Properties;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    internal class ModifiedStateObject
    {
        private bool _isModified = false;

        public void SetModified(bool modified)
        {
            if (!_isModified && _isModified != modified)
                _isModified = modified;
        }

        public void ResetModified()
        {
            _isModified = false;
        }

        public bool IsModified
        {
            get { return _isModified; }
        }
    }

    //sfsdf
    [DebuggerDisplay("ConfigElementPath = {ConfigElementPath}")]
	public abstract class ChoBaseConfigurationElement : ChoLoggableObject
	{
		#region Instance Data Members (Private)

		protected ChoConfigSection ConfigSection;
		protected object DefaultConfigObject;
		protected Type ConfigObjectType;

        private readonly object _syncRoot = new object();
		private Type _configSectionHandlerType;
        private readonly ModifiedStateObject _modifiedStateObject = new ModifiedStateObject();

#if DEBUG
        int counter = 0;
#endif
		#endregion

		#region Instance Properties

        internal DateTime LastLoadedTimeStamp = DateTime.MinValue;

        internal Type ConfigbObjectType
		{
			get { return ConfigObjectType; }
		}

        public string ConfigSectionName
        {
            get;
            set;
        }

		private string _configElementPath;
		internal string ConfigElementPath
		{
			get { return _configElementPath; }
			private set
			{
				if (value.IsNullOrWhiteSpace())
					throw new NullReferenceException("ConfigElementPath");

				_configElementPath = value;
                ConfigSectionName = GetConfigNameFromPath(_configElementPath);
			}
		}

		internal Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			set
			{
				if (_configSectionHandlerType == value)
					return;
				_configSectionHandlerType = value;
			}
		}

        internal IChoConfigStorage ConfigStorage
        {
            get;
            set;
        }

        internal virtual Type ConfigurationMetaDataType
        {
            get;
            set;
        }

        private ChoBaseConfigurationMetaDataInfo _metaDataInfo;
        public ChoBaseConfigurationMetaDataInfo MetaDataInfo
        {
            get { return _metaDataInfo; }
            set
            {
                if (value != null)
                {
                    _metaDataInfo = value;
                    ApplyConfigMetaData(_metaDataInfo);
                }
            }
        }

        internal Type DefaultConfigSectionHandlerType
		{
			get;
			set;
		}

        private ChoConfigurationBindingMode _bindingMode;
		public ChoConfigurationBindingMode BindingMode
		{
            get
            {
                return _bindingMode;
            }
			set
			{
                _bindingMode = value;
				switch (value)
				{
					case ChoConfigurationBindingMode.TwoWay:
                        _persistable = true;
						_watchChange = true;
						break;
					case ChoConfigurationBindingMode.OneWay:
                        _persistable = false;
						_watchChange = true;
						break;
					case ChoConfigurationBindingMode.OneWayToSource:
                        _persistable = true;
						_watchChange = false;
						break;
					case ChoConfigurationBindingMode.OneTime:
                        _persistable = false;
						_watchChange = false;
						break;
				}
			}
		}

        private bool _firstTime = true;
        private bool _inLoadingProcess = false;

        private bool _defaultable = true;
        public bool Defaultable
        {
            get { return _defaultable; }
            set { _defaultable = value; }
        }

        private bool _ignoreCase = true;
        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        public string ConfigFilePath
        {
            get;
            set;
        }

        private bool _persistable = true;
        public bool Persistable
        {
            get { return _persistable; }
        }

        private bool _watchChange;
        public bool WatchChange
        {
            get { return _watchChange; }
        }

		private bool _silent = true;
        internal bool Silent
		{
			get { return _silent; }
			set { _silent = value; }
		}

		private readonly ChoDictionaryService<string, object> _stateInfo = new ChoDictionaryService<string, object>("ConfigStateInfoDictService");
		public ChoDictionaryService<string, object> StateInfo
		{
			get { return _stateInfo; }
		}

		public object this[string key]
		{
			get { return _stateInfo[key]; }
			set { _stateInfo[key] = value; }
		}

		#endregion Instance Properties

		#region Constructors

        public ChoBaseConfigurationElement(string configElementPath)
		{
			ConfigElementPath = configElementPath;
		}

		#endregion Constructors

		#region Instance Members (Abstract)

        public abstract void GetConfig(Type configObjectType, bool refresh);
		public abstract void PersistConfig(ChoDictionaryService<string, object> stateInfo);
		protected abstract void ApplyConfigMetaData(ChoBaseConfigurationMetaDataInfo configurationMetaDataInfo);

		#endregion Instance Members (Abstract)

		#region Instance Members (Public)

		internal virtual object Construct(object configObject)
		{
			DefaultConfigObject = configObject;
            if (configObject != null)
            {
                ConfigObjectType = configObject.GetType();
                Reset();
            }
			Refresh(false);

			return ConfigObject;
		}

		public virtual object Construct(Type configObjType)
		{
			ConfigObjectType = configObjType;
			ChoObjectFactoryAttribute objectFactoryAttribute = ChoType.GetAttribute(configObjType, typeof(ChoObjectFactoryAttribute)) as ChoObjectFactoryAttribute;
			if (objectFactoryAttribute == null)
			{
				ChoType.SetCustomAttribute(configObjType, new ChoObjectFactoryAttribute(ChoObjectConstructionType.Singleton));
				objectFactoryAttribute = ChoType.GetAttribute(configObjType, typeof(ChoObjectFactoryAttribute)) as ChoObjectFactoryAttribute;
				if (objectFactoryAttribute == null)
					throw new ChoConfigurationConstructionException(String.Format("Failed to set custom attribute to object {0} of type.", configObjType.Name));
			}

			bool isModfied = false;
			Exception ex;
			DefaultConfigObject = ChoObjectManagementFactory.CreateInstance(configObjType, objectFactoryAttribute.ObjectConstructionType, true, out ex);
			SetNThrowException(ex, ref isModfied);
            Reset();

			Refresh(false);

			return ConfigObject;
		}

        private void Reset()
        {
            if (ConfigObject == null)
                return;

            ChoConfigurationObjectErrorManagerService.ResetObjectErrors(ConfigObject);

            //MemberInfo[] memberInfos = ChoType.GetMembers(ConfigObject.GetType(), typeof(ChoConfigurationPropertyAttribute));
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(ConfigObject.GetType());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                ChoPropertyInfoAttribute memberInfoAttribute = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                    string name = ChoType.GetMemberName(memberInfo);
                    object defaultValue = null;
                    bool isDefaultValueSpecified = ChoConfigurationMetaDataManager.TryConfigDefaultValue(this, name, memberInfoAttribute, out defaultValue);
                    if (memberInfoAttribute == null || !isDefaultValueSpecified)
                        continue;

                    try
                    {
                        //object newConvertedValue = memberInfoAttribute.DefaultValue;
                        object newConvertedValue = ChoConvert.ConvertFrom(ConfigObject, defaultValue, ChoType.GetMemberType(memberInfo),
                            ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                        SetConfigPropertyValue(newConvertedValue, memberInfo);
                        //ChoType.SetMemberValue(ConfigObject, memberInfo, newConvertedValue);
                    }
                    catch (Exception innerEx)
                    {
                        ChoConfigurationObjectErrorManagerService.SetObjectMemberError(ConfigObject, memberInfo.Name, String.Format("Failed to assign `{0}` default value. {1}", ChoString.ToString(defaultValue), innerEx.Message));
                    }
                }
            }
        }


		protected virtual void TraceOutput(bool isDirty)
		{
			if (!LogCondition)
				return;

			if (ConfigObject == null)
				throw new ChoConfigurationConstructionException("configObject");

			try
			{
                ChoStringMsgBuilder msg = new ChoStringMsgBuilder(String.Format("{0} {1}", isDirty ? "*" : " ", ChoObject.ToString(ConfigObject, ChoFormattableObject.ExtendedFormatName)));

				if (ConfigSection != null && MetaDataInfo != null)
				{
					msg.AppendNewLine();
					ChoStringMsgBuilder metaDataMsg = new ChoStringMsgBuilder(ChoObject.ToString(MetaDataInfo));
					msg.Append(metaDataMsg.ToString());
				}

				Log(msg.ToString());
			}
			catch (Exception ex)
			{
				Trace.Write(ex);
				if (!Silent)
					throw;
			}
		}

		#endregion Instance Members (Public)

		#region Instance Members (Internal)

		internal void Persist(bool traceOutput, ChoDictionaryService<string, object> stateInfo)
		{
			lock (SyncRoot)
			{
                if (ConfigSection == null || ConfigSection.ConfigData == null)
                {
                    if (_firstTime && _defaultable)
                        PersistInternal(traceOutput, stateInfo);
                    else
                        ChoConfigurationMetaDataManager.SetMetaDataSection(this);
                }
                else if (_persistable)
                {
                    PersistInternal(traceOutput, stateInfo);
                }
/*
                if (_bindingMode == ChoConfigurationBindingMode.OneTime)
                {
                    if (_firstTime)
                        PersistInternal(traceOutput, stateInfo);
                }
                else if (!_persistable || (ConfigSection == null || ConfigSection.ConfigData == null))
                {
                    ChoConfigurationMetaDataManager.SetMetaDataSection(this);
                }
                else
                {
                    PersistInternal(traceOutput, stateInfo);
                }
 * */
			}
		}

        private void PersistInternal(bool traceOutput, ChoDictionaryService<string, object> stateInfo)
        {
            if (stateInfo == null)
                stateInfo = StateInfo.Clone();
            else
                stateInfo.Merge(StateInfo);

            stateInfo[ChoConfigurationConstants.PERSIST_TIME_STAMP] = DateTime.Now;
            if (ConfigObject is ChoConfigurableObject)
            {
                if (((ChoConfigurableObject)ConfigObject).RaiseBeforeConfigurationObjectPersisted())
                    return;
            }

            PersistConfig(stateInfo);

            //Print the output to file
            if (traceOutput)
                TraceOutput(true);

            ////Call AfterConfigurationMemberPersist for each member
            //if (ConfigObject is ChoConfigurableObject)
            //    ((ChoConfigurableObject)ConfigObject).CallAfterConfigurationMemberPersist();

            //if (ConfigObject is ChoConfigurableObject)
            //    ((ChoConfigurableObject)ConfigObject).OnAfterConfigurationObjectPersisted();
        }

        internal void Refresh(bool refresh)
        {
            if (_inLoadingProcess)
                return;
      
            lock (SyncRoot)
            {
                if (ConfigObject is IChoConfigurationParametersOverridable)
                    ((IChoConfigurationParametersOverridable)ConfigObject).OverrideParameters(this);

                _modifiedStateObject.ResetModified();

                bool isDirty = false;
                bool errorHandled = false;
                bool canTraceOutput = true;
                bool hasErrors = false;
                this[ChoConfigurationConstants.FORCE_PERSIST] = false;

                _inLoadingProcess = true;

                ChoConfigSection prevConfigSection = ConfigSection;

                if (ConfigSection != null)
                    ConfigSection.StopWatching();

                try
                {
                    if (ConfigObject != null)
                    {
                        //ChoConfigurationObjectErrorManagerService.ResetObjectErrors(ConfigObject);

                        if (ConfigObject is ChoInterceptableObject)
                        {
                            ChoInterceptableObject interceptableObject = ConfigObject as ChoInterceptableObject;
                            interceptableObject.SetDirty(false);
                            interceptableObject.SetSilent(false);
                            interceptableObject.SetInitialized(false);
                            interceptableObject.IsConfigObjectSilent = Silent;
                        }
                    }

                    GetConfig(ConfigObjectType, refresh);
                }
                catch (ConfigurationErrorsException configErrorsEx)
                {
                    bool isModified = false;
                    errorHandled = SetNThrowException(configErrorsEx, ref isModified);
                    _modifiedStateObject.SetModified(isModified);
                }
                catch (TypeInitializationException typeEx)
                {
                    bool isModified = false;
                    errorHandled = SetNThrowException(typeEx, ref isModified);
                    _modifiedStateObject.SetModified(isModified);
                }
                catch (ChoFatalApplicationException)
                {
                    canTraceOutput = false;
                    throw;
                }
                catch (Exception ex)
                {
                    bool isModified = false;
                    errorHandled = SetNThrowException(ex, ref isModified);
                    _modifiedStateObject.SetModified(isModified);
                }
                finally
                {
                    if (ConfigSection != null && ConfigObject is ChoConfigurableObject)
                    {
                        //Call Notify Property Changed for all default values
                        CallNotifyPropertyChangedForAllDefaultableMembers();
                        ConfigSection.Initialize();
                    }

                    if (ConfigSection != null && ConfigSection.ConfigLoadException != null)
                    {
                        bool isModified = false;
                        errorHandled = SetNThrowException(ConfigSection.ConfigLoadException, ref isModified);
                        _modifiedStateObject.SetModified(isModified);
                    }

                    ////Update configuration meta data information
                    //if (ConfigSection != null && ConfigSection.MetaDataInfo != null)
                    //    ApplyConfigMetaData(ConfigSection.MetaDataInfo);

                    //Print the output to file
                    if (canTraceOutput)
                    {
                        //Set default trace output file name
                        if (LogFileName.IsNullOrEmpty())
                            LogFileName = ChoPath.AddExtension(ConfigObject.GetType().FullName, ChoReservedFileExt.Log);

                        //if (ConfigSection is IChoCustomConfigSection)
                        //{
                        //    if (ConfigSection.ConfigData != null)
                        //        ConfigSection.ConfigObject = ConfigSection.ConfigData as ChoConfigurableObject;
                        //    ChoObjectManagementFactory.SetInstance(ConfigObject);
                        //}

                        if ((ConfigSection == null || ConfigSection.ConfigData == null) && !_defaultable /*&& !_persistable*/)
                        {
                            throw new ChoConfigurationConstructionException(String.Format("Failed to load '[{0}]' configuration section.", this._configElementPath));
                        }
                        else
                        {
                            if (prevConfigSection != null)
                                prevConfigSection.Dispose();

                            if (ConfigObject is ChoConfigurableObject)
                                ((ChoConfigurableObject)ConfigObject).SetReadOnly(true);

                            bool hasConfigSectionDefined = ConfigSection != null ? ConfigSection.HasConfigSectionDefined : false;

                            try
                            {
                                if (!errorHandled)
                                {
                                    if (!(ConfigSection is IChoCustomConfigSection))
                                    {
                                        if (hasConfigSectionDefined)
                                        {
                                            _modifiedStateObject.SetModified(ExtractNPopulateValues(ref hasErrors, ref isDirty));
                                        }
                                        else
                                            _modifiedStateObject.SetModified(AssignToFallbackOrDefaultValues(ref isDirty));
                                    }
                                    else
                                    {
                                        if (hasConfigSectionDefined)
                                        {
                                            //isModfied = true;
                                            if (ConfigSection == null)
                                                _modifiedStateObject.SetModified(AssignToFallbackOrDefaultValues(ref isDirty));
                                            else if (!ChoObject.Equals(ConfigSection, prevConfigSection))
                                                _modifiedStateObject.SetModified(true);
                                            else
                                                _modifiedStateObject.SetModified(AssignToFallbackOrDefaultValues(ref isDirty));
                                        }
                                        else
                                            _modifiedStateObject.SetModified(AssignToFallbackOrDefaultValues(ref isDirty));
                                    }
                                }

                                if (!hasErrors)
                                    hasErrors = ChoConfigurationObjectErrorManagerService.ContainsObjectError(ConfigObject);

                                ChoObjectInitializer.Initialize(ConfigObject, false, ConfigSection != null ? ConfigObject : null);

                                SetWatcher(false);

                                if (ConfigObject is ChoInterceptableObject)
                                {
                                    ChoInterceptableObject interceptableObject = ConfigObject as ChoInterceptableObject;
                                    if (interceptableObject.Dirty)
                                    {
                                        isDirty = interceptableObject.Dirty;
                                    }
                                    //if (interceptableObject.IsModified)
                                    //{
                                    //    isModfied = interceptableObject.IsModified;
                                    //}
                                    interceptableObject.SetDirty(false);
                                    //interceptableObject.SetSilent(false);
                                    interceptableObject.SetInitialized(true);
                                }

                                if (ConfigObject is ChoConfigurableObject)
                                {
                                    bool invokeObjectLoaded = /* isDirty || */  _firstTime || _modifiedStateObject.IsModified;
                                    if (invokeObjectLoaded || hasErrors)
                                    {
                                        TraceOutput(false);
                                        if (invokeObjectLoaded)
                                            ((ChoConfigurableObject)ConfigObject).RaiseAfterConfigurationObjectLoaded();
                                    }

                                    //if (isDirty)
                                    //{
                                    //    TraceOutput(false);
                                    //    ((ChoConfigurableObject)ConfigObject).OnAfterConfigurationObjectLoaded();
                                    //}
                                    //else if (hasErrors)
                                    //    TraceOutput(false);
                                }
                                else
                                    TraceOutput(false);

                                if (!isDirty && ConfigSection != null)
                                    isDirty = ConfigSection.IsMetaDataDefinitionChanged;
                            }
                            finally
                            {
                                if (prevConfigSection != null)
                                {
                                    prevConfigSection.Dispose();
                                    prevConfigSection = null;
                                }

                                if (ConfigObject is ChoConfigurableObject)
                                    ((ChoConfigurableObject)ConfigObject).SetReadOnly(false);

                                if (!hasConfigSectionDefined || isDirty || hasErrors)
                                {
                                    if (_defaultable)
                                    {
                                        Persist(false, null);
                                    }
                                }
                                else if ((bool)this[ChoConfigurationConstants.FORCE_PERSIST])
                                    Persist(false, null);

                                _inLoadingProcess = false;
                                if (_watchChange && ConfigSection != null)
                                    ConfigSection.StartWatching();

                                ChoConfigurationObjectErrorManagerService.ResetObjectErrors(ConfigObject);
                                _firstTime = false;
                            }
                        }
                    }
                }
            }
        }

		#endregion Instance Members (Internal)

		#region Instance Members (Private)

		internal object ConfigObject
		{
			get
			{
				if (ConfigSection != null && ConfigSection.ConfigObject == null)
					ConfigSection.ConfigObject = DefaultConfigObject;

				if (ConfigSection is IChoCustomConfigSection)
					return DefaultConfigObject;
				else
					return ConfigSection == null || ConfigSection.ConfigObject == null ? DefaultConfigObject : ConfigSection.ConfigObject;
			}
		}

		private bool SetNThrowException(Exception ex, ref bool isModified)
		{
			bool handled = false;

			if (ex == null)
				return handled;

			if (ConfigObject != null && ConfigObject is ChoConfigurableObject)
				handled = ((ChoConfigurableObject)ConfigObject).RaiseConfigurationObjectLoadError(ex, ref isModified);

			if (!handled)
			{
				if (ConfigObject != null && ConfigObject is IChoExceptionHandledObject)
					handled = ((IChoExceptionHandledObject)ConfigObject).HandleException(ex, ref isModified);
			}

			//ChoConfigurationErrorsProfiler.Me.AppendLine(ChoApplicationException.ToString(ex));

			if (!Silent)
				throw new ChoConfigurationConstructionException("Failed to build configuration element.", ex);
			else
			{
				//if (_configSection is IChoCustomConfigSection)
				//    ChoConfigurationObjectErrorManagerService.SetObjectError(_configSection.ConfigData, ChoApplicationException.ToString(ex));
				//else
					ChoConfigurationObjectErrorManagerService.SetObjectError(ConfigObject, ChoApplicationException.ToString(ex));
			}
			return handled;
		}

        public void Refresh()
        {
            try
            {
                //Refresh(true);
                ChoQueuedExecutionService.Global.Enqueue(() => Refresh(true));
            }
            catch { }
        }

		private void OnConfigurationChanged(object sender, ChoConfigurationChangedEventArgs e)
		{
            this.LastLoadedTimeStamp = e.LastUpdatedTimeStamp;
			Refresh();
		}

        private void CallNotifyPropertyChangedForAllDefaultableMembers()
        {
            if (!(ConfigObject is ChoConfigurableObject)) return;

            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(ConfigObject.GetType());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //Set member values
                ChoPropertyInfoAttribute memberInfoAttribute = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                        continue;

                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                    if (memberInfoAttribute == null || !memberInfoAttribute.IsDefaultValueSpecified) continue;

                    ((ChoConfigurableObject)ConfigObject).OnPropertyChanged(memberInfo.Name);
                }
            }
        }

        private bool ExtractNPopulateValues(ref bool hasErrors, ref bool isDirty)
        {
            bool isModfied = false;
            object oldValue = null;
            object newValue = null;
            object origValue = null;
            object defaultValue = null;
            object fallbackValue = null;

            bool isConfigmemberDefined = false;

            //MemberInfo[] memberInfos = ChoType.GetMembers(ConfigObject.GetType(), typeof(ChoConfigurationPropertyAttribute));
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(ConfigObject.GetType());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //Set member values
                string name;
                ChoPropertyInfoAttribute memberInfoAttribute = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                        continue;

                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));

                    //if (memberInfoAttribute == null) continue;

                    oldValue = null;
                    newValue = null;
                    origValue = null;
                    defaultValue = null;
                    fallbackValue = null;

                    name = ChoType.GetMemberName(memberInfo);
                    isConfigmemberDefined = ConfigSection.HasConfigMemberDefined(name);

                    oldValue = ChoType.GetMemberValue(ConfigObject, memberInfo.Name);

                    object configFallbackValue = null;
                    object configDefaultValue = null;
                    bool isDefaultValueSpecified = ChoConfigurationMetaDataManager.TryConfigDefaultValue(this, name, memberInfoAttribute, out configDefaultValue);
                    ChoConfigurationMetaDataManager.TryConfigFallbackValue(this, name, memberInfoAttribute, out configFallbackValue);

                    if (configFallbackValue == null)
                    {
                        if (isDefaultValueSpecified)
                            defaultValue = origValue = newValue = configDefaultValue;
                    }
                    else
                        fallbackValue = origValue = newValue = configFallbackValue;

                    if (!isConfigmemberDefined)
                    {
                        if (!isDirty)
                            isDirty = memberInfoAttribute != null && memberInfoAttribute.Persistable ? true : false;
                    }
                    else
                        origValue = newValue = ConfigSection[name];

                    try
                    {
                        object newConvertedValue = ChoConvert.ConvertFrom(ConfigObject, newValue, ChoType.GetMemberType(memberInfo),
                            ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                        newValue = newConvertedValue;
                    }
                    catch { }

                    ChoConfigurableObject configObject = ConfigObject as ChoConfigurableObject;
                    if (configObject != null)
                    {
                        if (!configObject.IsMemeberValueEqualInternal(memberInfo, oldValue, newValue))
                        {
                            if (!configObject.RaiseBeforeConfigurationObjectMemberLoaded(memberInfo.Name, name, origValue, ref newValue))
                            {
                                try
                                {
                                    //ChoType.SetMemberValue(ConfigObject, memberInfo.Name, newValue != null ? ChoConvert.ConvertFrom(ConfigObject, newValue,
                                    //    ChoType.GetMemberType(memberInfo), ChoTypeConvertersCache.GetTypeConverters(memberInfo)) : null);
                                    SetConfigPropertyValue(newValue, memberInfo);
                                    if (!_firstTime)
                                        isModfied = true;

                                    configObject.RaiseAfterConfigurationObjectMemberLoaded(memberInfo.Name, name, newValue);
                                    ChoConfigurationObjectErrorManagerService.ResetObjectMemberError(ConfigObject, memberInfo.Name);
                                }
                                catch (Exception innerEx)
                                {
                                    if (!configObject.RaiseConfigurationObjectMemberLoadError(memberInfo.Name, name, origValue, innerEx))
                                    {
                                        if (Silent)
                                        {
                                            if (_firstTime)
                                            {
                                                AssignToFallbackOrDefaultValue(defaultValue, fallbackValue, memberInfo);
                                            }

                                            ChoConfigurationObjectErrorManagerService.SetObjectMemberError(ConfigObject, memberInfo.Name, String.Format(Resources.ConfigConstructMsg, ChoString.ToString(origValue), innerEx.Message));
                                        }
                                        else
                                            throw new ChoConfigurationConstructionException(String.Format(Resources.ConfigConstructExceptionMsg, ChoString.ToString(origValue), ConfigObject.GetType().FullName,
                                                memberInfo.Name), innerEx);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            //ChoType.SetMemberValue(ConfigObject, memberInfo.Name, newValue != null ? ChoConvert.ConvertFrom(ConfigObject, newValue,
                            //    ChoType.GetMemberType(memberInfo), ChoTypeConvertersCache.GetTypeConverters(memberInfo)) : null);
                            SetConfigPropertyValue(newValue, memberInfo);
                            if (!_firstTime)
                                isModfied = true;
                            ChoConfigurationObjectErrorManagerService.ResetObjectMemberError(ConfigObject, memberInfo.Name);
                        }
                        catch (Exception innerEx)
                        {
                            if (Silent)
                            {
                                if (_firstTime)
                                {
                                    AssignToFallbackOrDefaultValue(defaultValue, fallbackValue, memberInfo);
                                }
                                ChoConfigurationObjectErrorManagerService.SetObjectMemberError(ConfigObject, memberInfo.Name, String.Format(Resources.ConfigConstructMsg, ChoString.ToString(origValue), innerEx.Message));
                            }
                            else
                                throw new ChoConfigurationConstructionException(String.Format(Resources.ConfigConstructExceptionMsg, ChoString.ToString(origValue), ConfigObject.GetType().FullName,
                                    memberInfo.Name), innerEx);
                        }
                    }
                }
            }

            return isModfied;
        }

        private bool AssignToFallbackOrDefaultValues(ref bool isDirty)
        {
            bool isModfied = false;
            object defaultValue = null;
            object fallbackValue = null;

            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(ConfigObject.GetType());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //Set member values
                string name;
                ChoPropertyInfoAttribute memberInfoAttribute = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                        continue;

                    name = ChoType.GetMemberName(memberInfo);
                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                    //if (memberInfoAttribute == null) continue;

                    defaultValue = null;
                    fallbackValue = null;

                    bool isDefaultValueSpecified = ChoConfigurationMetaDataManager.TryConfigDefaultValue(this, name, memberInfoAttribute, out defaultValue);
                    ChoConfigurationMetaDataManager.TryConfigFallbackValue(this, name, memberInfoAttribute, out fallbackValue);

                    if (fallbackValue == null)
                    {
                        if (!isDefaultValueSpecified)
                            continue;
                    }

                    bool hasError = !ChoConfigurationObjectErrorManagerService.GetObjectMemberError(ConfigObject, memberInfo.Name).IsNullOrEmpty();
                    if (hasError)
                    {
                        isModfied = true;
                        if (_firstTime)
                        {
                            AssignToFallbackOrDefaultValue(defaultValue, fallbackValue, memberInfo);
                        }
                    }
                }
            }
            return isModfied;
        }

        private void AssignToFallbackOrDefaultValue(object defaultValue, object fallbackValue, MemberInfo memberInfo)
        {
            try
            {
                object convertedFallbackValue = ChoConvert.ConvertFrom(ConfigObject, fallbackValue, ChoType.GetMemberType(memberInfo),
                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                SetConfigPropertyValue(convertedFallbackValue, memberInfo);
            }
            catch
            {
                try
                {
                    object convertedDefaultValue = ChoConvert.ConvertFrom(ConfigObject, defaultValue, ChoType.GetMemberType(memberInfo),
                        ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                    SetConfigPropertyValue(convertedDefaultValue, memberInfo);
                }
                catch { }
            }
        }

        private void SetConfigPropertyValue(object newValue, MemberInfo memberInfo)
        {
            ChoType.SetMemberValue(ConfigObject, memberInfo, newValue);
            if (ConfigObject is ChoConfigurableObject)
            {
                ((ChoConfigurableObject)ConfigObject).OnPropertyChanged(memberInfo.Name);
            }
        }

		private void SetWatcher(bool refresh)
		{
            ChoConfigurationMetaDataManager.SetWatcher(this);

			if (ConfigSection != null)
			{
				if (!refresh)
				{
					//Hookup the configuration watch job
					if (_watchChange)
					{
                        ConfigSection.SetWatcher(new ChoConfigurationChangedEventHandler(OnConfigurationChanged));
					}
				}
				else
				{
					ConfigSection.StopWatching();
					if (ConfigObject is ChoInterceptableObject)
					{
						ChoInterceptableObject interceptableObject = ConfigObject as ChoInterceptableObject;
                        //((ChoInterceptableObject)ConfigObject).SetInitialized(false);
					}
				}
			}
		}

        private string GetConfigNameFromPath(string configSectionFullPath)
        {
            if (configSectionFullPath.IsNullOrWhiteSpace())
                return ConfigSectionName;

            int lastIndex = configSectionFullPath.LastIndexOf('/');
            if (lastIndex < 0)
                return configSectionFullPath;
            else
                return configSectionFullPath.Substring(lastIndex + 1);
        }

		private object SyncRoot
		{
			get
			{
				object syncRoot = null;
				if (ConfigSection != null)
					syncRoot = ConfigSection.SyncRoot;
				if (syncRoot == null)
					syncRoot = _syncRoot;

				return syncRoot;
			}
		}
		#endregion Instance Members (Private)

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);

            if (ConfigSection != null)
                ConfigSection.StopWatching();
        }
	}
}
