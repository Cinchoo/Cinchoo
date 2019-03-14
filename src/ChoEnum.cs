namespace Cinchoo.Core
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
        /// Convert a description value to enum value
        /// </summary>
        /// <typeparam name="T">The type of enum to considered for the conversion</typeparam>
        /// <param name="description">Description value to look into the enum values decorated with DescriptionAttribute.</param>
        /// <returns>Returns enum value correponding to the description if there is a match, otherwise returns Enum.Nothing</returns>
        //public static T ToEnum<T>(this string description) where T : struct
        //{
        //    T enumValue = default(T);
        //    if (!(enumValue is Enum))
        //        throw new ApplicationException("Type should be enum");

        //    Type type = enumValue.GetType();
        //    MemberInfo[] memInfos = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

        //    if (memInfos != null && memInfos.Length > 0)
        //    {
        //        foreach (MemberInfo memInfo in memInfos)
        //        {
        //            object[] attrs = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        //            if (attrs != null && attrs.Length > 0)
        //            {
        //                if (String.Compare(((DescriptionAttribute)attrs[0]).Description, description, true) == 0)
        //                    return (T)Enum.Parse(type, memInfo.Name);
        //            }
        //        }
        //    }

        //    return enumValue;
        //}

        /// <summary>
        /// Method used to convert a enum value to correponding description value attached to.
        /// </summary>
        /// <param name="enumValue">A enum value</param>
        /// <returns>Description value attached to the enum value if there is a match, otherwise Enumvalue.ToString() will be returned</returns>
        public static string ToDescription(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumValue.ToString();
        }
    }
}
