namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;

    #endregion NameSpaces

    public class ChoDateTimeConverter : TypeConverter
    {
        #region Instance Data Members (Private)

        protected string Format;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoDateTimeConverter(string format)
        {
            if (format == null || format.Length == 0)
                throw new NullReferenceException("Missing Format specification.");

            Format = format;
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
            if (sourceType == typeof(string) || sourceType == typeof(int))
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
            if (value.GetType() == typeof(string))
                return DateTime.ParseExact((string)value, Format, null);
            else if (value.GetType() == typeof(int))
                return DateTime.ParseExact(value.ToString(), Format, null);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateTime && !Format.IsNullOrWhiteSpace())
            {
                return ((DateTime)value).ToString(Format);
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
