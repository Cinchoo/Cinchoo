using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoDotNetAssemblyPlugIn : ChoPlugIn
    {
        public string TypeName;
        public string MethodName;
        public bool IsStaticMethod;
        public string Arguments;

        protected override void Validate()
        {
            base.Validate();
            ChoGuard.ArgumentNotNullOrEmpty(TypeName, "TypeName");
            ChoGuard.ArgumentNotNullOrEmpty(MethodName, "MethodName");
        }

        protected override object Execute(object value, out bool isHandled)
        {
            isHandled = false;

            Type type = ChoType.GetType(TypeName);
            if (type == null) return value;

            MethodInfo info = type.GetMethod(MethodName);
            if (info == null) return value;

            object obj = null;
            if (!IsStaticMethod)
                obj = ChoActivator.CreateInstance(type);

            string arguments = !Arguments.IsNullOrWhiteSpace() ? "{0} {1}".FormatString(value.ToNString(), ResolveText(Arguments)) : value.ToNString();

            return info.Invoke(obj, arguments.SplitNConvertToObjects(' '));
        }

        public override void InitializeBuilder(ChoPlugInBuilder builder)
        {
            base.InitializeBuilder(builder);

            ChoDotNetAssemblyPlugInBuilder b = builder as ChoDotNetAssemblyPlugInBuilder;
            b.TypeName = TypeName;
            b.MethodName = MethodName;
            b.IsStaticMethod = IsStaticMethod;
            b.Arguments = new Xml.Serialization.ChoCDATA(Arguments);
        }
    }
}
