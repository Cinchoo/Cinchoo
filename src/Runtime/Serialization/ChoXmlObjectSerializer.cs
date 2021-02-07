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
    public class ChoXmlObjectSerializer : IChoObjectSerializer
    {
        public object Deserialize(byte[] payload)
        {
            if (payload == null)
                return null;
            else
                return (Encoding.ASCII.GetString(payload)).ToObjectFromXml();
        }

        public byte[] Serialize(object value)
        {
            if (value != null)
                return Encoding.ASCII.GetBytes(value.ToNullNSXmlWithType());
            else
                return null;
        }
    }
}
