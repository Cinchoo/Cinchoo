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
    public sealed class ChoRegexStringValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private string _regex;

        // Methods
        public ChoRegexStringValidatorAttribute(string regex)
        {
            _regex = regex;
        }

        // Properties
        public string Regex
        {
            get { return _regex; }
        }

        public override object ValidatorInstance
        {
            get { return new RegexStringValidator(_regex); }
        }
    }
}
