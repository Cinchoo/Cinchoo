namespace System
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;

    #endregion NameSpaces

    public static class ChoTypeEx
    {
        public static object Default(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            else
                return null;
        }

        public static Attribute GetCustomAttribute(this Type type, Type attributeType)
        {
			return GetCustomAttribute(type, attributeType, false);
		}

		public static Attribute GetCustomAttribute(this Type type, Type attributeType, bool inherit)
		{
			object[] attributes = type.GetCustomAttributes(attributeType, inherit);

			return attributes == null || attributes.Length == 0 ? null : attributes[0] as Attribute;
		}

		public static T GetCustomAttribute<T>(this Type type) where T: Attribute
		{
			return GetCustomAttribute<T>(type, false);
		}

		public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
		{
			object[] attributes = type.GetCustomAttributes(typeof(T), inherit);

			return attributes == null || attributes.Length == 0 ? null : attributes[0] as T;
		}

        public static bool IsSimple(this Type type)
        {
            CheckTypeIsNotNull(type);

            if (type.IsPrimitive
                || typeof(string) == type 
                || type.IsEnum
                || typeof(DateTime) == type
                )
            {
                return true;
            }
            else
                return false;
        }

        public static bool IsMethodImplemented(this Type type, string methodName)
        {
            return IsMethodImplemented(type, methodName, new Type[] {}, null);
        }

        public static bool IsMethodImplemented(this Type type, string methodName, Type[] inParameterTypes, Type returnParameterType)
        {
            CheckTypeIsNotNull(type);
            if (methodName.IsNullOrEmpty())
                throw new ArgumentNullException("MethodName");

            if (IsSimple(type)) return false;

            var explicitMethods =
                from method in type.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                select method;

            bool returnParameterMatch = false;
            bool inParametersMatch = false;
            foreach (var explicitMethod in explicitMethods)
            {
                if (explicitMethod.Name == methodName)
                {
                    returnParameterMatch = returnParameterType == null || (returnParameterType != null && returnParameterType == explicitMethod.ReturnParameter.ParameterType);
                    inParametersMatch = IsInParametersMatch(explicitMethod.GetParameters(), inParameterTypes);

                    if (returnParameterMatch && inParametersMatch) return true;
                }
            }
            return false;
        }

        private static bool IsInParametersMatch(ParameterInfo[] funcInParameterTypes, Type[] inParameterTypes)
        {
            if (inParameterTypes == null && funcInParameterTypes == null) return true;
            if (inParameterTypes == null && funcInParameterTypes != null) return false;
            if (inParameterTypes != null && funcInParameterTypes == null) return false;
            if (inParameterTypes.Length != funcInParameterTypes.Length) return false;

            for (int index = 0; index < funcInParameterTypes.Length; index++)
            {
                if (funcInParameterTypes[index].ParameterType != inParameterTypes[index]) return false;
            }

            return true;
        }

        private static void CheckTypeIsNotNull(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type");
        }
    }
}
