namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoMemberItemFormatterAttribute : ChoMemberFormatterAttribute
    {
        #region Constructors

        public ChoMemberItemFormatterAttribute()
            : base()
        {
        }

        public ChoMemberItemFormatterAttribute(string name)
            : base(name)
        {
        }

        public ChoMemberItemFormatterAttribute(string name, int noOfTabs)
            : base(name)
        {
            NoOfTabs = noOfTabs;
        }

        #endregion
    }
}
