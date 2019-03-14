namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public interface IChoEqualityComparer
    {
        bool CanCompare(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType);
        bool Equals(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value1, object value2);
    }
}
