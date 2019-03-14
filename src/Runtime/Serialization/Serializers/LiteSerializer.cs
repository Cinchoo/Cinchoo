using System;
using System.Reflection;
using Cinchoo.Core.IO;

namespace Cinchoo.Core.Runtime.Serialization
{
	#region ILiteBuiltinSerializer Interface

	internal interface ILiteBuiltinSerializer
	{
	}

	#endregion ILiteBuiltinSerializer Interface

	#region ILiteSerializer Interface

	public interface ILiteSerializer
	{
		Type Type { get; }
		short Handle { get; set; }
		object Read(LiteBinaryReader reader);
		void Write(LiteBinaryWriter writer, object obj);
	}

	#endregion ILiteSerializer Interface

	#region LiteSerializer Class

	public class LiteSerializer : ILiteSerializer
	{
		#region Instance Data Members (Private)

		private short _handle;
		private Type _type;

		#endregion

		#region Constructors

		public LiteSerializer(Type type)
		{
			if (type == null) throw new NullReferenceException("Type can't be null.");

			_type = type;
		}

		#endregion Constructors

		#region ILiteSerializer Members

		public Type Type
		{
			get { return _type; }
		}

		public short Handle
		{
			get { return _handle; }
			set { _handle = value; }
		}

		public virtual object Read(LiteBinaryReader reader)
		{
			return null;
		}

		public virtual void Write(LiteBinaryWriter writer, object obj)
		{
		}

		#endregion

		#region Instance Members (Public)

		public virtual object CreateInstance()
		{
			return Activator.CreateInstance(Type,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
				null, null, null, null);
		}

		#endregion
	}

	#endregion LiteSerializer Class

	#region LiteContextBoundSerializer Class

	public abstract class LiteContextBoundSerializer : LiteSerializer
	{
		#region Constructors

		protected LiteContextBoundSerializer(Type type) : base(type)
		{
		}

		#endregion Constructors

		#region Instance Members (Public)

		public abstract object ReadFrom(LiteBinaryReader reader, object obj);
		public abstract void WriteTo(LiteBinaryWriter writer, object obj);
		public virtual object New(LiteBinaryReader reader)
		{
			return base.CreateInstance();
		}

		#endregion

		#region ILiteSerializer Overrides

		public sealed override object Read(LiteBinaryReader reader)
		{
			int cookie = reader.ReadInt32();
			object obj = reader.SerializationContext.GetObject(cookie);
			if (obj == null)
			{
				bool known = false;
				obj = New(reader);
				if (obj != null)
				{
					reader.SerializationContext.CacheObjectForRead(obj);
					known = true;
				}
				obj = ReadFrom(reader, obj);
				if (!known)
					reader.SerializationContext.CacheObjectForRead(obj);
			}
			return obj;
		}

		public sealed override void Write(LiteBinaryWriter writer, object obj)
		{
			int cookie = writer.SerializationContext.GetCookie(obj);
			if (cookie != LiteSerializationContext.InvalidCookie)
			{
				writer.Write(cookie);
				return;
			}

			cookie = writer.SerializationContext.CacheObjectForWrite(obj);
			writer.Write(cookie);
			WriteTo(writer, obj);
		}

		#endregion
	}

	#endregion LiteContextBoundSerializer Class
}
