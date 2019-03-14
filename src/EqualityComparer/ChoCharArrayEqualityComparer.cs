namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public class ChoCharArrayEqualityComparer : IChoEqualityComparer
    {
        #region IChoEqualityComparer Members

        public bool CanCompare(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
                return false;

            return typeof(char[]).IsAssignableFrom(sourceType);
        }

        public bool Equals(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value1, object value2)
        {
            if (value1 == null && value2 == null)
                return true;

            char[] charArr1 = value1 as char[];
            char[] charArr2 = value2 as char[];

            if (charArr1 == null || charArr2 == null)
                return false;
            if (charArr1.Length != charArr2.Length)
                return false;

            int index = 0;
            while (index < charArr1.Length)
            {
                if (charArr1[index] != charArr2[index])
                    return false;
                index++;
            }

            return true;
        }

        #endregion
    }
}
