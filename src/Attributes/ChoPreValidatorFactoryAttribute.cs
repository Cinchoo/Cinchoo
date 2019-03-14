namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ChoPreValidatorFactoryAttribute : Attribute
    {
        public Type ExternValidationManager;

        public object ValidationManagerInstance
        {
            get { return ChoObjectManagementFactory.CreateInstance(ExternValidationManager); }
        }
    }
}
