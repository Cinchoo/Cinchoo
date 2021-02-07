namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    using System.Collections;

    #endregion NameSpaces

    public sealed class ChoNotNullOrEmptyValidator : ChoValidator
    {
        public ChoNotNullOrEmptyValidator()
            : this(false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
        /// </summary>
        public ChoNotNullOrEmptyValidator(bool negated)
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
                return "The value much be null/empty.";
            else
                return "The value must not be null/empty.";
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

                if (value is String && String.IsNullOrEmpty(value as string))
                    throw new ChoValidationException(GetErrMsg());

                if (value is ICollection && ((ICollection)value).Count == 0)
                    throw new ChoValidationException(GetErrMsg());
            }

            if (Negated)
            {
                if (value != null)
                    throw new ChoValidationException(GetErrMsg());

                if (value is String && !String.IsNullOrEmpty(value as string))
                    throw new ChoValidationException(GetErrMsg());

                if (value is ICollection && ((ICollection)value).Count != 0)
                    throw new ChoValidationException(GetErrMsg());
            }
        }
    }
}
