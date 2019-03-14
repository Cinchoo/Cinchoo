namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core;

    #endregion NameSpaces

    public abstract class ChoObjectMemberAttribute : Attribute
    {
        #region Instance Properties (Public)

        private string _referenceId;
        public string ReferenceId
        {
            get { return _referenceId; }
            set
            {
                ChoGuard.ArgumentNotNull(value, "ReferenceId");
                _referenceId = value;
            }
        }

        private object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion Instance Properties (Public)
    }
}
