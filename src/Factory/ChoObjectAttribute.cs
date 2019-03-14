namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoObjectAttribute : ChoObjectNameableAttribute
    {
        #region Instance Properties (Public)

        private string _constructorArgs;
        public string DefaultConstructorArgs
        {
            get { return _constructorArgs; }
            set { _constructorArgs = value; }
        }

        private string _defaultConstructor;
        public string DefaultConstructor
        {
            get { return _defaultConstructor; }
            set { _defaultConstructor = value; }
        }

        #endregion Instance Properties (Public)

        #region Constructors

        public ChoObjectAttribute(string name)
            : base(name)
        {
        }

        #endregion Constructors
    }
}
