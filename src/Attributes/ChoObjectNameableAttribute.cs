namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    #endregion NameSpaces

    public abstract class ChoObjectNameableAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private string _name;

        #endregion Instance Data Members (Private)
        
        #region Constructors

        public ChoObjectNameableAttribute(string name)
        {
            Name = name;
        }

        public ChoObjectNameableAttribute(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            Name = type.Name;
        }

        protected ChoObjectNameableAttribute()
        {
        }

        #endregion Constructors

        #region IChoObjectNameable Members

        [XmlIgnore]
        public string Name
        {
            get { return _name; }
            set
            {
                ChoGuard.ArgumentNotNull(value, "Name");
                _name = value.SplitNTrim()[0];
            }
        }

        #endregion

        #region Shared Members (Public)

        public static string GetName(Type objType)
        {
            return GetName(objType, ChoTypeNameSpecifier.FullName);
        }

        public static string GetName(Type objType, ChoTypeNameSpecifier typeNameSpecifier)
        {
            ChoGuard.ArgumentNotNull(objType, "Type");
            ChoObjectNameableAttribute objectNameableAttribute = ChoType.GetAttribute(objType, typeof(ChoObjectNameableAttribute)) as ChoObjectNameableAttribute;

            string key = null;
            if (objectNameableAttribute != null)
                key = objectNameableAttribute.Name;
            else
                key = objType.GetName(typeNameSpecifier);

            return key;

        }

        #endregion Shared Members (Public)
    }
}
