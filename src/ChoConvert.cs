namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;

    #endregion NameSpaces

    public static class ChoConvert
    {
        #region Shared Members (Public)

		#region ConvertFrom Overloads

		public static object ConvertFrom(object target, object value, MemberInfo memberInfo)
        {
            return ConvertFrom(target, value, memberInfo, null);
        }

		public static object ConvertFrom(object target, object value, MemberInfo memberInfo, CultureInfo culture)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            Type objType = null;

            if (target is Type)
            {
                objType = target as Type;
                target = null;
            }
            else
                objType = target.GetType();

            return ChoConvert.ConvertFrom(target, value, ChoType.GetMemberType(memberInfo), 
                ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
        }

        public static object ConvertFrom(object target, MemberInfo memberInfo)
        {
            return ConvertFrom(target, memberInfo, null);
        }

		public static object ConvertFrom(object target, MemberInfo memberInfo, CultureInfo culture)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            Type objType = null;

            if (target is Type)
            {
                objType = target as Type;
                target = null;
            }
            else
                objType = target.GetType();

            if (target == null)
            {
                return ChoConvert.ConvertFrom(target, ChoType.GetMemberValue(objType, memberInfo.Name), ChoType.GetMemberType(memberInfo),
                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
            }
            else
            {
                return ChoConvert.ConvertFrom(target, ChoType.GetMemberValue(target, memberInfo.Name),ChoType.GetMemberType(memberInfo),
                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
            }
        }

        public static object ConvertFrom(object target, object value, Type targetType)
        {
			return ConvertFrom(target, value, targetType, null, null);
        }

        public static object ConvertFrom(object target, object value, Type targetType, object[] converters, object[] parameters)
        {
            return ConvertFrom(target, value, targetType, converters, parameters, null);
        }

        public static object ConvertFrom(object target, object value, Type targetType, object[] converters, object[] parameters, CultureInfo culture)
        {
            //value = ConvertInternal(value, targetType);

			Type sourceType = value == null ? typeof(object) : value.GetType();
            if (converters != null && converters.Length > 0)
            {
                object objConverter = null;
                object[] convParams = null;
                for (int index = 0; index < converters.Length; index++)
                {
                    objConverter = converters[index];
                    if (parameters != null && parameters.Length > 0)
                        convParams = parameters[index] as object[];
                    
                    if (objConverter is ChoParameterizedTypeConverter)
                        ((ChoParameterizedTypeConverter)objConverter).Target = target;

                    if (objConverter is TypeConverter)
                    {
                        TypeConverter converter = objConverter as TypeConverter;

                        if (converter.CanConvertFrom(sourceType))
                            value = converter.ConvertFrom(null, culture, value);
                    }
                    else if (objConverter is IChoValueConverter)
                    {
                        value = ((IChoValueConverter)objConverter).Convert(value, targetType, convParams, culture);
                    }
                    else if (objConverter is IChoMultiValueConverter && value is object[])
                    {
                        value = ((IChoMultiValueConverter)objConverter).Convert(value as object[], targetType, convParams, culture);
                    }
                    else if (objConverter is IValueConverter)
                        value = ((IValueConverter)objConverter).Convert(value, targetType, convParams, culture);
                }
                return value;
            }
            else
                value = ConvertInternal(value, targetType);

            try
            {
                if (value != null)
                    return Convert.ChangeType(value, targetType);
                else
                    return targetType.Default();
            }
            catch (Exception ex)
            {
				throw new ChoApplicationException(String.Format("Can't convert {0} object to {1} type.", sourceType, targetType), ex);
            }
        }

		#endregion ConvertFrom Overloads

		#region ConvertTo Overloads

		public static object ConvertTo(object target, object value, MemberInfo memberInfo, Type targetType)
		{
			return ConvertTo(target, value, memberInfo, targetType, null);
		}

		public static object ConvertTo(object target, object value, MemberInfo memberInfo, Type targetType, CultureInfo culture)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

			Type objType = null;

			if (target is Type)
			{
				objType = target as Type;
				target = null;
			}
			else
				objType = target.GetType();

            return ChoConvert.ConvertTo(target, value, targetType, ChoTypeDescriptor.GetTypeConverters(memberInfo), 
                ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
		}

		public static object ConvertTo(object target, MemberInfo memberInfo, Type targetType)
		{
			return ConvertTo(target, memberInfo, targetType, null);
		}

		public static object ConvertTo(object target, MemberInfo memberInfo, Type targetType, CultureInfo culture)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

			Type objType = null;

			if (target is Type)
			{
				objType = target as Type;
				target = null;
			}
			else
				objType = target.GetType();

			if (target == null)
			{
				return ChoConvert.ConvertTo(target, ChoType.GetMemberValue(objType, memberInfo.Name), targetType,
                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
			}
			else
			{
				return ChoConvert.ConvertTo(target, ChoType.GetMemberValue(target, memberInfo.Name), targetType,
                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), culture);
			}
		}

		public static object ConvertTo(object target, object value, Type targetType)
		{
			return ConvertTo(target, value, targetType, null, null);
		}

		public static object ConvertTo(object target, object value, Type targetType, object[] converters, object[] parameters)
		{
            return ConvertTo(target, value, targetType, converters, parameters, null);
		}

        public static object ConvertTo(object target, object value, Type targetType, object[] converters, object[] parameters, CultureInfo culture)
		{
			value = ConvertInternal(value, targetType);

			Type sourceType = value == null ? typeof(object) : value.GetType();

			if (converters != null && converters.Length > 0)
			{
                object objConverter = null;
                object[] convParams = null;
				for (int index = 0; index < converters.Length; index++)
				{
                    objConverter = converters[index];

                    if (parameters != null && parameters.Length > 0)
                        convParams = parameters[index] as object[];

					if (objConverter is ChoParameterizedTypeConverter)
						((ChoParameterizedTypeConverter)objConverter).Target = target;

					if (objConverter is TypeConverter)
					{
						TypeConverter converter = objConverter as TypeConverter;

						if (converter.CanConvertTo(targetType))
							value = converter.ConvertTo(null, culture, value, targetType);
					}
					else if (objConverter is IChoValueConverter)
					{
                        value = ((IChoValueConverter)objConverter).ConvertBack(value, targetType, convParams, culture);
					}
					else if (objConverter is IChoMultiValueConverter && value is object[])
					{
                        value = ((IChoMultiValueConverter)objConverter).ConvertBack(value as object[], targetType, convParams, culture);
					}
                    else if (objConverter is IValueConverter)
                        value = ((IValueConverter)objConverter).ConvertBack(value, targetType, convParams, culture);
                }
				return value;
			}

			try
			{
				return Convert.ChangeType(value, targetType);
			}
			catch (Exception ex)
			{
				throw new ChoApplicationException(String.Format("Can't convert {0} object to {1} type.", sourceType, targetType), ex);
			}
		}

		#endregion ConvertTo Overloads

		#endregion

		#region Shared Members (Private)

		private static object ConvertInternal(object value, Type targetType)
        {
            if (value == null) return null;
            if (targetType == value.GetType())
                return value;

            if ((value is string) && ((string)value).Length == 0)
                return null;

            if (targetType.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(targetType, value as string);
                else
                    return Enum.ToObject(targetType, value);
            }
            if (value is string && targetType == typeof(Guid)) return new Guid(value as string);
            if (value is string && targetType == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;

            return value;
        }

        #endregion
    }
}
