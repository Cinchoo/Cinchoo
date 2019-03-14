namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public abstract class ChoTypeAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private Type _type;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Public)

        public Type Type
        {
            get { return _type; }
            private set
            {
                ChoGuard.ArgumentNotNull(value, "Type");
                _type = value;
            }
        }

        #endregion Instance Properties (Public)

        #region Constructors

        public ChoTypeAttribute(Type type)
        {
            Type = type;
        }

        public ChoTypeAttribute(string typeName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(typeName, "Type Name");

            Type = ChoType.GetType(typeName);
        }

        #endregion Constructors
    }
}
