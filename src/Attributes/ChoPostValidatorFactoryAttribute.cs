namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ChoPostValidatorFactoryAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private Type _externValidationManagerType;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoPostValidatorFactoryAttribute(Type externValidationManagerType)
        {
            ChoGuard.ArgumentNotNull(externValidationManagerType, "externValidationManagerType");

            _externValidationManagerType = externValidationManagerType;
        }

        #endregion Constructors

        public object ValidationManagerInstance
        {
            get { return ChoObjectManagementFactory.CreateInstance(_externValidationManagerType); }
        }
    }
}
