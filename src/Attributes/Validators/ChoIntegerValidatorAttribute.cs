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

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ChoIntegerValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private bool _excludeRange;
        private int _max = 0x7fffffff;
        private int _min = -2147483648;

        // Properties
        public bool ExcludeRange
        {
            get { return _excludeRange; }
            set { _excludeRange = value; }
        }

        public int MaxValue
        {
            get { return _max; }
            set
            {
                if (_min > value)
                    throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _max = value;
            }
        }

        public int MinValue
        {
            get { return _min; }
            set
            {
                if (_max < value)
                    throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _min = value;
            }
        }

        public override object ValidatorInstance
        {
            get { return new IntegerValidator(_min, _max, _excludeRange); }
        }
    }
}
