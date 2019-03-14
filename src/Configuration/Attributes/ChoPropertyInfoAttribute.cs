namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoPropertyInfoAttribute : ChoMemberInfoAttribute
    {
        #region Constructors

        public ChoPropertyInfoAttribute()
        {
        }

        public ChoPropertyInfoAttribute(string name)
            : base(name)
        {
        }

        #endregion

        #region Instance Properties

        private string _defaultValue;
        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                IsDefaultValueSpecified = true;
                _defaultValue = value;
            }
        }

        private string _fallbackValue;
        public string FallbackValue
        {
            get { return _fallbackValue; }
            set
            {
                IsFallbackValueSpecified = true;
                _fallbackValue = value;
            }
        }

        private bool _persistable = true;
        public bool Persistable
        {
            get { return _persistable; }
            set { _persistable = value; }
        }

        private Type _sourceType;
        public Type SourceType
        {
            get { return _sourceType; }
            set { if (value != null) _sourceType = value; }
        }

        internal bool IsDefaultValueSpecified { get; set; }
        internal bool IsFallbackValueSpecified { get; set; }

        #endregion
    }
}
