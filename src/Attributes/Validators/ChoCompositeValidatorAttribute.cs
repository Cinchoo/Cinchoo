namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public enum ChoCompositionType { And, Or }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public sealed class ChoCompositeValidatorAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private ChoCompositionType _compositionType = ChoCompositionType.And;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCompositeValidatorAttribute()
        {
        }

        public ChoCompositeValidatorAttribute(ChoCompositionType compositionType)
        {
            _compositionType = compositionType;
        }

        #endregion Constructors

        public ChoCompositionType CompositionType
        {
            get { return _compositionType; }
        }
    }
}
