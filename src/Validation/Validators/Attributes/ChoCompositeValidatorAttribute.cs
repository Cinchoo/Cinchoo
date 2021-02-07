namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public enum ChoCompositionType { And, Or }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true, Inherited=false)]
    public sealed class ChoCompositeValidatorAttribute : ChoMemberInfoAttribute
    {
        #region Instance Data Members (Private)

        private ChoCompositionType _compositionType = ChoCompositionType.And;
        private string[] _validatorNames;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCompositeValidatorAttribute(ChoCompositionType compositionType, string validatorNames)
        {
            _compositionType = compositionType;
            _validatorNames = validatorNames.SplitNTrim();
        }

        #endregion Constructors

        public ChoCompositionType CompositionType
        {
            get { return _compositionType; }
        }

        public string[] ValidatorNames
        {
            get { return _validatorNames; }
        }
    }
}
