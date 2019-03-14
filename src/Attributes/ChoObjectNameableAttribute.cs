namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public enum ChoDefaultObjectKey { Name, FullName, AssemblyQualifiedName }

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

        public string Name
        {
            get { return _name; }
            protected set
            {
                ChoGuard.ArgumentNotNull(value, "Name");
                _name = value;
            }
        }

        #endregion

        #region Shared Members (Public)

        public static string GetName(Type objType)
        {
            return GetName(objType, ChoDefaultObjectKey.FullName);
        }

        public static string GetName(Type objType, ChoDefaultObjectKey typeName)
        {
            ChoGuard.ArgumentNotNull(objType, "Type");
            ChoObjectNameableAttribute objectNameableAttribute = ChoType.GetAttribute(objType, typeof(ChoObjectNameableAttribute)) as ChoObjectNameableAttribute;

            string key = null;
            if (objectNameableAttribute != null)
                key = objectNameableAttribute.Name;
            else
            {
                switch (typeName)
                {
                    case ChoDefaultObjectKey.FullName:
                        key = objType.FullName;
                        break;
                    case ChoDefaultObjectKey.AssemblyQualifiedName:
                        key = objType.AssemblyQualifiedName;
                        break;
                    default:
                        key = objType.Name;
                        break;
                }
            }

            return key;

        }

        #endregion Shared Members (Public)
    }
}
