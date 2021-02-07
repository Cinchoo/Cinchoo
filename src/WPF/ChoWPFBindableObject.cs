namespace Cinchoo.Core.WPF
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Dynamic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Diagnostics;

    #endregion NameSpaces

    public class ChoWPFBindableObject<T> : ChoAutoNotifyPropertyChangedObject<T>
    {
        public ChoWPFBindableObject()
            : base()
        {
        }

        public ChoWPFBindableObject(T instance)
            : base(instance)
        {
        }
    }
}
