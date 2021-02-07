using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoXmlSerializerDiscoverableAttribute : Attribute
    {
        public bool Ignore
        {
            get;
            private set;
        }

        public ChoXmlSerializerDiscoverableAttribute()
        {
            Ignore = false;
        }

        public ChoXmlSerializerDiscoverableAttribute(bool ignore)
        {
            Ignore = ignore;
        }
    }
}
