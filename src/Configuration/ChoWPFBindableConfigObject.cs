namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Dynamic;
    using System.ComponentModel;
    using System.Reflection;
    using Cinchoo.Core.Configuration;
    using System.Diagnostics;

    #endregion NameSpaces

    public class ChoWPFBindableConfigObject<T> : DynamicObject, INotifyPropertyChanged
        where T : ChoConfigurableObject
    {
        #region Instance Data Members (Private)

        private readonly T _instance;
        private readonly Type _type;

        #endregion Instance Data Members (Private)

        #region Events

        public event EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> BeforeConfigurationObjectMemberLoaded
        {
            add
            {
                _instance.BeforeConfigurationObjectMemberLoaded += value;
            }
            remove
            {
                _instance.BeforeConfigurationObjectMemberLoaded -= value;
            }
        }

        public event EventHandler<ChoConfigurationObjectMemberEventArgs> AfterConfigurationObjectMemberLoaded
        {
            add
            {
                _instance.AfterConfigurationObjectMemberLoaded += value;
            }
            remove
            {
                _instance.AfterConfigurationObjectMemberLoaded -= value;
            }
        }

        public event EventHandler<ChoConfigurationObjectMemberErrorEventArgs> ConfigurationObjectMemberLoadError
        {
            add
            {
                _instance.ConfigurationObjectMemberLoadError += value;
            }
            remove
            {
                _instance.ConfigurationObjectMemberLoadError -= value;
            }
        }

        public event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectLoaded
        {
            add
            {
                _instance.AfterConfigurationObjectLoaded += value;
            }
            remove
            {
                _instance.AfterConfigurationObjectLoaded -= value;
            }
        }

        public event EventHandler<ChoPreviewConfigurationObjectEventArgs> BeforeConfigurationObjectPersisted
        {
            add
            {
                _instance.BeforeConfigurationObjectPersisted += value;
            }
            remove
            {
                _instance.BeforeConfigurationObjectPersisted -= value;
            }
        }

        public event EventHandler<ChoPreviewConfigurationObjectMemberEventArgs> ConfigurationObjectMemberPersist
        {
            add
            {
                _instance.BeforeConfigurationObjectMemberPersist += value;
            }
            remove
            {
                _instance.BeforeConfigurationObjectMemberPersist -= value;
            }
        }

        public event EventHandler<ChoConfigurationObjectEventArgs> AfterConfigurationObjectPersisted
        {
            add
            {
                _instance.AfterConfigurationObjectPersisted += value;
            }
            remove
            {
                _instance.AfterConfigurationObjectPersisted -= value;
            }
        }

        public event EventHandler<ChoConfigurationObjectErrorEventArgs> ConfigurationObjectLoadError
        {
            add
            {
                _instance.ConfigurationObjectLoadError += value;
            }
            remove
            {
                _instance.ConfigurationObjectLoadError -= value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //public event PropertyChangedEventHandler PropertyChanged
        //{
        //    add
        //    {
        //        _instance.PropertyChanged += value;
        //    }
        //    remove
        //    {
        //        _instance.PropertyChanged -= value;
        //    }
        //}

        #endregion Events

        public ChoWPFBindableConfigObject()
        {
            _instance = Activator.CreateInstance<T>();
            _type = typeof(T);

            if (_instance is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)_instance).PropertyChanged += ((sender, e) =>
                {
                    PropertyChangedEventHandler propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                        propertyChanged(this, e);
                });
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            MemberInfo memberInfo = ChoTypeMembersCache.GetMemberInfo(_type, binder.Name);

            if (memberInfo != null)
            {
                result = ChoType.GetMemberValue(_instance, memberInfo);
                return true;
            }
            else
                return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            MemberInfo memberInfo = ChoTypeMembersCache.GetMemberInfo(_type, binder.Name);
            if (memberInfo == null)
                return false;

            string propertyName = ChoType.GetMemberName(memberInfo);

            object newConvertedValue = null;
            try
            {
                newConvertedValue = ChoConvert.ConvertFrom(value, memberInfo, _instance);
     //           newConvertedValue = ChoConvert.ConvertFrom(_instance, value, ChoType.GetMemberType(memberInfo),
     //ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                ChoType.SetMemberValue(_instance, memberInfo, newConvertedValue);
            }
            catch (ChoConfigurationObjectPostInvokeException)
            {
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (_instance.RaiseConfigurationObjectMemberLoadError(binder.Name, propertyName, value, ex))
                {
                    _instance.OnPropertyChanged(binder.Name);
                    return true;
                }
                else
                    return false;
            }
            _instance.OnPropertyChanged(binder.Name);
            return true;
        }

        #region Instance Members (Public)

        public void Refresh()
        {
            _instance.Refresh();
        }

        public void Persist()
        {
            _instance.Persist();
        }

        public void Log(string msg)
        {
            _instance.Log(msg);
        }

        public void Log(bool condition, string msg)
        {
            _instance.Log(condition, msg);
        }

        #endregion Instance Members (Public Virtual)

        #region Object Overrides

        public override string ToString()
        {
            return _instance.ToString();
        }

        #endregion

        public T UnderlyingSource
        {
            get { return _instance; }
        }

        public void ResetToMemberDefaultValue(string memberName)
        {
            _instance.ResetToMemberDefaultValue(memberName);
        }

        public void ResetToMemberDefaultValue(MemberInfo memberInfo)
        {
            _instance.ResetToMemberDefaultValue(memberInfo);
        }
    }
}
