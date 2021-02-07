namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(Int16))]
    public class ChoInt16 : IChoStringObjectFormatter<Int16>
    {
        #region Constants

        private const string Prefix = "16@";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^{0}(?<value>\d+)$", Prefix), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private Int16 _value;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoInt16()
        {
        }

        public ChoInt16(Int16 value)
        {
            _value = value;
        }

        public ChoInt16(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (Int16)Parse(formattedValue);
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
            return String.Format("{0}{1}", Prefix, _value);
        }

        public Int16 Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoInt16 x = new ChoInt16(99);
            return x.ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)
        
        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is Int16;
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
                return ((Int16)value).ToString(formatProvider);
            else
                return ((Int16)value).ToString(format);
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsInt16(string value)
        {
            if (value == null || value.Length == 0) return false;

            return _regEx.IsMatch(value) && GetValue(value).IsNumber();
        }

        public static bool TryParse(string value, out Int16 output)
        {
            output = 0;

            if (value == null || value.Length == 0) return false;

            if (IsInt16(value))
                return Int16.TryParse(GetValue(value), out output);
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            Int16 output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} Int16 value passed.", value));
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

        public static implicit operator Int16(ChoInt16 input)
        {
            return input == null ? (Int16)0 : input.Value;
        }

        #endregion Operators Overloading
    }
}
