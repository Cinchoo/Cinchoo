namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(DateTime))]
    public class ChoDateTime : IChoStringObjectFormatter<DateTime>
    {
        #region Constants

        private const string Delimiter = "#";
        private const string FormatSeparator = "^";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^{0}(?<value>.*)\{1}(?<format>.*){0}$|^{0}(?<value>.*){0}", Delimiter, FormatSeparator), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private DateTime _value = DateTime.Now;
        private string _formatString = "yyyy-MM-dd";

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoDateTime()
        {
        }

        public ChoDateTime(DateTime value)
        {
            _value = value;
        }

        public ChoDateTime(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (DateTime)Parse(formattedValue);
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
            return String.Format("{1}{0}{2}{3}{1}", _value, Delimiter, FormatSeparator, _formatString);
        }

        public DateTime Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoDateTime x = new ChoDateTime();
            return x.ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is DateTime;
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
                return ((DateTime)value).ToString(formatProvider);
            else
                return ((DateTime)value).ToString(format);
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsDateTime(string value)
        {
            if (value == null || value.Length == 0) return false;

            return _regEx.IsMatch(value);
        }

        public static bool TryParse(string value, out DateTime output)
        {
            output = DateTime.MinValue;

            if (value == null || value.Length == 0) return false;

            string actualValue = null;
            string format = null;
            GetValue(value, ref actualValue, ref format);

            if (IsDateTime(value))
            {
                if (!String.IsNullOrEmpty(format))
                    output = DateTime.ParseExact(actualValue, format, null);
                else
                    DateTime.TryParse(actualValue, out output);

                return true;
            }
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            DateTime output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} DateTime value passed.", value));
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static void GetValue(string input, ref string value, ref string format)
        {
            Match match = _regEx.Match(input);
            if (match.Success)
            {
                value = match.Groups["value"].ToString();
                format = match.Groups["format"].ToString();
            }
        }

        #endregion Shared Members (Private)
        
        #region Operators Overloading

        public static implicit operator DateTime(ChoDateTime input)
        {
            return input == null ? DateTime.MinValue : input.Value;
        }

        #endregion Operators Overloading
    }
}
