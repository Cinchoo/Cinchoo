namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;

    #endregion NameSpaces

    public class ChoToUpperCaseConverter : TypeConverter
    {
        #region Instance Data Members (Private)

        private string _format;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoToUpperCaseConverter(string format)
        {
            _format = format;
        }

        #endregion Constructors

        #region TypeConverter Overrides

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        // If the type of the value to convert is string, parses the string 
        // and returns the integer to set the value of the property to. 
        // This example first extends the integer array that supplies the 
        // standard values collection if the user-entered value is not 
        // already in the array.
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value != null && value.GetType() == typeof(string))
                return ((String)value).ToUpper();

            return base.ConvertFrom(context, culture, value);
        }

        #endregion
    }
}
