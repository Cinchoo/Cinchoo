namespace eSquare.Core.Attributes
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    /// <summary>
    /// Base class for all validator attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
    public abstract class ChoValidatorAttribute : Attribute
    {
        #region Public Instance Methods

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <exception cref="ValidationException">The validation fails.</exception>
        public abstract void Validate(object value);

        #endregion Public Instance Methods
    }
}
