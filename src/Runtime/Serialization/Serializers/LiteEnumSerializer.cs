using System;

using Cinchoo.Core.IO;

namespace Cinchoo.Core.Runtime.Serialization
{
	public class LiteEnumSerializer : LiteSerializer, ILiteBuiltinSerializer
	{
		#region Constructors

		public LiteEnumSerializer(Type type) : base(type)
		{
		}

		#endregion

		#region LiteSerializer Overrides

		public override object Read(LiteBinaryReader reader)
		{
//			// Find an appropriate surrogate by handle
//			short handle = reader.ReadInt16();
//			ILiteSerializationSurrogate typeSurr = reader.Context.SurrogateSelector.GetSurrogateForTypeHandle(handle);
//			return Enum.ToObject(ActualType, typeSurr.Read(reader));
			return null;
		}
		public override void Write(LiteBinaryWriter writer, object graph)
		{
//			Type enumType = Enum.GetUnderlyingType(ActualType);
//			ILiteSerializationSurrogate typeSurr = writer.Context.SurrogateSelector.GetSurrogateForType(enumType);
//			writer.Write(typeSurr.TypeHandle);
//			typeSurr.Write(writer, graph);
		}

		#endregion LiteSerializer Overrides
	}
}
