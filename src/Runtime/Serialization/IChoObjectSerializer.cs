using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Runtime.Serialization
{
    public interface IChoObjectSerializer
    {
        object Deserialize(byte[] payload);
        byte[] Serialize(object target);
    }
}
