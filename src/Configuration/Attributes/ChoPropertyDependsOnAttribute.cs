namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoPropertyDependsOnAttribute : ChoMemberInfoAttribute
    {
        public string[] DependsOn
        {
            get;
            private set;
        }

        public ChoPropertyDependsOnAttribute(string dependsOn)
        {
            if (dependsOn.IsNullOrWhiteSpace()) return;
            DependsOn = dependsOn.SplitNTrim();
        }
    }
}
