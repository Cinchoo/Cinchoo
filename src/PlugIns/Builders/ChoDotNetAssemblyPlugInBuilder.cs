using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    [ChoPlugIn("DotNetAssemblyPlugIn", typeof(ChoDotNetAssemblyPlugIn), typeof(ChoDotNetAssemblyPlugInBuilderProperty))]
    public class ChoDotNetAssemblyPlugInBuilder : ChoPlugInBuilder
    {
        private string _typeName;
        [XmlAttribute("typeName")]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (_typeName == value) return;
                ChoGuard.ArgumentNotNullOrEmpty(value, "TypeName");
                ChoDotNetAssemblyPlugInBuilderProperty.DiscoverType(value);
                _typeName = value;
                RaisePropertyChanged("TypeName");
            }
        }

        private string _methodName;
        [XmlAttribute("methodName")]
        public string MethodName
        {
            get { return _methodName; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "MethodName");
                if (_methodName == value) return;

                ChoDotNetAssemblyPlugInBuilderProperty.DiscoverMethod(_typeName, value);
                _methodName = value;
                RaisePropertyChanged("MethodName");
            }
        }

        private bool _isStaticMethod;
        [XmlAttribute("isStaticMethod")]
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

        private ChoCDATA _arguments;
        [XmlElement("arguments")]
        public ChoCDATA Arguments
        {
            get { return _arguments; }
            set
            {
                if (_arguments == value) return;
                _arguments = value;
                RaisePropertyChanged("Arguments");
            }
        }

        protected override void InitPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;

            base.InitPlugIn(plugIn);

            ChoDotNetAssemblyPlugIn o = plugIn as ChoDotNetAssemblyPlugIn;
            if (o == null) return;

            o.TypeName = TypeName;
            o.MethodName = MethodName;
            o.IsStaticMethod = IsStaticMethod;
            o.Arguments = Arguments.GetValue();
        }

        protected override void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            base.InitPlugInBuilderProperty(plugInBuilderProperty);

            ChoDotNetAssemblyPlugInBuilderProperty o = plugInBuilderProperty as ChoDotNetAssemblyPlugInBuilderProperty;
            if (o == null) return;

            o.TypeName = TypeName;
            o.MethodName = MethodName;
            o.IsStaticMethod = IsStaticMethod;
            o.Arguments = Arguments.GetValue();
        }

        protected override bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;
            base.ApplyPropertyValues(plugInBuilderProperty, propertyName);

            ChoDotNetAssemblyPlugInBuilderProperty o = plugInBuilderProperty as ChoDotNetAssemblyPlugInBuilderProperty;
            if (o == null) return false;

            if (propertyName == "TypeName")
                TypeName = o.TypeName;
            else if (propertyName == "MethodName")
                MethodName = o.MethodName;
            else if (propertyName == "IsStaticMethod")
                IsStaticMethod = o.IsStaticMethod;
            else if (propertyName == "Arguments")
                Arguments = new ChoCDATA(o.Arguments);
            else
                return false;

            return true;
        }

        protected override void Clone(ChoPlugInBuilder o)
        {
            base.Clone(o);
            ChoDotNetAssemblyPlugInBuilder p = o as ChoDotNetAssemblyPlugInBuilder;
            if (p == null) return;

            p.TypeName = TypeName;
            p.MethodName = MethodName;
            p.IsStaticMethod = IsStaticMethod;
            p.Arguments = Arguments;
        }
    }
}
