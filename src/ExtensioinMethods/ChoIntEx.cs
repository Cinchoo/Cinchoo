using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ChoIntEx
    {
        public static string RightJustifiedFilled(this int value, int length, char fillChar = '0')
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("Length must be positive.");

            if (value < 0)
            {
                if (length - 1 <= 0)
                    throw new ArgumentOutOfRangeException("Length must be > 1");
            }

            string format = null;
            if (value < 0)
                format = String.Format("{0}", new string(fillChar, length - 1));
            else
                format = String.Format("{0}", new string(fillChar, length));

            return value.ToString(format).Right(length);
        }
    }
}
