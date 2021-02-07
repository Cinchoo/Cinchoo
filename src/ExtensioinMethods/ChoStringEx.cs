#region Copyright & License
//
// Author: Raj Nagalingam (dotnetfm@google.com)
// Copyright (c) 2007-2008, NAG Groups LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace System
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Globalization;
    using System.Reflection;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    using Cinchoo.Core;
    using Cinchoo.Core.Text.RegularExpressions;
    using System.Xml.Linq;
    using System.Security;

    #endregion NameSpaces

    public static class ChoStringEx
    {
        #region Constants

        private const string HeaderDelimiter = "--";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _splitNameRegex = new Regex(@"[\W_]+", RegexOptions.Compiled);
        private static readonly Regex _headerRegex = new Regex(@"^\W*{0}.*".FormatString(HeaderDelimiter), RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex _whiteSpaceRegex = new Regex(@"\s");
        private static readonly Regex _openBraceRegex = new Regex(@"([^\\])[\{]", RegexOptions.Compiled);
        private static readonly Regex _closeBraceRegex = new Regex(@"([^\\])[\}]", RegexOptions.Compiled);
        private static readonly Regex _percentageRegex = new Regex(@"([^\\])[\%]", RegexOptions.Compiled);
        private static readonly Regex _tiltaRegex = new Regex(@"([^\\])[\~]", RegexOptions.Compiled);
        private static readonly Regex _caretRegex = new Regex(@"([^\\])[\^]", RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Conversion Methods

        public static long ToInt16(this string value)
        {
            Int16 result = 0;

            if (!string.IsNullOrEmpty(value))
                Int16.TryParse(value, out result);

            return result;
        }

        public static long ToInt32(this string value)
        {
            Int32 result = 0;

            if (!string.IsNullOrEmpty(value))
                Int32.TryParse(value, out result);

            return result;
        }

        public static long ToInt64(this string value)
        {
            Int64 result = 0;

            if (!string.IsNullOrEmpty(value))
                Int64.TryParse(value, out result);

            return result;
        }

        #endregion Conversion Methods

        #region Match Overloads

        public static bool Match(this string value, string pattern)
        {
            return Regex.IsMatch(value, pattern);
        }

        #endregion Match Overloads

        #region Format Overloads

        public static string FormatString(this string format, params object[] args)
        {
            try
            {
                return string.Format(format, args);
            }
            catch (Exception ex)
            {
                throw new ChoApplicationException(String.Format("Error formatting '{0}' text.", format), ex);
            }
        }

        #endregion Format Overloads

        #region String null/empty validation members

        /// <summary>
        /// Check a string is String.Empty or not.
        /// </summary>
        /// <param name="text">The string value will be checked for String.Empty.</param>
        /// <returns>True if the string is empty, otherwise returns false.</returns>
        public static bool IsEmpty(this string text)
        {
            return (text != null && text.Length == 0);
        }

        /// <summary>
        /// Check a string is null or not.
        /// </summary>
        /// <param name="text">The string value will be checked for null.</param>
        /// <returns>True if the string is null, otherwise returns false.</returns>
        public static bool IsNull(this string text)
        {
            return (text == null);
        }

        /// <summary>
        /// Check a string for String.Empty or null.
        /// </summary>
        /// <param name="text">The string value will be checked for String.Empty or null.</param>
        /// <returns>True if the string is empty or null, otherwise returns false.</returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Check a string for String.Empty or null.
        /// </summary>
        /// <param name="text">The string value will be checked for String.Empty or null.</param>
        /// <returns>True if the string is empty or null, otherwise returns false.</returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrEmpty(text) || text.Trim().Length == 0;
        }

        #endregion String null/empty validation members

        #region ToEnumValue Overloads

        public static object ToEnumvalue(this string fullyQualifiedEnumValue)
        {
            Regex regEx = new Regex(@"^(?<enumType>(\w+\.)*)(?<enumValue>\w+)$|^(?<enumType>(\w+\.)*)(?<enumValue>\w+)(?<assemblyName>\,\s*.*)$");
            Match match = regEx.Match(fullyQualifiedEnumValue);
            if (!match.Success)
                throw new ApplicationException(String.Format("Incorrect format value [{0}] passed.", fullyQualifiedEnumValue));

            string typeName = match.Groups["enumType"].ToString().Substring(0, match.Groups["enumType"].ToString().Length - 1);
            if (!String.IsNullOrEmpty(match.Groups["assemblyName"].ToString()))
                typeName += match.Groups["assemblyName"];

            Type enumType = Type.GetType(typeName);
            if (enumType == null)
                throw new ApplicationException(String.Format("Can't find [{0}] type.", match.Groups["enumType"].ToString()));

            return Enum.Parse(enumType, match.Groups["enumValue"].ToString());
        }

        #endregion ToEnumValue Overloads

        #region ContainsWhiteSpace Overloads

        public static bool ContainsWhitespaces(this string text)
        {
            if (text == null) return false;
            return _whiteSpaceRegex.IsMatch(text);
        }

        #endregion

        #region Indent Overloads

        /// <summary>
        /// Left aligns the characters in this string on each line, padding on the right with a Char.Tab (\t)
        /// character
        /// </summary>
        /// <param name="text">The string value which will be indented</param>
        /// <returns>A new System.String that is equivalent to this instance, but left-aligned and padded on the right with 
        /// as many paddingChar characters as needed to each line of this string.</returns>
        public static string Indent(this String text)
        {
            return Indent(text, 1, ChoCharEx.HorizontalTab);
        }

        /// <summary>
        /// Left aligns the characters in this string on each line, padding on the right with Char.Tab
        /// character, for a specified total length
        /// </summary>
        /// <param name="text">The string value which will be indented</param>
        /// <param name="totalWidth">The number of padding characters to be added at the beginning of each line in the resulting string.</param>
        /// <returns>A new System.String that is equivalent to this instance, but left-aligned and padded on the right with as many paddingChar characters as needed to each line of this string.</returns>
        public static string Indent(this String text, int totalWidth)
        {
            return Indent(text, totalWidth, ChoCharEx.HorizontalTab);
        }

        /// <summary>
        /// Left aligns the characters in this string on each line, padding on the right with
        /// the specified Unicode character, for a specified total length
        /// </summary>
        /// <param name="text">The string value which will be indented</param>
        /// <param name="totalWidth">The number of padding characters to be added at the beginning of each line in the resulting string. If the value is negative, it will be undented the specified character with number of specified width</param>
        /// <param name="paddingChar">A Unicode padding character.</param>
        /// <returns>A new System.String that is equivalent to this instance, but left-aligned and padded on the right with as many paddingChar characters as needed to each line of this string.</returns>
        public static string Indent(this String text, int totalWidth, char paddingChar)
        {
            if (text == null) return null;

            if (totalWidth == 0) return text;
            if (totalWidth < 0) return Unindent(text, totalWidth, paddingChar);

            string tabs = String.Empty;
            for (int index = 0; index < totalWidth; index++)
                tabs = tabs + paddingChar;

            string pattern = String.Format(@".*[{0}]*", Environment.NewLine);

            StringBuilder formattedtext = new StringBuilder();
            foreach (Match m in Regex.Matches(text, pattern))
            {
                if (m.ToString() == Environment.NewLine || String.IsNullOrEmpty(m.ToString().Trim()))
                    formattedtext.AppendFormat("{0}", m.ToString());
                else
                    formattedtext.AppendFormat("{0}{1}", tabs, m.ToString());
            }

            return formattedtext.ToString();
        }

        #endregion Indent Overloads

        #region Unindent Overloads

        /// <summary>
        /// Remove a Char.Tab character in this string on each line
        /// character
        /// </summary>
        /// <param name="text">The string value which will be unindented</param>
        /// <returns>A new System.String that is equivalent to this instance, but removed a Char.Tab characte on each line of this string.</returns>
        public static string Unindent(this String text)
        {
            return Unindent(text, 1, ChoCharEx.HorizontalTab);
        }

        public static string Unindent(this String text, int totalWidth)
        {
            return Unindent(text, totalWidth, ChoCharEx.HorizontalTab);
        }

        public static string Unindent(this String text, int totalWidth, char paddingChar)
        {
            if (text == null) return null;
            if (totalWidth == 0) return text;
            if (totalWidth < 0)
                return Indent(text, Math.Abs(totalWidth), paddingChar);

            string linePattern = String.Format(@".*[{0}]*", Environment.NewLine);
            string pattern = String.Format(@"{1}(?<text>.*[{0}]|.*)", Environment.NewLine, paddingChar);
            StringBuilder formattedMsg = new StringBuilder();

            for (int index = -1 * Math.Abs(totalWidth); index < 0; index++)
            {
                formattedMsg = new StringBuilder();
                foreach (Match m in Regex.Matches(text, linePattern))
                {
                    if (m.ToString() == Environment.NewLine || String.IsNullOrEmpty(m.ToString().Trim()))
                        formattedMsg.AppendFormat("{0}", m.ToString());
                    else
                    {
                        Match match = Regex.Match(m.ToString(), pattern);
                        if (!match.Success)
                            return text;
                        formattedMsg.AppendFormat("{0}", match.Groups["text"].ToString());
                    }
                }

                text = formattedMsg.ToString();
            }

            return formattedMsg.ToString();
        }

        #endregion Unindent Overloads

        #region ToSpacedWords Overloads

        /// <summary>
        /// Takes a NameIdentifier and spaces it out into words "Name Identifier".
        /// </summary>
        /// <param name="text">A string value which will be break into spaced words</param>
        /// <returns>A new System.String having spaced words.</returns>
        public static string ToSpacedWords(this String text)
        {
            if (String.IsNullOrEmpty(text)) return text;

            StringBuilder spacedNames = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && i < text.Length - 1 && Char.IsUpper(text[i]))
                {
                    spacedNames.Append(" ");
                }
                spacedNames.Append(text[i]);
            }

            return spacedNames.ToString();
        }

        #endregion ToSpacedWords Overloads

        #region ToCamelCase Member (Public)

        /// <summary>
        /// Converts a string to use camelCase.
        /// </summary>
        /// <param name="value">A string value to convert</param>
        /// <returns>A new System.String converted to Camel Case</returns>
        public static string ToCamelCase(this string value)
        {
            if (value == null || value.Trim().Length == 0)
                return value;

            string output = ToPascalCase(value);
            if (output.Length > 2)
                return char.ToLower(output[0]) + output.Substring(1);
            else
                return output.ToLower();
        }

        #endregion ToCamelCase Member (Public)

        #region ToProperCase Overloads

        /// <summary>
        /// Converts a string to Proper Case. This is an alias for ToPascalCase
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>The string converted to Proper Case</returns>
        public static string ToProperCase(this string value)
        {
            return ToPascalCase(value);
        }

        #endregion ToProperCase Overloads

        #region ToPascalCase Overloads

        /// <summary>
        /// Converts a string to use PascalCase.
        /// </summary>
        /// <param name="value">Text to convert</param>
        /// <returns></returns>
        public static string ToPascalCase(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            string[] names = _splitNameRegex.Split(value);
            StringBuilder output = new StringBuilder();

            if (names.Length > 1)
            {
                foreach (string name in names)
                {
                    if (name.Length > 1)
                    {
                        output.Append(char.ToUpper(name[0]));
                        output.Append(name.Substring(1).ToLower());
                    }
                    else
                        output.Append(name);
                }
            }
            else if (value.Length > 1)
            {
                output.Append(char.ToUpper(value[0]));
                output.Append(value.Substring(1));
            }
            else
                output.Append(value.ToUpper());

            return output.ToString();
        }

        #endregion ToPascalCase Overloads

        #region SplitNTrim Overloads (Public)

        /// <summary>
        /// Split the string into multiple strings by a ',', ';' separators and trim them each.
        /// </summary>
        /// <param name="text">A string value to be splited and trim.</param>
        /// <returns>A string array contains splitted and trimmed string values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] SplitNTrim(this string text)
        {
            return SplitNTrim(text, new char[] { ',', ';' });
        }

        /// <summary>
        /// Split the string into multiple strings by the separators and trim the each one.
        /// </summary>
        /// <param name="text">A string value to be splited and trim.</param>
        /// <param name="separators">List of separators used to split the string.</param>
        /// <returns>A string array contains splitted and trimmed string values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] SplitNTrim(this string text, char separator)
        {
            return SplitNTrim(text, new char[] { separator });
        }

        public static IEnumerable<string> SplitNTrimEx(this string inString, char separator)
        {
            if (inString.IsNullOrEmpty())
                yield return inString;

            string msg = inString;
            if (inString.IndexOf(separator) != -1)
            {
                int index = -1;
                bool hasEscapeChar = false;
                StringBuilder message = new StringBuilder();

                while (++index < inString.Length)
                {
                    if (inString[index] == ChoChar.Backslash)
                    {
                        if (!hasEscapeChar) //if first slash char found, flag and ignore
                        {
                            hasEscapeChar = true;
                        }
                        else  //if second slash char found, just append the char
                        {
                            hasEscapeChar = false;
                            message.Append(inString[index]);
                        }
                        continue;
                    }

                    if (hasEscapeChar) //if backslash char preceded by it, add and continue
                    {
                        hasEscapeChar = false;
                        message.Append(inString[index]);
                        continue;
                    }

                    if (inString[index] == separator)
                    {
                        yield return message.ToString();
                        message.Clear();

                        continue;
                    }
                    else
                    {
                        message.Append(inString[index]);
                    }
                }
                yield return message.ToString();
                message.Clear();
            }
            else
                yield return msg;
        }

        public static int IndexOfEx(this string inString, char separator)
        {
            if (inString.IsNullOrEmpty())
                return -1;

            string msg = inString;
            if (inString.IndexOf(separator) != -1)
            {
                int index = -1;
                bool hasEscapeChar = false;

                while (++index < inString.Length)
                {
                    if (inString[index] == ChoChar.Backslash)
                    {
                        if (!hasEscapeChar) //if first slash char found, flag and ignore
                        {
                            hasEscapeChar = true;
                        }
                        else  //if second slash char found, just append the char
                        {
                            hasEscapeChar = false;
                        }
                        continue;
                    }

                    if (hasEscapeChar) //if backslash char preceded by it, add and continue
                    {
                        hasEscapeChar = false;
                        continue;
                    }

                    if (inString[index] == separator)
                    {
                        return index;
                    }
                }
            }
                
            return -1;
        }

        public static int IndexOfEx(this string inString, string value)
        {
            ChoGuard.ArgumentNotNull(value, "Value");

            if (inString.IsNullOrEmpty())
                return -1;

            string msg = inString;
            if (inString.IndexOf(value) != -1)
            {
                int index = -1;
                bool hasEscapeChar = false;

                while (++index < inString.Length)
                {
                    if (inString[index] == ChoChar.Backslash)
                    {
                        if (!hasEscapeChar) //if first slash char found, flag and ignore
                        {
                            hasEscapeChar = true;
                        }
                        else  //if second slash char found, just append the char
                        {
                            hasEscapeChar = false;
                        }
                        continue;
                    }

                    if (hasEscapeChar) //if backslash char preceded by it, add and continue
                    {
                        hasEscapeChar = false;
                        continue;
                    }

                    if (inString[index] == value[0])
                    {
                        bool matchFound = false;
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (index + i < inString.Length && inString[index + i] == value[i])
                            {
                                matchFound = true;
                                continue;
                            }
                            else
                            {
                                matchFound = false;
                                index += i + 1;
                                break;
                            }
                        }

                        if (matchFound)
                            return index;
                    }
                }
            }

            return -1;
        }

        //public static int LastIndexOfEx(this string inString, string value)
        //{
        //    ChoGuard.ArgumentNotNull(value, "Value");

        //    if (inString.IsNullOrEmpty())
        //        return -1;

        //    string msg = inString;
        //    if (inString.LastIndexOf(value) != -1)
        //    {
        //        int index = inString.Length;
        //        bool hasEscapeChar = false;

        //        while (--index >= 0)
        //        {
        //            if (inString[index] == value[value.Length - 1])
        //            {
        //                bool matchFound = false;
        //                for (int i = value.Length - 1; i >= 0; i--)
        //                {
        //                    if (index + i < inString.Length && inString[index + i] == value[i])
        //                    {
        //                        matchFound = true;
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        matchFound = false;
        //                        index += i + 1;
        //                        break;
        //                    }
        //                }

        //                if (matchFound)
        //                    return index;
        //            }
        //        }
        //    }

        //    return -1;
        //}

        public static bool ContainsEx(this string inString, string value)
        {
            return IndexOfEx(inString, value) >= 0;
        }

        public static bool StartsWithEx(this string inString, string value)
        {
            ChoGuard.ArgumentNotNullOrEmpty(value, "Value");
            if (inString.IsNullOrEmpty()) return false;
            if (value.Length == 1)
                return inString.StartsWith(value);

            return IndexOfEx(inString, value) == 0;
        }

        //public static bool EndsWithEx(this string inString, string value)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(value, "Value");
        //    if (inString.IsNullOrEmpty()) return false;
        //    if (value.Length == 1)
        //        return inString.EndsWithEx(value);

        //    return LastIndexOfEx(inString, value) + value.Length + 1 == inString.Length;
        //}

        public static string Unescape(this string inString)
        {
            if (inString.IsNullOrEmpty())
                return inString;

            string msg = inString;
            int index = -1;
            bool hasEscapeChar = false;
            StringBuilder message = new StringBuilder();

            while (++index < inString.Length)
            {
                if (inString[index] == ChoChar.Backslash)
                {
                    if (!hasEscapeChar) //if first slash char found, flag and ignore
                    {
                        hasEscapeChar = true;
                    }
                    else  //if second slash char found, just append the char
                    {
                        hasEscapeChar = false;
                        message.Append(inString[index]);
                    }
                    continue;
                }

                if (hasEscapeChar) //if backslash char preceded by it, add and continue
                    hasEscapeChar = false;
             
                message.Append(inString[index]);
            }
            return message.ToString();
        }

        /// <summary>
        /// Split the string into multiple strings by the separators and trim the each one.
        /// </summary>
        /// <param name="text">A string value to be splited and trim.</param>
        /// <param name="separators">List of separators used to split the string.</param>
        /// <returns>A string array contains splitted and trimmed string values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] SplitNTrim(this string text, char[] separators)
        {
            return SplitNTrim(text, separators, StringSplitOptions.None);
        }

        public static string[] SplitNTrim(this string text, string value)
        {
            return SplitNTrim(text, value, StringSplitOptions.None);
        }

        public static string[] SplitNTrim(this string text, string value, StringSplitOptions stringSplitOptions)
        {
            if (text == null || text.Trim().Length == 0) return new string[] { };

            string word;
            List<string> tokenList = new List<string>();
            foreach (string token in Split(text, value, stringSplitOptions))
            {
                word = token != null ? token.Trim() : token;
                if (String.IsNullOrEmpty(word))
                {
                    if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries)
                        tokenList.Add(word);
                }
                else
                    tokenList.Add(word);
            }

            return tokenList.ToArray();
        }

        public static string[] SplitNTrim(this string text, char[] separators, StringSplitOptions stringSplitOptions)
        {
            if (text == null || text.Trim().Length == 0) return new string[] { };

            string word;
            List<string> tokenList = new List<string>();
            foreach (string token in Split(text, separators, stringSplitOptions))
            {
                word = token != null ? token.Trim() : token;
                if (String.IsNullOrEmpty(word))
                {
                    if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries)
                        tokenList.Add(word);
                }
                else
                    tokenList.Add(word);
            }

            return tokenList.ToArray();
        }

        #endregion SplitNTrim Overloads (Public)

        #region Split Overloads (Public)

        public static object[] SplitNConvertToObjects(this string text)
        {
            return SplitNConvertToObjects(text, new char[] { ',', ';' });
        }

        public static object[] SplitNConvertToObjects(this string text, char separator)
        {
            return SplitNConvertToObjects(text, new char[] { separator });
        }

        public static object[] SplitNConvertToObjects(this string text, char[] separators)
        {
            if (text == null) return new object[] { };
            if (text.IsNullOrWhiteSpace()) return new object[] { text };

            return (from x in text.SplitNTrim(separators)
                    select x.Evaluate()).ToArray();
        }

        /// <summary>
        /// Split the string into multiple strings by a ',', ';' separators.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text)
        {
            return SplitNTrim(text, new char[] { ',', ';' });
        }

        /// <summary>
        /// Split the string into multiple strings by a separator.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <param name="separator">A separator used to split the string.</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text, char separator)
        {
            return Split(text, new char[] { separator });
        }

        /// <summary>
        /// Split the string into multiple strings by the separators.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <param name="separators">List of separators used to split the string.</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text, char[] separators)
        {
            return Split(text, separators, StringSplitOptions.None);
        }

        public static string[] Split(this string text, string value)
        {
            return Split(text, value, StringSplitOptions.None);
        }

        /// <summary>
        /// Split the string into multiple strings by the separators.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <param name="separators">List of separators used to split the string.</param>
        /// <param name="ignoreEmptyWord">true, to ignore the empry words in the output list</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text, char[] separators, StringSplitOptions stringSplitOptions)
        {
            return Split(text, (object)separators, stringSplitOptions);
        }

        public static string[] Split(this string text, string value, StringSplitOptions stringSplitOptions)
        {
            return Split(text, (object)value, stringSplitOptions);
        }

        private static string[] Split(this string text, object separators, StringSplitOptions stringSplitOptions)
        {
            if (String.IsNullOrEmpty(text)) return new string[0];

            List<string> splitStrings = new List<string>();

            int len = separators is char[] ? 0 : ((string)separators).Length - 1;
            int i = 0;
            int quotes = 0;
            int singleQuotes = 0;
            int offset = 0;
            bool hasChar = false;
            string word = null;
            while (i < text.Length)
            {
                if (text[i] == '\"') { quotes++; }
                else if (text[i] == '\'') { singleQuotes++; }
                else if (text[i] == '\\'
                    && i + 1 < text.Length && Contains(text, ++i, separators))
                    hasChar = true;
                else if (Contains(text, i, separators) &&
                    ((quotes > 0 && quotes % 2 == 0) || (singleQuotes > 0 && singleQuotes % 2 == 0))
                    || Contains(text, i, separators) && quotes == 0 && singleQuotes == 0)
                {
                    if (hasChar)
                    {
                        word = NormalizeString(text.Substring(offset, i - len - offset).Replace("\\", String.Empty));
                        if (String.IsNullOrEmpty(word))
                        {
                            if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries)
                                splitStrings.Add(word);
                        }
                        else
                            splitStrings.Add(word);

                        hasChar = false;
                    }
                    else
                    {
                        string subString = text.Substring(offset, i - len - offset);
                        if (subString.Length == 2)
                            splitStrings.Add(subString);
                        else
                        {
                            word = NormalizeString(subString);
                            if (String.IsNullOrEmpty(word))
                            {
                                if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries)
                                    splitStrings.Add(word);
                            }
                            else
                                splitStrings.Add(word);
                        }
                    }

                    offset = i + 1;
                }
                i++;
            }

            //if (offset < text.Length)
                splitStrings.Add(hasChar ? NormalizeString(text.Substring(offset).Replace("\\", String.Empty)) : NormalizeString(text.Substring(offset)));

            return splitStrings.ToArray();
        }

        #endregion Split Overloads (Public)

        #region IsAlphaNumeric member

        private readonly static Regex _alphaNumericPattern = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);

        /// <summary>
        /// Function to test a string value contains only alphanumeric characters.
        /// </summary>
        /// <param name="text">A string to test for alphanumeric characters.</param>
        /// <returns>true, if the passed text is a valid alphanumeric characters. Otherwise false.</returns>
        public static bool IsAlphaNumeric(this string text)
        {
            Regex alphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !alphaNumericPattern.IsMatch(text);
        }

        #endregion IsAlphaNumeric member

        #region IsAlpha member

        private readonly static Regex _alphaPattern = new Regex("[^a-zA-Z]", RegexOptions.Compiled);

        // Function To test for Alphabets.
        /// <summary>
        /// Function to test a string value contains only alphabets.
        /// </summary>
        /// <param name="text">A string to test for alphabets.</param>
        /// <returns>true, if the passed text is a valid alphabets. Otherwise false.</returns>
        public static bool IsAlpha(this string text)
        {
            return !_alphaPattern.IsMatch(text);
        }

        #endregion IsAlpha member

        #region IsNumber Member

        private readonly static Regex _notNumberPattern = new Regex("[^0-9.-]");
        private readonly static Regex _objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
        private readonly static Regex _numberPattern = new Regex("(^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$)|(^([-]|[0-9])[0-9]*$)");

        /// <summary>
        /// Function to test a string value for a number.
        /// </summary>
        /// <param name="text">A string to test for number.</param>
        /// <returns>true, if the passed text is a valid number. Otherwise false.</returns>
        public static bool IsNumber(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return !_notNumberPattern.IsMatch(text) &&
                !_objTwoDotPattern.IsMatch(text) &&
                !_objTwoMinusPattern.IsMatch(text) &&
                _numberPattern.IsMatch(text);
        }

        #endregion IsNumber Member

        #region IsPositiveNumber Member

        private readonly static Regex _notPositivePattern = new Regex("[^0-9.]", RegexOptions.Compiled);
        private readonly static Regex _objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$", RegexOptions.Compiled);
        private readonly static Regex _objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*", RegexOptions.Compiled);

        /// <summary>
        /// Function to test a string value for positive number both integer & real.
        /// </summary>
        /// <param name="text">A string to test for positive number.</param>
        /// <returns>true, if the passed text is a valid positive number. Otherwise false.</returns>
        public static bool IsPositiveNumber(string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return !_notPositivePattern.IsMatch(text) &&
                _objPositivePattern.IsMatch(text) &&
                !_objTwoDotPattern.IsMatch(text);
        }

        #endregion IsPositiveNumber Member

        #region IsNaturalNumber Member

        private readonly static Regex _notNaturalPattern = new Regex("[^0-9]", RegexOptions.Compiled);
        private readonly static Regex _naturalPattern = new Regex("0*[1-9][0-9]*", RegexOptions.Compiled);

        /// <summary>
        /// Method to test a string value for positive integers.
        /// </summary>
        /// <param name="text">A string to test for positive integer.</param>
        /// <returns>true, if the passed text is a valid positive integer. Otherwise false.</returns>
        public static bool IsNaturalNumber(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return !_notNaturalPattern.IsMatch(text) &&
                _naturalPattern.IsMatch(text);
        }

        #endregion IsNaturalNumber Member

        #region IsWholeNumber Member

        private readonly static Regex _notWholePattern = new Regex("[^0-9]", RegexOptions.Compiled);

        /// <summary>
        /// Method to test a string value for positive integers with zero inclusive 
        /// </summary>
        /// <param name="text">A string to test for whole number.</param>
        /// <returns>true, if the passed text is a valid whole number. Otherwise false.</returns>
        public static bool IsWholeNumber(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return !_notWholePattern.IsMatch(text);
        }

        #endregion IsWholeNumber Member

        #region IsInteger Member

        private readonly static Regex _notIntPattern = new Regex("[^0-9-]", RegexOptions.Compiled);
        private readonly static Regex _objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$", RegexOptions.Compiled);

        /// <summary>
        /// Function to test the string value for integers both Positive & Negative.
        /// </summary>
        /// <param name="text">A string to test for integer.</param>
        /// <returns>true, if the passed text has valid integer value. Otherwise false.</returns>
        public static bool IsInteger(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return !_notIntPattern.IsMatch(text) && _objIntPattern.IsMatch(text);
        }

        #endregion IsInteger Member

        #region IsBoolean member

        /// <summary>
        /// Function to test the string for boolean.
        /// </summary>
        /// <param name="text">A string to test for boolean.</param>
        /// <returns>true, if the passed text has valid boolean value. Otherwise false.</returns>
        public static bool IsBoolean(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return text.Trim().ToLower() == "true" || text.Trim().ToLower() == "false";
        }

        #endregion IsBoolean member

        #region IsByte member

        private readonly static Regex _bytePattern = new Regex("^[0-2][0-5][0-5]$|^[0-9]{1,2}$", RegexOptions.Compiled);

        /// <summary>
        /// Function to test the string for byte.
        /// </summary>
        /// <param name="text">A string to test for byte value.</param>
        /// <returns>true, if the passed text has valid byte value. Otherwise false.</returns>
        public static bool IsByte(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return _bytePattern.IsMatch(text);
        }

        #endregion IsByte member

        #region IsSByte member

        private readonly static Regex _sBytePattern = new Regex("^[-][0-1][0-2][0-8]$|^[0-1][0-2][0-7]$|^[-][0-9]{1,2}$|^[0-9]{1,2}$", RegexOptions.Compiled);

        /// <summary>
        /// Function to test the string for signed byte.
        /// </summary>
        /// <param name="text">A string to test for signed byte value.</param>
        /// <returns>true, if the passed text has valid signed byte value. Otherwise false.</returns>
        public static bool IsSByte(this string text)
        {
            if (String.IsNullOrEmpty(text)) return false;

            return _sBytePattern.IsMatch(text);
        }

        #endregion IsSByte member

        #region IsPlural member

        private static readonly List<string> _invariants = new List<string>(new string[] { "alias", "news" });

        private static readonly Regex _pluralRegex1 = new Regex("(?<keep>[^aeiou])ies$", RegexOptions.Compiled);
        private static readonly Regex _pluralRegex2 = new Regex("(?<keep>[aeiou]y)s$", RegexOptions.Compiled);
        private static readonly Regex _pluralRegex3 = new Regex("(?<keep>[sxzh])es$", RegexOptions.Compiled);
        private static readonly Regex _pluralRegex4 = new Regex("(?<keep>[^sxzhy])s$", RegexOptions.Compiled);

        /// <summary>
        /// Determines if a string is in plural form based on some simple rules.
        /// </summary>
        /// <param name="text">The string to be checked for plural.</param>
        /// <returns>true, if the passed string is a plural string. Otherwise false. 
        /// If the passed string is one of the default invariant string list, it will return true.</returns>
        public static bool IsPlural(this string text)
        {
            return IsPlural(text, _invariants);
        }

        /// <summary>
        /// Determines if a string is in plural form based on some simple rules.
        /// </summary>
        /// <param name="text">The string to be checked for plural.</param>
        /// <returns>true, if the passed string is a plural string. Otherwise false. 
        /// If the passed string is one of the passed invariant string list, it will return true.</returns>
        public static bool IsPlural(this string text, List<string> invariants)
        {
            if (invariants != null && invariants.Contains(text)) return true;

            if (_pluralRegex1.IsMatch(text)
                || _pluralRegex2.IsMatch(text)
                || _pluralRegex3.IsMatch(text)
                || _pluralRegex4.IsMatch(text)
                )
                return true;

            return false;
        }

        #endregion IsPlural member

        #region ToPlural Members

        private static readonly Regex _singleRegex1 = new Regex("(?<keep>[^aeiou])y$", RegexOptions.Compiled);
        private static readonly Regex _singleRegex2 = new Regex("(?<keep>[aeiou]y)$", RegexOptions.Compiled);
        private static readonly Regex _singleRegex3 = new Regex("(?<keep>[sxzh])$", RegexOptions.Compiled);
        private static readonly Regex _singleRegex4 = new Regex("(?<keep>[^sxzhy])$", RegexOptions.Compiled);

        /// <summary>
        /// Converts a string to plural based on some simple rules.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPlural(this string text)
        {
            return ToPlural(text, _invariants);
        }

        public static string ToPlural(this string text, List<string> invariants)
        {
            // handle invariants
            if (invariants != null && invariants.Contains(text)) return text;
            if (!IsSingular(text)) return text;

            if (_singleRegex1.IsMatch(text))
                return _singleRegex1.Replace(text, "${keep}ies");
            else if (_singleRegex2.IsMatch(text))
                return _singleRegex2.Replace(text, "${keep}s");
            else if (_singleRegex3.IsMatch(text))
                return _singleRegex3.Replace(text, "${keep}es");
            else if (_singleRegex4.IsMatch(text))
                return _singleRegex4.Replace(text, "${keep}s");

            return text;
        }

        #endregion ToPlural Members

        #region IsSingular Members

        /// <summary>
        /// Determines if a string is in singular form based on some simple rules.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsSingular(this string text)
        {
            return IsSingular(text, _invariants);
        }

        public static bool IsSingular(this string text, List<string> invariants)
        {
            if (invariants != null && invariants.Contains(text)) return true;

            return !IsPlural(text);
        }

        #endregion IsSingular Members

        #region ToSingular Members

        /// <summary>
        /// Converts a string to singular based on some simple rules.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSingular(this string text)
        {
            return ToSingular(text, _invariants);
        }

        public static string ToSingular(this string text, List<string> invariants)
        {
            if (invariants != null && invariants.Contains(text)) return text;
            if (!IsPlural(text)) return text;

            if (_pluralRegex1.IsMatch(text))
                return _pluralRegex1.Replace(text, "${keep}y");
            else if (_pluralRegex2.IsMatch(text))
                return _pluralRegex2.Replace(text, "${keep}");
            else if (_pluralRegex3.IsMatch(text))
                return _pluralRegex3.Replace(text, "${keep}");
            else if (_pluralRegex4.IsMatch(text))
                return _pluralRegex4.Replace(text, "${keep}");

            return text;
        }

        #endregion ToSingular Members

        #region ToKeyValuePairs Members

        public static IEnumerable<Tuple<string, string>> ToKeyValuePairs(this string text, char separator = ';', char keyValueSeparator = '=')
        {
            if (!text.IsNullOrEmpty())
            {
                foreach (string keyValue in text.SplitNTrim(separator))
                {
                    if (keyValue.IsNullOrEmpty())
                        continue;

                    string[] keyValueTokens = keyValue.SplitNTrim(keyValueSeparator);
                    if (keyValueTokens != null && keyValueTokens.Length > 0)
                    {
                        string key = keyValueTokens[0];
                        string value = null;
                        if (keyValueTokens.Length > 1)
                            value = keyValueTokens[1];

                        yield return new Tuple<string, string>(key, value);
                    }
                }
            }
        }

        #endregion ToKeyValuePairs Members

        #region IndentXml Overloads

        public static string IndentXml(this string xml)
        {
            return IndentXml(xml, null);
        }

        public static string IndentXml(this string xml, NameTable nameTable)
        {
            if (xml.IsNullOrWhiteSpace())
                return xml;

            XmlDocument doc = nameTable == null ? new XmlDocument() : new XmlDocument(nameTable);

            try
            {
                doc.LoadXml(xml);
                return doc.ToIndentedXml();
            }
            catch (XmlException)
            {
                XmlDocumentFragment docFragment = doc.CreateDocumentFragment();
                docFragment.InnerXml = xml;
                return docFragment.ToIndentedXml();
            }

        }

        #endregion IndexXml Overloads

        #region ToObjectFromXml Overloads

        public static T ToObjectFromXml<T>(this string xml, XmlAttributeOverrides overrides = null)
        {
            return (T)ToObjectFromXml(xml, typeof(T), overrides);
        }

        public static object ToObjectFromXml(this string xml, Type type, XmlAttributeOverrides overrides = null)
        {
            if (xml.IsNullOrWhiteSpace())
                return null;
            if (type == null)
                throw new ArgumentNullException("Missing type.");

            using (StringReader reader = new StringReader(xml))
            {
                XmlSerializer serializer = overrides != null ? new XmlSerializer(type, overrides) : XmlSerializer.FromTypes(new[] { type }).GetNValue(0);
                return serializer.Deserialize(reader);
            }
        }

        public static object ToObjectFromXml(this string xml)
        {
            if (xml.IsNullOrWhiteSpace()) return null;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            Type objectType = Type.GetType(xmlDoc.DocumentElement.Attributes["_type_"].Value);
            if (objectType == null) return null;

            return xml.ToObjectFromXml(objectType);
        }

        #endregion ToObjectFromXml Overloads

        #region ToObjectFromJson Overloads

        //public static T ToObjectFromJson<T>(this string Json)
        //{
        //    return (T)ToObjectFromJson(Json, typeof(T));
        //}

        //public static object ToObjectFromJson(this string Json, Type type)
        //{
        //    if (Json.IsNullOrWhiteSpace())
        //        return null;
        //    if (type == null)
        //        throw new ArgumentNullException("Missing type.");

        //    using (StringReader reader = new StringReader(Json))
        //    {
        //        //JsonSerializer serializer = overrides != null ? new JsonSerializer(type, overrides) : JsonSerializer.FromTypes(new[] { type }).GetNValue(0);
        //        //return serializer.Deserialize(reader);
        //    }
        //}

        //public static object ToObjectFromJson(this string Json)
        //{
        //    if (Json.IsNullOrWhiteSpace()) return null;

        //    JsonDocument JsonDoc = new JsonDocument();
        //    JsonDoc.LoadJson(Json);
        //    Type objectType = Type.GetType(JsonDoc.DocumentElement.Attributes["_type_"].Value);
        //    if (objectType == null) return null;

        //    return Json.ToObjectFromJson(objectType);
        //}

        #endregion ToObjectFromJson Overloads

        #region Repeat Overloads

        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }

        #endregion Repeat Overloads

        #region Truncate Overloads

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) { return value; }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        #endregion Truncate Overloads

        #region ToSecureString Overloads

        public static SecureString ToSecureString(this string text)
        {
            ChoGuard.ArgumentNotNullOrEmpty(text, "Text");

            SecureString secureString = new SecureString();
            text.ToCharArray().ToList().ForEach(p => secureString.AppendChar(p));
            return secureString;
        }

        #endregion ToSecureString Overloads

        #region ToSafeSql Overloads

        public static string ToSafeSql(this string rawSql)
        {
            string cleanSql = String.Empty;
            int pos = 0;

            while (pos < rawSql.Length)
            {
                //** Double up single quotes, but only if they aren't already doubled ** 
                if (rawSql.Substring(pos, 1) == "'")
                {
                    cleanSql = cleanSql + "''";
                    if (pos != rawSql.Length)
                    {
                        if (rawSql.Substring(pos + 1, 1) == "'")
                            pos = pos + 1;
                    }
                }
                else
                {
                    cleanSql = cleanSql + rawSql.Substring(pos, 1);
                }
                pos++;
            }

            return cleanSql.Trim();
        }

        #endregion ToSafeSql Overloads

        #region CreateInstance Overloads

        public static T CreateInstance<T>(this string keyValueText)
        {
            if (keyValueText.IsNullOrWhiteSpace()) return default(T);

            Type objType = typeof(T);
            T obj = ChoActivator.CreateInstance<T>();
            obj.Load(keyValueText);
            return obj;
        }

        #endregion CreateInstance Overloads

        #region IndentXml Overloads

        //private static Lazy<XmlWriterSettings> xws = new Lazy<XmlWriterSettings>(() =>
        //    {
        //        XmlWriterSettings xws = new XmlWriterSettings();
        //        xws.Indent = true;
        //        xws.NewLineHandling = NewLineHandling.Replace;
        //        xws.NewLineChars = Environment.NewLine;
        //        xws.OmitXmlDeclaration = true;

        //        return xws;
        //    }, true);

        //public static string IndentXml(this string xml)
        //{
        //    if (xml.IsNullOrWhiteSpace()) return xml;

        //    StringBuilder sb = new StringBuilder();
        //    using (StringWriter sw = new StringWriter(sb))
        //    {
        //        using (XmlWriter xtw = XmlTextWriter.Create(sb, xws.Value))
        //        {
        //            xtw.WriteRaw(xml);
        //            xtw.Flush();
        //        }
        //    }

        //    return sb.ToString();
        //}

        #endregion IndentXml Overloads

        public static bool ContainsHeader(this string msg)
        {
            if (msg.IsNullOrEmpty())
                return false;

            return _headerRegex.IsMatch(msg);
        }

        /// <summary>
        /// Remove any non-word characters from a name (word characters are a-z, A-Z, 0-9, _)
        /// so that it may be used in code
        /// </summary>
        /// <param name="name">name to be cleaned</param>
        /// <returns>Cleaned up object name</returns>
        public static string GetCleanName(this string name)
        {
            return Regex.Replace(name, @"[\W]", "");
        }

        /// <summary>
        /// Return a tab
        /// </summary>
        public static string Spaces(int length)
        {
            if (length < 0)
                throw new ArgumentException("Length must be positive.");

            return new string(' ', length);
        }

        /// <summary>
        /// Return a tab
        /// </summary>
        public static string Tab()
        {
            return Tab(1);
        }

        /// <summary>
        /// Return a specified number of tabs
        /// </summary>
        /// <param name="n">Number of tabs</param>
        /// <returns>n tabs</returns>
        public static string Tab(int n)
        {
            return new String(ChoCharEx.HorizontalTab, n);
        }

        /// <summary>
        /// Return a newline
        /// </summary>
        public static string Newline()
        {
            return Newline(1);
        }

        /// <summary>
        /// Return a specified number of newlines
        /// </summary>
        /// <param name="n">Number of newlines</param>
        /// <returns>n tabs</returns>
        public static string Newline(int n)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        /// <summary>
        /// Return a newline and the specified number of tabs
        /// </summary>
        /// <param name="n">Number of tabs</param>
        /// <returns>newline with specified number of tabs</returns>
        public static string NewlineAndTabs(int n)
        {
            return (Newline() + Tab(n));
        }

        /// <summary>
        /// Checks the string for any characters present
        /// </summary>
        /// <param name="name">String to check</param>
        /// <returns>True if characters are present, otherwise false</returns>
        public static bool HasCharacters(this string name)
        {
            return Regex.IsMatch(name, @"[a-zA-Z]");
        }

        /// <summary>
        /// Checks the string for any numerics present
        /// </summary>
        /// <param name="name">String to check</param>
        /// <returns>True if numerics are present, otherwise false</returns>		
        public static bool HasNumerics(this string name)
        {
            return Regex.IsMatch(name, @"[0-9]");
        }

        /// <summary>
        /// Checks the string to see if it starts with a numeric
        /// </summary>
        /// <param name="name">String to check</param>
        /// <returns>True if string starts with a numeric, otherwise false</returns>
        public static bool StartsWithNumeric(this string name)
        {
            return Regex.IsMatch(name, @"^[0-9]+");
        }

        /// <summary>
        /// Checks the string to see if it starts with a character
        /// </summary>
        /// <param name="name">String to check</param>
        /// <returns>True if string starts with a character, otherwise false</returns>
        public static bool StartsWithCharacter(this string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+");
        }

        /// <summary>
        /// Checks the string to see if it is a valid variable name
        /// </summary>
        /// <param name="name">String to check</param>
        /// <returns>True if string is a valid variable name, otherwise false</returns>
        /// <remarks>Checks for (_ | {AlphaCharacter})({WordCharacter})*</remarks>
        public static bool IsValidVariableName(this string name)
        {
            return Regex.IsMatch(name, @"(_ | [a-zA-Z])([a-zA-Z_0-9])*");
        }

        #region WrapLongLines Overloads

        /// <summary>
        /// Wraps long lines at the specified column number breaking on the specified break
        /// character.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="columnNumber"></param>
        /// <param name="lineContinuationCharacter">the character for the language that indicates a line continuation</param>
        /// <param name="breakCharacter">The character that should be used for breaking the string</param>
        /// <returns>a wrapped line</returns>
        public static string WrapLongLines(this string text, int columnNumber)
        {
            return WrapLongLines(text, columnNumber, String.Empty, ' ', 4);
        }

        /// <summary>
        /// Wraps long lines at the specified column number breaking on the specified break
        /// character.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="columnNumber"></param>
        /// <param name="lineContinuationCharacter">the character for the language that indicates a line continuation</param>
        /// <param name="breakCharacter">The character that should be used for breaking the string</param>
        /// <returns>a wrapped line</returns>
        public static string WrapLongLines(this string text, int columnNumber, int tabs)
        {
            return WrapLongLines(text, columnNumber, String.Empty, ' ', tabs);
        }

        /// <summary>
        /// Wraps long lines at the specified column number breaking on the specified break
        /// character.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="columnNumber"></param>
        /// <param name="lineContinuationCharacter">the character for the language that indicates a line continuation</param>
        /// <param name="breakCharacter">The character that should be used for breaking the string</param>
        /// <returns>a wrapped line</returns>
        public static string WrapLongLines(this string text, int columnNumber, string lineContinuationCharacter, char breakCharacter)
        {
            return WrapLongLines(text, columnNumber, lineContinuationCharacter, breakCharacter, 4);
        }

        /// <summary>
        /// Wraps long lines at the specified column number breaking on the specified break
        /// character.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="columnNumber"></param>
        /// <param name="lineContinuationCharacter">the character for the language that indicates a line continuation</param>
        /// <param name="breakCharacter">The character that should be used for breaking the string</param>
        /// <param name="tabs">Number of tabs to indent the wrapped lines</param>
        /// <returns>a wrapped line</returns>
        public static string WrapLongLines(this string text, int columnNumber, string lineContinuationCharacter, char breakCharacter, int tabs)
        {
            if (String.IsNullOrEmpty(text)) return text;

            // if the line is less than column number just return it
            if (text.Length <= columnNumber)
                return text;

            StringBuilder result = new StringBuilder();
            int stringLength = text.Length;
            string subString;
            int startPosition = 0;

            // loop through ever column number characters
            while (startPosition < stringLength)
            {
                // check if the startPosition + columnNumber is greater than the stringLength
                if ((startPosition + columnNumber) > stringLength)
                {
                    // the substring is less than the columnNumber we're at the 
                    // last part so just add it and exit				
                    subString = text.Substring(startPosition);
                    result.Append(subString);
                    break;
                }
                // get the substring we're working with
                subString = text.Substring(startPosition, columnNumber);

                // not at the end so get the position of the last space
                int lastBreak = subString.LastIndexOf(breakCharacter);
                if (lastBreak < 0)
                    lastBreak = subString.Length - 1;

                lastBreak++;
                // check that we got one
                result.Append(subString.Substring(0, lastBreak));
                result.Append(lineContinuationCharacter);
                result.Append(Newline());
                result.Append(Tab(tabs));

                // set the next position
                startPosition += lastBreak;
            }

            return result.ToString();
        }

        #endregion WrapLongLines Overloads

        #region Contains Overloads (Public)

        public static bool Contains(char inChar, char[] findInChars)
        {
            foreach (char findInChar in findInChars)
            {
                if (findInChar == inChar) return true;
            }
            return false;
        }

        public static bool Contains(string text, int index, char[] findInChars)
        {
            char inChar = text[index];
            foreach (char findInChar in findInChars)
            {
                if (findInChar == inChar) return true;
            }
            return false;
        }

        public static bool Contains(string text, int index, string findInText)
        {
            index = index - (findInText.Length - 1);
            if (index < 0) return false;

            return text.IndexOf(findInText, index) == index;
        }

        #endregion Contains Overloads (Public)

        #region Other Members (Private)

        private static bool Contains(string text, int index, object findInChars)
        {
            if (findInChars is char[])
                return Contains(text, index, ((char[])findInChars));
            else if (findInChars is string)
                return Contains(text, index, ((string)findInChars));
            else
                return false;
        }

        private static string NormalizeString(string inString)
        {
            if (inString == null || inString.Length == 0) return inString;
            if (inString.Contains("\"\""))
                return inString.Replace("\"\"", "\"");
            //else if (inString.Contains("''"))
            //    return inString.Replace("''", "'");
            else
                return inString;
        }

        #endregion Contains Member (Private)

        #region ContainsMultiLines Overloads

        public static bool ContainsMultiLines(this string inString)
        {
            if (inString.IsNullOrEmpty())
                return false;

            return inString.IndexOf(Environment.NewLine) != inString.LastIndexOf(Environment.NewLine);
        }

        #endregion ContainsMultiLines Overloads

        #region ToEnum Overloads

        /// <summary>
        /// Convert a description value to enum value
        /// </summary>
        /// <typeparam name="T">The type of enum to considered for the conversion</typeparam>
        /// <param name="description">Description value to look into the enum values decorated with DescriptionAttribute.</param>
        /// <returns>Returns enum value correponding to the description if there is a match, otherwise returns Enum.Nothing</returns>
        public static T ToEnum<T>(this string description) where T : struct
        {
            return ChoEnumTypeDescCache.GetEnumValue<T>(description);
        }

        #endregion ToEnum Overloads

        #region NTrim Method

        public static string NTrim(this string text)
        {
            return text == null ? null : text.Trim();
        }

        #endregion NTrim Method

        #region ToByteArray Method

        public static byte[] ToByteArray(this string text)
        {
            if (text.IsNullOrEmpty())
                throw new ArgumentException("Text");

            byte[] byteArray = new byte[text.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                byteArray[j++] = byte.Parse(text.Substring(i, 3));
                i += 3;
            }
            while (i < text.Length);

            return byteArray;
        }

        #endregion ToByteArray Method

        #region Compare Overloads

        //
        // Summary:
        //     Compares two specified System.String objects.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   strB:
        //     The second System.String.
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.Value Condition Less than zero strA is less than strB. Zero strA
        //     equals strB. Greater than zero strA is greater than strB.
        public static int Compare(this string strA, string strB)
        {
            return String.Compare(strA, strB);
        }

        //
        // Summary:
        //     Compares two specified System.String objects, ignoring or honoring their
        //     case.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   strB:
        //     The second System.String.
        //
        //   ignoreCase:
        //     A System.Boolean indicating a case-sensitive or insensitive comparison. (true
        //     indicates a case-insensitive comparison.)
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.Value Condition Less than zero strA is less than strB. Zero strA
        //     equals strB. Greater than zero strA is greater than strB.
        public static int Compare(string strA, string strB, bool ignoreCase)
        {
            return String.Compare(strA, strB, ignoreCase);
        }

        //
        // Summary:
        //     Compares two specified System.String objects. A parameter specifies whether
        //     the comparison uses the current or invariant culture, honors or ignores case,
        //     and uses word or ordinal sort rules.
        //
        // Parameters:
        //   strA:
        //     The first System.String object.
        //
        //   strB:
        //     The second System.String object.
        //
        //   comparisonType:
        //     One of the System.StringComparison values.
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.Value Condition Less than zero strA is less than strB. Zero strA
        //     equals strB. Greater than zero strA is greater than strB.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     comparisonType is not a System.StringComparison value.
        //
        //   System.NotSupportedException:
        //     System.StringComparison is not supported.
        public static int Compare(string strA, string strB, StringComparison comparisonType)
        {
            return String.Compare(strA, strB, comparisonType);
        }

        //
        // Summary:
        //     Compares two specified System.String objects, ignoring or honoring their
        //     case, and using culture-specific information to influence the comparison.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   strB:
        //     The second System.String.
        //
        //   ignoreCase:
        //     A System.Boolean indicating a case-sensitive or insensitive comparison. (true
        //     indicates a case-insensitive comparison.)
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.Value Condition Less than zero strA is less than strB. Zero strA
        //     equals strB. Greater than zero strA is greater than strB.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     culture is null.
        public static int Compare(string strA, string strB, bool ignoreCase, CultureInfo culture)
        {
            return String.Compare(strA, strB, ignoreCase, culture);
        }

        public static int Compare(string strA, string strB, CultureInfo culture, CompareOptions options)
        {
            return String.Compare(strA, strB, culture, options);
        }

        //
        // Summary:
        //     Compares substrings of two specified System.String objects.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   indexA:
        //     The position of the substring within strA.
        //
        //   strB:
        //     The second System.String.
        //
        //   indexB:
        //     The position of the substring within strB.
        //
        //   length:
        //     The maximum number of characters in the substrings to compare.
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.Value Condition Less than zero The substring in strA is less than
        //     the substring in strB. Zero The substrings are equal, or length is zero.
        //     Greater than zero The substring in strA is greater than the substring in
        //     strB.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     indexA is greater than strA.System.String.Length.-or- indexB is greater than
        //     strB.System.String.Length.-or- indexA, indexB, or length is negative. -or-Either
        //     indexA or indexB is null, and length is greater than zero.
        public static int Compare(string strA, int indexA, string strB, int indexB, int length)
        {
            return String.Compare(strA, indexA, strB, indexB, length);
        }

        //
        // Summary:
        //     Compares substrings of two specified System.String objects, ignoring or honoring
        //     their case.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   indexA:
        //     The position of the substring within strA.
        //
        //   strB:
        //     The second System.String.
        //
        //   indexB:
        //     The position of the substring within strB.
        //
        //   length:
        //     The maximum number of characters in the substrings to compare.
        //
        //   ignoreCase:
        //     A System.Boolean indicating a case-sensitive or insensitive comparison. (true
        //     indicates a case-insensitive comparison.)
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.ValueCondition Less than zero The substring in strA is less than
        //     the substring in strB. Zero The substrings are equal, or length is zero.
        //     Greater than zero The substring in strA is greater than the substring in
        //     strB.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     indexA is greater than strA.System.String.Length.-or- indexB is greater than
        //     strB.System.String.Length.-or- indexA, indexB, or length is negative. -or-Either
        //     indexA or indexB is null, and length is greater than zero.
        public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase)
        {
            return String.Compare(strA, indexA, strB, indexB, length, ignoreCase);
        }

        //
        // Summary:
        //     Compares substrings of two specified System.String objects.
        //
        // Parameters:
        //   strA:
        //     The first System.String object.
        //
        //   indexA:
        //     The position of the substring within strA.
        //
        //   strB:
        //     The second System.String object.
        //
        //   indexB:
        //     The position of the substring within strB.
        //
        //   length:
        //     The maximum number of characters in the substrings to compare.
        //
        //   comparisonType:
        //     One of the System.StringComparison values.
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.Value Condition Less than zero The substring in the strA parameter
        //     is less than the substring in the strB parameter.Zero The substrings are
        //     equal, or the length parameter is zero. Greater than zero The substring in
        //     strA is greater than the substring in strB.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     indexA is greater than strA.System.String.Length.-or- indexB is greater than
        //     strB.System.String.Length.-or- indexA, indexB, or length is negative. -or-Either
        //     indexA or indexB is null, and length is greater than zero.
        //
        //   System.ArgumentException:
        //     comparisonType is not a System.StringComparison value.
        public static int Compare(string strA, int indexA, string strB, int indexB, int length, StringComparison comparisonType)
        {
            return String.Compare(strA, indexA, strB, indexB, length, comparisonType);
        }

        //
        // Summary:
        //     Compares substrings of two specified System.String objects, ignoring or honoring
        //     their case, and using culture-specific information to influence the comparison.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   indexA:
        //     The position of the substring within strA.
        //
        //   strB:
        //     The second System.String.
        //
        //   indexB:
        //     The position of the substring within the strB.
        //
        //   length:
        //     The maximum number of characters in the substrings to compare.
        //
        //   ignoreCase:
        //     A System.Boolean indicating a case-sensitive or insensitive comparison. (true
        //     indicates a case-insensitive comparison.)
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        // Returns:
        //     An integer indicating the lexical relationship between the two comparands.Value
        //     Condition Less than zero The substring in strA is less than the substring
        //     in strB. Zero The substrings are equal, or length is zero. Greater than zero
        //     The substring in strA is greater than the substring in strB.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     indexA is greater than strA.System.String.Length.-or- indexB is greater than
        //     strB.System.String.Length.-or- indexA, indexB, or length is negative. -or-Either
        //     indexA or indexB is null, and length is greater than zero.
        //
        //   System.ArgumentNullException:
        //     culture is null.
        public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase, CultureInfo culture)
        {
            return String.Compare(strA, indexA, strB, indexB, length, ignoreCase, culture);
        }

        #endregion Compare Overloads

        #region CompareOrdinal Overloads

        //
        // Summary:
        //     Compares two specified System.String objects by evaluating the numeric values
        //     of the corresponding System.Char objects in each string.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   strB:
        //     The second System.String.
        //
        // Returns:
        //     An integer indicating the lexical relationship between the two comparands.ValueCondition
        //     Less than zero strA is less than strB. Zero strA and strB are equal. Greater
        //     than zero strA is greater than strB.
        public static int CompareOrdinal(string strA, string strB)
        {
            return CompareOrdinal(strA, strB);
        }

        //
        // Summary:
        //     Compares substrings of two specified System.String objects by evaluating
        //     the numeric values of the corresponding System.Char objects in each substring.
        //
        // Parameters:
        //   strA:
        //     The first System.String.
        //
        //   indexA:
        //     The starting index of the substring in strA.
        //
        //   strB:
        //     The second System.String.
        //
        //   indexB:
        //     The starting index of the substring in strB.
        //
        //   length:
        //     The maximum number of characters in the substrings to compare.
        //
        // Returns:
        //     A 32-bit signed integer indicating the lexical relationship between the two
        //     comparands.ValueCondition Less than zero The substring in strA is less than
        //     the substring in strB. Zero The substrings are equal, or length is zero.
        //     Greater than zero The substring in strA is greater than the substring in
        //     strB.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     indexA is greater than strA. System.String.Length.-or- indexB is greater
        //     than strB. System.String.Length.-or- indexA, indexB, or length is negative.
        public static int CompareOrdinal(string strA, int indexA, string strB, int indexB, int length)
        {
            return CompareOrdinal(strA, indexA, strB, indexB, length);
        }

        #endregion CompareOrdinal Overloads

        #region Expression Evaluator Methods

        public static object Evaluate(this string text)
        {
            return Evaluate(text, null);
        }

        public static string ExpandProperties(this string text)
        {
            return ChoString.ExpandProperties(text);
        }

        public static object Evaluate(this string text, object state)
        {
            string msg = ChoString.ExpandProperties(state, text);
            if (ChoString.ContainsException(msg))
                throw new ChoExpressionParseException(msg);
            else
                return ChoString.ToObject(msg); // ChoObject.ConvertToObject(msg);
        }

        public static string ExpandProperties(this string text, object state)
        {
            return ChoString.ExpandProperties(state, text);
        }

        public static string SplitNExpandProperties(this string text, object state)
        {
            object ctx = state;
            foreach (string prop in text.SplitNTrim(';'))
            {
                ctx = ChoString.ExpandProperties(ctx, prop);
            }
            return ctx.ToNString();
        }

        #endregion Expression Evaluator Methods

        #region ToObject Method

        public static object ToObject(this string text)
        {
            return ChoString.ToObject(text);
        }

        #endregion ToObject Method

        public static IEnumerable<string> NSplit(this string inString, char delimiter, bool removeSeperator = true)
        {
            return NSplit(inString, delimiter, delimiter, removeSeperator);
        }

        public static IEnumerable<string> NSplit(this string inString, char startSeparator, char endSeparator, bool removeSeperator = true)
        {
            if (inString.IsNullOrEmpty())
                yield break;

            if (startSeparator == ChoChar.NUL)
                startSeparator = ',';

            if (endSeparator == ChoChar.NUL)
                endSeparator = startSeparator;

            string msg = inString;
            if (inString.IndexOf(startSeparator) != -1)
            {
                int index = -1;
                bool hasChoChar = false;
                StringBuilder message = new StringBuilder();
                StringBuilder token = new StringBuilder();
                while (++index < inString.Length)
                {
                    if (!hasChoChar && inString[index] == ChoChar.Backslash /*startSeparator*/
                        && index + 1 < inString.Length && inString[index + 1] == startSeparator)
                    {
                        index++;
                        message.Append(inString[index]);
                        //continue;
                        hasChoChar = true;
                    }
                    else if (inString[index] == startSeparator)
                    {
                        if (hasChoChar)
                        {
                            bool hadEndChoChar = false;
                            do
                            {
                                if (inString[index] == endSeparator && inString[index - 1] == ChoChar.Backslash /*endSeparator*/)
                                {
                                    if (!hadEndChoChar)
                                    {
                                        hadEndChoChar = true;
                                        message.Remove(message.Length - 1, 1);
                                        message.Append(inString[index]);
                                    }
                                    else
                                        message.Append(inString[index]);

                                    continue;
                                }
                                message.Append(inString[index]);
                            }
                            while (++index < inString.Length && inString[index] != startSeparator);

                            index--;
                            hasChoChar = false;
                        }
                        else
                        {
                            token.Remove(0, token.Length);
                            index++;
                            do
                            {
                                if (!hasChoChar && inString[index] == ChoChar.Backslash /*endSeparator*/
                                    && index + 1 < inString.Length && inString[index + 1] == endSeparator)
                                {
                                    hasChoChar = true;
                                }
                                else if (inString[index] == endSeparator)
                                {
                                    if (hasChoChar)
                                    {
                                        message.Append(startSeparator);
                                        message.Append(token);
                                        message.Append(inString[index]);
                                        bool hadEndChoChar = false;
                                        do
                                        {
                                            if (inString[index] == endSeparator && inString[index - 1] == ChoChar.Backslash /*endSeparator*/)
                                            {
                                                if (!hadEndChoChar)
                                                    hadEndChoChar = true;
                                                else
                                                    message.Append(inString[index]);

                                                continue;
                                            }
                                            message.Append(inString[index]);
                                        }
                                        while (++index < inString.Length && inString[index] == endSeparator);
                                    }
                                    else
                                    {
                                        if (message.Length > 0)
                                        {
                                            yield return message.ToString();
                                            message.Clear();
                                        }
                                        if (token.Length > 0)
                                        {
                                            if (removeSeperator)
                                                yield return token.ToString();
                                            else
                                                yield return startSeparator + token.ToString() + endSeparator;
                                            token.Clear();
                                        }
                                        else
                                        {
                                            if (removeSeperator)
                                                yield return String.Empty;
                                            else
                                                yield return startSeparator.ToString() + endSeparator.ToString();
                                        }
                                    }

                                    break;
                                }
                                else
                                    token.Append(inString[index]);
                            }
                            while (++index < inString.Length);
                        }
                    }
                    else
                        message.Append(inString[index]);
                }
                yield return message.ToString();
            }
            else
                yield return msg;
        }

        #region Replace Method

        public static string SearchNReplace(this string text, string searchPattern, string replacePattern)
        {
            if (text.IsNullOrEmpty()) return text;

            ChoGuard.ArgumentNotNullOrEmpty(searchPattern, "SearchPattern");
            ChoGuard.ArgumentNotNullOrEmpty(replacePattern, "ReplacePattern");

            if (ChoWildcard.IsWildcardPattern(searchPattern))
            {
                searchPattern = ChoWildcard.WildcardToRegex(searchPattern);
                Match match = Regex.Match(text, searchPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    int index = 0;
                    int counter = 0;
                    StringBuilder output = new StringBuilder();

                    char ch;
                    bool backspaceFound = false;
                    replacePattern = Regex.Escape(replacePattern);
                    while (index < replacePattern.Length)
                    {
                        ch = replacePattern[index];
                        if (ch == '\\')
                        {
                            if (index + 1 < replacePattern.Length &&
                                (replacePattern[index + 1] == '*' || replacePattern[index + 1] == '?')
                                )
                            {
                                if (index + 3 < replacePattern.Length && replacePattern[index + 1] == '*' &&
                                    replacePattern[index + 2] == '\\' && replacePattern[index + 3] == '*')
                                {
                                    output.Append('*');
                                    index = index + 4;
                                    continue;
                                }

                                ++counter;

                                string groupName = GetMatchingGroupName(match, replacePattern[index + 1] == '*' ? 'M' : 'S', ref counter);
                                if (groupName != null)
                                {
                                    output.AppendFormat(match.Groups[groupName].ToString());
                                }
                                else
                                    output.Append(replacePattern[index + 1]);

                                index++;
                            }
                            //else if (replacePattern[index + 1] == '\\')
                            //    output.Append(ch);
                            else if (index + 2 < replacePattern.Length
                                && replacePattern[index + 1] == '\\'
                                && replacePattern[index + 2] == '\\'
                                )
                            {
                                index++;
                                index++;
                                if (backspaceFound)
                                {
                                    output.Append(ch);
                                    backspaceFound = false;
                                }
                                else
                                    backspaceFound = true;

                                continue;
                            }
                            else if (index + 1 < replacePattern.Length && replacePattern[index + 1] == '\\')
                            {
                                output.Append(replacePattern[index + 1]);
                            }
                        }
                        else
                            output.Append(ch);

                        index++;
                        backspaceFound = false;
                    }

                    return output.ToString();
                    //replacePattern = ChoWildcardReplace.WildcardToRegex(replacePattern);
                    //return Regex.Replace(text, searchPattern, ChoWildcardReplace.WildcardToRegex(replacePattern), regexOptions);
                }
                else
                    return text;
            }
            else
                return text.Replace(searchPattern, replacePattern);
        }

        private static string GetMatchingGroupName(Match match, char wildcardChar, ref int counter)
        {
            string groupName = null;
            int lCounter = counter;

            while (lCounter <= match.Groups.Count)
            {
                groupName = String.Format("{0}{1}", wildcardChar, lCounter);
                if (match.Groups[groupName].Success)
                {
                    counter = lCounter;
                    return groupName;
                }

                lCounter++;
            }

            return groupName;
        }

        public static string ReplaceAt(this string input, int index, char newChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            StringBuilder builder = new StringBuilder(input);
            builder[index] = newChar;
            return builder.ToString();
        }

        #endregion Replace Method

        #region ContainsXml Methods

        public static bool ContainsXml(this string input)
        {
            if (input.IsNullOrWhiteSpace()) return false;

            try
            {
                XElement x = XElement.Parse("<wrapper>" + input + "</wrapper>");
                return !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
            }
            catch (XmlException)
            {
                return true;
            }
        }

        #endregion ContainsHTML Methods

        #region Unwrap Methods

        public static string Unwrap(this string input)
        {
            if (input.IsNull()) return input;
            if (input.StartsWith("\"") && input.EndsWith("\""))
                return input.Substring(1, input.Length - 2).Replace('^', ' ');
            if (input.StartsWith("'") && input.EndsWith("'"))
                return input.Substring(1, input.Length - 2).Replace('^', ' ');

            return input.Replace('^', ' ');
        }

        #endregion Unwrap Methods

        #region Fixed Length String Overloads

        public static string LeftJustifiedWithFilled(this string value, int length, char fillChar = ' ')
        {
            if (String.IsNullOrEmpty(value))
                return new String(fillChar, length);

            return value.PadRight(length).Left(length);
        }

        public static string RightJustifiedFilled(this string value, int length, char fillChar = ' ')
        {
            if (String.IsNullOrEmpty(value))
                return new String(fillChar, length);

            return value.PadLeft(length).Right(length);
        }

        public static string Right(this string value, int maxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(value))
            {
                //Set valid empty string as string could be null
                value = string.Empty;
            }
            else if (value.Length > maxLength)
            {
                //Make the string no longer than the max length
                value = value.Substring(value.Length - maxLength, maxLength);
            }

            //Return the string
            return value;
        }

        public static string Left(this string value, int maxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(value))
            {
                //Set valid empty string as string could be null
                value = string.Empty;
            }
            else if (value.Length > maxLength)
            {
                //Make the string no longer than the max length
                value = value.Substring(0, maxLength);
            }

            //Return the string
            return value;
        }

        #endregion Fixed Length String Overloads

        #region Take Overloads

        /// Like linq take - takes the first x characters
        public static string Take(this string theString, int count, bool ellipsis = false)
        {
            int lengthToTake = Math.Min(count, theString.Length);
            var cutDownString = theString.Substring(0, lengthToTake);

            if (ellipsis && lengthToTake < theString.Length)
                cutDownString += "...";

            return cutDownString;
        }

        #endregion Take Overloads

        #region Skip Overloads

        //like linq skip - skips the first x characters and returns the remaining string
        public static string Skip(this string theString, int count)
        {
            int startIndex = Math.Min(count, theString.Length);
            var cutDownString = theString.Substring(startIndex - 1);

            return cutDownString;
        }

        #endregion Skip Overloads

        #region Right and Left Overloads

        public static string Right(this string @this, char value)
        {
            if (@this.IsNullOrWhiteSpace()) return @this;
            int index = @this.IndexOf(value);
            if (index < 0) return @this;
            return @this.Substring(index + 1);
        }

        public static string Right(this string @this, char[] anyOf)
        {
            if (@this.IsNullOrWhiteSpace()) return @this;
            int index = @this.IndexOfAny(anyOf);
            if (index < 0) return @this;
            return @this.Substring(index + 1);
        }

        public static string Right(this string @this, string value, StringComparison comparision = StringComparison.CurrentCulture)
        {
            if (@this.IsNullOrWhiteSpace()) return @this;
            int index = @this.IndexOf(value, comparision);
            if (index < 0) return @this;
            return @this.Substring(index + 1);
        }

        public static string Left(this string @this, char value)
        {
            if (@this.IsNullOrWhiteSpace()) return @this;
            int index = @this.IndexOf(value);
            if (index < 0) return @this;
            return @this.Substring(0, index);
        }

        public static string Left(this string @this, char[] anyOf)
        {
            if (@this.IsNullOrWhiteSpace()) return @this;
            int index = @this.IndexOfAny(anyOf);
            if (index < 0) return @this;
            return @this.Substring(0, index);
        }

        public static string Left(this string @this, string value, StringComparison comparision = StringComparison.CurrentCulture)
        {
            if (@this.IsNullOrWhiteSpace()) return @this;
            int index = @this.IndexOf(value, comparision);
            if (index < 0) return @this;
            return @this.Substring(0, index);
        }

        #endregion

        public static string LastLine(this string text, string newLineChars = null)
        {
            if (text.IsNull()) return text;
            if (newLineChars.IsNull())
                newLineChars = Environment.NewLine;

            string[] lines = text.SplitNTrim(newLineChars);

            text = null;
            foreach (string line in lines)
            {
                if (line.IsNullOrWhiteSpace()) continue;
                text = line;
            }
            return text;
        }

        public static string EscapeXml(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return !SecurityElement.IsValidText(s)
                   ? SecurityElement.Escape(s) : s;
        }

        public static string UnescapeXml(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&amp;", "&");

            return returnString;
        }

        public static string EscapeSourceCode(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = _openBraceRegex.Replace(returnString, "$1\\{");
            returnString = _closeBraceRegex.Replace(returnString, "$1\\}");
            //returnString = _percentageRegex.Replace(returnString, "$1\\%");
            //returnString = _tiltaRegex.Replace(returnString, "$1\\~");
            //returnString = _caretRegex.Replace(returnString, "$1\\^");

            return returnString;
        }

        public static string UnescapeSourceCode(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = returnString.Replace("\\{", "{");
            returnString = returnString.Replace("\\}", "}");
            //returnString = returnString.Replace("\\%", "%");
            //returnString = returnString.Replace("\\~", "~");
            //returnString = returnString.Replace("\\^", "^");

            return returnString;
        }
    }
}
