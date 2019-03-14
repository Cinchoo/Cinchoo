namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public class ChoTypeShortNameAttribute : Attribute
    {
        #region Instance Properties

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoTypeShortNameAttribute(string name)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
        }

        public ChoTypeShortNameAttribute(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            _name = type.Name;
        }

        #endregion Constructors
    }
}
