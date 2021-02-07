using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoPlugInAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public Type PlugInType
        {
            get;
            private set;
        }

        public Type PlugInBuilderPropertyType
        {
            get;
            private set;
        }

        public ChoPlugInAttribute(string name, Type plugInType, Type plugInBuilderPropertyType)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
            Name = name;
            PlugInType = plugInType;
            PlugInBuilderPropertyType = plugInBuilderPropertyType;
        }
    }
}
