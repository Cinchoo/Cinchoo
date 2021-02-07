using Cinchoo.Core.Runtime.Remoting;
using Cinchoo.Core.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoAutoNotifyPropertyChangedObject<T> : DynamicObject, INotifyPropertyChanged
    {
        #region Instance Data Members (Private)

        private readonly T _instance;
        private readonly Type _type;

        #endregion Instance Data Members (Private)

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        public ChoAutoNotifyPropertyChangedObject()
            : this(Activator.CreateInstance<T>())
        {
        }

        public ChoAutoNotifyPropertyChangedObject(T instance)
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
                //object newConvertedValue = ChoConvert.ConvertFrom(_instance, value, ChoType.GetMemberType(memberInfo),
                //    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                object newConvertedValue = ChoConvert.ConvertFrom(value, memberInfo, _instance);
                ChoType.SetMemberValue(_instance, memberInfo, newConvertedValue);
            }
            catch (ChoFatalApplicationException)
            {
                throw;
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

        public T Instance
        {
            get { return _instance; }
        }
    }
}
