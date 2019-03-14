using System;
using Cinchoo.Core.IO;

namespace Cinchoo.Core.Runtime.Serialization
{
	public interface ILiteSerializable
	{
		void Deserialize(LiteBinaryReader reader);
		void Serialize(LiteBinaryWriter writer);
	}
}
