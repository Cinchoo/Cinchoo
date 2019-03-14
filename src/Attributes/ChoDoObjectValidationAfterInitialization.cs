namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoDoObjectValidationAfterInitializationAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private bool _doObjectValidation;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoDoObjectValidationAfterInitializationAttribute(bool doObjectValidation)
        {
            _doObjectValidation = doObjectValidation;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public bool DoObjectValidation
        {
            get { return _doObjectValidation; }
        }

        #endregion Instance Properties (Public)
    }
}
