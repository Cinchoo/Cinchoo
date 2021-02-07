namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using Cinchoo.Core.Xml.Serialization;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    public class ChoCDATAToStringConverter : TypeConverter
    {
        //private static readonly Regex _regex = new Regex(@"\<\!\[CDATA\[(?<text>[^\]]*)\]\]\>", RegexOptions.Multiline | RegexOptions.Compiled);

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
            if (value == null) return value;

            if (value is string)
            {
                string text = value as string;
                if (text.StartsWith("<![CDATA[") && text.EndsWith("]]>"))
                {
                    string HTMLtext = text.Substring(9, text.Length - 12);
                    return new ChoCDATA(HTMLtext);
                }
                else
                    return new ChoCDATA(value as string);
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

            if (value is ChoCDATA)
            {
                return ((ChoCDATA)value).ToString();
            }
            return base.ConvertFrom(context, culture, value);
        }

        #endregion
    }
}
