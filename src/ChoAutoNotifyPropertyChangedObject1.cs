using Cinchoo.Core.Runtime.Remoting;
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
    public class ChoAutoNotifyPropertyChangedObject : ChoRealProxy, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public object WrappedObject { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        public bool IsDirty
        {
            get;
            set;
        }

        public ChoAutoNotifyPropertyChangedObject(object wrappedObject)
        {
            if (wrappedObject == null)
                throw new ArgumentNullException("wrappedObject");

            WrappedObject = wrappedObject;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return from f in WrappedObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   select f.Name;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Locate property by name
            var propertyInfo = WrappedObject.GetType().GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | (binder.IgnoreCase ? BindingFlags.IgnoreCase : 0));
            if (propertyInfo == null || !propertyInfo.CanRead)
            {
                result = null;
                return false;
            }

            result = propertyInfo.GetValue(WrappedObject, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            MemberInfo memberInfo = ChoTypeMembersCache.GetMemberInfo(WrappedObject.GetType(), binder.Name);
            if (memberInfo == null)
                return false;

            // Locate property by name
            var propertyInfo = memberInfo as PropertyInfo; // WrappedObject.GetType().GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | (binder.IgnoreCase ? BindingFlags.IgnoreCase : 0));
            if (propertyInfo != null && !propertyInfo.CanWrite)
                return false;

            RaisePropertyChanging(binder.Name);

            ChoType.SetMemberValue(WrappedObject, memberInfo, value);

            //object newValue = value;
            //// Check the types are compatible
            //Type propertyType = propertyInfo.PropertyType;
            //if (!propertyType.IsAssignableFrom(value.GetType()))
            //{
            //    newValue = Convert.ChangeType(value, propertyType);
            //}

            //propertyInfo.SetValue(WrappedObject, newValue, null);
            RaisePropertyChanged(binder.Name);
            return true;
        }

        //public override bool TrySetMember(SetMemberBinder binder, object value)
        //{
        //    RaisePropertyChanging(binder.Name);

        //    MemberInfo memberInfo = ChoTypeMembersCache.GetMemberInfo(GetType(), binder.Name);
        //    if (memberInfo == null)
        //        return false;

        //    ChoType.SetMemberValue(this, memberInfo, value);

        //    RaisePropertyChanged(binder.Name);
        //    return true;
        //}

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName.IsNullOrWhiteSpace()) return;

            IsDirty = true;
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanging(string propertyName)
        {
            if (propertyName.IsNullOrWhiteSpace()) return;

            PropertyChangingEventHandler propertyChanging = PropertyChanging;
            if (propertyChanging != null)
                propertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
