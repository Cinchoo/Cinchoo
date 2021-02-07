namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Resources;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ChoSubclassTypeValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private Type _baseClass;

        // Methods
        public ChoSubclassTypeValidatorAttribute(Type baseClass)
        {
            _baseClass = baseClass;
        }

        // Properties
        public Type BaseClass
        {
            get { return _baseClass; }
        }

        public override object ValidatorInstance
        {
            get { return new SubclassTypeValidator(_baseClass); }
        }
    }
}
