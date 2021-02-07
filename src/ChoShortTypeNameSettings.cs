namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [Serializable]
    [ChoDoObjectValidationAfterInitialization(false)]
    public class ChoTypsShortName : IChoObjectInitializable
    {
        [ChoNotNullOrEmptyValidator()]
        [XmlAttribute("type")]
        public string TypeName;

        [ChoNotNullOrEmptyValidator()]
        [ChoRegexStringValidator(@"^\w\w+$")]
        [XmlAttribute("shortName")]
        public string TypeShortName;

        [XmlAttribute("override")]
        public bool Override;

        private Type _type;
        public Type Type
        {
            get { return _type; }
        }

        #region Object Overrides

        public override string ToString()
        {
            ChoValidationResults results = ChoValidation.Validate(this);
            if (results.Count == 0)
                return String.Format("SUCCESS - ShortName: {0}, Type: {1}, Override: {3} [TypeFound: {2}]", TypeShortName, TypeName, Type != null, Override);
            else
                return String.Format("ERROR - ShortName: {0}, Type: {1}, Override: {3} [TypeFound: {2}], {4}[{4}{5}]", 
                    TypeShortName, TypeName, Type != null, Override, Environment.NewLine, results.ToString().Indent());
        }

        #endregion Object Overrides

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            if (beforeFieldInit) return false;

            _type = ChoType.GetType(TypeName);

            return false;
        }

        #endregion
    }

    [Serializable]
    [ChoTypeFormatter("ShortTypeName Settings")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    [ChoConfigurationSection("cinchoo/typeShortNames")]
    [XmlRoot("typeShortNames")]
    public class ChoShortTypeNameSettings : IChoObjectInitializable
    {
        private static readonly ChoShortTypeNameSettings _instance = new ChoShortTypeNameSettings();

        #region Instance Data Members (Public)

        [XmlElement("typeShortName", typeof(ChoTypsShortName))]
        [ChoMemberFormatter("TypeShortNames", Formatter = typeof(ChoArrayToStringFormatter))]
        public ChoTypsShortName[] TypsShortNames;

        [XmlIgnore]
        [ChoIgnoreMemberFormatter]
        [ChoTypeConverter(typeof(ChoIgnoreInvalidArrayItemsConvertor))]
        public ChoTypsShortName[] ValidTypsShortNames;

        #endregion Instance Data Members (Public)

        #region Shared Properties

        public static ChoShortTypeNameSettings Me
        {
            get { return _instance; }
        }

        #endregion Shared Properties

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            if (beforeFieldInit) return false;

            ValidTypsShortNames = TypsShortNames;

            return true;
        }

        #endregion
    }
}
