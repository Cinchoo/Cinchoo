namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoNull
    {
        #region Constants

        public const string NullString = "[NULL]";

        #endregion Constants

        #region Constructors

        public ChoNull()
        {
        }

        public ChoNull(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            Parse(formattedValue);
        }

        #endregion Constructors

        #region Object Overrides

        public override string ToString()
        {
            return ToFormattedString();
        }

        #endregion Object Overrides

        #region Instance Memebrs (Public)

        public string ToFormattedString()
        {
            return NullString;
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            return value == null;
        }

        [ChoObjectFormatter]
        public static string Format(object value, string format)
        {
            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            return NullString;
        }

        public static bool IsNull(string value)
        {
            if (value == null || value.Length == 0) return false;

            return String.Compare(value, NullString, true) == 0;
        }

        public static bool TryParse(string value, out object output)
        {
            output = null;

            if (value == null || value.Length == 0) return false;

            if (String.Compare(value, NullString, true) == 0)
                return true;
            else
                return false;
        }

        public static object Parse(string value)
        {
            if (IsNull(value))
                return null;
            else
                throw new FormatException("Invalid Null string passed.");
        }

        #endregion Shared Members (Public)
    }
}
