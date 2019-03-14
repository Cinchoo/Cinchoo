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
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    public static class ChoStringEx
    {
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
            return Indent(text, 1, ChoChar.HorizontalTab);
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
            return Indent(text, totalWidth, ChoChar.HorizontalTab);
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
            return Unindent(text, 1, ChoChar.HorizontalTab);
        }

        public static string Unindent(this String text, int totalWidth, char paddingChar)
        {
            if (text == null) return null;
            if (totalWidth == 0) return text;

            StringBuilder formattedMsg = new StringBuilder();
            for (int index = -1 * Math.Abs(totalWidth); index < 0; index++)
            {
                string linePattern = String.Format(@".*[{0}]*", Environment.NewLine);

                formattedMsg = new StringBuilder();
                foreach (Match m in Regex.Matches(text, linePattern))
                {
                    string pattern = String.Format(@"{1}(?<text>.*[{0}])", Environment.NewLine, paddingChar);

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
    }
}
