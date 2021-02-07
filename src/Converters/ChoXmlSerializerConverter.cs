﻿namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using Cinchoo.Core.Xml.Serialization;
    using System.Text.RegularExpressions;
    using System.Xml;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ChoXmlSerializerConverterAttribute : ChoTypeConverterAttribute
    {
        public new object[] Parameters
        {
            get { throw new NotSupportedException(); }
        }

        public new object Parameter
        {
            get { throw new NotSupportedException(); }
        }

        private Type _targetObjectType;

        #region Constructors

        public ChoXmlSerializerConverterAttribute(Type targetObjectType)
        {
            if (targetObjectType == null)
                throw new NullReferenceException("Target Object Type.");

            _targetObjectType = targetObjectType;
        }

        public ChoXmlSerializerConverterAttribute(string targetObjectTypeName)
            : this(ChoType.GetType(targetObjectTypeName))
        {
        }

        #endregion Constructors

        public override object CreateInstance()
        {
            return new ChoXmlSerializerConverter(_targetObjectType);
        }
    }

    public class ChoXmlSerializerConverter : TypeConverter
    {
        private Type _targetType;

        public ChoXmlSerializerConverter(Type targetType)
        {
            if (targetType == null)
                throw new NullReferenceException("Missing target type.");

            _targetType = targetType;
        }

        #region TypeConverter Overrides

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        // If the type of the value to convert is string, parses the string 
        // and returns the integer to set the value of the property to. 
        // This example first extends the integer array that supplies the 
        // standard values collection if the user-entered value is not 
        // already in the array.
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null) return value;

            if (value is string)
            {
                string text = value as string;
                if (text.IsNullOrWhiteSpace()) return Activator.CreateInstance(_targetType);

                return ChoObject.XmlDeserialize(text, _targetType);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            return value.ToNullNSXml();
        }

        #endregion
    }
}
