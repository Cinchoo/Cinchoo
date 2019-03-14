namespace Cinchoo.Core.Xml.Serialization
{
    #region NameSpaces

    using System;
    using System.Xml.Serialization;

    #endregion NameSpaces

    [Serializable]
    [ChoTypeConverter(typeof(ChoCDATAToStringConverter))]
    public class ChoCDATA : ChoEquatableObject<ChoCDATA>, IXmlSerializable, IFormattable
    {
        #region Instance Data Members (Private)

        private string _text;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCDATA()
        {
        }

        public ChoCDATA(string text)
        {
            this._text = text;
        }

        #endregion

        #region Properties

        public string Value
        {
            get { return _text; }
        }

        #endregion

        #region IXmlSerializable Members

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(this._text);
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            _text = reader.ReadElementContentAsString();
        }

        #endregion

        public override string ToString()
        {
            return String.Format("<![CDATA[{0}]]>", _text);
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }

        #endregion

        #region IEquatable<ChoCDATA> Members

        public override bool Equals(ChoCDATA other)
        {
            if (object.ReferenceEquals(other, null))
                return false;

            return other.Value == Value;
        }

        #endregion
    }
}
