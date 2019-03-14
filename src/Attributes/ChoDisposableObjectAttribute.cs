namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoDisposableObjectAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private readonly bool _containsUnmanagedResources = false;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoDisposableObjectAttribute(bool containsUnmanagedResources)
        {
            _containsUnmanagedResources = containsUnmanagedResources;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public bool ContainsUnmanagedResources
        {
            get { return _containsUnmanagedResources; }
        }

        #endregion Instance Properies (Public)
    }
}
