namespace System
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;
    using Cinchoo.Core.WPF;

    #endregion NameSpaces

    /// <summary>
    /// Static utility class used to convert enum to and from description.
    /// Enum values should be decorated with DescriptionAttribute in order to use this class
    /// 
    /// Ex:
    ///     [Flags]
    ///     public enum Coolness : int
    ///     {
    ///         Nothing = 0,
    ///         [Description("Hot Weather")]
    ///         Hot = (1 << 0),
    ///         [Description("Cold Weather")]
    ///         Cold = (1 << 1),
    ///         [Description("Chill Weather")]
    ///         Chill = (1 << 2),
    ///     }
    /// </summary>
    public static class ChoEnum
    {
        /// <summary>
        /// Method used to convert a enum value to correponding description value attached to.
        /// </summary>
        /// <param name="enumValue">A enum value</param>
        /// <returns>Description value attached to the enum value if there is a match, otherwise Enumvalue.ToString() will be returned</returns>
        public static string ToDescription(this Enum enumValue)
        {
            return ChoEnumTypeDescCache.GetEnumDescription(enumValue);
        }

        public static IEnumerable<Tuple<int, string>> ToEnumPairValues<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type is not an enum.");
            
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var pairs =
                Enumerable.Range(0, names.Length)
                .Select(i => new Tuple<int, string>((int)values.GetValue(i), (string)names.GetValue(i)))
                .OrderBy(pair => pair.Item2);
            return pairs;
        }

        public static ChoObservableNodeList AsNodeList<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type is not an enum.");

            ChoObservableNodeList itemSource = new ChoObservableNodeList();
            foreach (string f in Enum.GetNames(type))
            {
                ChoNode a = new ChoNode(f);
                a.IsSelected = (from x in value.ToNString().SplitNTrim()
                                where x == f.ToString()
                                select x).FirstOrDefault() != null;


                itemSource.Add(a);
            }

            return itemSource;
        }
    }
}
