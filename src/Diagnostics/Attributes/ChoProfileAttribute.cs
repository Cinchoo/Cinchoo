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
    using System.Xml.Serialization;

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

        private bool? _condition = null;
        public bool Condition
        {
            get { return _condition == null ? ChoTraceSwitch.Switch.TraceVerbose : _condition.Value; }
        }

        private string _message;
        [XmlIgnore]
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
            OuterProfileName = ChoProfile.CURRENT_CONTEXT_PROFILE;
        }

        public ChoProfileAttribute(string message)
            : this(ChoTraceSwitch.Switch.TraceVerbose, message)
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

		public abstract IChoProfile ConstructProfile(object target, IChoProfile outerProfile);
        
        #endregion Instance Members (Public)

        public string OuterProfileName { get; set; }
    }
}
