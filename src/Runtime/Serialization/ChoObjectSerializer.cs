using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Runtime.Serialization
{
    public class ChoObjectSerializer
    {
        public readonly IChoObjectSerializer Serializer = null;

        public ChoObjectSerializer(Type serializerType)
        {
            ChoGuard.ArgumentNotNull(serializerType, "SerializerType");

            if (!typeof(IChoObjectSerializer).IsAssignableFrom(serializerType))
                throw new ArgumentException("Passed type is not a valid Serializer type.");

            Serializer = Activator.CreateInstance(serializerType) as IChoObjectSerializer;
        }

        public byte[] Serialize(object target)
        {
            if (target == null)
                return null;

            return Serializer.Serialize(target);
        }

        public object Deserialize(byte[] payload)
        {
            if (payload == null)
                return null;

            return Serializer.Deserialize(payload);
        }
    }

    public class ChoObjectSerializer<T> : ChoObjectSerializer where T: IChoObjectSerializer
    {
        public ChoObjectSerializer() : base(typeof(T))
        {
        }
    }
}
