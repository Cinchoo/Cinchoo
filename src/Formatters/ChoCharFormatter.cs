using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoCharFormatter : ICustomFormatter
    {
        public static readonly ChoCharFormatter Instance;

        static ChoCharFormatter()
        {
            Instance = new ChoCharFormatter();
        }

        // The Format method of the ICustomFormatter interface.
        // This must format the specified value according to the specified format settings.
        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (typeof(IEnumerable<char>).IsAssignableFrom(arg.GetType()))
            {
                return String.Concat(arg as IEnumerable<char>);
            }
            
            return arg.ToString();
        }
    }
}
