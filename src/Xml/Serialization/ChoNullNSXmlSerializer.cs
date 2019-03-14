namespace Cinchoo.Core.Xml.Serialization
{
    #region NameSpaces

    using System;
    using System.Xml.Serialization;
    using System.IO;
    using System.Xml;
    using System.Security.Policy;

    #endregion NameSpaces

    public class ChoNullNSXmlSerializer : XmlSerializer
    {
        #region Shared Data Members (Private)

        private static XmlSerializerNamespaces _xmlnsEmpty = new XmlSerializerNamespaces();

        #endregion

        #region Constructors

        static ChoNullNSXmlSerializer()
        {
            _xmlnsEmpty.Add("", "");
        }

        public ChoNullNSXmlSerializer(Type type)
            : base(type)
        {
        }

        public ChoNullNSXmlSerializer(XmlTypeMapping xmlTypeMapping)
            : base(xmlTypeMapping)
        {
        }

        public ChoNullNSXmlSerializer(Type type, string defaultNamespace)
            : base(type, defaultNamespace)
        {
        }

        public ChoNullNSXmlSerializer(Type type, Type[] extraTypes)
            : base(type, extraTypes)
        {
        }

        public ChoNullNSXmlSerializer(Type type, XmlAttributeOverrides overrides)
            : base(type, overrides)
        {
        }

        public ChoNullNSXmlSerializer(Type type, XmlRootAttribute root)
            : base(type, root)
        {
        }

        public ChoNullNSXmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
            : base(type, overrides, extraTypes, root, defaultNamespace)
        {
        }

        #endregion

        #region XmlSerialier Overrides (Public)

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.Stream.
        //
        // Parameters:
        //   stream:
        //     The System.IO.Stream used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public new void Serialize(Stream stream, object o)
        {
            base.Serialize(stream, o, _xmlnsEmpty);
        }
        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.TextWriter.
        //
        // Parameters:
        //   textWriter:
        //     The System.IO.TextWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        public new void Serialize(TextWriter textWriter, object o)
        {
            base.Serialize(textWriter, o, _xmlnsEmpty);
        }
        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.Xml.XmlWriter.
        //
        // Parameters:
        //   xmlWriter:
        //     The System.xml.XmlWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public new void Serialize(XmlWriter xmlWriter, object o)
        {
            base.Serialize(xmlWriter, o, _xmlnsEmpty);
        }

        #endregion
    }
}
