#region NameSpaces

using System;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

//using eSquare.Core.Data;
using eSquare.Core.Diagnostics;

#endregion NameSpaces

namespace eSquare.Core
{
    public static class ChoString
    {
        public const string NullString = "[NULL]";
        public const string DbNullString = "[DB_NULL]";

        #region Shared Members (Public)

        public static string ToString(object value)
        {
            return ToString(value, NullString, DbNullString);
        }

        public static string ToString(object value, string nullValue, string dbNullValue)
        {
            return value == null ? nullValue : value == DBNull.Value ? dbNullValue : value.ToString();
        }

        public static string ToCamelCase(string token)
        {
            if (token == null || token.Trim().Length == 0)
                return null;

            if (token.Length == 1)
                return token.ToLower();
            else
            {
                if (token == null || token.Trim().Length == 0)
                    return null;

                if (token.Length == 1)
                    return token.ToLower();
                else
                {
                    foreach (string abbr in ChoAppSettings.Me.Abbrs)
                    {
                        if (token.StartsWith(abbr))
                        {
                            return abbr.ToLower() + token.Substring(abbr.Length);
                        }
                    }
                    Regex capsPattern = new Regex("^([A-Z]+)");
                    Match match = capsPattern.Match(token);
                    if (match != null && match.Success)
                        return match.Value.ToLower() + token.Substring(match.Value.Length);
                    return token;
                }
            }
        }

        public static string IndentMsg(int noOfTabs2Insert, string msg)
        {
            if (noOfTabs2Insert <= 0) return msg;

            string tabs = String.Empty;
            for (int index = 0; index < noOfTabs2Insert; index++)
                tabs = tabs + "\t";

            string pattern = String.Format(@".*[{0}]*", Environment.NewLine);

            StringBuilder formattedMsg = new StringBuilder();
            foreach (Match m in Regex.Matches(msg, pattern))
            {
                formattedMsg.AppendFormat("{0}{1}", tabs, m.ToString());
            }

            return formattedMsg.ToString();
        }

        public static string HandleNewLine(string msg)
        {
            return msg.Replace("\n", Environment.NewLine);
        }

        public static string Join(object[] inValues)
        {
            return Join(inValues, null, ',');
        }

        public static string Join(object[] inValues, char seperator)
        {
            return Join(inValues, null, seperator);
        }

        public static string Join(object[] inValues, string defaultNullValue)
        {
            return Join(inValues, defaultNullValue, ',');
        }

        public static string Join(object[] inValues, string defaultNullValue, char seperator)
        {
            if (inValues == null || inValues.Length == 0) return String.Empty;

            StringBuilder outString = new StringBuilder();
            foreach (object inValue in inValues)
            {
                object convertedValue = inValue;
                if (inValue == null || inValue == DBNull.Value)
                {
                    if (defaultNullValue == null)
                        continue;
                    else
                        convertedValue = defaultNullValue;
                }
                if (outString.Length == 0)
                    outString.Append(convertedValue.ToString());
                else
                    outString.AppendFormat("{0}{1}", seperator, convertedValue.ToString());
            }

            return outString.ToString();
        }

        public static string[] Trim(string[] inValues)
        {
            if (inValues == null || inValues.Length == 0) return new string[] { };

            ArrayList stringList = new ArrayList();
            foreach (string inValue in inValues)
            {
                if (inValue == null) continue;
                stringList.Add(inValue);
            }

            return stringList.ToArray(typeof(string)) as string[];
        }

        private static bool Contains(char inChar, char[] findInChars)
        {
            foreach (char findInChar in findInChars)
            {
                if (findInChar == inChar) return true;
            }
            return false;
        }

        public static string[] Split(string inString)
        {
            return Split(inString, ',');
        }

        public static string[] Split(string inString, char seperator)
        {
            return Split(inString, new char[] { seperator });
        }

        public static string[] Split(string inString, char[] seperators)
        {
            if (inString == null || inString.Length == 0) return new string[0];

            ArrayList splitStrings = new ArrayList();

            int quotes = 0;
            int singleQuotes = 0;
            int offset = 0;
            int i = 0;
            bool hasChoChar = false;
            while (i < inString.Length)
            {
                if (inString[i] == '\"') { quotes++; }
                else if (inString[i] == '\'' ) { singleQuotes++; }
                else if (inString[i] == '\\' && i + 1 < inString.Length && Contains(inString[++i], seperators))
                {
                    hasChoChar = true;
                }
                else if (Contains(inString[i], seperators) && (quotes % 2 == 0 || singleQuotes % 2 == 0))
                {
                    if (hasChoChar)
                    {
                        splitStrings.Add(NormalizeString(inString.Substring(offset, i - offset).Replace("\\", String.Empty)));
                        hasChoChar = false;
                    }
                    else
                        splitStrings.Add(NormalizeString(inString.Substring(offset, i - offset)));

                    offset = i + 1;
                }
                i++;
            }

            if (offset < inString.Length)
                splitStrings.Add(hasChoChar ? inString.Substring(offset).Replace("\\", String.Empty) : inString.Substring(offset));

            return splitStrings.ToArray(typeof(string)) as string[];
        }

        private static string NormalizeString(string inString)
        {
            if (inString == null || inString.Length == 0) return inString;
            if (inString.Contains("\"\""))
                return inString.Replace("\"\"", "\"");
            else if (inString.Contains("''"))
                return inString.Replace("''", "'");
            else
                return inString;
        }

        public static string[] SplitNTrim(string inString)
        {
            return SplitNTrim(inString, new char[] { ',' });
        }

        public static string[] SplitNTrim(string inString, char seperator)
        {
            return SplitNTrim(inString, new char[] { seperator });
        }

        public static string[] SplitNTrim(string inString, char[] seperators)
        {
            if (inString == null || inString.Trim().Length == 0) return new string[] { };

            ArrayList tokenList = new ArrayList();
            foreach (string token in Split(inString, seperators))
                tokenList.Add(token.Trim());

            return tokenList.ToArray(typeof(string)) as string[];
        }

        public static object[] Split2Objects(string inString)
        {
            return Split2Objects(inString, new char[] { ',' });
        }

        public static object[] Split2Objects(string inString, char seperator)
        {
            return Split2Objects(inString, new char[] { seperator });
        }

        public static string Trim(string inString, char trimChar)
        {
            return Trim(inString, trimChar.ToString());
        }

        public static string Trim(string inString, string trimChars)
        {
            if (inString == null || inString.Length == 0) return inString;

            if (inString.StartsWith(trimChars))
            {
                if (inString.EndsWith(trimChars))
                    return inString.Substring(trimChars.Length, inString.Length - (trimChars.Length * 2));
                else
                    return inString.Substring(trimChars.Length);
            }
            else
                return inString;
        }

        public static object[] Split2Objects(string inString, char[] seperators)
        {
            if (inString == null || inString.Trim().Length == 0) return new object[] { };

            ArrayList objectList = new ArrayList();
            foreach (string token in SplitNTrim(inString, seperators))
            {
                if (token == null)
                    objectList.Add(null);

                switch (token[0])
                {
                    case '\"':
                        objectList.Add(Trim(token, token[0]));
                        break;
                    case '\'':
                        objectList.Add(Convert.ToChar(Trim(token, token[0])));
                        break;
                    case '#':
                        string[] dateTimeStringParts = Trim(token, token[0]).Split('#');
                        if (dateTimeStringParts.Length == 1)
                            objectList.Add(DateTime.Parse(dateTimeStringParts[0]));
                        else
                            objectList.Add(DateTime.ParseExact(dateTimeStringParts[0], dateTimeStringParts[1], null));
                        break;
                    default:
                        if (token.ToUpper() == NullString)
                            objectList.Add(null);
                        if (token.ToUpper() == DbNullString)
                            objectList.Add(DBNull.Value);
                        else if (IsInteger(token))
                            objectList.Add(Convert.ToInt32(token));
                        else if (IsBoolean(token))
                            objectList.Add(Convert.ToBoolean(token));
                        else if (IsNumber(token))
                            objectList.Add(Convert.ToDouble(token));
                        else
                            objectList.Add(token);
                        break;
                }
            }

            return objectList.ToArray() as object[];
        }

        public static string[] SplitNDiscardEmptyLines(string inString, char[] seperators)
        {
            if (inString == null || inString.Trim().Length == 0) return new string[] { };

            ArrayList tokenList = new ArrayList();
            foreach (string token in Split(inString, seperators))
            {
                if (token.Trim().Length == 0) continue;
                tokenList.Add(token.Trim());
            }

            return tokenList.ToArray(typeof(string)) as string[];
        }

        #region string validation routines

        // Function to test for Positive Integers.  
        public static bool IsNaturalNumber(string number)
        {
            Regex notNaturalPattern = new Regex("[^0-9]");
            Regex naturalPattern = new Regex("0*[1-9][0-9]*");
            return !notNaturalPattern.IsMatch(number) &&
                naturalPattern.IsMatch(number);
        }

        // Function to test for Positive Integers with zero inclusive  
        public static bool IsWholeNumber(string number)
        {
            Regex notWholePattern = new Regex("[^0-9]");
            return !notWholePattern.IsMatch(number);
        }

        // Function to Test for Integers both Positive & Negative  
        public static bool IsInteger(string number)
        {
            Regex notIntPattern = new Regex("[^0-9-]");
            Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
            return !notIntPattern.IsMatch(number) && objIntPattern.IsMatch(number);
        }

        // Function to Test for Integers both Positive & Negative  
        public static bool IsBoolean(string boolean)
        {
            return boolean.Trim().ToLower() == "true" || boolean.Trim().ToLower() == "false";
        }

        // Function to Test for Positive Number both Integer & Real 
        public static bool IsPositiveNumber(string number)
        {
            Regex notPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !notPositivePattern.IsMatch(number) &&
                objPositivePattern.IsMatch(number) &&
                !objTwoDotPattern.IsMatch(number);
        }

        // Function to test whether the string is valid number or not
        public static bool IsNumber(string number)
        {
            Regex notNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            string strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            string strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex numberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
            return !notNumberPattern.IsMatch(number) &&
                !objTwoDotPattern.IsMatch(number) &&
                !objTwoMinusPattern.IsMatch(number) &&
                numberPattern.IsMatch(number);
        }

        // Function To test for Alphabets. 
        public static bool IsAlpha(string toCheck)
        {
            Regex alphaPattern = new Regex("[^a-zA-Z]");
            return !alphaPattern.IsMatch(toCheck);
        }

        // Function to Check for AlphaNumeric.
        public static bool IsAlphaNumeric(string toCheck)
        {
            Regex alphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !alphaNumericPattern.IsMatch(toCheck);
        }

        public static int StripNumber(string inString)
        {
            Regex numericPattern = new Regex("[0-9]+");
            Match match = numericPattern.Match(inString);
            double index = -1;
            if (match != null && match.Success)
            {
                Double.TryParse(match.Value, System.Globalization.NumberStyles.Number, System.Globalization.NumberFormatInfo.CurrentInfo, out index);
            }
            return (int)index;
        }

        public static bool ParseUrl(string url, ref string protocal, ref string serverName, ref int port)
        {
            Regex r = new Regex(@"^(?<proto>\w+)://(?<server>[^/]+?):(?<port>\d+)?/",
                RegexOptions.Compiled);

            Match match = r.Match(url);

            if (match.Success)
            {
                protocal = match.Result("${proto}");
                serverName = match.Result("${server}");
                port = Convert.ToInt16(match.Result("${port}"));
            }

            return match.Success;
        }

        #endregion string validation routines

        // Methods
        internal static bool EqualsIgnoreCase(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool EqualsNE(string s1, string s2)
        {
            if (s1 == null)
            {
                s1 = string.Empty;
            }
            if (s2 == null)
            {
                s2 = string.Empty;
            }
            return string.Equals(s1, s2, StringComparison.Ordinal);
        }

        internal static string[] ObjectArrayToStringArray(object[] objectArray)
        {
            string[] array = new string[objectArray.Length];
            objectArray.CopyTo(array, 0);
            return array;
        }

        internal static bool StartsWith(string s1, string s2)
        {
            if (s2 == null)
            {
                return false;
            }
            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.Ordinal));
        }

        internal static bool StartsWithIgnoreCase(string s1, string s2)
        {
            if (s2 == null)
            {
                return false;
            }
            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
