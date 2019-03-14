namespace Cinchoo.Core.Configuration.Storage
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using Cinchoo.Core.Attributes;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoConfigStorageAttribute : ChoObjectNameableAttribute
    {
        #region Constructors

        public ChoConfigStorageAttribute(string name)
            : base(name)
        {
        }

        public ChoConfigStorageAttribute(Type configStorageType)
            : base(configStorageType)
        {
        }

        #endregion Constructors
    }
}
