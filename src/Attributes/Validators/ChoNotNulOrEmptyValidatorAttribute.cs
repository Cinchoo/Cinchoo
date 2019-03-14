namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ChoNotNulOrEmptyValidatorAttribute : ChoValidatorAttribute
    {
        #region Constructors

        public ChoNotNulOrEmptyValidatorAttribute()
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ChoNotNulOrEmptyValidator"/>.</para>
        /// </summary>
        public ChoNotNulOrEmptyValidatorAttribute(bool negated)
            : base(negated)
        {
        }

        #endregion Constructors

        // Properties
        public override object ValidatorInstance
        {
            get { return new ChoNotNulOrEmptyValidator(Negated); }
        }
    }
}
