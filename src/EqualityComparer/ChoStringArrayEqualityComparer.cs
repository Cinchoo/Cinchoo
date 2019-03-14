namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public class ChoStringArrayEqualityComparer : IChoEqualityComparer
    {
        #region IChoEqualityComparer Members

        public bool CanCompare(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
                return false;

            return typeof(string[]).IsAssignableFrom(sourceType);
        }

        public bool Equals(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value1, object value2)
        {
            if (value1 == null && value2 == null)
                return true;

            string[] stringArr1 = value1 as string[];
            string[] stringArr2 = value2 as string[];

            if (stringArr1 == null || stringArr2 == null)
                return false;
            if (stringArr1.Length != stringArr2.Length)
                return false;

            int index = 0;
            while (index < stringArr1.Length)
            {
                if (stringArr1[index] != stringArr2[index])
                    return false;
                index++;
            }

            return true;
        }

        #endregion
    }
}
