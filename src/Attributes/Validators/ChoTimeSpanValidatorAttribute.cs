namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Resources;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ChoTimeSpanValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private bool _excludeRange;
        private TimeSpan _max = TimeSpan.MaxValue;
        private TimeSpan _min = TimeSpan.MinValue;
        public const string TimeSpanMaxValue = "10675199.02:48:05.4775807";
        public const string TimeSpanMinValue = "-10675199.02:48:05.4775808";

        // Properties
        public bool ExcludeRange
        {
            get { return _excludeRange; }
            set { _excludeRange = value; }
        }

        public TimeSpan MaxValue
        {
            get { return _max; }
        }

        public string MaxValueString
        {
            get { return _max.ToString(); }
            set
            {
                TimeSpan span = TimeSpan.Parse(value);
                if (_min > span)
                    throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _max = span;
            }
        }

        public TimeSpan MinValue
        {
            get { return _min; }
        }

        public string MinValueString
        {
            get{ return _min.ToString(); }
            set
            {
                TimeSpan span = TimeSpan.Parse(value);
                if (_max < span)
                   throw new ArgumentOutOfRangeException("value", ConfigResources.GetString("Validator_min_greater_than_max"));

                _min = span;
            }
        }

        public override object ValidatorInstance
        {
            get { return new TimeSpanValidator(_min, _max, _excludeRange); }
        }
    }
}
