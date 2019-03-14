namespace Cinchoo.Core.ComponentModel
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoTypeProviderAttribute : ChoTypeAttribute
    {
        #region Instance Data Members (Private)

        private object[] _additionalParameters;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Internal)

        internal object[] AdditionalConstructorParameters
        {
            get { return _additionalParameters; }
        }

        #endregion Instance Properties (Internal)

        #region Instance Properties (Public)

        public string AdditionalParameters
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                if (String.IsNullOrEmpty(value)) return;
                _additionalParameters = ChoString.Split2Objects(value);
            }
        }

        #endregion Instance Properties (Public)

        #region Constructors

        public ChoTypeProviderAttribute(Type type) : base(type)
        {
        }

        public ChoTypeProviderAttribute(string typeName) : base(typeName)
        {
        }

        #endregion Constructors
}
}
