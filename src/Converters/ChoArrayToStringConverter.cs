namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.ComponentModel;

    #endregion NameSpaces

    public class ChoArrayToStringConverter : TypeConverter
    {
        #region Instance Data Members (Private)

        private string _format;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoArrayToStringConverter()
        {
        }

        public ChoArrayToStringConverter(string format)
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
            if (value == null) return null;
            StringBuilder msg = new StringBuilder();
            if (value is string)
            {
                return ((string)value).SplitNTrim();
            }
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
            if (value == null)
                return null;

            StringBuilder msg = new StringBuilder();
            if (value.GetType().IsArray)
            {
                foreach (object item in (Array)value)
                {
                    if (msg.Length == 0)
                        msg.Append(ChoString.ToString(item, String.Empty, String.Empty));
                    else
                        msg.AppendFormat(", {0}", ChoString.ToString(item, String.Empty, String.Empty));
                }
                return msg.ToString();
            }
            return base.ConvertFrom(context, culture, value);
        }

        #endregion
    }
}
