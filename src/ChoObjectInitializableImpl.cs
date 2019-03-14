namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Interfaces;

    #endregion NameSpaces

    public abstract class ChoObjectInitializableImpl : IChoObjectInitializable
    {
        #region IChoObjectInitializable Members

        public abstract void Initialize();

        #endregion

        #region Instance Members (Virtual)

        public virtual void PostInitialize()
        {
        }

        #endregion Instance Members (Virtual)
    }
}
