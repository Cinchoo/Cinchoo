using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core;

namespace System
{
    public class ChoByteArrayEx
    {
        //public static byte[] operator +(byte[] srcArray, byte[] destArray)
        //{
        //    return Combine(srcArray, destArray);
        //}

        public static byte[] SubArray(byte[] srcArray, int startIndex)
        {
            ChoGuard.ArgumentNotNull(srcArray, "Array");
            if (startIndex < 0)
                throw new ArgumentException("StartIndex must be >= 0");
            if (startIndex > srcArray.Length)
                return null;

            byte[] inbuffer = new byte[srcArray.Length - startIndex];
            Buffer.BlockCopy(srcArray, startIndex, inbuffer, 0, inbuffer.Length);

            return inbuffer;
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
