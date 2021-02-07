namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    using System.Collections;

    #endregion NameSpaces

    public sealed class ChoNotNullOrWhiteSpaceValidator : ChoValidator
    {
        public ChoNotNullOrWhiteSpaceValidator()
            : this(false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
        /// </summary>
        public ChoNotNullOrWhiteSpaceValidator(bool negated)
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
                return "The value much be null/whitespace.";
            else
                return "The value must not be null/whitespace.";
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
            if (!Negated)
            {
                if (value == null)
                    throw new ChoValidationException(GetErrMsg());

                if (value is String && String.IsNullOrWhiteSpace(value as string))
                    throw new ChoValidationException(GetErrMsg());
            }

            if (Negated)
            {
                if (value != null)
                    throw new ChoValidationException(GetErrMsg());

                if (value is String && !String.IsNullOrWhiteSpace(value as string))
                    throw new ChoValidationException(GetErrMsg());
            }
        }
    }
}
