namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Resources;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ChoStringValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private string _invalidChars;
        private int _maxLength = 0x7fffffff;
        private int _minLength;

        // Properties
        public string InvalidCharacters
        {
            get { return _invalidChars; }
            set { _invalidChars = value; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set
            {
                if (_minLength > value)
                    throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _maxLength = value;
            }
        }

        public int MinLength
        {
            get { return _minLength; }
            set
            {
                if (_maxLength < value)
                    throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _minLength = value;
            }
        }

        public override object ValidatorInstance
        {
            get { return new StringValidator(_minLength, _maxLength, _invalidChars); }
        }
    }
}
