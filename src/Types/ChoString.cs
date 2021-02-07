namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;
    using Cinchoo.Core;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    #region Delegates

    public delegate string ChoCustomPropertyReplaceHandler(object target, string msg);

    #endregion Delegates

    [DebuggerDisplay("{ToString()}")]
    [ChoStringObjectFormattable(typeof(string))]
    public class ChoString : IChoStringObjectFormatter<string>
    {
        #region Constants

        internal const string ExceptionStringToken = "[Exception:";

        #endregion Constants

        #region Constants

        public const string Empty = "";
        public const string EmptyString = "[EMPTY]";
        
        private const char StartSeparator = '%';
        private const char EndSeparator = '%';
        private const char FormatSeparator = '^';

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Regex _regEx = new Regex(String.Format(@"^{0}(?<value>.*){0}", ChoCharEx.DoubleQuotationMark), RegexOptions.Compiled);

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private string _value;

        #endregion Instance Data Members (Private)
        
        #region Constructors
        
        public ChoString()
        {
        }

        public ChoString(string formattedValue)
        {
            if (formattedValue == null || formattedValue.Length == 0)
                throw new ArgumentNullException("formattedValue");

            if (!TryParse(formattedValue, out _value))
                _value = formattedValue;
        }

        #endregion Constructors

        #region Object Overrides

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion Object Overrides

        #region Instance Memebrs (Public)

        public string ToFormattedString()
        {
            return String.Format("{0}{1}{0}", ChoCharEx.DoubleQuotationMark, _value);
        }

        public string Value
        {
            get { return _value; }
        }

        public string GetHelpText()
        {
            ChoString x = new ChoString("Test");
            return x.ToFormattedString();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is String;
        }

        [ChoObjectFormatter]
        public static string FormatObject(object value, string format)
        {
            if (value == null) return null;
            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            return value.ToString();
        }

        [ChoIsStringToObjectConvertable]
        public static bool IsString(string value)
        {
            if (value == null || value.Length == 0) return true;

            return _regEx.IsMatch(value);
        }

        public static bool TryParse(string value, out string output)
        {
            output = null;

            if (value == null || value.Length == 0) return true;

            if (IsString(value))
            {
                output = GetValue(value);
                return true;
            }
            else
                return false;
        }

        [ChoStringToObjectConverter]
        public static object Parse(string value)
        {
            string output;
            if (TryParse(value, out output))
                return output;
            else
                throw new FormatException(String.Format("Invalid {0} string value passed.", value));
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static string GetValue(string value)
        {
            Match match = _regEx.Match(value);
            if (match.Success)
                return match.Groups["value"].ToString();
            else
                return String.Empty;
        }

        #endregion Shared Members (Private)

        #region Operators Overloading

        public static implicit operator string(ChoString stringObject)
        {
            return stringObject == null ? null : stringObject.Value;
        }

        #endregion Operators Overloading

        #region Shared Members (Public)

        #region ToString Overloads (Public)

        public static string ToString(object value)
        {
            return ToString(value, ChoNull.NullString, ChoDbNull.DbNullString);
        }

        public static string ToString(object value, string nullValue, string dbNullValue)
        {
            return value == null ? nullValue : value == DBNull.Value ? dbNullValue : value.ToString();
        }

        #endregion ToString Overloads (Public)

        #region HandleNewLine Member (Public)

        public static string HandleNewLine(string msg)
        {
            return msg.Replace(ChoChar.LineFeed.ToString(), Environment.NewLine);
        }

        #endregion HandleNewLine Member (Public)

        #region Join Overloads (Public)

        public static string Join(object[] inValues)
        {
            return Join(inValues, null, ',');
        }

        public static string Join(object[] inValues, char separator)
        {
            return Join(inValues, null, separator);
        }

        public static string Join(object[] inValues, string defaultNullValue)
        {
            return Join(inValues, defaultNullValue, ',');
        }

        public static string Join(object[] inValues, string defaultNullValue, char separator)
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
                    outString.AppendFormat("{0}{1}", separator, convertedValue.ToString());
            }

            return outString.ToString();
        }

        #endregion Join Overloads

        #region Trim Overloads (Public)

        public static string[] Trim(string[] inValues)
        {
            if (inValues == null || inValues.Length == 0) return new string[] { };

            List<string> stringList = new List<string>();
            foreach (string inValue in inValues)
            {
                if (inValue == null)
                {
                    stringList.Add(inValue);
                    continue;
                }
                stringList.Add(inValue.Trim());
            }

            return stringList.ToArray();
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

        #endregion Trim Member (Public)

        //#region Contains Member (Public)

        //public static bool Contains(char inChar, char[] findInChars)
        //{
        //    foreach (char findInChar in findInChars)
        //    {
        //        if (findInChar == inChar) return true;
        //    }
        //    return false;
        //}

        //#endregion Contains Member (Public)

        #region Split2Objects Overloads (Public)

        public static object[] Split2Objects(string inString)
        {
            return Split2Objects(inString, new char[] { ',', ';' });
        }

        public static object[] Split2Objects(string inString, char separator)
        {
            return Split2Objects(inString, new char[] { separator });
        }

        public static object[] Split2Objects(string inString, char[] separators)
        {
            if (inString == null || inString.Trim().Length == 0) return new object[] { };

            ArrayList objectList = new ArrayList();

            foreach (string token in inString.SplitNTrim(separators))
                objectList.Add(ToObject(token));

            return objectList.ToArray() as object[];
        }

        #endregion Split2Objects Overloads (Public)

        #region String validation routines

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

        public static bool TryParseUrl(string url, ref string protocal, ref string serverName, ref int port)
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

        #region Equals Overloads (Public)

        // Methods
        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsNE(string s1, string s2)
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

        #endregion Equals Overloads (Public)

        #region StartWith Overloads (Public)

        public static bool StartsWith(string s1, string s2)
        {
            if (s2 == null)
            {
                return false;
            }
            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.Ordinal));
        }

        public static bool StartsWithIgnoreCase(string s1, string s2)
        {
            if (s2 == null)
            {
                return false;
            }
            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
        }

        #endregion StartWith Overloads (Public)

        #region ObjectArrayToStringArray Member

        public static string[] ObjectArrayToStringArray(object[] objectArray)
        {
            string[] array = new string[objectArray.Length];
            objectArray.CopyTo(array, 0);
            return array;
        }

        #endregion ObjectArrayToStringArray Member

        #region ExpandProperties Members (Public)

        public static string ExpandPropertiesEx(string inString)
        {
            return ExpandPropertiesEx(null, inString);
        }

        public static string ExpandPropertiesEx(object target, string inString)
        {
            string output = ExpandProperties(target, inString);

            if (ChoString.ContainsException(output))
                throw new ChoExpressionParseException(output);

            return output;
        }

        public static string ExpandProperties(string inString, char startSeparator, char endSeparator, char formatSeparator)
        {
            return ExpandProperties(null, inString, startSeparator, endSeparator, formatSeparator);
        }

        public static string ExpandProperties(string inString)
        {
            return ExpandProperties(null, inString);
        }

        public static string ExpandProperties(object target, string inString, char startSeparator, char endSeparator, char formatSeparator)
        {
            return ExpandProperties(target, inString, startSeparator, endSeparator, formatSeparator);
        }

        public static string ExpandProperties(object target, string inString)
        {
            return ExpandProperties(target, inString, StartSeparator, EndSeparator, FormatSeparator, ChoPropertyManagerSettings.Me.PropertyReplacers);
        }

        public static string ExpandProperties(string inString, char startSeparator, char endSeparator, char formatSeparator, IDictionary<string, object> properties)
        {
            return ExpandProperties(null, inString, startSeparator, endSeparator, formatSeparator, properties);
        }

        public static string ExpandProperties(object target, string inString, char startSeparator, char endSeparator, char formatSeparator, IDictionary<string, object> properties)
        {
            return ExpandProperties(target, inString, startSeparator, endSeparator, formatSeparator, new ChoDictionaryPropertyReplacer(properties));
        }

        //public static string ExpandProperties(string inString, char startSeparator, char endSeparator, char formatSeparator, ChoCustomPropertyReplaceHandler propertyReplaceHandler)
        //{
        //    return ExpandProperties(null, inString, startSeparator, endSeparator, formatSeparator, propertyReplaceHandler);
        //}

        //public static string ExpandProperties(object target, string inString, char startSeparator, char endSeparator, char formatSeparator, ChoCustomPropertyReplaceHandler propertyReplaceHandler)
        //{
        //    return ExpandProperties(target, inString, startSeparator, endSeparator, formatSeparator, new ChoCustomPropertyReplacer(propertyReplaceHandler));
        //}

        public static string ExpandProperties(string inString, IDictionary<string, object> properties)
        {
            return ExpandProperties(null, inString, properties);
        }

        public static string ExpandProperties(object target, string inString, IDictionary<string, object> properties)
        {
            return ExpandProperties(target, inString, StartSeparator, EndSeparator, FormatSeparator, new ChoDictionaryPropertyReplacer(properties));
        }

        public static string ExpandProperties(string inString, IChoKeyValuePropertyReplacer keyValuePropertyReplacer)
        {
            return ExpandProperties(null, inString, keyValuePropertyReplacer);
        }

        public static string ExpandProperties(object target, string inString, IChoKeyValuePropertyReplacer keyValuePropertyReplacer)
        {
            return ExpandProperties(target, inString, StartSeparator, EndSeparator, FormatSeparator, new ChoCustomKeyValuePropertyReplacer(keyValuePropertyReplacer));
        }

        //public static string ExpandProperties(string inString, ChoCustomPropertyReplaceHandler propertyReplaceHandler)
        //{
        //    return ExpandProperties(null, inString, propertyReplaceHandler);
        //}

        //public static string ExpandProperties(object target, string inString, ChoCustomPropertyReplaceHandler propertyReplaceHandler)
        //{
        //    return ExpandProperties(target, inString, StartSeparator, EndSeparator, FormatSeparator, new ChoCustomPropertyReplacer(propertyReplaceHandler));
        //}

        public static string ExpandProperties(string inString, IChoPropertyReplacer propertyReplacer)
        {
            return ExpandProperties(null, inString, propertyReplacer);
        }

        public static string ExpandProperties(object target, string inString, IChoPropertyReplacer propertyReplacer)
        {
            ChoGuard.ArgumentNotNull(propertyReplacer, "Property Replacer");

            return ExpandProperties(target, inString, new IChoPropertyReplacer[] { propertyReplacer });
        }

        public static string ExpandProperties(string inString, IChoPropertyReplacer[] propertyReplacers)
        {
            return ExpandProperties(null, inString, propertyReplacers);
        }

        public static string ExpandProperties(object target, string inString, IChoPropertyReplacer[] propertyReplacers)
        {
            return ExpandProperties(target, inString, StartSeparator, EndSeparator, FormatSeparator, propertyReplacers);
        }

        public static string ExpandProperties(string inString, char startSeparator, char endSeparator, char formatSeparator,
            IChoPropertyReplacer propertyReplacer)
        {
            return ExpandProperties(null, inString, startSeparator, endSeparator, formatSeparator, propertyReplacer);
        }

        public static string ExpandProperties(object target, string inString, char startSeparator, char endSeparator, char formatSeparator,
            IChoPropertyReplacer propertyReplacer)
        {
            ChoGuard.ArgumentNotNull(propertyReplacer, "Property Replacer");

            return ExpandProperties(target, inString, startSeparator, endSeparator, formatSeparator, new IChoPropertyReplacer[] { propertyReplacer });
        }

        public static string ExpandProperties(string inString, char startSeparator, char endSeparator, char formatSeparator,
            IChoPropertyReplacer[] propertyReplacers)
        {
            return ExpandProperties(null, inString, startSeparator, endSeparator, formatSeparator, propertyReplacers);
        }
        
        public static string ExpandProperties(object target, string inString, char startSeparator, char endSeparator, char formatSeparator,
            IChoPropertyReplacer[] propertyReplacers)
        {
            bool rec = false;
            int innerIndex = 0;

            return ExpandProperties(target, inString, startSeparator, endSeparator, formatSeparator, propertyReplacers,
                rec, ref innerIndex, false);
        }

        internal static string ExpandPropertiesInternal(object target, string inString, char startSeparator, char endSeparator, char formatSeparator,
            IChoPropertyReplacer[] propertyReplacers)
        {
            bool rec = false;
            int innerIndex = 0;

            return ExpandProperties(target, inString, startSeparator, endSeparator, formatSeparator, propertyReplacers,
                rec, ref innerIndex, true);
        }

        private static string ExpandProperties(object target, string inString, char startSeparator, char endSeparator, char formatSeparator,
            IChoPropertyReplacer[] propertyReplacers, bool rec, ref int innerIndex, bool internalCall)
        {
            if (inString.IsNullOrEmpty())
                return inString;

            ChoGuard.ArgumentNotNull(propertyReplacers, "Property Replacers");

            string msg = inString;
            if (inString.IndexOfEx(startSeparator) != -1
                || inString.IndexOfEx(endSeparator) != -1)
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
                            message.Append(ChoChar.Backslash);
                            message.Append(inString[index]);
                        }
                        continue;
                    }

                    if (hasEscapeChar) //if backslash char preceded by it, add and continue
                    {
                        hasEscapeChar = false;
                        message.Append(ChoChar.Backslash);
                        message.Append(inString[index]);
                        continue;
                    }

                    if (startSeparator == endSeparator)
                    {
                        #region Both Start and End Seperator are same

                        if (inString[index] == startSeparator)
                        {
                            if (hasEscapeChar) //if slash char found, just append the char
                            {
                                hasEscapeChar = false;
                                message.Append(ChoChar.Backslash);
                                message.Append(inString[index]);
                                continue;
                            }

                            if (index + 1 < inString.Length && inString[index + 1] == startSeparator)
                            {
                                index++;

                                bool hasInnerEscapeChar = false;
                                bool endCharFound = false;
                                StringBuilder token = new StringBuilder();
                                while (++index < inString.Length)
                                {
                                    endCharFound = false;

                                    if (inString[index] == ChoChar.Backslash)
                                    {
                                        if (!hasInnerEscapeChar) //if first slash char found, flag and ignore
                                        {
                                            hasInnerEscapeChar = true;
                                        }
                                        else  //if second slash char found, just append the char
                                        {
                                            hasInnerEscapeChar = false;
                                            message.Append(ChoChar.Backslash);
                                            token.Append(inString[index]);
                                        }
                                        continue;
                                    }

                                    if (hasInnerEscapeChar) //if backslash char preceded by it, add and continue
                                    {
                                        hasInnerEscapeChar = false;
                                        message.Append(ChoChar.Backslash);
                                        token.Append(inString[index]);
                                        continue;
                                    }

                                    if (inString[index] == endSeparator
                                        && (index + 1 < inString.Length && inString[index + 1] == endSeparator))
                                    {
                                        index++;

                                        endCharFound = true;
                                        if (token.Length > 0)
                                        {
                                            string[] propertyNameNFormat = token.ToString().SplitNTrimEx(formatSeparator).ToArray();

                                            StringBuilder message1 = new StringBuilder(message.ToString());
                                            if (!(propertyNameNFormat.Length >= 1 &&
                                                ReplaceToken(propertyReplacers, message1, propertyNameNFormat[0],
                                                    propertyNameNFormat.Length == 2 ? propertyNameNFormat[1] : null, target)))
                                            {
                                                message.AppendFormat("{0}{1}{2}", startSeparator, token, endSeparator);
                                            }
                                            else
                                            {
                                                message.Clear();
                                                message.Append(message1.ToString());
                                            }
                                            token.Remove(0, token.Length);
                                        }
                                        break;
                                    }
                                    else
                                        token.Append(inString[index]);
                                }
                                if (!endCharFound)
                                    message.AppendFormat("{0}{0}{1}", startSeparator, token);
                            }
                            else
                                message.Append(inString[index]);
                        }
                        else
                            message.Append(inString[index]);

                        #endregion Both Start and End Seperator are same
                    }
                    else
                    {
                        #region Start and End Seperators are different

                        if (inString[index] == startSeparator)
                        {
                            if (hasEscapeChar) //if slash char found, just append the char
                            {
                                hasEscapeChar = false;
                                message.Append(ChoChar.Backslash);
                                message.Append(inString[index]);
                                continue;
                            }

                            if (index + 1 < inString.Length && inString[index + 1] == startSeparator)
                            {
                                ++index;
                                ++index;
                                if (index < inString.Length)
                                {
                                    int inIndex = 0;
                                    string subStr = ExpandProperties(target, inString.Substring(index), startSeparator, endSeparator, formatSeparator, propertyReplacers, true, ref inIndex, internalCall);
                                    if (inIndex == 0)
                                    {
                                        message.AppendFormat("{0}{0}{1}", startSeparator, subStr);
                                        break;
                                    }
                                    else
                                    {
                                        message.Append(subStr);
                                        index += inIndex;
                                    }
                                    continue;
                                }
                            }
                            else
                                message.Append(inString[index]);
                        }
                        else if (inString[index] == endSeparator)
                        {
                            if (index + 1 < inString.Length && inString[index + 1] == endSeparator)
                            {
                                if (rec)
                                {
                                    index++;
                                    if (message.Length > 0)
                                    {
                                        string[] propertyNameNFormat = message.ToString().SplitNTrimEx(formatSeparator).ToArray();

                                        StringBuilder message1 = new StringBuilder();
                                        if (!(propertyNameNFormat.Length >= 1 &&
                                            ReplaceToken(propertyReplacers, message1, propertyNameNFormat[0],
                                                propertyNameNFormat.Length == 2 ? propertyNameNFormat[1] : null, target)))
                                        {
                                            string token = message.ToString();
                                            message.Remove(0, message.Length);
                                            message.AppendFormat("{0}{0}{1}{2}{2}", startSeparator, token, endSeparator);
                                        }
                                        else
                                        {
                                            message.Clear();
                                            message.Append(message1.ToString());
                                        }
                                    }
                                    innerIndex = index;
                                    return message.ToString();
                                }
                                else
                                {
                                    message.Append(inString[index]);
                                }
                            }
                            else
                                message.Append(inString[index]);
                        }
                        else
                            message.Append(inString[index]);

                        #endregion Start and End Seperators are different
                    }
                }
                msg = message.ToString();
            }

            if (!internalCall)
            {
                foreach (IChoPropertyReplacer propertyReplacer in propertyReplacers)
                {
                    if (!(propertyReplacer is IChoCustomPropertyReplacer)) continue;

                    IChoCustomPropertyReplacer customPropertyReplacer = propertyReplacer as IChoCustomPropertyReplacer;
                    string formattedMsg = msg;
                    bool retVal = customPropertyReplacer.Format(target, ref formattedMsg);

                    if (!String.IsNullOrEmpty(formattedMsg))
                        msg = formattedMsg;

                    if (!retVal)
                        break;
                }
            }

            return msg;
        }

        #endregion ExpandProperties Member (Public)

        #region ToObject Member (Public)

        internal static object ToObjectInternal(string inString, Type targetType = null)
        {
            if (inString == null || inString.Trim().Length == 0) return null;

            if (targetType != null && ChoTypesManager.TypeObjectsParseInfo.ContainsKey(targetType))
            {
                ChoTypeObjectParseInfo typeObjectInfo1 = ChoTypesManager.TypeObjectsParseInfo[targetType];
                if (typeObjectInfo1 == null) return inString;
                if (typeObjectInfo1.CheckParse(inString))
                    return typeObjectInfo1.Parse(inString);
                else
                    return inString;
            }

            ChoTypeObjectParseInfo[] typeObjectsInfo = ChoTypesManager.GetTypeObjectsParseInfo();

            foreach (ChoTypeObjectParseInfo typeObjectInfo in typeObjectsInfo)
            {
                if (typeObjectInfo.CheckParse(inString))
                    return typeObjectInfo.Parse(inString);
            }

            return inString;
        }

        public static object ToObject(string inString, Type targetType = null)
        {
            if (inString == null || inString.Trim().Length == 0) return null;

            if (targetType != null && ChoTypesManager.TypeObjectsParseInfo.ContainsKey(targetType))
            {
                ChoTypeObjectParseInfo typeObjectInfo1 = ChoTypesManager.TypeObjectsParseInfo[targetType];
                if (typeObjectInfo1 == null) return inString;
                if (typeObjectInfo1.CheckParse(inString))
                    return typeObjectInfo1.Parse(inString);
                else
                    return inString;
            }

            ChoTypeObjectParseInfo[] typeObjectsInfo = ChoTypesManager.GetTypeObjectsParseInfo();

            foreach (ChoTypeObjectParseInfo typeObjectInfo in typeObjectsInfo)
            {
                if (typeObjectInfo.CheckParse(inString))
                    return typeObjectInfo.Parse(inString);
            }

            if (inString.IsInteger())
            {
                int intValue;

                if (Int32.TryParse(inString, out intValue))
                    return intValue;

                long longValue;
                if (Int64.TryParse(inString, out longValue))
                    return longValue;

                double doubleValue;
                if (Double.TryParse(inString, out doubleValue))
                    return doubleValue;

                return inString;
            }
            else if (inString.IsNumber())
                return double.Parse(inString);
            else
                return inString;
        }

        #endregion ToObject Member (Public)

        #region Evaluate Members (Public)

        public static object Evaluate(string msg)
        {
            return Evaluate(null, msg);
        }

        public static object Evaluate(object target, string msg)
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            if (target != null)
                state.Add(ChoExpressionEvaluator.THIS, target);

            return new ChoExpressionEvaluator(new Stack<string>(), state).Evaluate(msg);
        }

        #endregion Resolve Members (Public)

        #region IsAbbr Members (Public)

        /// <summary>
        /// Determines if a string is an abbr item
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAbbr(string value)
        {
            if (value == null || value.Length == 0)
                return false;

            if (ChoAttribute.GetMe<ChoConfigurationSectionAttribute>(typeof(ChoStringSettings)).ConfigElementPath == value
                || value == "Cinchoo")
                return true;

            if (ChoStringSettings.Me.Abbrs != null)
            {
                foreach (string abbr in ChoStringSettings.Me.Abbrs)
                {
                    if (value == abbr) return true;
                }
            }

            return false;
        }

        #endregion IsAbbr Members (Public)

        #region IsInvariant Members (Public)

        /// <summary>
        /// Determines if a string is an invariant item
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInvariant(string value)
        {
            // cycle thorough all of the invariants
            foreach (string s in ChoStringSettings.Me.Invariants)
            {
                if (value.ToLower().EndsWith(s.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion IsInvariant Members (Public)

        internal static bool ContainsException(string inString)
        {
            if (inString.IsNullOrEmpty())
                return false;

            return inString.StartsWith(ExceptionStringToken);
        }

        #endregion

        #region Shared Members (Private)

        private static bool ReplaceToken(IChoPropertyReplacer[] propertyReplacers, StringBuilder message,
            string propertyName, string format, object context)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                if (propertyName == "@this")
                {
                    if (context != null)
                        message.Append(ChoObject.Format(context, format));
                    return true;
                }
                foreach (IChoPropertyReplacer propertyReplacer in propertyReplacers)
                {
                    if (!(propertyReplacer is IChoKeyValuePropertyReplacer)) continue;

                    IChoKeyValuePropertyReplacer dictionaryPropertyReplacer = propertyReplacer as IChoKeyValuePropertyReplacer;
                    if (dictionaryPropertyReplacer == null || !dictionaryPropertyReplacer.ContainsProperty(propertyName, context)) continue;
                    //if (propertyName.Contains("@this"))
                    //{
                    //    if (context == null)
                    //        context = message.ToString();

                    //    message.Clear();
                    //}

                    message.Append(dictionaryPropertyReplacer.ReplaceProperty(propertyName, format, context));
                    return true;
                }
            }

            string propertyValue;
            bool retValue = ChoApplication.OnPropertyResolve(propertyName, format, context, out propertyValue);
            if (retValue)
            {
                if (propertyValue != null)
                    message.Append(propertyValue);
                return true;
            }

            //if (!format.IsNullOrWhiteSpace())
            //{
            //    message.Append(ChoObject.Format(propertyName, format));
            //    return true;
            //}
            return false;
        }

        #endregion Shared Members (Private)
    }
}
