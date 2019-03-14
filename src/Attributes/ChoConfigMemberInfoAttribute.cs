namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoConfigMemberInfoAttribute : Attribute
    {
        #region Constructors

        public ChoConfigMemberInfoAttribute()
        {
        }

        #endregion

        #region Instance Properties

        private bool _persistable = true;
        public bool Persistable
        {
            get { return _persistable; }
            set { _persistable = value; }
        }

        #endregion
    }
}
