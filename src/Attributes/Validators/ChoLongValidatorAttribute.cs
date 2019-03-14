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
    public sealed class ChoLongValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private bool _excludeRange;
        private long _max = 0x7fffffffffffffffL;
        private long _min = -9223372036854775808L;

        // Properties
        public bool ExcludeRange
        {
            get { return _excludeRange; }
            set { _excludeRange = value; }
        }

        public long MaxValue
        {
            get { return _max; }
            set
            {
                if (_min > value)
                    throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _max = value;
            }
        }

        public long MinValue
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
            get { return new LongValidator(_min, _max, _excludeRange); }
        }
    }
}
