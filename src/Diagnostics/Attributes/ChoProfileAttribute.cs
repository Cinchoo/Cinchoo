namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class ChoProfileAttribute : Attribute
    {
        #region Instance Properties

        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set
            {
                ChoGuard.ArgumentNotNull(value, "Name");
                _name = value;
            }
        }

		public virtual Type NameFromTypeName
		{
			get { throw new NotSupportedException(); }
			set
			{
				ChoGuard.ArgumentNotNull(value, "Type");

				Name = value.Name;
			}
		}

		public virtual Type NameFromTypeFullName
		{
			get { throw new NotSupportedException(); }
			set
			{
				ChoGuard.ArgumentNotNull(value, "Type");

				Name = value.FullName;
			}
		}

		private string _startActions;
        public virtual string StartActions
        {
			get { return _startActions; }
			set { _startActions = value; }
        }

        private string _stopActions;
		public virtual string StopActions
		{
			get { return _stopActions; }
			set { _stopActions = value; }
		}

        private bool _condition = ChoTrace.ChoSwitch.TraceVerbose;
        public bool Condition
        {
            get { return _condition; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            protected set { _message = value; }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoProfileAttribute(bool condition, string message)
        {
            _condition = condition;
            _message = message;
        }

        public ChoProfileAttribute(string message)
            : this(ChoTrace.ChoSwitch.TraceVerbose, message)
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

		public abstract IChoProfile ConstructProfile(object target, IChoProfile outerProfile);
        
        #endregion Instance Members (Public)
    }
}
