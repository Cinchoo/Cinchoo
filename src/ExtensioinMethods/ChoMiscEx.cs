using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ChoMiscEx
    {
        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        #region Join Overloads

        public static string Join(this IEnumerable<object> array, string seperator)
        {
            if (array == null)
                return "";

            return string.Join(seperator, array.ToArray());
        }

        public static string Join(this object[] array, string seperator)
        {
            if (array == null)
                return "";

            return string.Join(seperator, array);
        }

        #endregion Join Overloads
    }
}
