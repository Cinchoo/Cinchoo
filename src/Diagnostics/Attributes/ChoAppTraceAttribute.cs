namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoAppTraceAttribute : ChoProfileAttribute
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

        public ChoAppTraceAttribute(string message)
            : base(message)
        {
        }

        public ChoAppTraceAttribute(bool condition, string message)
            : base(condition, message)
        {
        }

        #endregion Constructors

        #region ChoProfileAttribute Overrides (Public)

		public override IChoProfile ConstructProfile(object target, IChoProfile outerProfile)
        {
            lock (_padLock)
            {
                string message = ChoPropertyManager.ExpandProperties(this, Message);

                IChoProfile tracer = new ChoTrace(Condition, message);
                return tracer;
            }
        }

        #endregion ChoProfileAttribute Overrides (Public)
    }
}
