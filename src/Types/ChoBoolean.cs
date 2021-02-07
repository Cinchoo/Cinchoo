namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(Boolean))]
    public class ChoBoolean : IChoStringObjectFormatter<bool>
    {
        #region Constants

        private const string Postfix = "B";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^(?<value>true)$|^(?<value>false)$|^(?<value>[01]){0}$", Postfix), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private bool _value;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoBoolean()
        {
        }

        public ChoBoolean(bool value)
        {
            _value = value;
        }

        public ChoBoolean(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (bool)Parse(formattedValue);
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

        public bool Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoBoolean x = new ChoBoolean(true);
            return ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is Boolean;
        }

        [ChoObjectFormatter]
        public static string Format(object value, string format)
        {
            if (value == null) return null;
            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            IFormatProvider formatProvider = null;
            if (ChoFormatProviderSettings.Me.TryGetValue(format, out formatProvider) && formatProvider != null)
                return ((bool)value).ToString(formatProvider);
            else
                return value.ToString();
        }

        public static string Format(object value, IFormatProvider formatProvider)
        {
            if (value == null) return null;

            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            return formatProvider == null ? value.ToString() : ((bool)value).ToString(formatProvider);
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsBoolean(string value)
        {
            if (value == null || value.Length == 0) return false;

            return _regEx.IsMatch(value)
                || String.Compare(value, "true", true) == 0
                || String.Compare(value, "false", true) == 0
                || value == "1"
                || value == "0";
        }

        public static bool TryParse(string value, out bool output)
        {
            output = false;

            if (value == null || value.Length == 0) return false;

            if (IsBoolean(value))
            {
                string actualValue = GetValue(value);
                if (String.Compare(actualValue, "true", true) == 0 || actualValue == "1")
                {
                    output = true;
                    return true;
                }
                else if (String.Compare(actualValue, "false", true) == 0 || actualValue == "0")
                {
                    output = false;
                    return true;

                }
                else
                    return false;
            }
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            bool output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} bool value passed.", value));
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static string GetValue(string value)
        {
            Match match = _regEx.Match(value);
            if (match.Success)
                return match.Groups["value"].ToString();
            else
                return value;
        }

        #endregion Shared Members (Private)
    
        #region Operators Overloading

        public static implicit operator bool(ChoBoolean input)
        {
            return input == null ? false : input.Value;
        }

        #endregion Operators Overloading
    }
}
