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
    public sealed class ChoPositiveTimeSpanValidatorAttribute : ChoValidatorAttribute
    {
        // Properties
        public override object ValidatorInstance
        {
            get { return new PositiveTimeSpanValidator(); }
        }
    }
}
