namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public sealed class ChoNotNullValidator : ChoValidator
    {
        public ChoNotNullValidator()
            : this(false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
        /// </summary>
        public ChoNotNullValidator(bool negated)
            : base(negated)
        { }

        // Summary:
        //     Determines whether an object can be validated based on type.
        //
        // Parameters:
        //   type:
        //     The object type.
        //
        // Returns:
        //     true if the type parameter value matches the expected type; otherwise, false.
        public override bool CanValidate(Type type)
        {
            return true;
        }

        protected override string GetErrMsg()
        {
            if (Negated)
                return "The value must not be null.";
            else
                return "The value much be null.";
        }

        //
        // Summary:
        //     Determines whether the value of an object is valid.
        //
        // Parameters:
        //   value:
        //     The object value.
        public override void Validate(object value)
        {
            if (!Negated && value == null)
                throw new ChoValidationException(GetErrMsg());

            if (Negated && value != null)
                throw new ChoValidationException(GetErrMsg());
        }
    }
}
