using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    public struct ChoNullable<T> : IXmlSerializable, IEquatable<ChoNullable<T>>
        where T : struct
    {
        public static ChoNullable<T> Default = new ChoNullable<T>();
        private bool _hasValue;
        private T _value;

        public ChoNullable(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public ChoNullable(T? value)
        {
            if (value != null)
            {
                _value = value.Value;
                _hasValue = true;
            }
            else
            {
                _value = default(T);
                _hasValue = false;
            }
        }

        //public ChoNullable()
        //{
        //    _hasValue = false;
        //}

        [XmlIgnore]
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException();
                return _value;
            }
            set
            {
                _value = value;
                _hasValue = true;
            }
        }

        [XmlIgnore]
        public bool HasValue
        {
            get
            {
                return _hasValue;
            }
        }

        public T GetValueOrDefault()
        {
            return _value;
        }
        public T GetValueOrDefault(T defaultValue)
        {
            return HasValue ? _value : defaultValue;
        }

        public static explicit operator T(ChoNullable<T> value)
        {
            return value.Value;
        }

        public static implicit operator ChoNullable<T>(T value)
        {
            return new ChoNullable<T>(value);
        }

        public static implicit operator ChoNullable<T>(T? value)
        {
            return new ChoNullable<T>(value);
        }

        //public override bool Equals(object other)
        //{
        //    if (!HasValue)
        //        return (other == null);
        //    if (other == null)
        //        return false;
        //    return _value.Equals(other);
        //}

        public override string ToString()
        {
            if (!HasValue)
                return String.Empty;
            return _value.ToString();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string text = reader.ReadElementContentAsString();
            if (!text.IsNullOrWhiteSpace() &&
                String.Compare(text, "{NULL}", true) != 0
                && String.Compare(text, "{NIL}", true) != 0)
            {
                Value = (T)ChoConvert.ConvertTo(text.Evaluate(), typeof(T));
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (HasValue)
                writer.WriteString(Value.ToString());
            else
                writer.WriteString("{NIL}");
        }

        #region Object Overrrides

        public override bool Equals(object other)
        {
            if (!(other is ChoNullable<T>)) return false;

            return Equals((ChoNullable<T>)other);
        }

        public bool Equals(ChoNullable<T> other)
        {
            if (!other.HasValue && !HasValue)
                return true;

            if ((other.HasValue && !HasValue)
                || (!other.HasValue && HasValue))
                return false;

            return ChoObject.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            if (!HasValue)
                return 0;
            return _value.GetHashCode();
        }

        #endregion Object Overrides

        #region Operator Overloads

        public static bool operator ==(ChoNullable<T> a, ChoNullable<T> b)
        {
            return ChoObject.Equals<ChoNullable<T>>(a, b);
        }

        public static bool operator !=(ChoNullable<T> a, ChoNullable<T> b)
        {
            return !ChoObject.Equals<ChoNullable<T>>(a, b);
        }

        #endregion Operator Overloads 
    }
}
