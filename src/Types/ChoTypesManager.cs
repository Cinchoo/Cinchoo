namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Linq;

    using Cinchoo.Core.Text;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    #region ChoTypeObjectParseInfo Struct

    internal class ChoTypeObjectParseInfo
    {
        public Func<string, bool> CheckParse;
        public Func<string, object> Parse;

        public bool IsValid()
        {
            return CheckParse != null && Parse != null;
        }
    }

    #endregion ChoTypeObjectParseInfo Struct

    #region ChoTypeObjectFormatInfo Struct

    internal struct ChoTypeObjectFormatInfo
    {
        public Func<object, bool> CanFormat;
        public Func<object, string, string> Format;

        public bool IsValid()
        {
            return CanFormat != null && Format != null;
        }
    }

    #endregion ChoTypeObjectFormatInfo Struct

    public class ChoTypesManager
    {
        #region Shared Data Members (Private)

        internal readonly static Dictionary<Type, ChoTypeObjectParseInfo> TypeObjectsParseInfo = new Dictionary<Type, ChoTypeObjectParseInfo>();
        internal readonly static Dictionary<Type, ChoTypeObjectFormatInfo> TypeObjectsFormatInfo = new Dictionary<Type, ChoTypeObjectFormatInfo>();
        private static ChoTypeObjectParseInfo[] _typeObjectsParseInfoArr = new ChoTypeObjectParseInfo[] {};
        private static ChoTypeObjectFormatInfo[] _typeObjectsFormatInfoArr = new ChoTypeObjectFormatInfo[] { };
        private static string _helpText;

        #endregion Shared Data Members (Private)

        #region Shared Constructor

        internal static void Initialize()
        {
			//ChoStreamProfile.Clean(ChoReservedDirectoryName.Others, ChoType.GetLogFileName(typeof(ChoTypesManager)));

            StringBuilder topMsg = new StringBuilder();

            ChoStringMsgBuilder parseMethodsMsg = new ChoStringMsgBuilder("Below are the loaded parse methods");
            ChoStringMsgBuilder formatMethodsMsg = new ChoStringMsgBuilder("Below are the loaded format methods");
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Below are the loaded type objects");
            foreach (Type type in ChoType.GetTypes(typeof(ChoStringObjectFormattableAttribute)))
            {
                if (ChoTypesManagerSettings.IsExcludedType(type)) continue;

                ChoTypeObjectParseInfo typeObjectParseInfo = new ChoTypeObjectParseInfo();
                ChoTypeObjectFormatInfo typeObjectFormatInfo = new ChoTypeObjectFormatInfo();
                try
                {
                    //typeObjectFormatInfo.TypeObject = typeObjectParseInfo.TypeObject = ChoObject.CreateInstance(type);
                    msg.AppendFormatLine(type.FullName);
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    msg.AppendFormatLine("{0}: [{1}]", type.FullName, ex.Message);
                }

                LoadObjectParser(parseMethodsMsg, type, typeObjectParseInfo);
                LoadObjectFormatter(formatMethodsMsg, type, typeObjectFormatInfo);
            }

            _typeObjectsParseInfoArr = TypeObjectsParseInfo.Values.ToArray();
            _typeObjectsFormatInfoArr = TypeObjectsFormatInfo.Values.ToArray();

            //_typeObjectsParseInfo.Clear();
            //_typeObjectsFormatInfo.Clear();

            topMsg.Append(parseMethodsMsg.ToString() + Environment.NewLine + Environment.NewLine);
            topMsg.Append(formatMethodsMsg.ToString() + Environment.NewLine + Environment.NewLine);
            topMsg.Append(msg.ToString() + Environment.NewLine + Environment.NewLine);
            _helpText = topMsg.ToString();
        }

        internal static string GetHelpText()
        {
            return _helpText;
        }

        private static void LoadObjectParser(ChoStringMsgBuilder parseMethodsMsg, Type type, ChoTypeObjectParseInfo typeObjectParseInfo)
        {
            ChoStringObjectFormattableAttribute attr = ChoType.GetAttribute<ChoStringObjectFormattableAttribute>(type);
            try
            {
                MethodInfo isParseMethodInfo = ChoType.GetMethod(type, typeof(ChoIsStringToObjectConvertable), true);
                if (isParseMethodInfo != null && isParseMethodInfo.IsStatic)
                {
                    if (isParseMethodInfo.ReturnParameter == null
                        || isParseMethodInfo.ReturnParameter.ParameterType != typeof(bool))
                        throw new ChoApplicationException(String.Format("{0}: Incorrect Check Parse routine signature found. It should have bool return parameter.", type.Name));

                    ParameterInfo[] parameters = isParseMethodInfo.GetParameters();
                    if (parameters == null
                        || parameters.Length != 1
                        || parameters[0].ParameterType != typeof(string))
                        throw new ChoApplicationException(String.Format("{0}: Incorrect Check Parse routine signature found. It should have one and only input string parameter.", type.Name));

                    MethodInfo parseMethodInfo = ChoType.GetMethod(type, typeof(ChoStringToObjectConverterAttribute), true);

                    if (parseMethodInfo != null)
                    {
                        parameters = parseMethodInfo.GetParameters();
                        if (parameters == null
                            || parameters.Length != 1
                            || parameters[0].ParameterType != typeof(string))
                            throw new ChoApplicationException(String.Format("{0}: Incorrect Parse routine signature found. It should have one and only input string parameter.", type.Name));

                        typeObjectParseInfo.CheckParse = isParseMethodInfo.CreateDelegate<Func<string, bool>>();
                        typeObjectParseInfo.Parse = parseMethodInfo.CreateDelegate<Func<string, object>>();

                        parseMethodsMsg.AppendFormatLine("{0} [EX: {1}]", type.FullName, ((IChoStringObjectFormatter)Activator.CreateInstance(type)).GetHelpText());
                    }
                }
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                parseMethodsMsg.AppendFormatLine("{0}: [{1}]", type.FullName, ex.Message);
            }

            Type type1 = attr.SupportedType == null ? type.GetType() : attr.SupportedType;
            if (typeObjectParseInfo.IsValid())
            {
                if (TypeObjectsParseInfo.ContainsKey(type1))
                    TypeObjectsParseInfo[type1] = typeObjectParseInfo;
                else
                    TypeObjectsParseInfo.Add(type1, typeObjectParseInfo);
            }
        }

        private static void LoadObjectFormatter(ChoStringMsgBuilder formatMethodsMsg, Type type, ChoTypeObjectFormatInfo typeObjectFormatInfo)
        {
            ChoStringObjectFormattableAttribute attr = ChoType.GetAttribute<ChoStringObjectFormattableAttribute>(type);

            try
            {
                MethodInfo isFormatMethodInfo = ChoType.GetMethod(type, typeof(ChoCanObjectFormattableAttribute), true);
                if (isFormatMethodInfo != null && isFormatMethodInfo.IsStatic)
                {
                    if (isFormatMethodInfo.ReturnParameter == null
                        || isFormatMethodInfo.ReturnParameter.ParameterType != typeof(bool))
                        throw new ChoApplicationException(String.Format("{0}: Incorrect Check Format routine signature found. It should have bool return parameter.", type.Name));

                    ParameterInfo[] parameters = isFormatMethodInfo.GetParameters();
                    if (parameters == null
                        || parameters.Length != 1
                        || parameters[0].ParameterType != typeof(object))
                        throw new ChoApplicationException(String.Format("{0}: Incorrect Check Format routine signature found. It should have one and only input object parameter.", type.Name));

                    MethodInfo formatMethodInfo = ChoType.GetMethod(type, typeof(ChoObjectFormatterAttribute), true);
                    if (formatMethodInfo != null)
                    {
                        parameters = formatMethodInfo.GetParameters();
                        if (formatMethodInfo.ReturnParameter == null
                            || formatMethodInfo.ReturnParameter.ParameterType != typeof(string))
                            throw new ChoApplicationException(String.Format("{0}: Incorrect Format routine signature found. It should have string return parameter.", type.Name));

                        if (parameters == null
                            || parameters.Length != 2
                            || parameters[0].ParameterType != typeof(object)
                            || parameters[1].ParameterType != typeof(string))
                            throw new ChoApplicationException(String.Format("{0}: Incorrect Format routine signature found. It should have two input parameters of types [object, string].", type.Name));

                        typeObjectFormatInfo.CanFormat = isFormatMethodInfo.CreateDelegate<Func<object, bool>>();
                        typeObjectFormatInfo.Format = formatMethodInfo.CreateDelegate<Func<object, string, string>>();

                        formatMethodsMsg.AppendFormatLine(type.FullName);
                    }
                }
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                formatMethodsMsg.AppendFormatLine("{0}: [{1}]", type.FullName, ex.Message);
            }

            Type type1 = attr.SupportedType == null ? type.GetType() : attr.SupportedType;
            if (typeObjectFormatInfo.IsValid())
            {
                if (TypeObjectsFormatInfo.ContainsKey(type1))
                    TypeObjectsFormatInfo[type1] = typeObjectFormatInfo;
                else
                    TypeObjectsFormatInfo.Add(type1, typeObjectFormatInfo);
            }
        }

        #endregion Shared Constructor

        #region Shared Members (Public)

        internal static ChoTypeObjectParseInfo[] GetTypeObjectsParseInfo()
        {
            return _typeObjectsParseInfoArr;
        }

        internal static ChoTypeObjectFormatInfo[] GetTypeObjectsFormatInfo()
        {
            return _typeObjectsFormatInfoArr;
        }

        #endregion Shared Members (Public)
    }
}
