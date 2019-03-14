namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoAppStreamProfileAttribute : ChoStreamProfileAttribute
    {
        #region Instance Data Members (Private)

        private object _padLock = new object();

        #endregion Instance Data Members (Private)

        #region Instance Properties

        public new string Name
        {
            get { throw new NotSupportedException(); }
        }

        public ChoProfileIntializationAction Mode
        {
            get { return ChoProfileIntializationAction.Append; }
        }

        public bool DelayedAutoStart
        {
            get { throw new NotSupportedException(); }
        }

        public new Type NameFromTypeName
        {
            get { throw new NotSupportedException(); }
        }

        public new Type NameFromTypeFullName
        {
            get { throw new NotSupportedException(); }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoAppStreamProfileAttribute(string message)
            : base(message)
        {
        }

        public ChoAppStreamProfileAttribute(bool condition, string message)
            : base(condition, message)
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

		public override IChoProfile ConstructProfile(object target, IChoProfile outerProfile)
        {
            lock (_padLock)
            {
				string message = ChoPropertyManager.ExpandProperties(target, Message);

                return new ChoProfile(Condition, message);
            }
        }

        #endregion Instance Members (Public)
    }
}
