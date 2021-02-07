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
    public abstract class ChoNotifyPropertyChangedObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName.IsNullOrWhiteSpace()) return;

            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                foreach (string depends in ChoPropertyDependsOnCache.Instance.GetDependsOn(GetType(), propertyName))
                {
                    propertyChanged(this, new PropertyChangedEventArgs(depends));
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void RaisePropertyChanging(string propertyName)
        {
            if (propertyName.IsNullOrWhiteSpace()) return;

            PropertyChangingEventHandler propertyChanging = PropertyChanging;
            if (propertyChanging != null)
            {
                propertyChanging(this, new PropertyChangingEventArgs(propertyName));
                foreach (string depends in ChoPropertyDependsOnCache.Instance.GetDependsOn(GetType(), propertyName))
                {
                    propertyChanging(this, new PropertyChangingEventArgs(depends));
                }
            }
        }
    }
}
