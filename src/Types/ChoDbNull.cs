namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using Cinchoo.Core;

    #endregion NameSpaces

    [ChoStringObjectFormattable(typeof(DBNull))]
    public class ChoDbNull : IChoStringObjectFormatter<DBNull>
    {
        #region Constants

        public const string DbNullString = "[DB_NULL]";

        #endregion Constants

        #region Instance Data Members (Private)

        private DBNull _value = DBNull.Value;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoDbNull()
        {
        }

        public ChoDbNull(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            _value = (DBNull)Parse(formattedValue);
        }

        #endregion Constructors

        #region Object Overrides

        public override string ToString()
        {
            return DBNull.Value.ToString();
        }

        #endregion Object Overrides

        #region Instance Memebrs (Public)

        public string ToFormattedString()
        {
            return DbNullString;
        }

        public DBNull Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoDbNull x = new ChoDbNull();
            return x.ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is DBNull;
        }

        [ChoObjectFormatter]
        public static string Format(object value, string format)
        {
            if (value == null) return null;
            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            IFormatProvider formatProvider = null;
            if (ChoFormatProviderSettings.Me.TryGetValue(format, out formatProvider) && formatProvider != null)
                return ((DBNull)value).ToString(formatProvider);
            else
                return DbNullString;
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsDbNull(string value)
        {
            if (value == null || value.Length == 0) return false;

            return String.Compare(value, DbNullString, true) == 0;
        }

        public static bool TryParse(string value, out DBNull output)
        {
            output = null;

            if (value == null || value.Length == 0) return false;
            
            if (String.Compare(value, DbNullString, true) == 0)
            {
                output = DBNull.Value;
                return true;
            }
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            if (IsDbNull(value))
                return DBNull.Value;
            else
                throw new FormatException("Invalid DBNull string passed.");
        }

        #endregion Shared Members (Public)
    }
}
