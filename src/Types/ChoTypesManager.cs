namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    using Cinchoo.Core.Text;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    #region ChoTypeObjectParseInfo Struct

    internal struct ChoTypeObjectParseInfo
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

        private static List<ChoTypeObjectParseInfo> _typeObjectsParseInfo = new List<ChoTypeObjectParseInfo>();
        private static List<ChoTypeObjectFormatInfo> _typeObjectsFormatInfo = new List<ChoTypeObjectFormatInfo>();
        private static ChoTypeObjectParseInfo[] _typeObjectsParseInfoArr = null;
        private static ChoTypeObjectFormatInfo[] _typeObjectsFormatInfoArr = null;

        #endregion Shared Data Members (Private)

        #region Shared Constructor

        internal static void Initialize()
        {
			//ChoStreamProfile.Clean(ChoReservedDirectoryName.Others, ChoType.GetLogFileName(typeof(ChoTypesManager)));
            
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

            _typeObjectsParseInfoArr = _typeObjectsParseInfo.ToArray();
            _typeObjectsFormatInfoArr = _typeObjectsFormatInfo.ToArray();

            _typeObjectsParseInfo.Clear();
            _typeObjectsFormatInfo.Clear();

			//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Others, ChoType.GetLogFileName(typeof(ChoTypesManager)), msg.ToString());
			//ChoStreamProfile.WriteNewLine(ChoReservedDirectoryName.Others, ChoType.GetLogFileName(typeof(ChoTypesManager)));
			//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Others, ChoType.GetLogFileName(typeof(ChoTypesManager)), parseMethodsMsg.ToString());
			//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Others, ChoType.GetLogFileName(typeof(ChoTypesManager)), formatMethodsMsg.ToString());
        }

        private static void LoadObjectParser(ChoStringMsgBuilder parseMethodsMsg, Type type, ChoTypeObjectParseInfo typeObjectParseInfo)
        {
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

                        parseMethodsMsg.AppendFormatLine(type.FullName);
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

            if (typeObjectParseInfo.IsValid())
                _typeObjectsParseInfo.Add(typeObjectParseInfo);
        }

        private static void LoadObjectFormatter(ChoStringMsgBuilder formatMethodsMsg, Type type, ChoTypeObjectFormatInfo typeObjectFormatInfo)
        {

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

            if (typeObjectFormatInfo.IsValid())
                _typeObjectsFormatInfo.Add(typeObjectFormatInfo);
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
