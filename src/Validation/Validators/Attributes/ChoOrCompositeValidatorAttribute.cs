namespace eSquare.Core.Attributes.Validators
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Validation.Validators;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ChoOrCompositeValidatorAttribute : ChoValidatorAttribute
    {
        #region Constructors

        public ChoOrCompositeValidatorAttribute()
        {
        }

        #endregion Constructors

        // Properties
        public override object ValidatorInstance
        {
            get { return new ChoOrCompositeValidator(); }
        }
    }
}
