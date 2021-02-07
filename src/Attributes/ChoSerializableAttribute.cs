using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoSerializableAttribute : Attribute
    {
        public readonly ChoTypeNameSpecifier TypeNameSpecifier;
        public ChoSerializableAttribute(ChoTypeNameSpecifier typeNameSpecifier)
        {
            TypeNameSpecifier = typeNameSpecifier;
        }
    }

}
