using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core.Collections.Generic
{
    [Serializable]
    [XmlType(TypeName = "KeyValuePair")]
    public struct ChoKeyValuePair<K, V>
    {
        public K Key
        { get; set; }

        public V Value
        { get; set; }
    }
}
