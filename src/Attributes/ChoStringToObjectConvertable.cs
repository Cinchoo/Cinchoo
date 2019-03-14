using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoStringToObjectConvertable : Attribute
    {
    }
}
