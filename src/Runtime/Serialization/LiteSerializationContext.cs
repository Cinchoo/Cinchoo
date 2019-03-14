#region NameSpaces

using System;
using System.Collections;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteSerializationContext
	{
		#region Constants

		internal const int InvalidCookie = -1;
		public const bool UltraCompact = true;

		#endregion

		#region Instance Data Members (Private)

		private LiteSerializerFactory _serializerFactory;
		private ArrayList _objectList = new ArrayList();
		private Hashtable _cookieList = new Hashtable();

		#endregion

		#region Constructors

		public LiteSerializationContext()
		{
			_serializerFactory = LiteSerializerFactory.Default;
		}

		public LiteSerializationContext(LiteSerializerFactory serializerFactory)
		{
			if (serializerFactory == null)
				throw new NullReferenceException("Serializer Factory can't be null.");

			_serializerFactory = serializerFactory;
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public LiteSerializerFactory SerializerFactory
		{
			get { return _serializerFactory; }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Public)

		public int GetCookie(object obj)
		{
			if (_cookieList.ContainsKey(obj))
				return (int)_cookieList[obj];

			return InvalidCookie;
		}

		public object GetObject(int key)
		{
			if (key > LiteSerializationContext.InvalidCookie && key < _objectList.Count)
				return _objectList[key];

			return null;
		}

		public int CacheObjectForRead(object obj)
		{
			int cookie = _objectList.Count;
			_objectList.Add(obj);
			return cookie;
		}

		public int CacheObjectForWrite(object obj)
		{
			int cookie = _cookieList.Count;
			_cookieList[obj] = cookie;
			return cookie;
		}

		#endregion Instance Members (Public)
	}
}
