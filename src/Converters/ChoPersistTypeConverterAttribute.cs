namespace Cinchoo.Core.Converters
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.Generic;

    #endregion

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ChoPersistTypeConverterAttribute : ChoTypeConverterAttribute
    {
        #region Constructors

        public ChoPersistTypeConverterAttribute(Type converterType) : base(converterType)
        {
        }

        public ChoPersistTypeConverterAttribute(string typeConverterName) : base(typeConverterName)
        {
        }

        #endregion Constructors
    }
}
