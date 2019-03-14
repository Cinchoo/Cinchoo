namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    
    using Cinchoo.Core.Factory;

    #endregion NameSpaces

    public enum ChoObjectConstructionType
    {
        Singleton,
        Prototype
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoObjectFactoryAttribute : Attribute
    {
        #region Instance Properties

        private readonly ChoObjectConstructionType _objectConstructionType = ChoObjectConstructionType.Prototype;
        internal ChoObjectConstructionType ObjectConstructionType
        {
            get { return _objectConstructionType; }
        }

        private readonly IChoCustomObjectFactory _customObjectFactory;
        internal IChoCustomObjectFactory CustomObjectFactory
        {
            get { return _customObjectFactory; }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoObjectFactoryAttribute(ChoObjectConstructionType objectConstructionType)
        {
            _objectConstructionType = objectConstructionType;
        }

        public ChoObjectFactoryAttribute(IChoCustomObjectFactory customObjectFactory)
        {
            _customObjectFactory = customObjectFactory;
        }

        #endregion Constructors
    }
}
