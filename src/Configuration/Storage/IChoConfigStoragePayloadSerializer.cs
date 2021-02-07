using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Configuration
{
    public interface IChoConfigStoragePayloadSerializer
    {
        Dictionary<string, object> Deserialize(object payload);
        object Serialize(Dictionary<string, object> data);
    }
}
