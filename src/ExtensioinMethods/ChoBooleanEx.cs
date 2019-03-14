namespace System
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    ///<summary>
    ///Defines a textual boolean value
    ///</summary>
    ///<remarks></remarks>
    public enum BooleanText
    {
        AcceptedDeclined,
        ActiveInactive,
        CheckedUnchecked,
        CorrectIncorrect,
        EnabledDisabled,
        OnOff,
        YesNo
    }

    public static class ChoBooleanEx
    {
        #region Shared Data Members (Private)

        private static Regex _regEx = new Regex("[A-Z][a-z]+", RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion Shared Data Members (Private)

        /// <summary>
        /// Assesses a boolean value and returns a textual value
        /// </summary>
        /// <param name="text">The textual value to be returned</param>
        /// <returns>A textual representation of the boolean value</returns>
        /// <remarks></remarks>
        public static string ToString(this bool value, BooleanText text)
        {
            MatchCollection matches = _regEx.Matches(text.ToString());
            return value.ToString(matches[0].Value, matches[1].Value);
        }
        /// <summary>
        /// Assesses a boolean value and returns a specified word in respect to true or false
        /// </summary>
        /// <param name="trueValue">The textual value to be returned if the current boolean is true</param>
        /// <param name="falseValue">The textual value to be returned if the current boolean is false</param>
        /// <returns>A string representation of the current boolean value</returns>
        /// <remarks></remarks>
        public static string ToString(this bool value, string trueValue, string falseValue)
        {
            return (value) ? trueValue : falseValue;
        }

        public static Dictionary<TKey1, Dictionary<TKey2, TValue>> Pivot<TSource, TKey1, TKey2, TValue>(
            this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector, Func<IEnumerable<TSource>, TValue> aggregate)
        {
            return source.GroupBy(key1Selector).Select(
            x => new
            {
                X = x.Key,
                Y = x.GroupBy(key2Selector).Select(
                z => new
                {
                    Z = z.Key,
                    V = aggregate(z)
                }
                ).ToDictionary(e => e.Z, o => o.V)
            }
            ).ToDictionary(e => e.X, o => o.Y);
        }
    }
}
