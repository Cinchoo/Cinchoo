namespace Cinchoo.Core
{
	#region NameSpaces

	using System;

    using Cinchoo.Core.Collections.Generic;
	using Cinchoo.Core.Factory;

    #endregion NameSpaces

    internal enum ObjectBuildState { None, Constructing, Intializing, Constructed }

	public class ChoObjectContext
	{
	}

	public static class ChoObjectManagementFactoryEx
	{
		#region Shared Data Members (Private)

		private static ChoDictionary<string, ChoObjectContext> _objectContextPool = ChoDictionary<string, ChoObjectContext>.Synchronized(new ChoDictionary<string, ChoObjectContext>());

		#endregion Shared Data Members (Private)

		#region Shared Data Members (Private)

		public static ChoObjectContext GetContext()
		{
			return GetContext(null);
		}

		public static ChoObjectContext GetContext(string objectContextName)
		{
			return null;
		}

		public static void RemoveContext(string objectContextName)
		{
		}

		public static void Clear()
		{
		}

		#endregion Shared Data Members (Private)
	}

	[ChoAppDomainEventsRegisterableType]
	public static class ChoObjectManagementFactory
	{
		#region Shared Members (Private)

		private static readonly object _padLock = new object();
		private static readonly ChoDictionary<Type, Object> _globalObjectCache = ChoDictionary<Type, Object>.Synchronized(new ChoDictionary<Type, object>());
		private static readonly ChoDictionary<Type, ObjectBuildState> _globalObjectBuildStateCache = ChoDictionary<Type, ObjectBuildState>.Synchronized(new ChoDictionary<Type, ObjectBuildState>());

		#endregion Shared Members (Private)

		#region Shared Members (Public)

		public static object GetObject(Type type)
		{
			return GetObject(type, false);
		}

		internal static object GetObject(Type type, bool beforeFieldInit)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			object instance;
			if (IsConstructing(type))
			{
				instance = New(type, null);
				if (instance != null)
					ChoObjectInitializer.Initialize(instance, beforeFieldInit);
			}
			else
			{
				_globalObjectCache.TryGetValue(type, out instance);
			}
			return instance;
		}

		public static object CreateInstance(string typeName)
		{
			return CreateInstance(typeName, false);
		}

		public static T CreateInstance<T>(string typeName)
		{
			return CreateInstance<T>(typeName, false);
		}

		internal static object CreateInstance(string typeName, bool beforeFieldInit)
		{
			return CreateInstance(ResolveType(typeName), beforeFieldInit);
		}

		internal static T CreateInstance<T>(string typeName, bool beforeFieldInit)
		{
			return CreateInstance<T>(ResolveType(typeName), beforeFieldInit);
		}

		public static object CreateInstance(Type objType)
		{
			return CreateInstance(objType, false);
		}

		public static T CreateInstance<T>()
		{
			return CreateInstance<T>(typeof(T), false);
		}

		public static T CreateInstance<T>(Type objType)
		{
			return CreateInstance<T>(objType, false);
		}

		internal static object CreateInstance(Type objType, bool beforeFieldInit)
		{
			return CreateInstance(objType, ChoObjectConstructionType.Prototype, beforeFieldInit);
		}

		internal static T CreateInstance<T>(bool beforeFieldInit)
		{
			return CreateInstance<T>(typeof(T), ChoObjectConstructionType.Prototype, beforeFieldInit);
		}

		internal static T CreateInstance<T>(Type objType, bool beforeFieldInit)
		{
			return CreateInstance<T>(objType, ChoObjectConstructionType.Prototype, beforeFieldInit);
		}

		public static object CreateInstance(string typeName, ChoObjectConstructionType defaultObjectConstructionType)
		{
			return CreateInstance(typeName, defaultObjectConstructionType, false);
		}

		public static T CreateInstance<T>(string typeName, ChoObjectConstructionType defaultObjectConstructionType)
		{
			return CreateInstance<T>(typeName, defaultObjectConstructionType, false);
		}

		internal static object CreateInstance(string typeName, ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit)
		{
			return CreateInstance(ResolveType(typeName), defaultObjectConstructionType, beforeFieldInit);
		}

		internal static T CreateInstance<T>(string typeName, ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit)
		{
			return CreateInstance<T>(ResolveType(typeName), defaultObjectConstructionType, beforeFieldInit);
		}

		public static object CreateInstance(Type objType, ChoObjectConstructionType defaultObjectConstructionType)
		{
			return CreateInstance(objType, defaultObjectConstructionType, false);
		}

		public static T CreateInstance<T>(ChoObjectConstructionType defaultObjectConstructionType)
		{
			return CreateInstance<T>(typeof(T), defaultObjectConstructionType, false);
		}

		public static T CreateInstance<T>(Type objType, ChoObjectConstructionType defaultObjectConstructionType)
		{
			return CreateInstance<T>(objType, defaultObjectConstructionType, false);
		}

		internal static object CreateInstance(Type objType, ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit)
		{
			return CreateInstance<object>(objType, defaultObjectConstructionType, beforeFieldInit);
		}

		internal static object CreateInstance(Type objType, ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit, out Exception exception)
		{
			return CreateInstance<object>(objType, defaultObjectConstructionType, beforeFieldInit, out exception);
		}

		internal static T CreateInstance<T>(ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit)
		{
			return CreateInstance<T>(typeof(T), defaultObjectConstructionType, beforeFieldInit);
		}

		internal static T CreateInstance<T>(Type objType, ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit)
		{
			Exception ex = null;
			T retObject = CreateInstance<T>(objType, defaultObjectConstructionType, beforeFieldInit, out ex);
			if (ex != null)
				throw new ChoApplicationException("Failed to create object.", ex);
			else
				return retObject;
		}

		internal static T CreateInstance<T>(Type objType, ChoObjectConstructionType defaultObjectConstructionType,
			bool beforeFieldInit, out Exception exception)
		{
			exception = null;

			ChoGuard.ArgumentNotNull(objType, "objType");

			T instance = default(T);
			ChoObjectConstructionType objectConstructionType = defaultObjectConstructionType;

			if (IsConstructing(objType))
			{
				instance = New<T>(objType, null);
				if (instance != null)
					ChoObjectInitializer.Initialize(instance, beforeFieldInit);
			}
			else
			{
				try
				{
					_globalObjectBuildStateCache[objType] = ObjectBuildState.Constructing;
					IChoCustomObjectFactory customObjectFactory = null;
					ChoObjectFactoryAttribute objectFactoryAttribute = ChoType.GetAttribute(objType, typeof(ChoObjectFactoryAttribute)) as ChoObjectFactoryAttribute;

					if (objectFactoryAttribute != null)
					{
						objectConstructionType = objectFactoryAttribute.ObjectConstructionType;
						customObjectFactory = objectFactoryAttribute.CustomObjectFactory;
					}

					switch (objectConstructionType)
					{
						case ChoObjectConstructionType.Singleton:
							//lock (_globalObjectCache.SyncRoot)
							//{
								if (_globalObjectCache.ContainsKey(objType))
									return (T)_globalObjectCache[objType];
								else
								{
									instance = New<T>(objType, customObjectFactory);
									_globalObjectCache[objType] = instance;
								}
							//}
							break;
						default:
							instance = New<T>(objType, customObjectFactory);
							break;
					}

					if (instance != null)
					{
						try
						{
							ChoObjectInitializer.Initialize(instance, beforeFieldInit);
						}
						catch (TypeInitializationException)
						{
							throw;
						}
						catch (ChoFatalApplicationException)
						{
							throw;
						}
						catch (Exception ex)
						{
							exception = ex;
						}
					}
				}
				finally
				{
					_globalObjectBuildStateCache[objType] = ObjectBuildState.Constructed;
				}
			}

			return instance;
		}

		#endregion Shared Members (Public)

		#region Shared Members (Internal)

		internal static object SyncRoot
		{
			get { return _padLock; }
		}

		internal static bool IsCached(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _globalObjectCache.ContainsKey(type);
		}

		internal static ObjectBuildState GetObjectState(Type type)
		{
			if (!_globalObjectBuildStateCache.ContainsKey(type)) return ObjectBuildState.None;
			return _globalObjectBuildStateCache[type];
		}

		internal static void SetObjectState(Type type, ObjectBuildState objectState)
		{
			SetObjectState(type, objectState, false);
		}

		internal static void SetObjectState(Type type, ObjectBuildState objectState, bool force)
		{
			if (!force && !_globalObjectCache.ContainsKey(type)) return;

			_globalObjectBuildStateCache[type] = objectState;
		}

		internal static bool IsConstructing(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _globalObjectBuildStateCache.ContainsKey(type) && _globalObjectBuildStateCache[type] == ObjectBuildState.Constructing;
		}

		internal static void SetInstance(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			SetInstance(obj.GetType(), obj);
		}

		//internal static void SetInstance(object obj, ChoObjectConstructionType objectConstructionType)
		//{
		//    if (obj == null)
		//        throw new ArgumentNullException("obj");

		//    SetInstance(obj.GetType(), obj, objectConstructionType);
		//}

		internal static void SetInstance(Type objType, object obj)
		{
			_globalObjectCache[objType] = obj;

			//ChoObjectFactoryAttribute objectFactoryAttribute = ChoType.GetAttribute(objType, typeof(ChoObjectFactoryAttribute)) as ChoObjectFactoryAttribute;
			//if (objectFactoryAttribute == null || objectFactoryAttribute.ObjectConstructionType != ChoObjectConstructionType.Singleton) return;

			//SetInstance(objType, obj, objectFactoryAttribute.ObjectConstructionType);
		}

		//internal static void SetInstance(Type objType, object obj, ChoObjectConstructionType objectConstructionType)
		//{
		//    if (objectConstructionType != ChoObjectConstructionType.Singleton) return;

		//    _globalObjectCache[objType] = obj;
		//}

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		private static Type ResolveType(string typeName)
		{
			Type type = ChoType.GetType(typeName);
			if (type == null)
				throw new ChoApplicationException(String.Format("Failed to find {0} type.", typeName));

			return type;
		}

		private static object New(Type objType, IChoCustomObjectFactory customObjectFactory)
		{
			return New<object>(objType, customObjectFactory);
		}

		private static T New<T>(Type objType, IChoCustomObjectFactory customObjectFactory)
		{
			if (customObjectFactory == null)
				return (T)ChoType.CreateInstance(objType);
				//return (T)ChoRealProxy.GetProxy(ChoType.CreateInstance(objType));
			else
				return (T)customObjectFactory.CreateInstance(objType);
		}

		#endregion Shared Members (Private)

		#region Other Members

		[ChoAppDomainUnloadMethod("Disposing the singleton objects...")]
		private static void DisposeAll()
		{
			if (_globalObjectCache != null && _globalObjectCache.Count > 0)
			{
				foreach (object obj in _globalObjectCache.ToValuesArray())
				{
					if (obj != null && obj is IDisposable)
					{
						((IDisposable)obj).Dispose();
					}
				}
			}
		}

		#endregion Other Members
	}
}
