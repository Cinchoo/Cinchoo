using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ChoDoubleEx
    {
        public static double Floor(this double d, int decimals = 2)
        {
            return Math.Floor(d * Math.Pow(10, decimals)) / Math.Pow(10, decimals);
        }

        public static string RightJustifiedFilled(this double value, int length, int decimalPlaces = 2, char fillChar = '0', char decimalFileChar = '0')
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("Length must be >= 0.");
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException("Length must be positive.");

            if (value >= 0)
            {
                if (length < decimalPlaces)
                    throw new ArgumentOutOfRangeException("Length must be >= decimalPlaces");
            }
            else
            {
                if (length < decimalPlaces + 1)
                    throw new ArgumentOutOfRangeException("Length must be >= decimalPlaces + 1");
            }

            string format = null;
            if (value < 0)
                format = String.Format("{0}.{1}", new string(fillChar, length - decimalPlaces - 1), new string(decimalFileChar, decimalPlaces));
            else
                format = String.Format("{0}.{1}", new string(fillChar, length - decimalPlaces), new string(decimalFileChar, decimalPlaces));

            return value.Floor(decimalPlaces).ToString(format).Replace(".", "").Right(length);
        }

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
