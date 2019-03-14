namespace Cinchoo.Core
{
	#region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Factory;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Services;

	#endregion NameSpaces

	public static class ChoType
	{
		#region TypeInfo Class (Private)

		private class TypeInfo
		{
			public MemberInfo[] MemberInfos;
			public FieldInfo[] FieldInfos;
			public PropertyInfo[] PropertyInfos;
		}

		#endregion TypeInfo Class (Private)

		#region Shared Data Members (Private)

		private static Hashtable _typeInfos = Hashtable.Synchronized(new Hashtable());
		private static Dictionary<Type, Attribute[]> _typeAttributesCache = new Dictionary<Type, Attribute[]>();
		private static readonly object _typeAttributesCacheLockObject = new object();

		private static Dictionary<MemberInfo, Dictionary<Type, List<Attribute>>> _typeMemberAttributesCache = new Dictionary<MemberInfo, Dictionary<Type, List<Attribute>>>();
		private static Dictionary<MemberInfo, List<Attribute>> _typeMemberAllAttributesCache = new Dictionary<MemberInfo, List<Attribute>>();
		private static readonly object _typeMemberAttributesCacheLockObject = new object();

		private static Dictionary<Type, Dictionary<Type, MemberInfo[]>> _typeMembersDictionaryCache = new Dictionary<Type, Dictionary<Type, MemberInfo[]>>();
		private static readonly object _typeMembersDictionaryCacheLockObject = new object();

		private static readonly ChoDictionaryService<string, MethodInfo> _typeMethodsCache = new ChoDictionaryService<string, MethodInfo>("TypeMethodsCache");

        private static readonly object _padLock = new object();
        //private readonly Dictionary<PointerPair, Attribute> attributeCache = new Dictionary<PointerPair, Attribute>();
        private static readonly Dictionary<IntPtr, MemberInfo[]> _memberCache = new Dictionary<IntPtr, MemberInfo[]>();
        private static readonly Dictionary<IntPtr, Func<object>> _constructorCache = new Dictionary<IntPtr, Func<object>>();
        private static readonly Dictionary<IntPtr, Func<object, object>> _getterCache = new Dictionary<IntPtr, Func<object, object>>();
        private static readonly Dictionary<IntPtr, Action<object, object>> _setterCache = new Dictionary<IntPtr, Action<object, object>>();
        private static readonly Dictionary<IntPtr, MethodHandler> _methodCache = new Dictionary<IntPtr, MethodHandler>();

		#endregion

		#region Constructors

		static ChoType()
		{
#if _DYNAMIC_
            ChoTrace.WriteLine("DYMANIC Enabled.");
#else
            ChoTrace.WriteLine("DYMANIC Disabled.");
#endif
            //ChoAssembly.Initialize();
		}

		#endregion

		#region Shared Members (Public)

        private static readonly Dictionary<Tuple<Type, Type>, bool> _dictTypesDefinedGenericType = new Dictionary<Tuple<Type, Type>, bool>();
        private static readonly object _dictTypesDefinedGenericTypeLock = new object();

        public static bool IsImplGenericTypeDefinition(this Type type, Type genericType)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            Tuple<Type, Type> tuple = new Tuple<Type, Type>(type, genericType);
            bool isDefined = false;
            if (_dictTypesDefinedGenericType.TryGetValue(tuple, out isDefined))
                return isDefined;

            lock (_dictTypesDefinedGenericTypeLock)
            {
                if (_dictTypesDefinedGenericType.TryGetValue(tuple, out isDefined))
                    return isDefined;
                
                isDefined = type.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == genericType);

                _dictTypesDefinedGenericType.Add(tuple, isDefined);

                return isDefined;
            }
        }

		#region GetTypeFromXmlSectionNode Overloads

		public static Type GetTypeFromXmlSectionNode(XmlNode sectionNode)
		{
			if (sectionNode == null)
				throw new ArgumentNullException("sectionNode");

			return GetTypeFromXmlSectionName(sectionNode.Name);
		}

		public static Type GetTypeFromXmlSectionName(string sectionName)
		{
			if (sectionName == null || sectionName.Length == 0)
				throw new ArgumentNullException("sectionName");

			Type[] types = ChoType.GetTypes(typeof(XmlRootAttribute));
			Trace.TraceInformation("SectionName: {0}, XmlRootAttribute Types: {1}".FormatString(sectionName, types != null ? types.Length : 0));

			if (types == null || types.Length == 0) return null;

			foreach (Type type in types)
			{
				if (type == null) continue;
				
				XmlRootAttribute xmlRootAttribute = ChoType.GetAttribute(type, typeof(XmlRootAttribute)) as XmlRootAttribute;
				if (xmlRootAttribute == null) continue;

				if (xmlRootAttribute.ElementName == sectionName)
					return type;
			}

			return null;
		}

		public static Type GetTypeFromConfigSectionNode(XmlNode sectionNode)
		{
			if (sectionNode == null)
				throw new ArgumentNullException("sectionNode");

			return GetTypeFromConfigSectionNode(sectionNode.Name);
		}

		public static Type GetTypeFromConfigSectionNode(string sectionName)
		{
			if (sectionName == null || sectionName.Length == 0)
				throw new ArgumentNullException("sectionName");

			Type[] types = ChoType.GetTypes(typeof(ChoConfigurationSectionAttribute));
			if (types == null || types.Length == 0) return null;

			foreach (Type type in types)
			{
				if (type == null) continue;

				ChoConfigurationSectionAttribute configurationElementMapAttribute = ChoType.GetAttribute(type, typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
				if (configurationElementMapAttribute == null) continue;

				if (configurationElementMapAttribute.ConfigElementPath.EndsWith(sectionName))
					return type;
			}

			return null;
		}

		#endregion GetTypeFromXmlSectionNode Overloads

		#region GetConstructor Overloads (Public)

		public static ConstructorInfo GetConstructor(string typeName, Type[] parameterTypes)
		{
			return GetConstructor(GetType(typeName), parameterTypes);
		}

		public static ConstructorInfo GetConstructor(Type type, Type[] parameterTypes)
		{
			if (type == null)
				throw new NullReferenceException("Missing type.");

			return type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null);
		}

		#endregion GetConstructor Overloads (Public)

		#region GetDefaultConstructor Overloads (Public)

		public static ConstructorInfo GetDefaultConstructor(string typeName)
		{
			return GetDefaultConstructor(GetType(typeName));
		}

		public static ConstructorInfo GetDefaultConstructor(Type type)
		{
			if (type == null)
				throw new NullReferenceException("Missing type.");

			return type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, System.Type.EmptyTypes, null);
		}

		#endregion GetDefaultConstructor Overloads (Public)

		#region Other Members

		public static Type GetTypeByReferenceId(string referenceId)
		{
			foreach (Type type in GetTypes(typeof(ChoObjectAttribute)))
			{
				ChoObjectAttribute objectAttibute = GetAttribute(type, typeof(ChoObjectAttribute)) as ChoObjectAttribute;
				if (objectAttibute == null) continue;

				if (objectAttibute.Name == referenceId) return type;
			}

			return null;
		}

		public static Type GetType(string typeName)
		{
			return Type.GetType(typeName);
		}

        public static string GetMemberName(MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            ChoMemberInfoAttribute memberInfoAttribute = memberInfo.GetCustomAttribute<ChoMemberInfoAttribute>();
            return memberInfoAttribute != null && !memberInfoAttribute.Name.IsNullOrWhiteSpace() ? memberInfoAttribute.Name : memberInfo.Name;
        }

        public static string GetMemberName(MemberInfo memberInfo, ChoMemberInfoAttribute memberInfoAttribute)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            return memberInfoAttribute != null && !memberInfoAttribute.Name.IsNullOrWhiteSpace() ? memberInfoAttribute.Name : memberInfo.Name;
        }

		#endregion Other Members

		#region CreateInstance Overloads

		public static object CreateInstance(string typeName)
		{
			return CreateInstance(GetType(typeName));
		}

		public static T CreateInstance<T>()
		{
			return (T)CreateInstance(typeof(T));
		}

		public static T CreateInstance<T>(Type objType)
		{
			return (T)CreateInstance(objType);
		}

		public static object CreateInstance(Type type)
		{
			object instance = null;

			#region Create Object Instance

			ChoObjectAttribute objectAttribute = GetAttribute(type, typeof(ChoObjectAttribute)) as ChoObjectAttribute;
			if (objectAttribute != null)
			{
				#region Create Instance based on ObjectConstructorAttribute

				MemberInfo[] memberInfos = GetMemberInfos(type, typeof(ChoObjectConstructorAttribute));

				if (memberInfos != null && memberInfos.Length > 0)
				{
					MemberInfo matchedConstructor = memberInfos[0];
					ChoObjectConstructorAttribute objectConstructorAttribute = null;

					if (String.IsNullOrEmpty(objectAttribute.DefaultConstructor))
					{
						foreach (MemberInfo memberInfo in memberInfos)
						{
							objectConstructorAttribute = ChoType.GetMemberAttribute(memberInfo, typeof(ChoObjectConstructorAttribute)) as ChoObjectConstructorAttribute;
							if (objectConstructorAttribute == null || objectConstructorAttribute.Id == objectAttribute.DefaultConstructor)
							{
								matchedConstructor = memberInfo;
								break;
							}
						}
						objectConstructorAttribute = ChoType.GetMemberAttribute(matchedConstructor, typeof(ChoObjectConstructorAttribute)) as ChoObjectConstructorAttribute;
					}
					else
						objectConstructorAttribute = ChoType.GetMemberAttribute(memberInfos[0], typeof(ChoObjectConstructorAttribute)) as ChoObjectConstructorAttribute;

					if (objectConstructorAttribute != null)
					{
						ConstructorInfo constructorInfo = type.GetConstructor(objectConstructorAttribute.ConstructorArgsTypes);
						if (constructorInfo != null)
							instance = constructorInfo.Invoke(objectConstructorAttribute.ConstructorArgs);
					}
					else
						instance = Activator.CreateInstance(type, objectConstructorAttribute.ConstructorArgs);
				}
			
				#endregion Create Instance based on ObjectConstructorAttribute
			}
			
			if (instance == null)
			{
				#region Create Instance using Default Constructor

				ConstructorInfo defaultConstructorInfo = GetDefaultConstructor(type);
				if (defaultConstructorInfo == null)
					throw new NullReferenceException(String.Format("Missing default constructor for {0} type.", type.Name));

				instance = defaultConstructorInfo.Invoke(null);

				#endregion Create Instance using Default Constructor
			}

			#endregion Create Object Instance

			#region Set the Field values

			if (instance != null)
			{
				MemberInfo[] memberInfos = GetMemberInfos(type, typeof(ChoObjectFieldAttribute));
				if (memberInfos != null && memberInfos.Length > 0)
				{
					foreach (MemberInfo memberInfo in memberInfos)
					{
						ChoObjectFieldAttribute objectFieldAttribute = GetMemberAttribute(memberInfo, typeof(ChoObjectFieldAttribute)) as ChoObjectFieldAttribute;
						if (objectFieldAttribute == null) continue;

						if (String.IsNullOrEmpty(objectFieldAttribute.ReferenceId))
                            SetFieldValue(instance, memberInfo.Name, ChoObject.ConvertValueToObjectMemberType(instance, memberInfo, objectFieldAttribute.Value));
						else if (objectAttribute != null && objectAttribute.Name != objectFieldAttribute.ReferenceId)
						{
							Type refType = ChoType.GetTypeByReferenceId(objectFieldAttribute.ReferenceId);
							if (refType != null)
								SetFieldValue(instance, memberInfo.Name, ChoObject.ConvertValueToObjectMemberType(instance, memberInfo, ChoObjectManagementFactory.CreateInstance(refType)));
						}
					}
				}
			}

			#endregion Set the Field Values

			#region Set the Property values

			if (instance != null)
			{
				MemberInfo[] memberInfos = GetMemberInfos(type, typeof(ChoObjectPropertyAttribute));
				if (memberInfos != null && memberInfos.Length > 0)
				{
					foreach (MemberInfo memberInfo in memberInfos)
					{
						ChoObjectPropertyAttribute objectPropertyAttribute = GetMemberAttribute(memberInfo, typeof(ChoObjectPropertyAttribute)) as ChoObjectPropertyAttribute;
						if (objectPropertyAttribute == null) continue;

						if (String.IsNullOrEmpty(objectPropertyAttribute.ReferenceId))
							SetPropertyValue(instance, memberInfo.Name, ChoObject.ConvertValueToObjectMemberType(instance, memberInfo, objectPropertyAttribute.Value));
						else if (objectAttribute != null && objectAttribute.Name != objectPropertyAttribute.ReferenceId)
						{
							Type refType = ChoType.GetTypeByReferenceId(objectPropertyAttribute.ReferenceId);
							if (refType != null)
								SetPropertyValue(instance, memberInfo.Name, ChoObject.ConvertValueToObjectMemberType(instance, memberInfo, ChoObjectManagementFactory.CreateInstance(refType)));
						}
					}
				}
			}

			#endregion Set the Property Values

			return instance;
		}

		public static object CreateInstance(string typeName, string csvParameters)
		{
			return CreateInstance(GetType(typeName), csvParameters);
		}

		public static object CreateInstance(string typeName, object[] parameters)
		{
			return CreateInstance(GetType(typeName), parameters);
		}

		public static object CreateInstance(Type type, string csvParameters)
		{
			return CreateInstance(type, ChoString.Split2Objects(csvParameters));
		}

		public static object CreateInstance(Type type, object[] parameters)
		{
			Type[] types = ChoType.ConvertToTypes(parameters);
			//ConstructorInfo constructorInfo = type.GetConstructor(types);
			ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, types, null);
			if (constructorInfo == null)
				throw new ChoApplicationException(String.Format("Can't find a constructor of matching inputs [{0}] in '{1}' type.", ChoString.Join(types), type.FullName));

			return constructorInfo.Invoke(parameters);
		}

		#endregion CreateInstance Overloads

		#region HasConstructor Overloads

		public static bool HasDefaultConstructor(Type type)
		{
			ConstructorInfo constructorInfo = type.GetConstructor(new Type[] {});
			return constructorInfo != null;
		}

		public static bool HasConstructor(Type type, object[] parameters)
		{
			Type[] types = ChoType.ConvertToTypes(parameters);
			ConstructorInfo constructorInfo = type.GetConstructor(types);
			return constructorInfo != null;
		}

		#endregion HasConstructor Overloads

		#region HasProperty Overloads

		public static bool HasProperty(Type type, string name)
		{
			PropertyInfo propertyInfo = null;
			return HasProperty(type, name, out propertyInfo);
		}

		public static bool HasProperty(Type type, string name, out PropertyInfo propertyInfo)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return propertyInfo != null;
		}

		#endregion HasProperty Overloads

		#region HasGetProperty Overloads

		public static bool HasGetProperty(Type type, string name)
		{
			PropertyInfo propertyInfo = null;
			return HasGetProperty(type, name, out propertyInfo);
		}

		public static bool HasGetProperty(Type type, string name, out PropertyInfo propertyInfo)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return propertyInfo != null && propertyInfo.CanRead;
		}

		#endregion HasGetProperty Overloads

		#region HasSetProperty Overloads

		public static bool HasSetProperty(Type type, string name)
		{
			PropertyInfo propertyInfo = null;
			return HasSetProperty(type, name, out propertyInfo);
		}

		public static bool HasSetProperty(Type type, string name, out PropertyInfo propertyInfo)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return propertyInfo != null && propertyInfo.CanWrite;
		}

		#endregion HasSetProperty Overloads

		#region HasField Overloads

		public static bool HasField(Type type, string name)
		{
			FieldInfo fieldInfo = null;
			return HasField(type, name, out fieldInfo);
		}

		public static bool HasField(Type type, string name, out FieldInfo fieldInfo)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return fieldInfo != null;
		}

		#endregion HasField Overloads

		#region CreateInstanceWithReflectionPermission Overloads

		public static object CreateInstanceWithReflectionPermission(string typeName)
		{
			return CreateInstanceWithReflectionPermission(GetType(typeName));
		}

		[ReflectionPermission(SecurityAction.Assert, Unrestricted=true)]
		public static object CreateInstanceWithReflectionPermission(Type type)
		{
			return Activator.CreateInstance(type, true);
		}

		[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
		public static object CreateInstanceWithReflectionPermission(Type type, object[] args)
		{
			return Activator.CreateInstance(type, args);
		}

		#endregion CreateInstanceWithReflectionPermission Overloads

		#region InvokeMethod Overloads

		public static string GetReadableMethodName(object target, string name, params Type[] argsTypes)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			StringBuilder methodName = new StringBuilder(String.Format("{0}_{1}", target.GetType().AssemblyQualifiedName, name));
			if (!ChoGuard.IsArgumentNotNullOrEmpty(argsTypes))
			{
				foreach (Type type in argsTypes)
					methodName.AppendFormat("_{0}", type.FullName);
			}

			string readableMethodName = methodName.ToString();
			if (!_typeMethodsCache.ContainsKey(readableMethodName))
			{
				MethodInfo methodInfo = target.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, argsTypes, null);
				_typeMethodsCache.SetValue(readableMethodName, methodInfo);
			}

			return readableMethodName;
		}

		public static bool HasMethod(object target, string name, Type[] argsTypes, ref string readableMethodName)
		{
			if (readableMethodName.IsNullOrEmpty())
				readableMethodName = GetReadableMethodName(target, name, argsTypes);

			if (!_typeMethodsCache.ContainsKey(readableMethodName))
			{
				try
				{
					MethodInfo methodInfo = target.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, argsTypes, null);
					_typeMethodsCache.SetValue(readableMethodName, methodInfo);
					return true;
				}
				catch (TargetInvocationException ex)
				{
					throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, name), ex.InnerException);
				}
			}
			else
				return true;
		}

		public static bool HasMethod(object target, string name, object[] args)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			try
			{
				return target.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, ConvertToTypesArray(args), null) != null;
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, name), ex.InnerException);
			}
		}

		public static object InvokeMethod(string readableMethodName, object target, object[] args)
		{
			ChoGuard.ArgumentNotNull(readableMethodName, "ReadableMethodName");

			//RuntimeMethodHandle methodHandle = _typeMethodsCache.GetValue(readableMethodName);
			MethodInfo methodIndo = _typeMethodsCache.GetValue(readableMethodName);
			if (methodIndo == null)
				throw new ChoApplicationException(String.Format("{0} method not found.", readableMethodName));

			try
			{
				return methodIndo.Invoke(target, args);
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Method: {0}]:", target.GetType().FullName, readableMethodName), ex.InnerException);
			}
		}

		public static object InvokeMethod(object target, MethodInfo methodInfo, object[] args)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNull(methodInfo, "MethodInfo");

			try
			{
				return methodInfo.Invoke(target, args);
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, methodInfo.Name), ex.InnerException);
			}
		}

		public static object InvokeMethod(object target, string name, object[] args)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			try
			{
				MethodInfo methodInfo = target.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, ConvertToTypesArray(args), null);
				if (methodInfo != null)
					return methodInfo.Invoke(target, args);
				else
					throw new ChoApplicationException(String.Format("Can't find {0} method in {1} type.", name, target.GetType().FullName));
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, name), ex.InnerException);
			}
		}

		public static bool HasMethod(Type type, string name, object[] args)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			try
			{
				return type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, ConvertToTypesArray(args), null) != null;
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Type: {0}, Member: {1}]:", type.FullName, name), ex.InnerException);
			}
		}

		public static object InvokeMethod(Type type, string name, object[] args)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			try
			{
				MethodInfo methodInfo = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, ConvertToTypesArray(args), null);
				if (methodInfo != null)
					return methodInfo.Invoke(null, args);
				else
					throw new ChoApplicationException(String.Format("Can't find {0} method in {1} type.", name, type.FullName));
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Type: {0}, Member: {1}]:", type.FullName, name), ex.InnerException);
			}
		}

		#endregion InvokeMethod Overloads

		#region Get & Set Field Value methods

		public static object GetFieldValue(object target, string name)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			if (target is Type) return GetStaticFieldValue(target as Type, name);

			FieldInfo fieldInfo = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (fieldInfo == null)
				throw new ChoApplicationException(String.Format("Missing {0} field in {1} object.", name, target.GetType().FullName));

			return GetFieldValue(target, fieldInfo);
		}

		public static object GetFieldValue(object target, FieldInfo fieldInfo)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(fieldInfo, "FieldInfo");

			if (target is Type) return GetStaticFieldValue(target as Type, fieldInfo);

			try
			{
#if _DYNAMIC_
                Func<object, object> getter;
                if (!_getterCache.TryGetValue(fieldInfo.FieldHandle.Value, out getter))
                {
                    lock (_padLock)
                    {
                        if (!_getterCache.TryGetValue(fieldInfo.FieldHandle.Value, out getter))
                            _getterCache.Add(fieldInfo.FieldHandle.Value, getter = fieldInfo.CreateGetMethod());
                    }
                }
                return getter(target);
#else
                return fieldInfo.GetValue(target);
#endif
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, fieldInfo.Name), ex.InnerException);
				//throw ex.InnerException;
			}
		}

		public static void SetFieldValue(object target, string name, object val)
		{
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            FieldInfo fieldInfo = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (fieldInfo == null)
                throw new ChoApplicationException(String.Format("Can't find {0} field in {1} object.", name, target.GetType().FullName));

            SetFieldValue(target, fieldInfo, val);
        }

        public static void SetFieldValue(object target, FieldInfo fieldInfo, object val)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNullOrEmpty(fieldInfo, "FieldInfo");

            if (val == null && fieldInfo.FieldType.IsValueType)
                val = fieldInfo.FieldType.Default();

            if (target is Type)
            {
                SetStaticFieldValue(target as Type, fieldInfo, val);
                return;
            }

            ChoValidation.Validate(fieldInfo as MemberInfo, val);

            try
            {
#if _DYNAMIC_
                Action<object, object> setter;
                if (!_setterCache.TryGetValue(fieldInfo.FieldHandle.Value, out setter))
                {
                    lock (_padLock)
                    {
                        if (!_setterCache.TryGetValue(fieldInfo.FieldHandle.Value, out setter))
                            _setterCache.Add(fieldInfo.FieldHandle.Value, setter = fieldInfo.CreateSetMethod());
                    }
                }
                setter(target, val);
#else
                fieldInfo.SetValue(target, val);
#endif
            }
            catch (TargetInvocationException ex)
            {
                throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, fieldInfo.Name), ex.InnerException);
                //throw ex.InnerException;
            }
        }

		#endregion Get & Set Field Value methods

		#region Get & Set Static Field Value methods

		public static object GetStaticFieldValue(Type type, string name)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			FieldInfo fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (fieldInfo == null)
				throw new ChoApplicationException(String.Format("Can't find {0} field in {1} object.", name, type.FullName));

			return GetStaticFieldValue(type, fieldInfo);
		}

		public static object GetStaticFieldValue(Type type, FieldInfo fieldInfo)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNull(fieldInfo, "FieldInfo");

			try
			{
#if _DYNAMIC_
                Func<object, object> getter;
                if (!_getterCache.TryGetValue(fieldInfo.FieldHandle.Value, out getter))
                {
                    lock (_padLock)
                    {
                        if (!_getterCache.TryGetValue(fieldInfo.FieldHandle.Value, out getter))
                            _getterCache.Add(fieldInfo.FieldHandle.Value, getter = fieldInfo.CreateGetMethod());
                    }
                }
                return getter(null);
#else
                return fieldInfo.GetValue(null);
#endif
            }
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Type: {0}, Member: {1}]:", type.FullName, fieldInfo.Name), ex.InnerException);
				//throw ex.InnerException;
			}
		}

		public static void SetStaticFieldValue(Type type, string name, object val)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			FieldInfo fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (fieldInfo == null)
				throw new ChoApplicationException(String.Format("Can't find {0} field in {1} object.", name, type.FullName));

            SetStaticFieldValue(null, fieldInfo, val);
		}

        public static void SetStaticFieldValue(Type type, FieldInfo fieldInfo, object val)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(fieldInfo, "FieldInfo");

            ChoValidation.Validate(fieldInfo as MemberInfo, val);

            try
            {
#if _DYNAMIC_
                Action<object, object> setter;
                if (!_setterCache.TryGetValue(fieldInfo.FieldHandle.Value, out setter))
                {
                    lock (_padLock)
                    {
                        if (!_setterCache.TryGetValue(fieldInfo.FieldHandle.Value, out setter))
                            _setterCache.Add(fieldInfo.FieldHandle.Value, setter = fieldInfo.CreateSetMethod());
                    }
                }
                setter(null, val);
#else
                fieldInfo.SetValue(null, val);
#endif
            }
            catch (TargetInvocationException ex)
            {
                throw new TargetInvocationException(String.Format("[Type: {0}, Member: {1}]:", type.FullName, fieldInfo.Name), ex.InnerException);
                //throw ex.InnerException;
            }
        }

		#endregion Get & Set Static Field Value methods

		#region Get & Set Property Value methods

		public static object GetPropertyValue(object target, string name)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			if (target is Type) return GetStaticPropertyValue(target as Type, name);

			PropertyInfo propertyInfo = target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (propertyInfo == null)
				throw new ChoApplicationException(String.Format("Can't find {0} property in {1} object.", name, target.GetType().FullName));

			return GetPropertyValue(target, propertyInfo);
		}

		public static object GetPropertyValue(object target, PropertyInfo propertyInfo)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNull(propertyInfo, "PropertyInfo");

			if (target is Type) return GetStaticPropertyValue(target as Type, propertyInfo);

			try
			{
#if _DYNAMIC_
                Func<object, object> getter;
                var key = propertyInfo.GetGetMethod().MethodHandle.Value;
                if (!_getterCache.TryGetValue(key, out getter))
                {
                    lock (_padLock)
                    {
                        if (!_getterCache.TryGetValue(key, out getter))
                            _getterCache.Add(key, getter = propertyInfo.CreateGetMethod());
                    }
                }
                return getter(target);
#else
                return propertyInfo.GetValue(target, new object[] { });
#endif
            }
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, propertyInfo.Name), ex.InnerException);
				//throw ex.InnerException;
			}
		}

		public static void SetPropertyValue(object target, string name, object val)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			PropertyInfo propertyInfo = target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (propertyInfo == null)
				throw new ChoApplicationException(String.Format("Can't find {0} property in {1} object.", name, target.GetType().FullName));

            SetPropertyValue(target, propertyInfo, val);
		}

        public static void SetPropertyValue(object target, PropertyInfo propertyInfo, object val)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(propertyInfo, "PropertyInfo");

            if (val == null && propertyInfo.PropertyType.IsValueType)
                val = propertyInfo.PropertyType.Default();

            if (target is Type)
            {
                SetStaticPropertyValue(target as Type, propertyInfo, val);
                return;
            }

            ChoValidation.Validate(propertyInfo as MemberInfo, val);

            try
            {
#if _DYNAMIC_
                Action<object, object> setter;
                var key = propertyInfo.GetSetMethod().MethodHandle.Value;
                if (!_setterCache.TryGetValue(key, out setter))
                {
                    lock (_padLock)
                    {
                        if (!_setterCache.TryGetValue(key, out setter))
                            _setterCache.Add(key, setter = propertyInfo.CreateSetMethod());
                    }
                }
                setter(target, val);
#else
                propertyInfo.SetValue(target, val, null);
#endif
            }
            catch (TargetInvocationException ex)
            {
                throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, propertyInfo.Name), ex.InnerException);
                //throw ex.InnerException;
            }
        }

		public static object GetPropertyValue(object target, string name, object[] index)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
			ChoGuard.ArgumentNotNull(index, "Index");

			try
			{
				PropertyInfo propertyInfo = target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				if (propertyInfo == null)
					throw new ChoApplicationException(String.Format("Can't find {0} property in {1} object.", name, target.GetType().FullName));

				return propertyInfo.GetValue(target, index);
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, name), ex.InnerException);
				//throw ex.InnerException;
			}
		}

		public static void SetPropertyValue(object target, string name, object[] index, object val)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
			ChoGuard.ArgumentNotNull(index, "Index");

			try
			{
				PropertyInfo propertyInfo = target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				if (propertyInfo == null)
					throw new ChoApplicationException(String.Format("Can't find {0} property in {1} object.", name, target.GetType().FullName));

                propertyInfo.SetValue(target, val, index);
			}
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", target.GetType().FullName, name), ex.InnerException);
				//throw ex.InnerException;
			}
		}

		#endregion Get & Set Property Value methods

		#region Get & Set Static Property Value methods

		public static object GetStaticPropertyValue(Type type, string name)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			return GetStaticPropertyValue(type, propertyInfo);
		}

		public static object GetStaticPropertyValue(Type type, PropertyInfo propertyInfo)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(propertyInfo, "PropertyInfo");

			try
			{
#if _DYNAMIC_
                Func<object, object> getter;
                var key = propertyInfo.GetGetMethod().MethodHandle.Value;
                if (!_getterCache.TryGetValue(key, out getter))
                {
                    lock (_padLock)
                    {
                        if (!_getterCache.TryGetValue(key, out getter))
                            _getterCache.Add(key, getter = propertyInfo.CreateGetMethod());
                    }
                }
                return getter(null);
#else
                return propertyInfo.GetValue(null, new object[] { });
#endif
            }
			catch (TargetInvocationException ex)
			{
				throw new TargetInvocationException(String.Format("[Type: {0}, Member: {1}]:", type.FullName, propertyInfo.Name), ex.InnerException);
				throw ex.InnerException;
			}
		}

		public static void SetStaticPropertyValue(Type type, string name, object val)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (propertyInfo == null)
				throw new ChoApplicationException(String.Format("Can't find {0} property in {1} object.", name, type.FullName));

            SetStaticPropertyValue(type, propertyInfo, val);
		}

        public static void SetStaticPropertyValue(Type type, PropertyInfo propertyInfo, object val)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNullOrEmpty(propertyInfo, "PropertyInfo");

            ChoValidation.Validate(propertyInfo as MemberInfo, val);

            try
            {
#if _DYNAMIC_
                Action<object, object> setter;
                var key = propertyInfo.GetSetMethod().MethodHandle.Value;
                if (!_setterCache.TryGetValue(key, out setter))
                {
                    lock (_padLock)
                    {
                        if (!_setterCache.TryGetValue(key, out setter))
                            _setterCache.Add(key, setter = propertyInfo.CreateSetMethod());
                    }
                }
                setter(null, val);
#else
                propertyInfo.SetValue(null, val, null);
#endif
            }
            catch (TargetInvocationException ex)
            {
                throw new TargetInvocationException(String.Format("[Object: {0}, Member: {1}]:", type.FullName, propertyInfo.Name), ex.InnerException);
                //throw ex.InnerException;
            }
        }

		#endregion Get & Set Static Property Value methods

		#region Get & Set Member Value methods

		public static object GetMemberValue(Type type, object target, string memberName)
		{
			//Call the object member, return the value
			if (ChoType.HasGetProperty(type, memberName))
			{
				if (target != null)
					return ChoType.GetPropertyValue(target, memberName);
				else
					return ChoType.GetStaticPropertyValue(type, memberName);
			}
			else if (ChoType.HasField(type, memberName))
			{
				if (target != null)
					return ChoType.GetFieldValue(target, memberName);
				else
					return ChoType.GetStaticFieldValue(type, memberName);
			}
			else
				throw new ApplicationException(String.Format("Can't find {0} member in {1} type.", memberName, target.GetType()));
		}

		public static object GetMemberValue(object target, string name)
		{
			if (target == null || name == null) return null;
			if (target is Type) return GetStaticMemberValue(target as Type, name);

			MemberInfo[] memberInfos = target.GetType().GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (memberInfos == null || memberInfos.Length == 0) return null;

			return GetMemberValue(target, memberInfos[0]);
		}

        [ChoHiddenMember]
        public static bool IsValidObjectMember(MemberInfo memberInfo)
        {
            if ((memberInfo.MemberType != MemberTypes.Field
                && memberInfo.MemberType != MemberTypes.Property)
                || memberInfo.Name == "Item" //Indexer
                )
                return false;

            return true;
        }

		public static object GetMemberValue(object target, MemberInfo memberInfo)
		{
			if (target == null || memberInfo == null) return null;
			if (target is Type) return GetStaticMemberValue(target as Type, memberInfo);

			MemberTypes memberType = memberInfo.MemberType;

			if (memberType == MemberTypes.Property)
				return GetPropertyValue(target, (PropertyInfo)memberInfo);
			else if (memberType == MemberTypes.Field)
				return GetFieldValue(target, (FieldInfo)memberInfo);
			else
				return null;
		}

        public static void SetMemberValue(object target, MemberInfo memberInfo, object value)
        {
            if (target == null || memberInfo == null)
                return;

            if (target is Type)
            {
                SetStaticMemberValue(target as Type, memberInfo, value);
                return;
            }

            MemberTypes memberType = memberInfo.MemberType;

            if (memberType == MemberTypes.Property)
                SetPropertyValue(target, (PropertyInfo)memberInfo, value);
            else if (memberType == MemberTypes.Field)
                SetFieldValue(target, (FieldInfo)memberInfo, value);
        }

		public static void SetMemberValue(object target, string name, object val)
		{
			if (target == null || name == null) return;
			if (target is Type)
			{
				SetStaticMemberValue(target as Type, name, val);
				return;
			}

			MemberInfo[] memberInfos = target.GetType().GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (memberInfos == null || memberInfos.Length == 0) return;

			MemberTypes memberType = memberInfos[0].MemberType;

			if (memberType == MemberTypes.Property)
				SetPropertyValue(target, name, val);
			else if (memberType == MemberTypes.Field)
				SetFieldValue(target, name, val);
		}

		#endregion Get & Set Member Value methods

		#region Get & Set Static Member Value methods

		public static object GetStaticMemberValue(Type type, string name)
		{
			if (type == null || name == null) return null;

			MemberInfo[] memberInfos = type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (memberInfos == null || memberInfos.Length == 0) return null;

			return GetStaticMemberValue(type, memberInfos[0]);
		}

		public static object GetStaticMemberValue(Type type, MemberInfo memberInfo)
		{
			if (type == null || memberInfo == null) return null;

			MemberTypes memberType = memberInfo.MemberType;

			if (memberType == MemberTypes.Property)
				return GetStaticPropertyValue(type, (PropertyInfo)memberInfo);
			else if (memberType == MemberTypes.Field)
				return GetStaticFieldValue(type, (FieldInfo)memberInfo);
			else
				return null;
		}

        public static void SetStaticMemberValue(Type type, MemberInfo memberInfo, object val)
        {
            if (type == null || memberInfo == null)
                return;

            MemberTypes memberType = memberInfo.MemberType;

            if (memberType == MemberTypes.Property)
                SetStaticPropertyValue(type, (PropertyInfo)memberInfo, val);
            else if (memberType == MemberTypes.Field)
                SetStaticFieldValue(type, (FieldInfo)memberInfo, val);
        }

		public static void SetStaticMemberValue(Type type, string name, object val)
		{
			if (type == null || name == null) return;

			MemberInfo[] memberInfos = type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (memberInfos == null || memberInfos.Length == 0) return;

			MemberTypes memberType = memberInfos[0].MemberType;

			if (memberType == MemberTypes.Property)
				SetStaticPropertyValue(type, name, val);
			else if (memberType == MemberTypes.Field)
				SetStaticFieldValue(type, name, val);
		}

		#endregion Get & Set Static Member Value methods

		#region GetMembers Overloads

		public static MemberInfo[] GetMembers(Type type)
		{
			if (type == null)
				throw new NullReferenceException("Missing Type.");

			TypeInfo typeInfo = GetTypeInfo(type);
			if (typeInfo.MemberInfos == null)
			{
				OrderedDictionary myMemberInfos = new OrderedDictionary();
				foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
				{
					if (myMemberInfos.Contains(memberInfo.Name))
					{
						if (memberInfo.DeclaringType.FullName == type.FullName)
							myMemberInfos[memberInfo.Name] = memberInfo;
						else
							continue;
					}
					else
						myMemberInfos.Add(memberInfo.Name, memberInfo);
				}

				typeInfo.MemberInfos = new ArrayList(myMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
			}
			return typeInfo.MemberInfos;
		}

		#endregion GetMembers Overloads

		#region GetMemberType Overloads

		public static Type GetMemberType(Type targetType, string memberName)
		{
			foreach (MemberInfo memberInfo in ChoType.GetMembers(targetType))
			{
				if (memberInfo.Name == memberName) return GetMemberType(memberInfo);
			}
			return null;
		}

		public static Type GetMemberType(MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo)
				return ((FieldInfo)memberInfo).FieldType;
			else if (memberInfo is PropertyInfo)
				return ((PropertyInfo)memberInfo).PropertyType;
			else
				throw new InvalidDataException("Invalid member info");
		}

		public static bool TryGetMemberType(MemberInfo memberInfo, out Type type)
		{
			type = null;
			if (memberInfo is FieldInfo)
				type = ((FieldInfo)memberInfo).FieldType;
			else if (memberInfo is PropertyInfo)
				type = ((PropertyInfo)memberInfo).PropertyType;
			else
				return false;

			return true;
		}

		#endregion GetMemberType Overloads

		#region GetMemberAttributes Overloads

		public static T[] GetMemberAttributes<T>(MemberInfo memberInfo) where T: Attribute
		{
			return (T[])GetMemberAttributes(memberInfo, typeof(T));
		}

		public static Attribute[] GetMemberAttributes(MemberInfo memberInfo, Type attributeType)
		{
			return GetMemberAttributes(memberInfo, attributeType, true);
		}

		public static T[] GetMemberAttributes<T>(MemberInfo memberInfo, bool inherit) where T : Attribute
		{
			return (T[])GetMemberAttributes(memberInfo, typeof(T), inherit);
		}

		public static Attribute[] GetMemberAttributes(MemberInfo memberInfo, Type attributeType, bool inherit)
		{
			if (memberInfo == null)
				throw new NullReferenceException("memberInfo");
			if (attributeType == null)
				throw new NullReferenceException("attributeType");

			return GetAttributes(memberInfo, attributeType, inherit) as Attribute[];
		}

		#endregion GetMemberAttributes Overloads

		#region GetMemberAttribute Overloads

		public static T GetMemberAttribute<T>(MemberInfo memberInfo) where T: Attribute
		{
			return (T)GetMemberAttribute(memberInfo, typeof(T));
		}

		public static Attribute GetMemberAttribute(MemberInfo memberInfo, Type attributeType)
		{
			return GetMemberAttribute(memberInfo, attributeType, false);
		}

		public static T GetMemberAttribute<T>(MemberInfo memberInfo, bool inherit) where T : Attribute
		{
			return (T)GetMemberAttribute(memberInfo, typeof(T), inherit);
		}

		public static Attribute GetMemberAttribute(MemberInfo memberInfo, Type attributeType, bool inherit)
		{
			Attribute[] attributes = GetMemberAttributes(memberInfo, attributeType, inherit);
			if (attributes == null || attributes.Length == 0) return null;
			return attributes[0];
		}

		#endregion GetMemberAttributes Overloads

		#region HasMemberAttribute Overloads

		public static bool HasMemberAttribute<T>(MemberInfo memberInfo) where T : Attribute
		{
			return GetMemberAttribute(memberInfo, typeof(T)) != null;
		}

		public static bool HasMemberAttribute(MemberInfo memberInfo, Type attributeType)
		{
			return GetMemberAttribute(memberInfo, attributeType, false) != null;
		}

		public static bool HasMemberAttribute<T>(MemberInfo memberInfo, bool inherit) where T : Attribute
		{
			return GetMemberAttribute(memberInfo, typeof(T), inherit) != null;
		}

		public static bool HasMemberAttribute(MemberInfo memberInfo, Type attributeType, bool inherit)
		{
			return GetMemberAttribute(memberInfo, attributeType, inherit) != null;
		}

		#endregion HasMemberAttribute Overloads

		#region GetMemberAttributesByBaseType Overloads

		public static T[] GetMemberAttributesByBaseType<T, baseT>(MemberInfo memberInfo)
			where T : Attribute
			where baseT : Type
		{
			return Array.ConvertAll<Attribute, T>(GetMemberAttributesByBaseType(memberInfo, typeof(T), typeof(baseT)),
				delegate(Attribute attribute) { return attribute as T; });
		}

		public static Attribute[] GetMemberAttributesByBaseType(MemberInfo memberInfo, Type attributeType, Type baseType)
		{
			if (memberInfo == null)
				throw new NullReferenceException("memberInfo");
			if (attributeType == null)
				throw new NullReferenceException("attributeType");
			if (baseType == null)
				throw new NullReferenceException("interfaceType");

			List<Attribute> attributes = new List<Attribute>();
			foreach (Attribute attribute in GetAttributes(memberInfo, attributeType, true))
			{
				if (baseType.IsAssignableFrom(attribute.GetType()))
					attributes.Add(attribute);
			}

			return attributes.ToArray();
		}

		public static T[] GetMemberAttributesByBaseType<T>(MemberInfo memberInfo)
			where T : Attribute
		{
			return Array.ConvertAll<Attribute, T>(GetMemberAttributesByBaseType(memberInfo, typeof(T)),
				delegate(Attribute attribute) { return attribute as T; });
		}

		public static Attribute[] GetMemberAttributesByBaseType(MemberInfo memberInfo, Type baseType)
		{
			if (memberInfo == null)
				throw new NullReferenceException("memberInfo");
			if (baseType == null)
				throw new NullReferenceException("interfaceType");

			List<Attribute> attributes = new List<Attribute>();
			foreach (Attribute attribute in GetAttributes(memberInfo, true))
			{
				if (baseType.IsAssignableFrom(attribute.GetType()))
					attributes.Add(attribute);
			}

			return attributes.ToArray();
		}

		#endregion GetMemberAttributesByBaseType Overloads

		#region GetMemberAttributeByBaseType Overloads

		public static T GetMemberAttributeByBaseType<T, baseT>(MemberInfo memberInfo)
			where T : Attribute
			where baseT : Type
		{
			return (T)GetMemberAttributeByBaseType(memberInfo, typeof(T), typeof(baseT));
		}

		public static Attribute GetMemberAttributeByBaseType(MemberInfo memberInfo, Type attributeType, Type baseType)
		{
			Attribute[] attributes = GetMemberAttributesByBaseType(memberInfo, attributeType, baseType);
			if (attributes == null || attributes.Length == 0) return null;
			return attributes[0];
		}

		public static T GetMemberAttributeByBaseType<T>(MemberInfo memberInfo)
			where T : Attribute
		{
			return (T)GetMemberAttributeByBaseType(memberInfo, typeof(T));
		}

		public static Attribute GetMemberAttributeByBaseType(MemberInfo memberInfo, Type baseType)
		{
			Attribute[] attributes = GetMemberAttributesByBaseType(memberInfo, baseType);
			if (attributes == null || attributes.Length == 0) return null;
			return attributes[0];
		}

		#endregion GetMemberAttributeByBaseType Overloads

		#region GetMembers Overloads

		public static Dictionary<string, MemberInfo> GetMembersDictionary(Type type, Type attributeType)
		{
			Dictionary<string, MemberInfo> memberInfos = new Dictionary<string, MemberInfo>();
			foreach (MemberInfo memberInfo in GetMemberInfos(type, attributeType))
				memberInfos.Add(memberInfo.Name, memberInfo);

			return memberInfos;
		}

		public static MemberInfo[] GetMemberInfos(Type type, Type attributeType)
		{
			if (type == null)
				throw new NullReferenceException("type");
			if (attributeType == null)
				throw new NullReferenceException("attributeType");

			if (_typeMembersDictionaryCache.ContainsKey(type)
				&& _typeMembersDictionaryCache[type] != null
				&& _typeMembersDictionaryCache[type].ContainsKey(attributeType))
				return _typeMembersDictionaryCache[type][attributeType];

			lock (_typeMembersDictionaryCacheLockObject)
			{
				if (_typeMembersDictionaryCache.ContainsKey(type)
					&& _typeMembersDictionaryCache[type] != null
					&& _typeMembersDictionaryCache[type].ContainsKey(attributeType))
					return _typeMembersDictionaryCache[type][attributeType];

				if (!_typeMembersDictionaryCache.ContainsKey(type) || _typeMembersDictionaryCache[type] == null)
				{
					_typeMembersDictionaryCache[type] = new Dictionary<Type, MemberInfo[]>();
					_typeMembersDictionaryCache[type].Add(attributeType, null);
				}

				OrderedDictionary myMemberInfos = new OrderedDictionary();
				foreach (MemberInfo memberInfo in GetMembers(type))
				{
					if (!(memberInfo is PropertyInfo)
						&& !(memberInfo is FieldInfo)
						&& !(memberInfo is MethodInfo)
						&& !(memberInfo is ConstructorInfo))
						continue;

					object memberAttribute = ChoType.GetMemberAttribute(memberInfo, attributeType);
					if (memberAttribute == null)
						continue;
					myMemberInfos.Add(memberInfo.Name, memberInfo);
				}

				_typeMembersDictionaryCache[type][attributeType] = new ArrayList(myMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];

				return _typeMembersDictionaryCache[type][attributeType];
			}
		}

		#endregion GetMembers Overloads

		#region GetMethod Overloads

		public static MethodInfo GetMethod(Type type, Type attributeType)
		{
			return GetMethod(type, attributeType, false);
		}

		public static MethodInfo GetMethod(Type type, Type attributeType, bool includeStaticMethods)
		{
			MethodInfo[] methodInfos = GetMethods(type, attributeType, includeStaticMethods);
			return methodInfos == null || methodInfos.Length == 0 ? null : methodInfos[0];
		}

		#endregion GetMethod Overloads

		#region GetMethods Overloads

		public static MethodInfo[] GetMethods(Type attributeType)
		{
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach (Type type in ChoType.GetTypes<object>())
			{
				methods.AddRange(ChoType.GetMethods(type, attributeType, true));
			}
			return methods.ToArray();
		}

		public static MethodInfo[] GetMethods(Type type, Type attributeType)
		{
			return GetMethods(type, attributeType, false);
		}

		public static MethodInfo[] GetMethods(Type type, Type attributeType, bool includeStaticMethods)
		{
			if (type == null)
				throw new NullReferenceException("type");
			if (attributeType == null)
				throw new NullReferenceException("attributeType");

			OrderedDictionary myMemberInfos = new OrderedDictionary();
			foreach (MemberInfo memberInfo in GetMembers(type))
			{
				if (!(memberInfo is MethodInfo))
					continue;

				object memberAttribute = ChoType.GetMemberAttribute(memberInfo, attributeType);
				if (memberAttribute == null) continue;
				myMemberInfos.Add(memberInfo.Name, memberInfo);
			}

			return new ArrayList(myMemberInfos.Values).ToArray(typeof(MethodInfo)) as MethodInfo[];
		}

		#endregion GetMethods Overloads

		#region GetMember Overloads

		public static MemberInfo GetMemberInfo(Type type, Type attributeType)
		{
			MemberInfo[] memberInfos = GetMemberInfos(type, attributeType);
			return memberInfos == null || memberInfos.Length == 0 ? null : memberInfos[0];
		}

		public static MemberInfo GetMemberInfo(Type type, string memberName)
		{
			if (type == null)
				throw new NullReferenceException("type");
			if (memberName == null)
				throw new NullReferenceException("memberName");

			foreach (MemberInfo memberInfo in GetMembers(type))
			{
				if (memberInfo.Name == memberName) return memberInfo;
			}
			return null;
		}

		#endregion GetMember Overloads

		#region GetFields Overloads

		public static FieldInfo[] GetFields(Type type)
		{
			if (type == null)
				throw new NullReferenceException("Missing Type.");

			TypeInfo typeInfo = GetTypeInfo(type);
			if (typeInfo.FieldInfos == null)
			{
				OrderedDictionary myFieldInfos = new OrderedDictionary();
				foreach (FieldInfo fieldInfo in type.GetFields())
				{
					if (myFieldInfos.Contains(fieldInfo.Name))
					{
						if (fieldInfo.DeclaringType.FullName == type.FullName)
							myFieldInfos[fieldInfo.Name] = fieldInfo;
						else
							continue;
					}
					else
						myFieldInfos.Add(fieldInfo.Name, fieldInfo);
				}

				typeInfo.FieldInfos = new ArrayList(myFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
			}
			return typeInfo.FieldInfos;
		}

		#endregion GetFields Overloads

		#region GetProperties Overloads

		public static PropertyInfo[] GetProperties(Type type)
		{
			if (type == null)
				throw new NullReferenceException("Missing Type.");

			TypeInfo typeInfo = GetTypeInfo(type);
			if (typeInfo.PropertyInfos == null)
			{
				OrderedDictionary myPropertyInfos = new OrderedDictionary();
				foreach (PropertyInfo propertyInfo in type.GetProperties())
				{
					if (myPropertyInfos.Contains(propertyInfo.Name))
					{
						if (propertyInfo.DeclaringType.FullName == type.FullName)
							myPropertyInfos[propertyInfo.Name] = propertyInfo;
						else
							continue;
					}
					else
						myPropertyInfos.Add(propertyInfo.Name, propertyInfo);
				}

				typeInfo.PropertyInfos = new ArrayList(myPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];
			}
			return typeInfo.PropertyInfos;
		}

		#endregion GetProperties Overloads

		#region GetTypeFromString Overloads

		/// <summary>
		/// Loads the type specified in the type string.
		/// </summary>
		/// <param name="relativeType">A sibling type to use to load the type.</param>
		/// <param name="typeName">The name of the type to load.</param>
		/// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
		/// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>
		/// <remarks>
		/// <para>
		/// If the type name is fully qualified, i.e. if contains an assembly name in 
		/// the type name, the type will be loaded from the system using 
		/// <see cref="Type.GetType(string,bool)"/>.
		/// </para>
		/// <para>
		/// If the type name is not fully qualified, it will be loaded from the assembly
		/// containing the specified relative type. If the type is not found in the assembly 
		/// then all the loaded assemblies will be searched for the type.
		/// </para>
		/// </remarks>
		public static Type GetTypeFromString(Type relativeType, string typeName, bool throwOnError, bool ignoreCase)
		{
			return GetTypeFromString(relativeType.Assembly, typeName, throwOnError, ignoreCase);
		}

		/// <summary>
		/// Loads the type specified in the type string.
		/// </summary>
		/// <param name="typeName">The name of the type to load.</param>
		/// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
		/// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>		
		/// <remarks>
		/// <para>
		/// If the type name is fully qualified, i.e. if contains an assembly name in 
		/// the type name, the type will be loaded from the system using 
		/// <see cref="Type.GetType(string,bool)"/>.
		/// </para>
		/// <para>
		/// If the type name is not fully qualified it will be loaded from the
		/// assembly that is directly calling this method. If the type is not found 
		/// in the assembly then all the loaded assemblies will be searched for the type.
		/// </para>
		/// </remarks>
		public static Type GetTypeFromString(string typeName, bool throwOnError, bool ignoreCase)
		{
			return GetTypeFromString(Assembly.GetCallingAssembly(), typeName, throwOnError, ignoreCase);
		}

		/// <summary>
		/// Loads the type specified in the type string.
		/// </summary>
		/// <param name="relativeAssembly">An assembly to load the type from.</param>
		/// <param name="typeName">The name of the type to load.</param>
		/// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
		/// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>
		/// <remarks>
		/// <para>
		/// If the type name is fully qualified, i.e. if contains an assembly name in 
		/// the type name, the type will be loaded from the system using 
		/// <see cref="Type.GetType(string,bool)"/>.
		/// </para>
		/// <para>
		/// If the type name is not fully qualified it will be loaded from the specified
		/// assembly. If the type is not found in the assembly then all the loaded assemblies 
		/// will be searched for the type.
		/// </para>
		/// </remarks>
		public static Type GetTypeFromString(Assembly relativeAssembly, string typeName, bool throwOnError, bool ignoreCase)
		{
			// Check if the type name specifies the assembly name
			if (typeName.IndexOf(',') == -1)
			{
				// Attempt to lookup the type from the relativeAssembly
				Type type = relativeAssembly.GetType(typeName, false, ignoreCase);
				if (type != null) return type;

				Assembly[] loadedAssemblies = null;
				try
				{
					loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				}
				catch (System.Security.SecurityException)
				{
					// Insufficient permissions to get the list of loaded assemblies
				}

				if (loadedAssemblies != null)
				{
					// Search the loaded assemblies for the type
					foreach (Assembly assembly in loadedAssemblies)
					{
						type = assembly.GetType(typeName, false, ignoreCase);
						if (type != null)
						{
							// Found type in loaded assembly
							ChoTrace.Debug("SystemInfo: Loaded type [" + typeName + "] from assembly [" + assembly.FullName + "] by searching loaded assemblies.");
							return type;
						}
					}
				}

				// Didn't find the type
				if (throwOnError)
				{
					throw new TypeLoadException("Could not load type [" + typeName + "]. Tried assembly [" + relativeAssembly.FullName + "] and all loaded assemblies");
				}
				return null;
			}
			else
			{
				return Type.GetType(typeName, throwOnError, ignoreCase);
			}
		}

		#endregion GetTypeFromString Overloads

		#region GetInheritedTypes Overloads

		public static Type[] GetInheritedTypes(Type baseAttributeType)
		{
			return GetInheritedTypes(baseAttributeType, ChoCodeBase.Me.Paths);
		}

		public static Type[] GetInheritedTypes(Type baseAttributeType, IChoProfile outerProfile)
		{
			return GetInheritedTypes(baseAttributeType, ChoCodeBase.Me.Paths, outerProfile);
		}

		public static Type[] GetInheritedTypes(Type baseAttributeType, string directory)
		{
			if (directory == null) return null;

			return GetInheritedTypes(baseAttributeType, new string[] { directory });
		}

		public static Type[] GetInheritedTypes(Type baseAttributeType, string[] directories)
		{
			return GetInheritedTypes(baseAttributeType, directories, null);
		}

		public static Type[] GetInheritedTypes(Type baseAttributeType, string[] directories, IChoProfile outerProfile)
		{
			if (baseAttributeType == null || directories == null || directories.Length == 0)
				return null;

			using (ChoBufferProfileEx profile = new ChoBufferProfileEx(true, ChoType.GetLogFileName(baseAttributeType), String.Format("Loading types having {0} base type...", baseAttributeType.Name), outerProfile))
			{
				ChoNotNullableArrayList types = new ChoNotNullableArrayList();
				foreach (string directory in directories)
				{
					if (directory == null) continue;
					foreach (string file in Directory.GetFiles(directory, "*.dll"))
					{
						if (file == null) continue;

						using (ChoBufferProfileEx fileProfile = new ChoBufferProfileEx(true, String.Format("Loading types from {0} file...", file), profile))
						{
							try
							{
								Assembly assembly = Assembly.LoadFile(file);
								foreach (Type type in assembly.GetTypes())
								{
									if (type == null) continue;

									if (baseAttributeType.IsAssignableFrom(type))
									{
										fileProfile.AppendLine(type.FullName);
										types.Add(type);
									}
								}
							}
							catch (Exception ex)
							{
								fileProfile.Append(ex);
							}
						}
					}
				}

				return types.ToArray(typeof(Type)) as Type[];
			}
		}

		#endregion GetInheritedTypes Overloads

		#region GetTypes (By Attribute) Overloads

		public static Type[] GetTypes(Type attributeType)
		{
			return GetTypes(attributeType, ChoCodeBase.Me.Paths);
		}

		public static Type[] GetTypes(Type attributeType, IChoProfile outerProfile)
		{
			return GetTypes(attributeType, ChoCodeBase.Me.Paths, outerProfile);
		}

		public static Type[] GetTypes(Type attributeType, string directory)
		{
			if (directory == null) return null;

			return GetTypes(attributeType, new string[] { directory });
		}

		public static Type[] GetTypes(Type attributeType, string[] directories)
		{
			return GetTypes(attributeType, directories, null);
		}

		public static Type[] GetTypes(Type attributeType, string[] directories, IChoProfile outerProfile)
		{
			if (directories == null || directories.Length == 0)
				directories = ChoCodeBase.AppPaths;

			if (attributeType == null || directories == null || directories.Length == 0)
				return new Type[] { };

			using (ChoBufferProfileEx profile = new ChoBufferProfileEx(true, ChoType.GetLogFileName(attributeType), String.Format("Loading types having {0} attributeType...", attributeType.Name), outerProfile))
			{
				ChoNotNullableArrayList types = new ChoNotNullableArrayList();

				foreach (Assembly assembly in ChoGlobalAssemblyFactory.Me.Assemblies)
				{
                    object[] typeDiscoverableAssemblyAttribute = assembly.GetCustomAttributes(typeof(ChoTypeDiscoverableAssemblyAttribute), false);
                    if (typeDiscoverableAssemblyAttribute != null && typeDiscoverableAssemblyAttribute.Length > 0)
                    {
                        using (ChoBufferProfileEx fileProfile = new ChoBufferProfileEx(true, String.Format("Loading types from {0} assembly...", assembly.FullName), profile))
                            ExtractTypes(attributeType, types, fileProfile, assembly);
                    }
				}

				return types.ToArray(typeof(Type)) as Type[];
			}
		}

		#endregion GetTypes (By Attribute) Overloads

		#region GetTypes (By BaseType) Overloads

		public static Type[] GetTypes()
		{
			return GetTypes<object>();
		}

		public static Type[] GetTypes<T>()
		{
			return GetTypes<T>(ChoCodeBase.Me.Paths);
		}

		public static Type[] GetTypes<T>(IChoProfile outerProfile)
		{
			return GetTypes<T>(ChoCodeBase.Me.Paths, outerProfile);
		}

		public static Type[] GetTypes<T>(string directory)
		{
			if (directory == null) return null;

			return GetTypes<T>(new string[] { directory });
		}

		public static Type[] GetTypes<T>(string[] directories)
		{
			return GetTypes<T>(directories, null);
		}

		public static Type[] GetTypes<T>(string[] directories, IChoProfile outerProfile)
		{
			if (directories == null || directories.Length == 0)
				directories = ChoCodeBase.AppPaths;

			if (directories == null || directories.Length == 0)
				return null;

			using (ChoBufferProfileEx profile = new ChoBufferProfileEx(true, ChoType.GetLogFileName(typeof(T)), String.Format("Loading types having {0} attributeType...", typeof(T).FullName), outerProfile))
			{
				ChoNotNullableArrayList types = new ChoNotNullableArrayList();
				foreach (Assembly assembly in ChoGlobalAssemblyFactory.Me.Assemblies)
				{
					using (ChoBufferProfileEx fileProfile = new ChoBufferProfileEx(true, String.Format("Loading types from {0} assembly...", assembly.FullName), profile))
						ExtractTypes(typeof(T), types, fileProfile, assembly);
				}

				return types.ToArray(typeof(Type)) as Type[];
			}
		}

		#endregion GetTypes (By Attribute) Overloads

		#region GetAssemblies Overloads

		public static Assembly[] GetAssemblies(string[] directories, IChoProfile outerProfile)
		{
			List<Assembly> assemblies = new List<Assembly>();
			if (directories != null && directories.Length > 0)
			{
				foreach (string directory in directories)
				{
					if (directory == null) continue;
					foreach (string file in Directory.GetFiles(directory, "*.dll"))
					{
						if (file == null) continue;

						Assembly assembly = null;
						try
						{
							assembly = Assembly.LoadFile(file);
						}
						catch (Exception ex)
						{
							outerProfile.AppendLine(String.Format("Failed to load {0} assembly. {1}.", file, ex.Message));
						}

						if (assembly == null) continue;
						if (assemblies.Contains(assembly)) continue;

						assemblies.Add(assembly);

						foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
						{
							assembly = null;

							try
							{
								assembly = Assembly.Load(assemblyName);
							}
							catch (Exception ex)
							{
								outerProfile.AppendLine(String.Format("Failed to load {0} assembly. {1}.", assemblyName.FullName, ex.Message));
							}

							if (assembly == null || assemblies.Contains(assembly)) continue;
							assemblies.Add(assembly);
						}
					}
				}
			}
			return assemblies.ToArray();
		}

		#endregion GetAssemblies Overloads

		#region HasAttribute Overloads

		public static bool HasAttribute<T>(Type type) where T : Attribute
		{
			return HasAttribute(type, typeof(T));
		}

		public static bool HasAttribute(Type type, Type attributeType)
		{
			return GetAttribute(type, attributeType) != null;
		}

		public static bool HasAttribute(Type type, string memberName, Type attributeType)
		{
			return ChoType.GetMemberAttribute(ChoType.GetMemberInfo(type, memberName), attributeType) != null;
		}

		#endregion HasAttribute Overloads

		#region GetAttribute Overloads

		public static T GetAttribute<T>(Type type) where T:Attribute
		{
			if (type == null)
				throw new NullReferenceException("type");

			foreach (Attribute attribute in GetCustomAttributes(type, true))
			{
				if (typeof(T).IsAssignableFrom(attribute.GetType()))
					return (T)attribute;
			}

			return null;
		}

		public static Attribute GetAttribute(Type type, Type attributeType)
		{
			if (type == null)
				throw new NullReferenceException("type");
			if (attributeType == null)
				throw new NullReferenceException("interfaceType");

			foreach (Attribute attribute in GetCustomAttributes(type, true))
			{
				if (attributeType.IsAssignableFrom(attribute.GetType()))
					return attribute;
			}

			return null;
		}

		public static T[] GetAttributes<T>(Type type) where T : Attribute
		{
			if (type == null)
				throw new NullReferenceException("type");

			List<T> attributes = new List<T>();
			foreach (Attribute attribute in GetCustomAttributes(type, true))
			{
				if (typeof(T).IsAssignableFrom(attribute.GetType()))
					attributes.Add((T)attribute);
			}

			return attributes.ToArray();
		}

		public static Attribute[] GetAttributes(Type type, Type attributeType)
		{
			if (type == null)
				throw new NullReferenceException("type");
			if (attributeType == null)
				throw new NullReferenceException("interfaceType");

			List<Attribute> attributes = new List<Attribute>();
			foreach (Attribute attribute in GetCustomAttributes(type, true))
			{
				if (attributeType.IsAssignableFrom(attribute.GetType()))
					attributes.Add(attribute);
			}

			return attributes.ToArray();
		}

		#endregion GetAttribute Overloads

		#region SetCustomAttributes Overloads

		public static void SetCustomAttribute(Type type, Attribute attribute)
		{
			SetCustomAttributes(type, new Attribute[] { attribute });
		}

		public static void SetCustomAttributes(Type type, Attribute[] attributes)
		{
			lock (_typeAttributesCacheLockObject)
			{
				if (!_typeAttributesCache.ContainsKey(type))
					_typeAttributesCache[type] = attributes;
				else
					_typeAttributesCache[type] = ChoArray.Combine<Attribute>(_typeAttributesCache[type], attributes);
			}
		}

		public static void SetCustomAttribute(MemberInfo memberInfo, Attribute attribute)
		{
			SetCustomAttributes(memberInfo, new Attribute[] { attribute });
		}

		public static void SetCustomAttributes(MemberInfo memberInfo, Attribute[] attributes)
		{
			lock (_typeMemberAttributesCacheLockObject)
			{
				if (!_typeMemberAttributesCache.ContainsKey(memberInfo))
				{
					_typeMemberAttributesCache[memberInfo] = new Dictionary<Type, List<Attribute>>();
					_typeMemberAllAttributesCache[memberInfo] = new List<Attribute>();
				}

				List<Attribute> allAttributesList = _typeMemberAllAttributesCache[memberInfo];
				Dictionary<Type, List<Attribute>> attributeDictionary = _typeMemberAttributesCache[memberInfo];
				foreach (Attribute attribute in attributes)
				{
					if (!attributeDictionary.ContainsKey(attribute.GetType()))
						attributeDictionary[attribute.GetType()] = new List<Attribute>();

					attributeDictionary[attribute.GetType()].Add(attribute);
					allAttributesList.Add(attribute);
				}
			}
		}

		#endregion SetCustomAttributes Overloads

		#region GetCustomAttributes Overloads

		public static Attribute[] GetCustomAttributes(Type type, bool inherit)
		{
			lock (_typeAttributesCacheLockObject)
			{
				if (!_typeAttributesCache.ContainsKey(type))
					SetCustomAttributes(type, ChoArray.ConvertTo<Attribute>(type.GetCustomAttributes(inherit)));

				return _typeAttributesCache[type];
			}
		}

		#endregion GetCustomAttributes Overloads

		#region GetCustomAttributes (MemberInfo) Overloads

		public static Attribute GetAttribute(MemberInfo memberInfo, bool inherit)
		{
			Attribute[] attributes = GetAttributes(memberInfo, null, inherit);
			return attributes == null || attributes.Length == 0 ? null : attributes[0];
		}

		public static Attribute[] GetAttributes(MemberInfo memberInfo, bool inherit)
		{
			return GetAttributes(memberInfo, null, inherit);
		}

		public static T GetAttribute<T>(MemberInfo memberInfo, bool inherit) where T : Attribute
		{
            return (T)memberInfo.GetCustomAttribute(typeof(T), inherit);
            //foreach (Attribute attribute in GetAttributes(memberInfo, typeof(T), inherit))
            //{
            //    if (typeof(T).IsAssignableFrom(attribute.GetType()))
            //        return (T)attribute;
            //}
        }

		public static T[] GetAttributes<T>(MemberInfo memberInfo, bool inherit) where T: Attribute
		{
			return (T[])GetAttributes(memberInfo, typeof(T), inherit);
		}

		public static Attribute[] GetAttributes(MemberInfo memberInfo, Type attributeType, bool inherit)
		{
			if (Monitor.TryEnter(_typeMemberAttributesCacheLockObject, 1))
			{
				try
				{
					if (!_typeMemberAttributesCache.ContainsKey(memberInfo))
						SetCustomAttributes(memberInfo, ChoArray.ConvertTo<Attribute>(memberInfo.GetCustomAttributes(inherit)));

					if (attributeType == null)
						return _typeMemberAllAttributesCache[memberInfo].ToArray();
					else
						return _typeMemberAttributesCache[memberInfo].ContainsKey(attributeType) ? _typeMemberAttributesCache[memberInfo][attributeType].ToArray() : new Attribute[] { };
				}
				finally
				{
					Monitor.Exit(_typeMemberAttributesCacheLockObject);
				}
			}
			else if (attributeType != null)
				return ChoArray.ConvertTo<Attribute>(memberInfo.GetCustomAttributes(attributeType, inherit));
			else
				return ChoArray.ConvertTo<Attribute>(memberInfo.GetCustomAttributes(inherit));
		}

		#endregion GetAttributes (MemberInfo) Overloads

		#region Get/SetAttributeNameParameterValue Overloads (Public)

		public static void SetAttributeNameParameterValue(Attribute attribute, string paramName, object paramValue)
		{
			ChoType.SetMemberValue(attribute, paramName, paramValue);
		}

		public static void SetAttributeNameParameterValue(Type type, string memberName, Type attributeType, string paramName, object paramValue)
		{
			ChoType.SetMemberValue(ChoType.GetMemberAttribute(ChoType.GetMemberInfo(type, memberName), attributeType), paramName, paramValue);
		}

		public static void SetAttributeNameParameterValue(Type type, Type attributeType, string paramName, object paramValue)
		{
			ChoType.SetMemberValue(ChoType.GetAttribute(type, attributeType), paramName, paramValue);
		}

		public static object GetAttributeNameParameterValue(Attribute attribute, string paramName)
		{
			return ChoType.GetMemberValue(attribute, paramName);
		}

		public static object GetAttributeNameParameterValue(Type type, string memberName, Type attributeType, string paramName)
		{
			return ChoType.GetMemberValue(ChoType.GetMemberAttribute(ChoType.GetMemberInfo(type, memberName), attributeType), paramName);
		}

		public static object GetAttributeNameParameterValue(Type type, Type attributeType, string paramName)
		{
			return ChoType.GetMemberValue(ChoType.GetAttribute(type, attributeType), paramName);
		}

		#endregion Get/SetAttributeNameParameterValue Overloads (Public)

		#region SetMemberDefaultValue Overloads

		public static void SetMemberDefaultValue(object target, string memberName)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

            MemberInfo memberInfo = ChoTypeMembersCache.GetMemberInfo(target.GetType(), memberName);
            if (memberInfo != null)
            {
                if (memberInfo.GetCustomAttribute<ChoPropertyInfoAttribute>() != null)
                {
                    try
                    {
                        ChoType.SetMemberValue(target, memberInfo, memberInfo.GetConvertedDefaultValue());
                    }
                    catch (Exception ex)
                    {
                        throw new ChoFatalApplicationException(String.Format("Failed to assign default value to {0}.{1}", target.GetType().FullName, memberName), ex);
                    }
                }
            }
		}

		#endregion SetMemberDefaultValue Overloads

		#region Other helper members (Public)

        [ChoHiddenMember]
        public static IEnumerable<MemberInfo> GetGetFieldsNProperties(this Type type)
        {
            return GetGetFieldsNProperties(type, BindingFlags.Public | BindingFlags.Instance);
        }

        private static readonly object _toStringMemberCacheSyncRoot = new object();
        private static readonly Dictionary<IntPtr, MemberInfo[]> _toStringMemberCache = new Dictionary<IntPtr, MemberInfo[]>();
        [ChoHiddenMember]
        public static IEnumerable<MemberInfo> GetGetFieldsNProperties(this Type type, BindingFlags flags)
        {
            MemberInfo[] properties;
            if (!_toStringMemberCache.TryGetValue(type.TypeHandle.Value, out properties))
            {
                lock (_toStringMemberCacheSyncRoot)
                {
                    if (!_toStringMemberCache.TryGetValue(type.TypeHandle.Value, out properties))
                    {
                        properties = type.GetProperties(flags | BindingFlags.FlattenHierarchy)
                        .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null && p.GetGetMethod().GetParameters().Length == 0)
                        .Cast<MemberInfo>()
                        .Union(type.GetFields(flags | BindingFlags.FlattenHierarchy).Cast<MemberInfo>()).ToArray();

                        _toStringMemberCache.Add(type.TypeHandle.Value, properties);
                    }
                }
            }

            return properties;
        }

        public static bool IsReadOnlyMember(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo && ((FieldInfo)memberInfo).IsInitOnly)
                return true;
            if (memberInfo is PropertyInfo && (((PropertyInfo)memberInfo).GetSetMethod(true) == null || ((PropertyInfo)memberInfo).GetSetMethod(true).IsPrivate))
                return true;

            return false;
        }

        public static bool IsReadableMember(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
                return true;
            if (memberInfo is PropertyInfo && (((PropertyInfo)memberInfo).GetGetMethod(true) == null || ((PropertyInfo)memberInfo).GetGetMethod(true).IsPrivate))
                return false;

            return true;
        }

		public static string GetLogFileName(Type type)
		{
			return GetLogFileName(type, ChoReservedFileExt.Log);
		}

		public static string GetLogFileName(Type type, string extension)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			ChoGuard.ArgumentNotNullOrEmpty(extension, "Extension");

			return ChoPath.AddExtension(type.FullName, extension);
		}

		public static Type[] ConvertToTypes(object[] objects)
		{
			List<Type> types = new List<Type>();
			if (objects != null)
			{
				foreach (object constructorArg in objects)
					types.Add(constructorArg.GetType());
			}

			return types.ToArray();
		}

		public static bool IsRealProxyObject(Type objType)
		{
			return typeof(ChoInterceptableObject).IsAssignableFrom(objType);
		}

		public static bool IsConfigurableObject(Type objType)
		{
			return typeof(ChoConfigurableObject).IsAssignableFrom(objType);
		}

		public static string GetTypeName(object target)
		{
			if (target == null) return "UNKNOWN";
			if (target is Type)
				return _GetTypeName(target as Type);
			else
				return _GetTypeName(target.GetType());
		}

		private static string _GetTypeName(Type type)
		{
			if (type == typeof(int))
				return "int";
			else if (type == typeof(long))
				return "long";
			else if (type == typeof(double))
				return "double";
			else if (type == typeof(string))
				return "string";
			else if (type == typeof(bool))
				return "bool";
			else if (type == typeof(DateTime))
				return "datetime";
			else if (type == typeof(TimeSpan))
				return "timespan";
			else
				return type.FullName;
		}

		#endregion Other helper members (Public)
		
		public static Type[] ConvertToTypes(ParameterInfo[] parameterInfos)
		{
			List<Type> types = new List<Type>();
			if (parameterInfos != null)
			{
				foreach (ParameterInfo parameterInfo in parameterInfos)
					types.Add(parameterInfo.ParameterType);
			}

			return types.ToArray();
		}

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		private static void ExtractTypes(Type attributeType, ChoNotNullableArrayList types, ChoBufferProfileEx fileProfile, Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type == null) continue;
				object[] attributes = type.GetCustomAttributes(attributeType, false);
				if (attributes == null || attributes.Length == 0) continue;
				fileProfile.AppendLine(type.FullName);
				types.Add(type);
			}
		}

		private static TypeInfo GetTypeInfo(Type type)
		{
			lock (_typeInfos.SyncRoot)
			{
				if (!_typeInfos.Contains(type.FullName))
					_typeInfos.Add(type.FullName, new TypeInfo());
			}

			return (TypeInfo)_typeInfos[type.FullName];
		}

		private static Type[] ConvertToTypesArray(object[] args)
		{
			if (args == null) return new Type[0];

			Type[] types = new Type[args.Length];

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					types[i] = typeof(object);
				}
				else
				{
					types[i] = args[i].GetType();
				}
			}

			return types;
		}

		#endregion Shared Members (Private)
	}
}
