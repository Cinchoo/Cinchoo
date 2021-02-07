using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoDotNetAssemblyPlugInBuilderProperty : ChoPlugInBuilderProperty
    {
        private string _typeName;

        [Category("Miscellaneous")]
        [Description(".NET Type Fully qualified Name")]
        [DisplayName("TypeName")]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "TypeName");
                if (_typeName == value) return;
                DiscoverType(value);

                _typeName = value;
                RaisePropertyChanged("TypeName");
            }
        }

        private string _methodName;

        [Category("Miscellaneous")]
        [Description("Method Name")]
        [DisplayName("MethodName")]
        public string MethodName
        {
            get { return _methodName; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "MethodName");
                if (_methodName == value) return;
                DiscoverMethod(_typeName, value);
                _methodName = value;
                RaisePropertyChanged("MethodName");
            }
        }

        private bool _isStaticMethod;
        [Category("Miscellaneous")]
        [Description("Static Method")]
        [DisplayName("StaticMethod")]
        public bool IsStaticMethod
        {
            get { return _isStaticMethod; }
            set
            {
                if (_isStaticMethod == value) return;
                _isStaticMethod = value;
                RaisePropertyChanged("IsStaticMethod");
            }
        }

        private string _arguments;
        [Category("Miscellaneous")]
        [Description("Arguments to method")]
        [DisplayName("Arguments")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Arguments
        {
            get { return _arguments; }
            set
            {
                if (_arguments == value) return;

                _arguments = value;
                RaisePropertyChanged("Arguments");
            }
        }

        internal static Type DiscoverType(string value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "TypeName");
            Type type = ChoType.GetType(value);
            if (type == null)
                throw new ArgumentException("Can't find '{0}' type.".FormatString(value));
            return type;
        }

        internal static void DiscoverMethod(string typeName, string value)
        {
            Type type = DiscoverType(typeName);
            MethodInfo info = type.GetMethod(value);
            if (info == null)
                throw new ArgumentException("Can't find '{0}' method in '{1}' type.".FormatString(value, typeName));
        }
    }
}
