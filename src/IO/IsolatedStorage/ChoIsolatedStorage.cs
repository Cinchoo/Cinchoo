using System;
using System.Text;

namespace Cinchoo.Core.IO.IsolatedStorage
{
    public static class ChoIsolatedStorage
    {
        #region Shared Data Members (Private)

        private static char[] _base32Char = new char[] { 
															'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 
															'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5'
														};

        #endregion

        #region Shared Members (Public)

        internal static string ToBase32StringSuitableForDirName(byte[] buff)
        {
            StringBuilder builder = new StringBuilder();
            int length = buff.Length;
            int num7 = 0;
            do
            {
                byte num = (num7 < length) ? buff[num7++] : ((byte)0);
                byte num2 = (num7 < length) ? buff[num7++] : ((byte)0);
                byte index = (num7 < length) ? buff[num7++] : ((byte)0);
                byte num4 = (num7 < length) ? buff[num7++] : ((byte)0);
                byte num5 = (num7 < length) ? buff[num7++] : ((byte)0);
                builder.Append(_base32Char[num & 0x1f]);
                builder.Append(_base32Char[num2 & 0x1f]);
                builder.Append(_base32Char[index & 0x1f]);
                builder.Append(_base32Char[num4 & 0x1f]);
                builder.Append(_base32Char[num5 & 0x1f]);
                builder.Append(_base32Char[((num & 0xe0) >> 5) | ((num4 & 0x60) >> 2)]);
                builder.Append(_base32Char[((num2 & 0xe0) >> 5) | ((num5 & 0x60) >> 2)]);
                index = (byte)(index >> 5);
                if ((num4 & 0x80) != 0)
                {
                    index = (byte)(index | 8);
                }
                if ((num5 & 0x80) != 0)
                {
                    index = (byte)(index | 0x10);
                }
                builder.Append(_base32Char[index]);
            }
            while (num7 < length);
            return builder.ToString();
        }

        #endregion
    }
}
