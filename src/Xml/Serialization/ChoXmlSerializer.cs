namespace eSquare.Core.Xml.Serialization
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Reflection;
    using System.Xml.Serialization;

    using eSquare.Core.Converters;

    #endregion NameSpaces

    public class ChoXmlSerializer : XmlSerializer
    {
        #region Constructors

        public ChoXmlSerializer(Type type)
            : base()
        {
            this.T
        }

        #endregion

        #region XmlSerizalier Overrides

        protected override object Deserialize(XmlSerializationReader reader)
        {
            object target = base.Deserialize(reader);

            //Do the post member conversion logic here....

            return target;
        }

        #endregion XmlSerializer Overrides
    }
}
