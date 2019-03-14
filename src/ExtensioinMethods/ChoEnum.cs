namespace System
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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
    }
}
