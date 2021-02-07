namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Proxies;
    using System.Runtime.Remoting.Messaging;

    using Cinchoo.Core.Reflection;
    using Cinchoo.Core;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Properties;
    using System.Runtime.Serialization;
    using System.Globalization;

    #endregion NameSpaces

    [Serializable]
    public abstract class ChoConfigurableObject : ChoInterceptableObject, INotifyPropertyChanged, ICloneable
    {
        #region Instance Data Members (Private)

        [ChoHiddenMember]
        [NonSerialized]
        private readonly ChoConfigurationSectionAttribute _configurationElementAttribute;

        #endregion Instance Data Members (Private)

        #region Constructors

        [ChoHiddenMember]
        public ChoConfigurableObject()
        {
            _configurationElementAttribute = ChoType.GetAttribute(GetType(), typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
            if (_configurationElementAttribute == null)
                throw new ChoFatalApplicationException("Missing configuration object attribute defined for '{0}' type.".FormatString(GetType().Name));

            //Discover and Hook the event handlers
            if (BeforeConfigurationObjectMemberLoaded == null)
                EventHandlerEx.LoadHandlers<ChoPreviewConfigurationObjectMemberEventArgs>(ref BeforeConfigurationObjectMemberLoaded, ChoType.GetMethods(GetType(), typeof(ChoBeforeConfigurationObjectMemberLoadedHandlerAttribute)), this);
            if (BeforeConfigurationObjectMemberSet == null)
                EventHandlerEx.LoadHandlers<ChoPreviewConfigurationObjectMemberEventArgs>(ref BeforeConfigurationObjectMemberSet, ChoType.GetMethods(GetType(), typeof(ChoBeforeConfigurationObjectMemberSetHandlerAttribute)), this);
            if (AfterConfigurationObjectMemberLoaded == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectMemberEventArgs>(ref AfterConfigurationObjectMemberLoaded, ChoType.GetMethods(GetType(), typeof(ChoAfterConfigurationObjectMemberLoadedHandlerAttribute)), this);
            if (AfterConfigurationObjectMemberSet == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectMemberEventArgs>(ref AfterConfigurationObjectMemberSet, ChoType.GetMethods(GetType(), typeof(ChoAfterConfigurationObjectMemberSetHandlerAttribute)), this);
            if (ConfigurationObjectMemberLoadError == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectMemberErrorEventArgs>(ref ConfigurationObjectMemberLoadError, ChoType.GetMethods(GetType(), typeof(ChoConfigurationObjectMemberLoadErrorHandlerAttribute)), this);
            if (ConfigurationObjectMemberSetError == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectMemberErrorEventArgs>(ref ConfigurationObjectMemberSetError, ChoType.GetMethods(GetType(), typeof(ChoConfigurationObjectMemberSetErrorHandlerAttribute)), this);

            if (BeforeConfigurationObjectPersisted == null)
                EventHandlerEx.LoadHandlers<ChoPreviewConfigurationObjectEventArgs>(ref BeforeConfigurationObjectPersisted, ChoType.GetMethods(GetType(), typeof(ChoBeforeConfigurationObjectPersistedHandlerAttribute)), this);
            if (AfterConfigurationObjectPersisted == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectEventArgs>(ref AfterConfigurationObjectPersisted, ChoType.GetMethods(GetType(), typeof(ChoAfterConfigurationObjectPersistedHandlerAttribute)), this);

            if (BeforeConfigurationObjectMemberPersist == null)
                EventHandlerEx.LoadHandlers<ChoPreviewConfigurationObjectMemberEventArgs>(ref BeforeConfigurationObjectMemberPersist, ChoType.GetMethods(GetType(), typeof(ChoBeforeConfigurationObjectMemberPersistHandlerAttribute)), this);
            if (AfterConfigurationObjectMemberPersist == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectMemberEventArgs>(ref AfterConfigurationObjectMemberPersist, ChoType.GetMethods(GetType(), typeof(ChoAfterConfigurationObjectMemberPersistHandlerAttribute)), this);

            if (BeforeConfigurationObjectLoadedInternal == null)
                EventHandlerEx.LoadHandlers<ChoPreviewConfigurationObjectEventArgs>(ref BeforeConfigurationObjectLoadedInternal, ChoType.GetMethods(GetType(), typeof(ChoBeforeConfigurationObjectLoadedHandlerAttribute)), this);
            if (AfterConfigurationObjectLoadedInternal == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectEventArgs>(ref AfterConfigurationObjectLoadedInternal, ChoType.GetMethods(GetType(), typeof(ChoAfterConfigurationObjectLoadedHandlerAttribute)), this);
            if (ConfigurationObjectLoadError == null)
                EventHandlerEx.LoadHandlers<ChoConfigurationObjectErrorEventArgs>(ref ConfigurationObjectLoadError, ChoType.GetMethods(GetType(), typeof(ChoConfigurationObjectLoadErrorHandlerAttribute)), this);

            //Type configObjType = GetType();
            //ChoConfigurationSectionAttribute configurationElement = ChoType.GetAttribute(configObjType, typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
            //if (configurationElement == null || configurationElement.GetMe(configObjType) == null)
            //    throw new ChoConfigurationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1001, configObjType.Name));

            //configurationElement.GetMe(configObjType).Construct(this);
        }

        #endregion Constructors

        #region Events

        [ChoHiddenMember]
        public event EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> BeforeConfigurationObjectMemberLoaded;

        [ChoHiddenMember]
        public event EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> BeforeConfigurationObjectMemberSet;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectMemberEventArgs> AfterConfigurationObjectMemberLoaded;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectMemberEventArgs> AfterConfigurationObjectMemberSet;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectMemberErrorEventArgs> ConfigurationObjectMemberLoadError;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectMemberErrorEventArgs> ConfigurationObjectMemberSetError;

        [ChoHiddenMember]
        internal event EventHandler<ChoPreviewConfigurationObjectEventArgs> BeforeConfigurationObjectLoadedInternal;
        [ChoHiddenMember]
        public event EventHandler<ChoPreviewConfigurationObjectEventArgs> BeforeConfigurationObjectLoaded
        {
            add
            {
                BeforeConfigurationObjectLoadedInternal += value;
                //if (value != null && Initialized)
                //    value(this, new ChoPreviewConfigurationObjectEventArgs());
            }
            remove
            {
                BeforeConfigurationObjectLoadedInternal -= value;
            }
        }

        [ChoHiddenMember]
        internal event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectLoadedInternal;
        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectLoaded
        {
            add
            {
                AfterConfigurationObjectLoadedInternal += value;
                if (value != null && Initialized)
                    value(this, new ChoConfigurationObjectEventArgs());
            }
            remove
            {
                AfterConfigurationObjectLoadedInternal -= value;
            }
        }

        [ChoHiddenMember]
        public event EventHandler<ChoPreviewConfigurationObjectEventArgs> BeforeConfigurationObjectPersisted;

        [ChoHiddenMember]
        public event EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> BeforeConfigurationObjectMemberPersist;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectMemberEventArgs> AfterConfigurationObjectMemberPersist;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectPersisted;

        [ChoHiddenMember]
        public event EventHandler<ChoConfigurationObjectErrorEventArgs> ConfigurationObjectLoadError;

        #endregion Events

        #region Instance Members (Public Virtual)

        [ChoHiddenMember]
        public virtual void Refresh()
        {
            if (_configurationElementAttribute == null)
                return;

            _configurationElementAttribute.GetMe(GetType()).Refresh(true);
        }

        [ChoHiddenMember]
        public virtual void Persist()
        {
            if (_configurationElementAttribute == null)
                return;

            _configurationElementAttribute.GetMe(GetType()).Persist(true, null);
        }

        #endregion Instance Members (Public Virtual)

        #region ChoIntercerptableObject Overrides

        [ChoHiddenMember]
        internal bool IsMemeberValueEqualInternal(MemberInfo memberInfo, object oldValue, object newValue)
        {
            //bool isEqual = Object.Equals(oldValue, newValue);
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(oldValue, newValue))
                return true;
            if (object.ReferenceEquals(oldValue, null))
                return false;
            if (object.ReferenceEquals(newValue, null))
                return false;

            bool isEquatableObj = oldValue.GetType().IsImplGenericTypeDefinition(typeof(IEquatable<>));
            if (isEquatableObj)
                return oldValue.Equals(newValue);

            isEquatableObj = newValue.GetType().IsImplGenericTypeDefinition(typeof(IEquatable<>));
            if (isEquatableObj)
                return newValue.Equals(oldValue);

            ChoEqualityComparerAttribute attribute = memberInfo.GetCustomAttribute(typeof(ChoEqualityComparerAttribute)) as ChoEqualityComparerAttribute;
            if (attribute != null)
                return attribute.IsEqualCompare(oldValue, newValue);
            else
                return IsMemberValueEqual(memberInfo.Name, oldValue, newValue);
        }

        [ChoHiddenMember]
        protected virtual bool IsMemberValueEqual(string memberName, object oldValue, object newValue)
        {
            return false;
        }

        [ChoHiddenMember]
        protected override bool PreInvoke(ChoMemberInfo memberInfo)
        {
            object oldValue = ChoType.GetMemberValue(this, memberInfo.Info);
            object newValue = memberInfo.Value;

            bool isEqual = IsMemeberValueEqualInternal(memberInfo.Info, oldValue, newValue);

            if (!isEqual)
            {
                if (memberInfo.DirtyOperation)
                {
                    string propertyName = ChoType.GetMemberName(memberInfo.Info);

                    try
                    {
                        bool cancel = RaiseBeforeConfigurationObjectMemberSet(memberInfo.Name, propertyName, oldValue, ref newValue);
                        //if (!cancel)
                        //{
                        //    //memberInfo.Value = newValue;
                        //    ChoType.SetMemberValue(this, memberInfo.Info, newValue);
                        //    this.OnPropertyChanged(memberInfo.Name);
                        //}
                        return !cancel;
                    }
                    catch (Exception ex)
                    {
                        if (!RaiseConfigurationObjectMemberSetError(memberInfo.Name, propertyName, newValue, ex))
                            throw new ChoConfigurationObjectPostInvokeException("Error while setting configuration member value.", memberInfo.Exception);
                    }
                }
                return true;
            }
            else
                return !isEqual;
        }

        [ChoHiddenMember]
        protected override void PostInvoke(ChoMemberInfo memberInfo)
        {
            object newValue = memberInfo.Value;

            if (memberInfo.DirtyOperation)
            {
                string propertyName = ChoType.GetMemberName(memberInfo.Info);

                if (memberInfo.Exception == null)
                {
                    RaiseAfterConfigurationObjectMemberSet(memberInfo.Name, propertyName, newValue);
                    OnPropertyChanged(memberInfo.Name);
                }
                else
                {
                    if (!RaiseConfigurationObjectMemberSetError(memberInfo.Name, propertyName, newValue, memberInfo.Exception))
                        memberInfo.Exception = new ChoConfigurationObjectPostInvokeException("Error while setting '{0}' configuration member value.".FormatString(memberInfo.Name), memberInfo.Exception);
                    else
                        memberInfo.Exception = null;
                }
            }
            if (memberInfo.DirtyOperation /*&& Dirty */&& !IsReadOnly() && Initialized)
            {
                ChoPropertyInfoAttribute memberInfoAttribute = ChoType.GetMemberAttribute(memberInfo.Info, typeof(ChoPropertyInfoAttribute)) as ChoPropertyInfoAttribute;
                if (memberInfoAttribute == null || memberInfoAttribute.Persistable)
                    Persist();

                SetDirty(false);
            }
        }

        #endregion ChoIntercerptableObject Overrides

        #region Instance Members (Public)

        internal void InvokeOverrideMetaDataInfo(ChoBaseConfigurationMetaDataInfo metaDataInfo)
        {
            OverrideMetaDataInfo(metaDataInfo);
        }

        protected virtual void OverrideMetaDataInfo(ChoBaseConfigurationMetaDataInfo metaDataInfo)
        {
        }

        public void ResetToMemberDefaultValue(string memberName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

            MemberInfo memberInfo = ChoTypeMembersCache.GetMemberInfo(GetType(), memberName);
            ResetToMemberDefaultValue(memberInfo);
        }

        public void ResetToMemberDefaultValue(MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNullOrEmpty(memberInfo, "MemberInfo");
            ChoType.SetMemberValue(this, memberInfo, memberInfo.GetConvertedDefaultValue());
        }

        [ChoHiddenMember]
        public void Log(string msg)
        {
            if (_configurationElementAttribute == null)
                return;
            _configurationElementAttribute.GetMe(GetType()).Log(msg);
        }

        [ChoHiddenMember]
        public void Log(bool condition, string msg)
        {
            if (_configurationElementAttribute == null)
                return;
            _configurationElementAttribute.GetMe(GetType()).Log(condition, msg);
        }

        #endregion Instance Members (Public)

        #region Object Overrides

        [ChoHiddenMember]
        public override string ToString()
        {
            return ChoObject.ToString(this);
        }

        #endregion Object Overrides

        #region Instance Members (Internal)

        protected virtual bool OnBeforeConfigurationObjectLoaded()
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeConfigurationObjectLoaded()
        {
            //Wire INotifyPropertyChanged members for any changes
            UnwireNotifyPropertyChangedMembersForChanges(GetType());

            bool cancel = OnBeforeConfigurationObjectLoaded();
            if (cancel) return cancel;

            EventHandler<ChoPreviewConfigurationObjectEventArgs> beforeConfigurationObjectLoadedInternal = BeforeConfigurationObjectLoadedInternal;
            if (beforeConfigurationObjectLoadedInternal != null)
            {
                ChoPreviewConfigurationObjectEventArgs previewConfigurationObjectEventArgs = new ChoPreviewConfigurationObjectEventArgs();

                beforeConfigurationObjectLoadedInternal(this, previewConfigurationObjectEventArgs);
                return previewConfigurationObjectEventArgs.Cancel;
            }

            return false;
        }

        private void UnwireNotifyPropertyChangedMembersForChanges(Type type)
        {
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(type);
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //Set member values
                INotifyPropertyChanged memberValue = null;
                ChoPropertyInfoAttribute memberInfoAttribute = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                        continue;

                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));

                    memberValue = ChoType.GetMemberValue(this, memberInfo.Name) as INotifyPropertyChanged;
                    if (memberValue == null)
                        continue;

                    memberValue.PropertyChanged -= MemberValuePropertyChanged;
                    UnwireNotifyPropertyChangedMembersForChanges(memberValue.GetType());
                }
            }
        }

        protected virtual void OnAfterConfigurationObjectLoaded()
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterConfigurationObjectLoaded()
        {
            Initialized = true;

            //Wire INotifyPropertyChanged members for any changes
            WireNotifyPropertyChangedMembersForChanges(GetType());

            OnAfterConfigurationObjectLoaded();

            EventHandler<ChoConfigurationObjectEventArgs> afterConfigurationObjectLoadedInternal = AfterConfigurationObjectLoadedInternal;
            if (afterConfigurationObjectLoadedInternal != null)
            {
                ChoConfigurationObjectEventArgs configurationObjectEventArgs = new ChoConfigurationObjectEventArgs();
                afterConfigurationObjectLoadedInternal(this, configurationObjectEventArgs);
            }
        }

        private void WireNotifyPropertyChangedMembersForChanges(Type type)
        {
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(type);
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //Set member values
                INotifyPropertyChanged memberValue = null;
                ChoPropertyInfoAttribute memberInfoAttribute = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                        continue;

                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));

                    memberValue = ChoType.GetMemberValue(this, memberInfo.Name) as INotifyPropertyChanged;
                    if (memberValue == null)
                        continue;

                    memberValue.PropertyChanged += MemberValuePropertyChanged;
                    WireNotifyPropertyChangedMembersForChanges(memberValue.GetType());
                }
            }
        }

        private void MemberValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Initialized)
                Persist();
        }

        protected virtual bool OnConfigurationObjectLoadError(Exception ex)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseConfigurationObjectLoadError(Exception ex, ref bool isDirty)
        {
            //Wire INotifyPropertyChanged members for any changes
            WireNotifyPropertyChangedMembersForChanges(GetType());
            
            bool handled = OnConfigurationObjectLoadError(ex);
            if (handled) return handled;

            EventHandler<ChoConfigurationObjectErrorEventArgs> configurationObjectLoadError = ConfigurationObjectLoadError;
            if (configurationObjectLoadError != null)
            {
                ChoConfigurationObjectErrorEventArgs configurationObjectErrorEventArgs = new ChoConfigurationObjectErrorEventArgs(ex);
                configurationObjectLoadError(this, configurationObjectErrorEventArgs);
                if (!isDirty)
                    isDirty = configurationObjectErrorEventArgs.Dirty;
                return configurationObjectErrorEventArgs.Handled;
            }

            return false;
        }

        protected virtual bool OnBeforeConfigurationObjectMemberLoaded(string memberName, string propertyName, ref object value)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeConfigurationObjectMemberLoaded(string memberName, string propertyName, object originalValue, ref object value)
        {
            bool cancel = OnBeforeConfigurationObjectMemberLoaded(memberName, propertyName, ref value);
            if (cancel) return cancel;

            EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> beforeConfigurationObjectMemberLoaded = BeforeConfigurationObjectMemberLoaded;
            if (beforeConfigurationObjectMemberLoaded != null)
            {
                ChoPreviewConfigurationObjectMemberEventArgs previewConfigurationObjectMemberEventArgs = new ChoPreviewConfigurationObjectMemberEventArgs(memberName, propertyName, value, originalValue);
                beforeConfigurationObjectMemberLoaded(this, previewConfigurationObjectMemberEventArgs);
                value = previewConfigurationObjectMemberEventArgs.Value;
                return previewConfigurationObjectMemberEventArgs.Cancel;
            }

            return false;
        }

        protected virtual bool OnBeforeConfigurationObjectMemberSet(string memberName, string propertyName, ref object value)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeConfigurationObjectMemberSet(string memberName, string propertyName, object originalValue, ref object value)
        {
            bool cancel = OnBeforeConfigurationObjectMemberSet(memberName, propertyName, ref value);
            if (cancel) return cancel;

            EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> beforeConfigurationObjectMemberSet = BeforeConfigurationObjectMemberSet;
            if (beforeConfigurationObjectMemberSet != null)
            {
                ChoPreviewConfigurationObjectMemberEventArgs previewConfigurationObjectMemberEventArgs = new ChoPreviewConfigurationObjectMemberEventArgs(memberName, propertyName, value, originalValue);
                beforeConfigurationObjectMemberSet(this, previewConfigurationObjectMemberEventArgs);
                value = previewConfigurationObjectMemberEventArgs.Value;
                return previewConfigurationObjectMemberEventArgs.Cancel;
            }

            return false;
        }

        protected virtual void OnAfterConfigurationObjectMemberLoaded(string memberName, string propertyName, object value)
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterConfigurationObjectMemberLoaded(string memberName, string propertyName, object value)
        {
            OnAfterConfigurationObjectMemberLoaded(memberName, propertyName, value);

            EventHandler<ChoConfigurationObjectMemberEventArgs> afterConfigurationObjectMemberLoaded = AfterConfigurationObjectMemberLoaded;
            if (afterConfigurationObjectMemberLoaded != null)
            {
                ChoConfigurationObjectMemberEventArgs configurationObjectMemberEventArgs = new ChoConfigurationObjectMemberEventArgs(memberName, propertyName, value);
                afterConfigurationObjectMemberLoaded(this, configurationObjectMemberEventArgs);
            }

            //OnPropertyChanged(memberName);
        }

        protected virtual void OnAfterConfigurationObjectMemberSet(string memberName, string propertyName, object value)
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterConfigurationObjectMemberSet(string memberName, string propertyName, object value)
        {
            OnAfterConfigurationObjectMemberSet(memberName, propertyName, value);

            EventHandler<ChoConfigurationObjectMemberEventArgs> afterConfigurationObjectMemberSet = AfterConfigurationObjectMemberSet;
            if (afterConfigurationObjectMemberSet != null)
            {
                ChoConfigurationObjectMemberEventArgs configurationObjectMemberEventArgs = new ChoConfigurationObjectMemberEventArgs(memberName, propertyName, value);
                afterConfigurationObjectMemberSet(this, configurationObjectMemberEventArgs);
            }

            //OnPropertyChanged(memberName);
        }

        protected virtual bool OnConfigurationObjectMemberLoadError(string memberName, string propertyName, object value, Exception ex)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseConfigurationObjectMemberLoadError(string memberName, string propertyName, object value, Exception ex)
        {
            bool handled = OnConfigurationObjectMemberLoadError(memberName, propertyName, value, ex);
            if (handled) return handled;

            EventHandler<ChoConfigurationObjectMemberErrorEventArgs> configurationObjectMemberLoadError = ConfigurationObjectMemberLoadError;
            if (configurationObjectMemberLoadError != null)
            {
                ChoConfigurationObjectMemberErrorEventArgs configurationObjectMemberErrorEventArgs = new ChoConfigurationObjectMemberErrorEventArgs(memberName, propertyName, value, ex);
                configurationObjectMemberLoadError(this, configurationObjectMemberErrorEventArgs);
                return configurationObjectMemberErrorEventArgs.Handled;
            }

            return false;
        }

        protected virtual bool OnConfigurationObjectMemberSetError(string memberName, string propertyName, object value, Exception ex)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseConfigurationObjectMemberSetError(string memberName, string propertyName, object value, Exception ex)
        {
            bool handled = OnConfigurationObjectMemberSetError(memberName, propertyName, value, ex);
            if (handled) return handled;

            EventHandler<ChoConfigurationObjectMemberErrorEventArgs> configurationObjectMemberSetError = ConfigurationObjectMemberSetError;
            if (configurationObjectMemberSetError != null)
            {
                ChoConfigurationObjectMemberErrorEventArgs configurationObjectMemberErrorEventArgs = new ChoConfigurationObjectMemberErrorEventArgs(memberName, propertyName, value, ex);
                configurationObjectMemberSetError(this, configurationObjectMemberErrorEventArgs);
                return configurationObjectMemberErrorEventArgs.Handled;
            }

            return false;
        }

        protected virtual bool OnBeforeConfigurationObjectPersisted()
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeConfigurationObjectPersisted()
        {
            bool cancel = OnBeforeConfigurationObjectPersisted();
            if (cancel) return cancel;

            EventHandler<ChoPreviewConfigurationObjectEventArgs> beforeConfigurationObjectPersisted = BeforeConfigurationObjectPersisted;
            if (beforeConfigurationObjectPersisted != null)
            {
                ChoPreviewConfigurationObjectEventArgs previewConfigurationObjectEventArgs = new ChoPreviewConfigurationObjectEventArgs();

                beforeConfigurationObjectPersisted(this, previewConfigurationObjectEventArgs);
                return previewConfigurationObjectEventArgs.Cancel;
            }

            return false;
        }

        protected virtual bool OnBeforeConfigurationObjectMemberPersist(string memberName, string propertyName, ref object value)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeConfigurationObjectMemberPersist(string memberName, string propertyName, ref object value)
        {
            bool cancel = OnBeforeConfigurationObjectMemberPersist(memberName, propertyName, ref value);
            if (cancel) return cancel;

            EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> beforeConfigurationObjectMemberPersist = BeforeConfigurationObjectMemberPersist;
            if (beforeConfigurationObjectMemberPersist != null)
            {
                ChoPreviewConfigurationObjectMemberEventArgs previewConfigurationObjectMemberEventArgs = new ChoPreviewConfigurationObjectMemberEventArgs(memberName, propertyName, value, null);
                beforeConfigurationObjectMemberPersist(this, previewConfigurationObjectMemberEventArgs);
                value = previewConfigurationObjectMemberEventArgs.Value;
                return previewConfigurationObjectMemberEventArgs.Cancel;
            }

            return false;
        }

        protected virtual void OnAfterConfigurationObjectMemberPersist(string memberName, string propertyName, object value)
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterConfigurationObjectMemberPersist(string memberName, string propertyName, object value)
        {
            OnAfterConfigurationObjectMemberPersist(memberName, propertyName, value);

            EventHandler<ChoConfigurationObjectMemberEventArgs> afterConfigurationObjectMemberPersist = AfterConfigurationObjectMemberPersist;
            if (afterConfigurationObjectMemberPersist != null)
            {
                ChoConfigurationObjectMemberEventArgs configurationObjectMemberEventArgs = new ChoConfigurationObjectMemberEventArgs(memberName, propertyName, value);

                afterConfigurationObjectMemberPersist(this, configurationObjectMemberEventArgs);
            }
        }

        [ChoHiddenMember]
        internal void CallAfterConfigurationMemberPersist()
        {
            //if (!(ConfigObject is ChoConfigurableObject))
            //    return;
            if (AfterConfigurationObjectMemberPersist != null)
            {
                MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(GetType());
                if (memberInfos == null || memberInfos.Length == 0)
                    return;

                ChoPropertyInfoAttribute memberInfoAttribute = null;
                string name = null;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                        continue;

                    object memberValue = ChoType.GetMemberValue(this, memberInfo.Name);
                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                    name = ChoType.GetMemberName(memberInfo, memberInfoAttribute);

                    RaiseAfterConfigurationObjectMemberPersist(memberInfo.Name, name, memberValue);
                }
            }
        }

        protected virtual void OnAfterConfigurationObjectPersisted()
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterConfigurationObjectPersisted()
        {
            OnAfterConfigurationObjectPersisted();

            EventHandler<ChoConfigurationObjectEventArgs> afterConfigurationObjectPersisted = AfterConfigurationObjectPersisted;
            if (afterConfigurationObjectPersisted != null)
            {
                ChoConfigurationObjectEventArgs configurationObjectEventArgs = new ChoConfigurationObjectEventArgs();
                afterConfigurationObjectPersisted(this, configurationObjectEventArgs);
            }
        }

        internal void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChangedInternal;
            if (handler != null)
            {
                foreach (PropertyChangedEventHandler propertyChangedEventHandler in handler.GetInvocationList())
                {
                    try
                    {
                        propertyChangedEventHandler((MarshalByRefObject)this, new PropertyChangedEventArgs(name));
                    }
                    catch (ChoFatalApplicationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex);
                    }

                    foreach (string depends in ChoPropertyDependsOnCache.Instance.GetDependsOn(GetType(), name))
                    {
                        try
                        {
                            propertyChangedEventHandler((MarshalByRefObject)this, new PropertyChangedEventArgs(depends));
                        }
                        catch (ChoFatalApplicationException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            ChoTrace.Error(ex);
                        }
                    }
                }
            }
        }

        #endregion Instance Members (Internal)

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler PropertyChangedInternal;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedInternal += value;
            }
            remove
            {
                PropertyChangedInternal -= value;
            }
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            return this;
        }

        #endregion

        #region AsDictionary Overrides

        public IEnumerable<KeyValuePair<string, object>> AsDictionary()
        {
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(GetType());
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
                    name = ChoType.GetMemberName(memberInfo);

                    yield return new KeyValuePair<string, object>(name, ChoType.GetMemberValue(GetType(), memberInfo));
                }
            }
        }

        #endregion AsDictionary Overrides

        #region Helper Methods

        public string ToXml()
        {
            return ChoObjectEx.ToXml(this);
        }

        public void LoadXml(string xml)
        {
            if (xml.IsNullOrWhiteSpace()) return;
            xml.ToObjectFromXml(GetType());
        }

        #endregion
    }
}
