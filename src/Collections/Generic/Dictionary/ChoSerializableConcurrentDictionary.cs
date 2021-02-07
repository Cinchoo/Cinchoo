namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Text;
using System.Collections.Concurrent;

    #endregion NameSpaces

    [Serializable()]
    public class ChoSerializableConcurrentDictionary<TKey, TVal> : ConcurrentDictionary<TKey, TVal>, IXmlSerializable, ISerializable
    {
        #region Constants
        
        private const string DictionaryNodeName = "Dictionary";
        private const string ItemNodeName = "Item";
        private const string KeyNodeName = "Key";
        private const string ValueNodeName = "Value";
        
        #endregion

        #region Private Members

        private XmlSerializer keySerializer = null;
        private XmlSerializer valueSerializer = null;

        #endregion

        #region Constructors
        
        public ChoSerializableConcurrentDictionary()
        {
        }

        public ChoSerializableConcurrentDictionary(IDictionary<TKey, TVal> dictionary)
            : base(dictionary)
        {
        }

        public ChoSerializableConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public ChoSerializableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TVal>> collection, IEqualityComparer<TKey> comparer)
            : base(collection, comparer)
        {
        }

        public ChoSerializableConcurrentDictionary(int concurrencyLevel, int capacity)
            : base(concurrencyLevel, capacity)
        {
        }

        public ChoSerializableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TVal>> collection, IEqualityComparer<TKey> comparer)
            : base(concurrencyLevel, collection, comparer)
        {
        }

        public ChoSerializableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
            : base(concurrencyLevel, capacity, comparer)
        {
        }

        #endregion

        #region ISerializable Members

        protected ChoSerializableConcurrentDictionary(SerializationInfo info, StreamingContext context)
        {
            int itemCount = info.GetInt32("ItemCount");
            for (int i = 0; i < itemCount; i++)
            {
                KeyValuePair<TKey, TVal> kvp = (KeyValuePair<TKey, TVal>)info.GetValue(String.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
                this.TryAdd(kvp.Key, kvp.Value);
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemCount", this.Count);
            int itemIdx = 0;
            foreach (KeyValuePair<TKey, TVal> kvp in this)
            {
                info.AddValue(String.Format("Item{0}", itemIdx), kvp, typeof(KeyValuePair<TKey, TVal>));
                itemIdx++;
            }
        }

        #endregion

        #region IXmlSerializable Members

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            //writer.WriteStartElement(DictionaryNodeName);
            foreach (KeyValuePair<TKey, TVal> kvp in this)
            {
                writer.WriteStartElement(ItemNodeName);
                writer.WriteStartElement(KeyNodeName);
                KeySerializer.Serialize(writer, kvp.Key);
                writer.WriteEndElement();
                writer.WriteStartElement(ValueNodeName);
                ValueSerializer.Serialize(writer, kvp.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            //writer.WriteEndElement();
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            // Move past container
            if (!reader.Read())
            {
                throw new XmlException("Error in Deserialization of Dictionary");
            }

            //reader.ReadStartElement(DictionaryNodeName);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement(ItemNodeName);
                reader.ReadStartElement(KeyNodeName);
                TKey key = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement(ValueNodeName);
                TVal value = (TVal)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                this.TryAdd(key, value);
                reader.MoveToContent();
            }
            //reader.ReadEndElement();

            reader.ReadEndElement(); // Read End Element to close Read of containing node
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        #endregion

        #region Private Properties

        protected XmlSerializer ValueSerializer
        {
            get
            {
                if (valueSerializer == null)
                {
                    valueSerializer = new ChoNullNSXmlSerializer(typeof(TVal));
                }
                return valueSerializer;
            }
        }

        private XmlSerializer KeySerializer
        {
            get
            {
                if (keySerializer == null)
                {
                    keySerializer = new ChoNullNSXmlSerializer(typeof(TKey));
                }
                return keySerializer;
            }
        }

        #endregion
    }
}
