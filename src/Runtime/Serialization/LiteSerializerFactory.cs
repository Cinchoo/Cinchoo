#region NameSpaces

using System;
using System.Collections;

using Cinchoo.Core;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Configuration;
using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSerializerFactory
	{
		#region Shared Data Members (Private)

		private static readonly ILiteSerializer _nullSerializer = new LiteNullSerializer();
		private static readonly ILiteSerializer _defaultSerializer = new LiteObjectSerializer(typeof(object));
		private static LiteSerializerFactory _default = new LiteSerializerFactory();

		private IDictionary _typeSerializerMap = Hashtable.Synchronized(new Hashtable());
		private IDictionary _handleSerializerMap = Hashtable.Synchronized(new Hashtable());
		private short _newhandle = short.MinValue;

		#endregion

		#region Constructors

		public LiteSerializerFactory() : this(true)
		{
		}

		public LiteSerializerFactory(bool useBuiltinSerializers)
		{
			RegisterSerializer(_nullSerializer);
			RegisterSerializer(_defaultSerializer);

			if (useBuiltinSerializers) RegisterBuiltinSerializers();

			using (ChoBufferProfileEx lProfile = new ChoBufferProfileEx("Finding Serializer components..."))
			{
				Type[] liteSerializableTypes = ChoType.GetTypes(typeof(LiteSerializableAttribute), LiteSerializationSettings.Me.Paths, lProfile);
				if (liteSerializableTypes != null)
				{
					foreach (Type liteSerializableType in liteSerializableTypes)
					{
						if (!typeof(ILiteSerializable).IsAssignableFrom(liteSerializableType))
						{
							if (!LiteSerializationSettings.Me.Silent)
								throw new ApplicationException(String.Format("{0} type is not ILiteSerializable. Should derive from ILiteSerializable interface.", liteSerializableType.Name));
							else
								continue;
						}
						
						using (ChoBufferProfileEx typeProfile = new ChoBufferProfileEx(String.Format("Registering {0} serializer type...", liteSerializableType.Name), lProfile))
						{
							try
							{
								RegisterLiteSerializableType(liteSerializableType);
							}
							catch (Exception ex)
							{
								typeProfile.Append(ex.Message);
							}
						}
					}
				}

				Type[] liteSerializers = ChoType.GetTypes(typeof(LiteSerializerAttribute), LiteSerializationSettings.Me.Paths, lProfile);
				if (liteSerializers != null)
				{
					foreach (Type liteSerializer in liteSerializers)
					{
						if (!typeof(LiteSerializer).IsAssignableFrom(liteSerializer))
						{
							if (!LiteSerializationSettings.Me.Silent)
								throw new ApplicationException(String.Format("{0} type is not LiteSerializer. Should derive from LiteSerializer class.", liteSerializer.Name));
							else
								continue;
						}
						
						using (ChoBufferProfileEx typeProfile = new ChoBufferProfileEx(String.Format("Registering {0} serializer...", liteSerializer.Name), lProfile))
						{
							try
							{
                                RegisterSerializer(ChoObjectManagementFactory.CreateInstance(liteSerializer) as LiteSerializer);
							}
							catch (Exception ex)
							{
								typeProfile.Append(ex.Message);
							}
						}
					}
				}

				//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoType.GetLogFileName(typeof(LiteSerializerFactory)), lProfile.ToString(), true);
			}
		}

		#endregion Constructors

		#region Shared Members (Public)

		public static LiteSerializerFactory Default
		{
			get { return _default; }
		}

		#endregion

		#region Instance Members (Internal)

		internal ILiteSerializer GetSerializerForObject(object obj)
		{
			if (obj == null) return _nullSerializer;
			return GetSerializerForType(obj.GetType());
		}

		internal ILiteSerializer GetSerializerForType(Type type)
		{
			ILiteSerializer serializer = (ILiteSerializer)_typeSerializerMap[type.FullName];
			if (serializer == null) serializer = _defaultSerializer;
			return serializer;
		}

		internal ILiteSerializer GetSerializerForTypeStrict(Type type)
		{
			return (ILiteSerializer)_typeSerializerMap[type.FullName];
		}

		internal ILiteSerializer GetSerializerForHandle(short handle)
		{
			ILiteSerializer serializer = (ILiteSerializer)_handleSerializerMap[handle];
			if (serializer == null) serializer = _defaultSerializer;
			return serializer;
		}

		public void RegisterBuiltinSerializers()
		{
			RegisterSerializer(new LiteBooleanSerializer());
			RegisterSerializer(new LiteByteSerializer());
			RegisterSerializer(new LiteCharSerializer());
			RegisterSerializer(new LiteSingleSerializer());
			RegisterSerializer(new LiteDoubleSerializer());
			RegisterSerializer(new LiteDecimalSerializer());
			RegisterSerializer(new LiteInt16Serializer());
			RegisterSerializer(new LiteInt32Serializer());
			RegisterSerializer(new LiteInt64Serializer());
			RegisterSerializer(new LiteStringSerializer());
			RegisterSerializer(new LiteDateTimeSerializer());
			RegisterSerializer(new LiteGuidSerializer());
			RegisterSerializer(new LiteSByteSerializer());
			RegisterSerializer(new LiteUInt16Serializer());
			RegisterSerializer(new LiteUInt32Serializer());
			RegisterSerializer(new LiteUInt64Serializer());

			RegisterSerializer(new LiteObjectArraySerializer());
			RegisterSerializer(new LiteBooleanArraySerializer());
			RegisterSerializer(new LiteByteArraySerializer());
			RegisterSerializer(new LiteCharArraySerializer());
			RegisterSerializer(new LiteSingleArraySerializer());
			RegisterSerializer(new LiteDoubleArraySerializer());
			RegisterSerializer(new LiteDecimalArraySerializer());
			RegisterSerializer(new LiteInt16ArraySerializer());
			RegisterSerializer(new LiteInt32ArraySerializer());
			RegisterSerializer(new LiteInt64ArraySerializer());
			RegisterSerializer(new LiteStringArraySerializer());
			RegisterSerializer(new LiteDateTimeArraySerializer());
			RegisterSerializer(new LiteGuidArraySerializer());
			RegisterSerializer(new LiteSByteArraySerializer());
			RegisterSerializer(new LiteUInt16ArraySerializer());
			RegisterSerializer(new LiteUInt32ArraySerializer());
			RegisterSerializer(new LiteUInt64ArraySerializer());

			RegisterSerializer(new LiteArraySerializer(typeof(Array)));
			RegisterSerializer(new LiteIListSerializer(typeof(ArrayList)));
			RegisterSerializer(new LiteDictionarySerializer(typeof(Hashtable)));
			RegisterSerializer(new LiteDictionarySerializer(typeof(SortedList)));
		}

		#endregion Instance Members (Internal)

		#region Instance Members (Public)

		public bool RegisterSerializer(ILiteSerializer serializer)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer");

			return RegisterSerializer(serializer, ++_newhandle);
		}

		public bool RegisterSerializer(ILiteSerializer serializer, short handle)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer");

			lock (_typeSerializerMap.SyncRoot)
			{
				if (_handleSerializerMap.Contains(handle))
					throw new ArgumentException("Specified type handle is already registered.");
				if (GetSerializerForTypeStrict(serializer.Type) != null)
					throw new ArgumentException("Type '" + serializer.Type.FullName + "' is already registered");

				if (!_typeSerializerMap.Contains(serializer.Type.FullName))
				{
					serializer.Handle = handle;
					_typeSerializerMap.Add(serializer.Type.FullName, serializer);
					_handleSerializerMap.Add(serializer.Handle, serializer);
					return true;
				}
			}
			return false;
		}

		public void UnregisterSerializer(ILiteSerializer serializer)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer");

			lock (_typeSerializerMap.SyncRoot)
			{
				_typeSerializerMap.Remove(serializer.Type.FullName);
				_handleSerializerMap.Remove(serializer.Handle);
			}
		}

		public void UnregisterAllSerializers()
		{
			lock (_typeSerializerMap.SyncRoot)
			{
				_typeSerializerMap.Clear();
				_handleSerializerMap.Clear();

				_newhandle = short.MinValue;
				RegisterSerializer(_nullSerializer);
				RegisterSerializer(_defaultSerializer);
			}
		}

		public void RegisterLiteSerializableType(Type type, short typeHandle)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (GetSerializerForTypeStrict(type) != null)
				throw new ArgumentException("Type '" + type.FullName + "' is already registered");

			ILiteSerializer serializer = null;
			if (typeof(IDictionary).IsAssignableFrom(type))
				serializer = new LiteDictionarySerializer(type);
			else if (type.IsArray)
				serializer = new LiteArraySerializer(type);
			else if (typeof(IList).IsAssignableFrom(type))
				serializer = new LiteIListSerializer(type);
			else if (typeof(ILiteSerializable).IsAssignableFrom(type))
				serializer = new LiteILiteSerializableSerializer(type);
			else if (typeof(Enum).IsAssignableFrom(type))
				serializer = new LiteEnumSerializer(type);

			if (serializer == null)
				throw new ArgumentException("No appropriate serializer found for type " + type.FullName);

			RegisterSerializer(serializer, typeHandle);
		}

		public void RegisterLiteSerializableType(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (GetSerializerForTypeStrict(type) != null)
				throw new ArgumentException("Type '" + type.FullName + "' is already registered");

			ILiteSerializer serializer = null;
			if (typeof(IDictionary).IsAssignableFrom(type))
				serializer = new LiteDictionarySerializer(type);
			else if (type.IsArray)
				serializer = new LiteArraySerializer(type);
			else if (typeof(IList).IsAssignableFrom(type))
				serializer = new LiteIListSerializer(type);
			else if (typeof(ILiteSerializable).IsAssignableFrom(type))
				serializer = new LiteILiteSerializableSerializer(type);
			else if (typeof(Enum).IsAssignableFrom(type))
				serializer = new LiteEnumSerializer(type);

			if (serializer == null)
				throw new ArgumentException("No appropriate serializer found for type " + type.FullName);

			RegisterSerializer(serializer);
		}


		public void UnregisterLiteSerializableType(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (GetSerializerForTypeStrict(type) == null) return;

			if (type.IsArray ||
				typeof(IDictionary).IsAssignableFrom(type) ||
				typeof(IList).IsAssignableFrom(type) ||
				typeof(ILiteSerializable).IsAssignableFrom(type) ||
				typeof(Enum).IsAssignableFrom(type))
			{
				ILiteSerializer serializer = GetSerializerForTypeStrict(type);
				UnregisterSerializer(serializer);
			}
		}

		#endregion
	}
}
