namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(byte))]
    public class ChoByte : IChoStringObjectFormatter<byte>
    {
        #region Constants

        private const string Postfix = "!";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^(?<value>.*){0}$", Postfix), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private byte _value;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoByte()
        {
        }

        public ChoByte(byte value)
        {
            _value = value;
        }

        public ChoByte(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (byte)Parse(formattedValue);
        }

        #endregion Constructors

        #region Object Overrides

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion Object Overrides

        #region Instance Memebrs (Public)

        public string ToFormattedString()
        {
            return String.Format("{0}{1}", _value, Postfix);
        }

        public byte Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoByte x = new ChoByte(1);
            return ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        public static string ToString(byte[] byteArr)
        {
            byte value;
            string tempStr = String.Empty;
            for (int index = 0; index <= byteArr.GetUpperBound(0); index++)
            {
                value = byteArr[index];
                if (value < (byte)10)
                    tempStr += "00" + value.ToString();
                else if (value < (byte)100)
                    tempStr += "0" + value.ToString();
                else
                    tempStr += value.ToString();
            }
            return tempStr;
        }

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is Byte;
        }

        [ChoObjectFormatter]
        public static string Format(object value, string format)
        {
            if (value == null) return null;
            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            if (String.IsNullOrEmpty(format)) return value.ToString();

            IFormatProvider formatProvider = null;
            if (ChoFormatProviderSettings.Me.TryGetValue(format, out formatProvider) && formatProvider != null)
                return ((bool)value).ToString(formatProvider);
            else
                return ((Byte)value).ToString(format);
        }

        public static string Format(object value, IFormatProvider formatProvider)
        {
            if (value == null) return null;

            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            return formatProvider == null ? value.ToString() : ((Byte)value).ToString(formatProvider);
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsByte(string value)
        {
            if (value == null || value.Length == 0) return false;

            return _regEx.IsMatch(value) && GetValue(value).IsNumber();
        }

        public static bool TryParse(string value, out byte output)
        {
            output = 0;

            if (value == null || value.Length == 0) return false;

            if (IsByte(value))
                return Byte.TryParse(GetValue(value), out output);
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            byte output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} byte value passed.", value));
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static string GetValue(string value)
        {
            Match match = _regEx.Match(value);
            if (match.Success)
                return match.Groups["value"].ToString();
            else
                return String.Empty;
        }

        #endregion Shared Members (Private)
    
        #region Operators Overloading

        public static implicit operator byte(ChoByte input)
        {
            return input == null ? (byte)0 : input.Value;
        }

        #endregion Operators Overloading
    }
}
