using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Storage
{
    #region NameSpaces

    using System;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// Helper class used to convert size in string format to long
    /// </summary>
    public static class ChoStorageScale
    {
        #region Constants

        public const long KB = 1024;
        public const long MB = KB * KB;
        public const long GB = MB * KB;
        public const long TB = GB * KB;

        #endregion

        #region Shared Data Memebers (Private)

        private static Regex _sizeRegEx = new Regex("(?<Size>\\d+)(?<Scale>[KMGT]B)*", RegexOptions.Compiled);

        #endregion

        #region Shated Member Functions (Public)


        /// <summary>
        /// Converts the string representation of a storage scale to its 64-bit signed long
        /// equivalent. A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">A string containing a storage scale to convert.</param>
        /// <returns>
        /// Contains the 64-bit signed long value equivalent
        /// to the storage value, if the conversion succeeded, or throws exception.
        /// </returns>
        public static long Parse(string value)
        {
            //Initialize it as MB
            long size = MB;

            if (value == null)
                throw new ArgumentNullException("value");

            Match match = _sizeRegEx.Match(value);
            if (match.Success)
            {
                size = Int32.Parse(match.Groups["Size"].ToString());
                switch (match.Groups["Scale"].ToString())
                {
                    case "KB":
                        size *= KB;
                        break;
                    case "MB":
                        size *= MB;
                        break;
                    case "GB":
                        size *= GB;
                        break;
                    case "TB":
                        size *= TB;
                        break;
                    default:
                        throw new FormatException(String.Format("Invalid `{0}` scale is passed.", match.Groups["Scale"].ToString()));
                }
            }
            else
                throw new FormatException("Invalid storage scale is passed.");

            if (size < 0)
                throw new ArgumentException(String.Format("Storage scale value should be non-negative. Passed: {0}.", size));

            return size;
        }

        /// <summary>
        /// Converts the string representation of a storage scale to its 64-bit signed long
        /// equivalent. A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">A string containing a storage scale to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the 64-bit signed long value equivalent
        /// to the storage value, if the conversion succeeded, or zero if the
        /// conversion failed. The conversion fails if the value parameter is null, is not
        /// of the correct format, or represents a number less than 0
        /// or greater than System.Int64.MaxValue. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, out long result)
        {
            result = 0;

            try
            {
                result = Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
