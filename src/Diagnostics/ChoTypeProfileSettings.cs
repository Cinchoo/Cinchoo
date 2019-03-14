namespace eSquare.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using eSquare.Core.Attributes;
    using eSquare.Core.Interfaces;
    using eSquare.Core.Validation.Validators;
    using eSquare.Core.Attributes.Validators;
    using eSquare.Core.Configuration;
    using eSquare.Core.Formatters;
    using eSquare.Core.Converters;
    using eSquare.Core.Validation;
    using eSquare.Core.Types;

    #endregion NameSpaces

    [Serializable]
    [ChoTypeFormatter("Type Profile Settings")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    [ChoXmlSerializationConfigurationElementMap("eSquare/typeProfileSettings", WatchChange = true, IgnoreError = true, Defaultable = true)]
    [XmlRoot("typeProfileSettings")]
    public class ChoTypeProfileSettings
    {
    }
}
