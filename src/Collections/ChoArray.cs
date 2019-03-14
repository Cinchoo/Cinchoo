namespace Cinchoo.Core.Collections
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoArray
    {
        #region Shared Members (Public)

        public static object GetFirstNotNullableObject(ICollection srcArray)
        {
            if (srcArray == null
                || srcArray.Count == 0)
                return null;

            foreach (object arrayItem in srcArray)
            {
                if (arrayItem != null) return arrayItem;
            }
            return null;
        }

        public static ICollection Trim(ICollection srcArray)
        {
            return new ChoNotNullableArrayList(srcArray).ToArray();
        }

        public static Array SubArray(ICollection srcArray, int startIndex)
        {
            return SubArray(srcArray, startIndex, srcArray.Count - startIndex);
        }

        public static Array SubArray(ICollection srcArray, int startIndex, int length)
        {
            if (srcArray == null
                || srcArray.Count == 0
                || startIndex < 0
                || length <= 0
                ) return new Array[0];

            object[] retArray = new object[length];
            int index = 0;
            int counter = 0;

            foreach (object arrayItem in srcArray)
            {
                if (counter < startIndex)
                {
                    counter++;
                    continue;
                }
                retArray[index++] = arrayItem;

                if (index == length) break;
            }

            return retArray;
        }

        public static ICollection Split(ICollection srcArray, int chunkSize)
        {
            if (chunkSize == 0) throw new ApplicationException("ChunkSize should be greater than zero.");
            ArrayList subArrays = new ArrayList();

            if (srcArray == null || srcArray.Count <= chunkSize)
                subArrays.Add(srcArray);
            else
            {
                int index = 0;
                object[] subArray = new object[chunkSize];
                foreach (object arrayItem in srcArray)
                {
                    if (index == chunkSize)
                    {
                        subArrays.Add(ChoArray.ConvertTo(subArray, subArray[0].GetType()));
                        index = 0;
                        subArray = new object[chunkSize];
                    }
                    subArray[index++] = arrayItem;
                }

                if (subArray.Length > 0)
                {
                    subArray = ChoArray.Trim(subArray) as object[];
                    subArrays.Add(ChoArray.ConvertTo(subArray, subArray[0].GetType()));
                }
            }

            return subArrays.ToArray();
        }

        public static ICollection Convert(ICollection srcArray, TypeCode typeCode)
        {
            if (srcArray == null) return null;

            int index = 0;
            object[] destArray = new object[srcArray.Count];
            foreach (object srcItem in srcArray)
                destArray[index++] = System.Convert.ChangeType(srcItem, typeCode);

            return destArray;
        }

        #region ConvertTo Overloads

        public static T[] ConvertTo<T>(ICollection srcArray)
        {
            if (srcArray == null) return null;

            return new ArrayList(srcArray).ToArray(typeof(T)) as T[];
        }

        public static object[] ConvertTo(ICollection srcArray, Type type)
        {
            if (srcArray == null) return null;

            return new ArrayList(srcArray).ToArray(type) as object[];
        }

        #endregion ConvertTo Overloads

        #region Combine Overloads

        public static T[] Combine<T>(params IEnumerable<T>[] arrays)
        {
            if (arrays == null) return null;

            List<T> destArray = new List<T>();
            foreach (IEnumerable<T> array in arrays)
            {
                if (array == null) continue;
                destArray.AddRange(array);
            }

            return destArray.ToArray();
        }

        public static object[] Combine(Type type, params ICollection[] arrays)
        {
            ArrayList destArray = new ArrayList();

            foreach (ICollection array in arrays)
            {
                if (array == null) continue;
                foreach (object element in array)
                    destArray.Add(element);
            }

            if (type == null)
                return destArray.ToArray();
            else
                return ConvertTo(destArray, type);
        }

        public static object[] Combine(params ICollection[] arrays)
        {
            return Combine(null, arrays);
        }

        #endregion Combine Overloads

        #endregion
    }
}
