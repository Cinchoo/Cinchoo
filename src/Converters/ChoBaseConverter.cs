namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;

    #endregion NameSpaces

    public abstract class ChoBaseConverter : TypeConverter
    {
        #region Instance Data Members (Private)

        protected string Format;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoBaseConverter(string format)
        {
            Format = format;
        }

        #endregion Constructors
    }
}
