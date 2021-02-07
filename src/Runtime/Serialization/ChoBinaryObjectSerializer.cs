using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cinchoo.Core.Runtime.Serialization
{
    public class ChoBinaryObjectSerializer : IChoObjectSerializer
    {
        public object Deserialize(byte[] payload)
        {
            if (payload == null)
                return null;
            else
                return ChoObject.Deserialize(payload);
        }

        public byte[] Serialize(object value)
        {
            if (value != null)
                return ChoObject.Serialize(value);
            else
                return null;
        }
    }
}
