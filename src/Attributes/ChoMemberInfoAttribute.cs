namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    using Cinchoo.Core.Reflection;

    #endregion NameSpaces

    public abstract class ChoMemberInfoAttribute : ChoObjectNameableAttribute
    {
        #region Constructors

        public ChoMemberInfoAttribute(string name)
            : base(name)
        {
        }

        protected ChoMemberInfoAttribute()
            : base()
        {
        }

        #endregion
    }
}
