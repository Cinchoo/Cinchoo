namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(ulong))]
    [CLSCompliant(false)]
    public class ChoULong : IChoStringObjectFormatter<ulong>
    {
        #region Constants

        private const string Prefix = "UL";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^{0}(?<value>\d+)$", Prefix), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private ulong _value;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoULong()
        {
        }

        public ChoULong(ulong value)
        {
            _value = value;
        }

        public ChoULong(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (ulong)Parse(formattedValue);
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

        public ulong Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoULong x = new ChoULong(10);
            return x.ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is ulong;
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
                return ((ulong)value).ToString(formatProvider);
            else
                return ((ulong)value).ToString(format);
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsULong(string value)
        {
            if (value == null || value.Length == 0) return false;

            return _regEx.IsMatch(value) && GetValue(value).IsNumber();
        }

        public static bool TryParse(string value, out ulong output)
        {
            output = 0;

            if (value == null || value.Length == 0) return false;

            if (IsULong(value))
                return ulong.TryParse(GetValue(value), out output);
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            ulong output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} ulong value passed.", value));
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

        public static implicit operator ulong(ChoULong ulongObject)
        {
            return ulongObject == null ? (ulong)0 : ulongObject.Value;
        }

        #endregion Operators Overloading
    }
}
