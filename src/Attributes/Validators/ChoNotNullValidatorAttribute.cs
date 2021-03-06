namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ChoNotNullValidatorAttribute : ChoValidatorAttribute
    {
        #region Constructors

        public ChoNotNullValidatorAttribute()
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
        /// </summary>
        public ChoNotNullValidatorAttribute(bool negated) : base(negated)
        {
        }

        #endregion Constructors

        // Properties
        public override object ValidatorInstance
        {
            get { return new ChoNotNullValidator(Negated); }
        }
    }
}
