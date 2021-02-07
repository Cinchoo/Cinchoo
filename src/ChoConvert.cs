namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;
    using System.Linq;

    #endregion NameSpaces

    //public static class ChoConvert1
    //{
    //    #region Shared Members (Public)

    //    #region ConvertFrom Overloads

    //    public static object ConvertFrom(object value, Type targetType, CultureInfo culture = null)
    //    {
    //        return ConvertFrom(null, value, targetType, culture);
    //    }

    //    public static object ConvertFrom(object target, object value, Type targetType, CultureInfo culture = null)
    //    {
    //        return ConvertFrom(target, value, targetType, null, null, culture);
    //    }

    //    public static object ConvertFrom(object target, string memberName, object value, CultureInfo culture = null)
    //    {
    //        ChoGuard.ArgumentNotNull(target, "Target");
    //        ChoGuard.ArgumentNotNull(memberName, "MemberName");

    //        Type objType = null;

    //        if (target is Type)
    //        {
    //            objType = target as Type;
    //            target = null;
    //        }
    //        else
    //            objType = target.GetType();

    //        return ConvertFrom(target, ChoType.GetMemberInfo(objType, memberName), culture);
    //    }

    //    public static object ConvertFrom(object target, MemberInfo memberInfo, object value, CultureInfo culture = null)
    //    {
    //        ChoGuard.ArgumentNotNull(target, "Target");
    //        ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

    //        Type objType = null;

    //        if (target is Type)
    //        {
    //            objType = target as Type;
    //            target = null;
    //        }
    //        else
    //            objType = target.GetType();

    //        if (target == null)
    //        {
    //            return ChoConvert.ConvertFrom(target, value, ChoType.GetMemberType(memberInfo),
    //                ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
    //        }
    //        else
    //        {
    //            return ChoConvert.ConvertFrom(target, value, ChoType.GetMemberType(memberInfo),
    //                ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
    //        }
    //    }

    //    private static object ConvertFrom(object target, object value, Type targetType, object[] converters, object[] parameters, CultureInfo culture)
    //    {
    //        Type sourceType = value == null ? typeof(object) : value.GetType();
    //        if (converters != null && converters.Length > 0)
    //        {
    //            object objConverter = null;
    //            object[] convParams = null;
    //            for (int index = 0; index < converters.Length; index++)
    //            {
    //                objConverter = converters[index];
    //                if (parameters != null && parameters.Length > 0)
    //                    convParams = parameters[index] as object[];

    //                if (objConverter is ChoParameterizedTypeConverter)
    //                    ((ChoParameterizedTypeConverter)objConverter).Target = target is Type ? null : target;

    //                if (objConverter is TypeConverter)
    //                {
    //                    TypeConverter converter = objConverter as TypeConverter;

    //                    if (converter.CanConvertFrom(sourceType))
    //                        value = converter.ConvertFrom(null, culture, value);
    //                }
    //                else if (objConverter is IChoValueConverter)
    //                {
    //                    value = ((IChoValueConverter)objConverter).Convert(value, targetType, convParams, culture);
    //                }
    //                else if (objConverter is IChoMultiValueConverter && value is object[])
    //                {
    //                    value = ((IChoMultiValueConverter)objConverter).Convert(value as object[], targetType, convParams, culture);
    //                }
    //                else if (objConverter is IValueConverter)
    //                    value = ((IValueConverter)objConverter).Convert(value, targetType, convParams, culture);
    //            }
    //            return value;
    //        }
    //        else
    //            value = ConvertInternal(value, targetType);

    //        try
    //        {
    //            if (value != null)
    //                return Convert.ChangeType(value, targetType);
    //            else
    //                return targetType.Default();
    //        }
    //        catch (Exception ex)
    //        {
    //            if (sourceType.IsSimple())
    //                throw new ChoApplicationException(String.Format("Can't convert '{2}' value [Type: {0}] to {1} type.", sourceType, targetType, value), ex);
    //            else
    //                throw new ChoApplicationException(String.Format("Can't convert {0} object to {1} type.", sourceType, targetType), ex);
    //        }
    //    }

    //    #endregion ConvertFrom Overloads

    //    #region ConvertTo Overloads

    //    public static object ConvertTo(object value, Type targetType, CultureInfo culture = null)
    //    {
    //        return ConvertTo(null, value, targetType, culture);
    //    }

    //    public static object ConvertTo(object target, object value, Type targetType, CultureInfo culture = null)
    //    {
    //        return ConvertTo(target, value, targetType, null, null, culture);
    //    }

    //    public static object ConvertTo(object target, string memberName, Type targetType, CultureInfo culture = null)
    //    {
    //        ChoGuard.ArgumentNotNull(target, "Target");
    //        ChoGuard.ArgumentNotNull(memberName, "MemberName");

    //        Type objType = null;

    //        if (target is Type)
    //        {
    //            objType = target as Type;
    //            target = null;
    //        }
    //        else
    //            objType = target.GetType();

    //        return ConvertTo(target, ChoType.GetMemberInfo(objType, memberName), targetType, culture);
    //    }

    //    public static object ConvertTo(object target, MemberInfo memberInfo, Type targetType, CultureInfo culture = null)
    //    {
    //        ChoGuard.ArgumentNotNull(target, "Target");
    //        ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

    //        Type objType = null;

    //        if (target is Type)
    //        {
    //            objType = target as Type;
    //            target = null;
    //        }
    //        else
    //            objType = target.GetType();

    //        if (target == null)
    //        {
    //            return ChoConvert.ConvertTo(target, ChoType.GetMemberValue(objType, memberInfo.Name), targetType,
    //                ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
    //        }
    //        else
    //        {
    //            return ChoConvert.ConvertTo(target, ChoType.GetMemberValue(target, memberInfo.Name), targetType,
    //                ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
    //        }
    //    }

    //    private static object ConvertTo(object target, object value, Type targetType, object[] converters, object[] parameters, CultureInfo culture)
    //    {
    //        value = ConvertInternal(value, targetType);

    //        Type sourceType = value == null ? typeof(object) : value.GetType();

    //        if (converters != null && converters.Length > 0)
    //        {
    //            object objConverter = null;
    //            object[] convParams = null;
    //            for (int index = 0; index < converters.Length; index++)
    //            {
    //                objConverter = converters[index];

    //                if (parameters != null && parameters.Length > 0)
    //                    convParams = parameters[index] as object[];

    //                if (objConverter is ChoParameterizedTypeConverter)
    //                    ((ChoParameterizedTypeConverter)objConverter).Target = target;

    //                if (objConverter is TypeConverter)
    //                {
    //                    TypeConverter converter = objConverter as TypeConverter;

    //                    if (converter.CanConvertTo(targetType))
    //                        value = converter.ConvertTo(null, culture, value, targetType);
    //                }
    //                else if (objConverter is IChoValueConverter)
    //                {
    //                    value = ((IChoValueConverter)objConverter).ConvertBack(value, targetType, convParams, culture);
    //                }
    //                else if (objConverter is IChoMultiValueConverter && value is object[])
    //                {
    //                    value = ((IChoMultiValueConverter)objConverter).ConvertBack(value as object[], targetType, convParams, culture);
    //                }
    //                else if (objConverter is IValueConverter)
    //                    value = ((IValueConverter)objConverter).ConvertBack(value, targetType, convParams, culture);
    //            }
    //            return value;
    //        }

    //        try
    //        {
    //            return Convert.ChangeType(value, targetType);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ChoApplicationException(String.Format("Can't convert {0} object to {1} type.", sourceType, targetType), ex);
    //        }
    //    }

    //    #endregion ConvertTo Overloads

    //    #endregion

    //    #region Shared Members (Private)

    //    private static object ConvertInternal(object value, Type targetType)
    //    {
    //        if (value == null) return null;
    //        if (targetType == value.GetType())
    //            return value;

    //        if ((value is string) && ((string)value).Length == 0)
    //            return null;

    //        if (targetType.IsEnum)
    //        {
    //            if (value is string)
    //                return Enum.Parse(targetType, value as string);
    //            else
    //                return Enum.ToObject(targetType, value);
    //        }
    //        if (value is string && targetType == typeof(Guid)) return new Guid(value as string);
    //        if (value is string && targetType == typeof(Version)) return new Version(value as string);
    //        if (!(value is IConvertible)) return value;
    //        if (value is string)
    //            return ChoString.ToObjectInternal(value as string, targetType);

    //        return value;
    //    }

    //    #endregion
    //}

    public class ChoConvertEventArgs : EventArgs
    {
        public object Value;
        public Type ConversionType;
        public IFormatProvider Provider;
        public object Output;
        public bool Handled;
    }

    public static class ChoConvert
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.CurrentCulture;
        private const string ImplicitOperatorMethodName = "op_Implicit";
        private const string ExplicitOperatorMethodName = "op_Explicit";

        public static event EventHandler<ChoConvertEventArgs> ChangeType;

        public static bool TryConvertTo(object value, Type targetType, CultureInfo culture, out object output)
        {
            output = null;

            try
            {
                output = ChoConvert.ConvertTo(value, targetType, culture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object ConvertTo(object value, Type targetType, CultureInfo culture = null)
        {
            if (value == null)
                return ChoConvert.ConvertTo(value, targetType, value, null, null, culture);
            else
                return ChoConvert.ConvertTo(value, targetType, value, ChoTypeDescriptor.GetTypeConverters(value.GetType()), ChoTypeDescriptor.GetTypeConverterParams(value.GetType()), culture);
        }
        
        public static bool TryConvertFrom(object value, MemberInfo memberInfo, object sourceObject, CultureInfo culture, out object output)
        {
            output = null;
            ChoGuard.ArgumentNotNull((object)memberInfo, "MemberInfo");

            try
            {
                output = ChoConvert.ConvertFrom(value, memberInfo, sourceObject, culture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object ConvertFrom(object value, MemberInfo memberInfo, object sourceObject = null, CultureInfo culture = null)
        {
            ChoGuard.ArgumentNotNull((object)memberInfo, "MemberInfo");

            return ChoConvert.ConvertFrom(value, ChoType.GetMemberType(memberInfo), sourceObject, ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
        }

        private static object ConvertFrom(object value, Type targetType, object sourceObject, object[] converters, object[] parameters, CultureInfo culture)
        {
            object srcValue = value;
            if (targetType == null)
                return value;

            object output = null;
            if (TryChangeType(value, targetType, culture, out output))
                return output;

            if (targetType == typeof(object))
                return value;
            if (value == null)
                return ChoTypeEx.Default(targetType);

            targetType = targetType.IsNullableType() ? targetType.GetUnderlyingType() : targetType;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;
            if (targetType == value.GetType())
                return value;

            if (culture == null)
                culture = DefaultCulture;

            Type sourceType = value.GetType();
            Exception innerException = null;
            try
            {

                if (converters != null && converters.Length > 0)
                {
                    object[] objArray = (object[])null;
                    for (int index = 0; index < converters.Length; ++index)
                    {
                        object obj = converters[index];
                        if (parameters != null && parameters.Length > 0)
                            objArray = parameters[index] as object[];
                        if (obj is ChoParameterizedTypeConverter)
                            ((ChoParameterizedTypeConverter)obj).Target = sourceObject;
                        if (obj is TypeConverter)
                        {
                            TypeConverter typeConverter = obj as TypeConverter;
                            if (typeConverter.CanConvertFrom(sourceType))
                                value = typeConverter.ConvertFrom((ITypeDescriptorContext)null, culture, value);
                        }
                        else if (obj is IChoValueConverter)
                            value = ((IChoValueConverter)obj).Convert(value, targetType, (object)objArray, culture);
                        else if (obj is IChoMultiValueConverter && value is object[])
                            value = ((IChoMultiValueConverter)obj).Convert(value as object[], targetType, (object)objArray, culture);
                        else if (obj is IValueConverter)
                            value = ((IValueConverter)obj).Convert(value, targetType, (object)objArray, culture);
                    }

                    if (value != srcValue)
                        return value;
                }

                //Convert using IConvertable
                if (value is IConvertible)
                {
                    try
                    {
                        value = Convert.ChangeType(value, targetType, culture);
                        if (srcValue != value)
                            return value;
                    }
                    catch (Exception innEx)
                    {
                        innerException = innEx;
                    }
                }

                if (TryConvertXPlicit(value, targetType, ExplicitOperatorMethodName, ref value))
                {
                    return value;
                }
                if (TryConvertXPlicit(value, targetType, ImplicitOperatorMethodName, ref value))
                {
                    return value;
                }

                if (ChoConvert.TryConvertToSpecialValues(value, targetType, culture, out value))
                    return value;

                throw new ChoApplicationException("Object conversion failed.");
            }
            catch (Exception ex)
            {
                if (ChoTypeEx.IsSimple(sourceType))
                    throw new ChoApplicationException(string.Format("Can't convert '{2}' value from '{0}' type to '{1}' type.", (object)sourceType, (object)targetType, value), ex);
                else
                    throw new ChoApplicationException(string.Format("Can't convert object from '{0}' type to '{1}' type.", (object)sourceType, (object)targetType), ex);
            }
        }

        private static bool TryChangeType(object value, Type targetType, CultureInfo culture, out object output)
        {
            ChoConvertEventArgs e = new ChoConvertEventArgs() { Value = value, ConversionType = targetType, Provider = culture };
            ChangeType.Raise(null, e);
            output = e.Output;
            return e.Handled;
        }

        private static bool TryConvertXPlicit(object value, Type destinationType, string operatorMethodName, ref object result)
        {
            if (TryConvertXPlicit(value, value.GetType(), destinationType, operatorMethodName, ref result))
            {
                return true;
            }
            if (TryConvertXPlicit(value, destinationType, destinationType, operatorMethodName, ref result))
            {
                return true;
            }
            return false;
        }

        private static bool TryConvertXPlicit(object value, Type invokerType, Type destinationType, string xPlicitMethodName, ref object result)
        {
            var methods = invokerType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods.Where(m => m.Name == xPlicitMethodName))
            {
                if (destinationType.IsAssignableFrom(method.ReturnType))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Count() == 1 && parameters[0].ParameterType == value.GetType())
                    {
                        try
                        {
                            result = method.Invoke(null, new[] { value });
                            return true;
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch
                        {
                            // ReSharper restore EmptyGeneralCatchClause
                        }
                    }
                }
            }
            return false;
        }

        private static bool TryConvertToSpecialValues(object value, Type targetType, CultureInfo culture, out object result)
        {
            result = null;
            if (value is string && ((string)value).Length == 0)
            {
                result = ChoTypeEx.Default(targetType);
                return true;
            }
            if (targetType.IsEnum)
            {
                if (value is string)
                {
                    result = Enum.Parse(targetType, value as string);
                    if (Enum.IsDefined(targetType, result))
                        return true;
                    else
                        return false;
                }
                else
                {
                    result = Enum.ToObject(targetType, value);
                    if (Enum.IsDefined(targetType, result))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                if (value is string && targetType == typeof(Guid))
                {
                    result = (object)new Guid(value as string);
                    return true;
                }
                else if (value is string && targetType == typeof(Version))
                {
                    result = (object)new Version(value as string);
                    return true;
                }
                else if (targetType == typeof(string))
                {
                    result = value.ToString();
                    return true;
                }
                else
                {
                    result = ChoString.ToObjectInternal(value as string, targetType);
                    return true;
                }
            }
        }
        
        public static bool TryConvertTo(object value, MemberInfo memberInfo, Type targetType, object sourceObject, CultureInfo culture, out object output)
        {
            output = null;
            ChoGuard.ArgumentNotNull((object)memberInfo, "MemberInfo");
            
            try
            {
                output = ChoConvert.ConvertTo(value, targetType, sourceObject, ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object ConvertTo(object value, MemberInfo memberInfo, Type targetType, object sourceObject = null, CultureInfo culture = null)
        {
            ChoGuard.ArgumentNotNull((object)memberInfo, "MemberInfo");

            return ChoConvert.ConvertTo(value, targetType, sourceObject, ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
        }

        private static object ConvertTo(object value, Type targetType, object sourceObject, object[] converters, object[] parameters, CultureInfo culture)
        {
            object srcValue = value;
            if (targetType == null)
                return value;

            object output = null;
            if (TryChangeType(value, targetType, culture, out output))
                return output;

            if (targetType == typeof(object))
                return value;
            if (value == null)
                return ChoTypeEx.Default(targetType);

            targetType = targetType.IsNullableType() ? targetType.GetUnderlyingType() : targetType;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;
            if (targetType == value.GetType())
                return value;

            if (culture == null)
                culture = DefaultCulture;

            Exception innerException = null;
            Type sourceType = value.GetType();
            try
            {
                object obj2 = null;
                object[] parameter = null;
                if (converters != null && converters.Length > 0)
                {
                    //object[] objArray = (object[])null;
                    for (int i = 0; i < converters.Length; i++)
                    {
                        obj2 = converters[i];
                        if ((parameters != null) && (parameters.Length > 0))
                        {
                            parameter = parameters[i] as object[];
                        }
                        if (obj2 is ChoParameterizedTypeConverter)
                        {
                            ((ChoParameterizedTypeConverter)obj2).Target = sourceObject;
                        }
                        if (obj2 is TypeConverter)
                        {
                            TypeConverter converter = obj2 as TypeConverter;
                            if (converter.CanConvertTo(targetType))
                            {
                                value = converter.ConvertTo(null, culture, value, targetType);
                            }
                        }
                        else if (obj2 is IChoValueConverter)
                        {
                            value = ((IChoValueConverter)obj2).ConvertBack(value, targetType, parameter, culture);
                        }
                        else if ((obj2 is IChoMultiValueConverter) && (value is object[]))
                        {
                            value = ((IChoMultiValueConverter)obj2).ConvertBack(value as object[], targetType, parameter, culture);
                        }
                        else if (obj2 is IValueConverter)
                        {
                            value = ((IValueConverter)obj2).ConvertBack(value, targetType, parameter, culture);
                        }
                    }
                    if (srcValue != value)
                        return value;
                }

                //Convert using IConvertable
                if (value is IConvertible)
                {
                    try
                    {
                        value = Convert.ChangeType(value, targetType, culture);
                        if (srcValue != value)
                            return value;
                    }
                    catch (Exception innEx)
                    {
                        innerException = innEx;
                    }
                }

                if (TryConvertXPlicit(value, targetType, ExplicitOperatorMethodName, ref value))
                {
                    return value;
                }
                if (TryConvertXPlicit(value, targetType, ImplicitOperatorMethodName, ref value))
                {
                    return value;
                }

                if (ChoConvert.TryConvertToSpecialValues(value, targetType, culture, out value))
                    return value;

                throw new ChoApplicationException("Object conversion failed.");
            }
            catch (Exception ex)
            {
                if (ChoTypeEx.IsSimple(sourceType))
                    throw new ChoApplicationException(string.Format("Can't convert '{2}' value from '{0}' type to '{1}' type.", (object)sourceType, (object)targetType, value), ex);
                else
                    throw new ChoApplicationException(string.Format("Can't convert object from '{0}' type to '{1}' type.", (object)sourceType, (object)targetType), ex);
            }
        }
    }
}