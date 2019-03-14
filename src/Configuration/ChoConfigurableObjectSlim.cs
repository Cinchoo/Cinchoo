//namespace Cinchoo.Core.Configuration
//{
//    #region NameSpaces

//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using Cinchoo.Core.Attributes;

//    #endregion NameSpaces

//    public class ChoConfigurableObjectSlim : IChoConfigurableObject
//    {
//        #region Instance Data Members (Private)

//        private bool _readOnly;

//        #endregion Instance Data Members (Private)

//        #region Events

//        public event EventHandler<ChoConfigurationObjectEventArgs> ConfigurationSourceChange;
//        internal event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectLoadedInternal;
//        public event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectLoaded
//        {
//            add
//            {
//                AfterConfigurationObjectLoadedInternal += value;
//                if (value != null)
//                    value(this, new ChoConfigurationObjectEventArgs(this));
//            }
//            remove
//            {
//                AfterConfigurationObjectLoadedInternal -= value;
//            }
//        }
//        public event EventHandler<ChoConfigurationObjectEventArgs> BeforeConfigurationObjectPersisted;
//        public event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectPersisted;

//        #endregion Events

//        #region Instance Members (Public Virtual)

//        public virtual void Refresh()
//        {
//            ChoConfigurationSectionAttribute configurationElement = ChoType.GetAttribute(GetType(), typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
//            if (configurationElement == null) return;

//            configurationElement.GetMe(GetType()).Refresh(true);
//        }

//        public virtual void Persist()
//        {
//            ChoConfigurationSectionAttribute configurationElementMap = ChoType.GetAttribute(GetType(), typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
//            if (configurationElementMap == null) return;

//            configurationElementMap.GetMe(GetType()).Persist();
//        }

//        #endregion Instance Members (Public Virtual)

//        #region Instance Members (Internal)

//        internal void SetReadOnly(bool flag)
//        {
//            _readOnly = flag;
//        }

//        internal bool IsReadOnly()
//        {
//            return _readOnly;
//        }

//        #endregion Instance Members (Internal)

//        #region Object Overrides

//        public override string ToString()
//        {
//            return ChoObject.ToString(this);
//        }

//        #endregion Object Overrides

//        #region Instance Members (Internal)

//        public void OnConfigurationSourceChange()
//        {
//            if (ConfigurationSourceChange != null)
//                ConfigurationSourceChange(this, new ChoConfigurationObjectEventArgs(this));
//        }

//        public void OnAfterConfigurationObjectLoaded()
//        {
//            if (AfterConfigurationObjectLoadedInternal != null)
//                AfterConfigurationObjectLoadedInternal(this, new ChoConfigurationObjectEventArgs(this));
//        }

//        public void OnBeforeConfigurationObjectPersisted()
//        {
//            if (BeforeConfigurationObjectPersisted != null)
//                BeforeConfigurationObjectPersisted(this, new ChoConfigurationObjectEventArgs(this));
//        }

//        public void OnAfterConfigurationObjectPersisted()
//        {
//            if (AfterConfigurationObjectPersisted != null)
//                AfterConfigurationObjectPersisted(this, new ChoConfigurationObjectEventArgs(this));
//        }

//        public void CloneEvents(IChoConfigurableObject configurableObject)
//        {
//            ChoGuard.ArgumentNotNull(configurableObject, "ConfigObject");

//            ChoEventHandler.Copy<ChoConfigurationObjectEventArgs>(((ChoConfigurableObjectSlim)configurableObject).ConfigurationSourceChange, ConfigurationSourceChange);
//            ChoEventHandler.Copy<ChoConfigurationObjectEventArgs>(((ChoConfigurableObjectSlim)configurableObject).AfterConfigurationObjectLoadedInternal,
//                delegate(Delegate delegateHandler)
//                {
//                    AfterConfigurationObjectLoadedInternal += delegateHandler as EventHandler<ChoConfigurationObjectEventArgs>;
//                });
//            ChoEventHandler.Copy<ChoConfigurationObjectEventArgs>(((ChoConfigurableObjectSlim)configurableObject).BeforeConfigurationObjectPersisted, BeforeConfigurationObjectPersisted);
//            ChoEventHandler.Copy<ChoConfigurationObjectEventArgs>(((ChoConfigurableObjectSlim)configurableObject).AfterConfigurationObjectPersisted, AfterConfigurationObjectPersisted);
//        }

//        public void ResetEvents()
//        {
//            ConfigurationSourceChange = null;
//            AfterConfigurationObjectLoadedInternal = null;
//            BeforeConfigurationObjectPersisted = null;
//            AfterConfigurationObjectPersisted = null;
//        }

//        #endregion Instance Members (Internal)
//    }
//}
