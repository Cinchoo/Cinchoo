namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ChoNotNullOrWhiteSpaceValidatorAttribute : ChoValidatorAttribute
    {
        #region Constructors

        public ChoNotNullOrWhiteSpaceValidatorAttribute()
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ChoNotNullOrWhiteSpaceValidator"/>.</para>
        /// </summary>
        public ChoNotNullOrWhiteSpaceValidatorAttribute(bool negated)
            : base(negated)
        {
        }

        #endregion Constructors

        // Properties
        public override object ValidatorInstance
        {
            get { return new ChoNotNullOrWhiteSpaceValidator(Negated); }
        }
    }
}
