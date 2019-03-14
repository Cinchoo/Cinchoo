namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Diagnostics;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;

	#endregion NameSpaces

	[DebuggerDisplay("Value = {ToString()}")]
    [Serializable]
    [XmlRoot("ChoTuple")]
	public class ChoTuple<T1, T2> : IXmlSerializable
	{
		public static readonly ChoTuple<T1, T2> Default = default(ChoTuple<T1, T2>);

        public ChoTuple()
        {
        }

		public ChoTuple(T1 first, T2 second) //: this()
		{
			First = first;
			Second = second;
		}

		public T1 First { get; set; }
		public T2 Second { get; set; }

		public override string ToString()
		{
			return string.Concat(First, ",", Second);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ChoTuple<T1, T2>)) return false;

			ChoTuple<T1, T2> tuple = obj as ChoTuple<T1, T2>;
			return EqualityComparer<T1>.Default.Equals(tuple.First, First) && EqualityComparer<T2>.Default.Equals(tuple.Second, Second);
		}

		public override int GetHashCode()
		{
			int hashFirst = First == null ? 0 : First.GetHashCode();
			int hashSecond = Second == null ? 0 : Second.GetHashCode();
			return hashFirst ^ hashSecond;
		}

        #region IXmlSerializable Members

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            Boolean isEmptyElement = reader.IsEmptyElement; //First
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                isEmptyElement = reader.IsEmptyElement; //First Content
                reader.ReadStartElement();
                if (!isEmptyElement)
                {
                    XmlSerializer serializer1 = new XmlSerializer(typeof(T1));
                    First = (T1)serializer1.Deserialize(reader);
                    reader.ReadEndElement();
                }
            }

            isEmptyElement = reader.IsEmptyElement; //Second
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                isEmptyElement = reader.IsEmptyElement; //Second Content
                reader.ReadStartElement();
                if (!isEmptyElement)
                {
                    XmlSerializer serializer2 = new XmlSerializer(typeof(T2));
                    Second = (T2)serializer2.Deserialize(reader);
                    reader.ReadEndElement();
                }
            }
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ChoTuple");

            writer.WriteStartElement("First");
            if (First != null)
                writer.WriteRaw(First.ToNullNSXml());
            writer.WriteEndElement();

            writer.WriteStartElement("Second");
            if (Second != null)
                writer.WriteRaw(Second.ToNullNSXml());
            writer.WriteEndElement();
            
            writer.WriteEndElement();
        }

        #endregion
    }

	[DebuggerDisplay("Value = {ToString()}")]
	public class ChoTuple<T1, T2, T3>
	{
		public static readonly ChoTuple<T1, T2, T3> Default = default(ChoTuple<T1, T2, T3>);

		public ChoTuple(T1 first, T2 second, T3 third) //: this()
		{
			First = first;
			Second = second;
			Third = third;
		}

		public T1 First { get; internal set; }
		public T2 Second { get; internal set; }
		public T3 Third { get; internal set; }

		public override string ToString()
		{
			return string.Concat(First, ",", Second, ",", Third);
		}
	}
}
