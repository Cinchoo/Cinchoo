namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(char))]
    public partial class ChoChar : IChoStringObjectFormatter<char>
    {
        #region Constants

        private const string Delimiter = "'";

        #endregion Constants

        #region Constants (Public)

        //[Description("NewLine")]
        //public const char NewLine = Environment.NewLine;

        [Description("Horizontal Tab")]
        public const char HorizontalTab = '\t';

        [Description("Null Character")]
        public const char NUL = '\0';

        [Description("Veritcal Tab")]
        public const char VerticalTab = '\v';

        [Description("Choape")]
        public const char Choape = (char)0x1B;

        [Description("Backspace")]
        public const char BackSpace = '\b';

        [Description("Carriage Return")]
        public const char CarriageReturn = '\r';

        [Description("LineFeed")]
        public const char LineFeed = '\n';

        [Description("Formfeed")]
        public const char Formfeed = '\f';

        [Description("Alert")]
        public const char BEL = '\a';

        [Description("Backslash")]
        public const char Backslash = '\\';

        [Description("Question Mark")]
        public const char QuestionMark = (char)0x63;

        [Description("Single Quotation Mark")]
        public const char SingleQuotationMark = '\'';

        [Description("Double Quotation Mark")]
        public const char DoubleQuotationMark = '\"';

        #endregion Constants (Public)

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^{0}(?<value>.){0}$", Delimiter), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private char _value;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoChar()
        {
        }

        public ChoChar(char value)
        {
            _value = value;
        }

        public ChoChar(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (char)Parse(formattedValue);
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
            return String.Format("{1}{0}{1}", _value, Delimiter);
        }

        public char Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoChar x = new ChoChar('A');
            return x.ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is Char;
        }

        [ChoObjectFormatter]
        public static string Format(object value, string format)
        {
            if (value == null) return null;
            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            IFormatProvider formatProvider = null;
            if (ChoFormatProviderSettings.Me.TryGetValue(format, out formatProvider) && formatProvider != null)
                return ((char)value).ToString(formatProvider);
            else
                return value.ToString();
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsChar(string value)
        {
            if (value == null || value.Length == 0) return false;

            return _regEx.IsMatch(value);
        }

        public static bool TryParse(string value, out char output)
        {
            output = '\0';

            if (value == null || value.Length == 0) return false;

            if (IsChar(value))
                return Char.TryParse(GetValue(value), out output);
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            char output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} char value passed.", value));
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

        public static implicit operator char(ChoChar input)
        {
            return input == null ? '\0' : input.Value;
        }

        #endregion Operators Overloading
    }
}
