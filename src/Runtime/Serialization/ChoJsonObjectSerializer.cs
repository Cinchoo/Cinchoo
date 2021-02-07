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
    //public class ChoJsonsObjectSerializer : IChoObjectSerializer
    //{
    //    public object Deserialize(object payload)
    //    {
    //        if (payload is string)
    //            return ((string)payload).ToObjectFromXml();
    //        else
    //            return null;
    //    }

    //    public object Serialize(object value)
    //    {
    //        if (value != null)
    //            return value.ToNullNSXmlWithType();
    //        else
    //            return null;
    //    }
    //}
}
