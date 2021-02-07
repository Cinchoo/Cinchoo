using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoEventArgs<T> : EventArgs
    {
        public readonly T Value;

        public ChoEventArgs(T value)
        {
            Value = value;
        }
    }
}
