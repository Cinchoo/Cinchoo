namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoAppBufferProfileAttribute : ChoBufferProfileAttribute
    {
        #region Instance Data Members (Private)

        private object _padLock = new object();

        #endregion Instance Data Members (Private)
               
        #region Instance Properties

        public new ChoProfileIntializationAction Name
        {
            get { throw new NotSupportedException(); }
        }

        public ChoProfileIntializationAction Mode
        {
            get { return ChoProfileIntializationAction.Append; }
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

        public ChoAppBufferProfileAttribute(string message)
            : base(message)
        {
        }

        public ChoAppBufferProfileAttribute(bool condition, string message)
            : base(condition, message)
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

		public override IChoProfile ConstructProfile(object target, IChoProfile outerProfile)
        {
            lock (_padLock)
            {
                string message = ChoPropertyManager.ExpandProperties(this, Message);
                
                return new ChoBufferProfileEx(Condition, null, message, (ChoBaseProfile)outerProfile, true, null);
            }
        }

        #endregion Instance Members (Public)
    }
}
