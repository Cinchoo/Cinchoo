using System;
using System.Collections.Generic;
using System.Text;

namespace eSquare.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoIsStringToObjectConvertable : Attribute
    {
    }
}
