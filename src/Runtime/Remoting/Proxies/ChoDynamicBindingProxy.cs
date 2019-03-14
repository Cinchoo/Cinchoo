namespace Cinchoo.Core.Runtime.Remoting.Proxies
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

    public class ChoDynamicBindingProxy<T> : DynamicObject, INotifyPropertyChanged
    {
        #region Instance Data Members (Private)

        private readonly T _instance;
        private readonly Type _type;

        #endregion Instance Data Members (Private)

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events
        
        public ChoDynamicBindingProxy() : this(Activator.CreateInstance<T>())
        {
        }

        public ChoDynamicBindingProxy(T instance)
        {
            ChoGuard.ArgumentNotNull(instance, "Instance");

            _instance = instance;
            _type = instance.GetType();

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

            try
            {
                object newConvertedValue = ChoConvert.ConvertFrom(_instance, value, ChoType.GetMemberType(memberInfo), ChoTypeConvertersCache.GetTypeConverters(memberInfo));
                ChoType.SetMemberValue(_instance, memberInfo, newConvertedValue);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return false;
            }

            if (!(_instance is INotifyPropertyChanged))
            {
                PropertyChangedEventHandler propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                    propertyChanged(this, new PropertyChangedEventArgs(binder.Name));
            }
            return true;
        }
    }
}
