namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ChoNotNullOrEmptyValidatorAttribute : ChoValidatorAttribute
    {
        #region Constructors

        public ChoNotNullOrEmptyValidatorAttribute()
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ChoNotNullOrEmptyValidator"/>.</para>
        /// </summary>
        public ChoNotNullOrEmptyValidatorAttribute(bool negated)
            : base(negated)
        {
        }

        #endregion Constructors

        // Properties
        public override object ValidatorInstance
        {
            get { return new ChoNotNullOrEmptyValidator(Negated); }
        }
    }
}
