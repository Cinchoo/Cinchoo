#region NameSpaces

using System;
using System.Runtime.Serialization.Formatters.Binary;

using Cinchoo.Core.IO;

#endregion NameSpaces

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteArraySerializer : LiteContextBoundSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteArraySerializer(Type type) : base(type)
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object New(LiteBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return Array.CreateInstance(Type.GetElementType(), length);
		}

		public override object ReadFrom(LiteBinaryReader reader, object obj)
		{
			Array array = (Array)obj;
			for (int i = 0; i < array.Length; i++)
				array.SetValue(reader.ReadObject(), i);
			return array;
		}

		public override void WriteTo(LiteBinaryWriter writer, object obj)
		{
			Array array = (Array)obj;
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
				writer.WriteObject(array.GetValue(i));
		}

		#endregion
	}
}
