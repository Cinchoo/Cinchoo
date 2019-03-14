namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;

    #endregion NameSpaces

    #region Event Args Classes

    public class ChoCommandLineArgNotFoundEventArgs : EventArgs
    {
        #region Instance Data Members (Public)

        public readonly string CmdLineArgSwitch;
        public readonly string CmdLineArgValue;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoCommandLineArgNotFoundEventArgs(string cmdLineArgSwitch, string cmdLineArgValue)
        {
            CmdLineArgSwitch = cmdLineArgSwitch;
            CmdLineArgValue = cmdLineArgValue;
        }

        #endregion Constructors
    }

    public class ChoPreviewCommandLineArgObjectEventArgs : ChoCommandLineArgObjectEventArgs
    {
        #region Instance Data Members (Public)

        public bool Cancel = false;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoPreviewCommandLineArgObjectEventArgs(string[] commandLineArgs) : base(commandLineArgs)
        {
        }

        #endregion Constructors
    }

    public class ChoCommandLineArgObjectEventArgs : EventArgs
    {
        #region Instance Data Members (Public)

        public string[] CommandLineArgs;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoCommandLineArgObjectEventArgs(string[] commandLineArgs)
        {
            CommandLineArgs = commandLineArgs;
        }

        #endregion Constructors
    }

    public class ChoCommandLineArgObjectErrorEventArgs : ChoCommandLineArgObjectEventArgs
    {
        #region Instance Data Members (Public)

        public Exception Exception = null;
        public bool Handled = false;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoCommandLineArgObjectErrorEventArgs(string[] commandLineArgs, Exception ex)
            : base(commandLineArgs)
        {
            Exception = ex;
        }

        #endregion Constructors
    }

    public class ChoCommandLineArgEventArgs : EventArgs
    {
        #region Instance Data Members (Public)

        public readonly string MemberName;
        public object Value = null;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoCommandLineArgEventArgs(string memberName, object value)
        {
            MemberName = memberName;
            Value = value;
        }

        #endregion Constructors
    }

    public class ChoPreviewCommandLineArgEventArgs : ChoCommandLineArgEventArgs
    {
        #region Instance Data Members (Public)

        public bool Cancel = false;
        public readonly object DefaultValue = null;
        public readonly object FallbackValue = null;

        #endregion Instance Data Members (Public)


        #region Constructors

        internal ChoPreviewCommandLineArgEventArgs(string memberName, object value, object defaultValue, object fallbackValue)
            : base(memberName, value)
        {
            DefaultValue = defaultValue;
            FallbackValue = fallbackValue;
        }

        #endregion Constructors
    }

    public class ChoCommandLineArgErrorEventArgs : ChoCommandLineArgEventArgs
    {
        #region Instance Data Members (Public)

        public Exception Exception;
        public bool Handled = false;

        #endregion Instance Data Members (Public)


        #region Constructors

        internal ChoCommandLineArgErrorEventArgs(string memberName, object state, Exception ex)
            : base(memberName, state)
        {
            Exception = ex;
        }

        #endregion Constructors
    }

    #endregion Event Args Classes

    public abstract class ChoCommandLineArgObject : ContextBoundObject
    {
        #region Instance Data Members (Private)

        [ChoHiddenMember]
        private readonly ChoCommandLineArgObjectAttribute _commandLineArgsObjectAttribute;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Internal)

        internal string[] CommandLineArgs
        {
            get;
            set;
        }

        #endregion Instance Properties (Internal)

        #region Events

        #region BeforeCommandLineArgLoaded Event

        [ChoHiddenMember]
        private EventHandler<ChoPreviewCommandLineArgEventArgs> _beforeCommandLineArgLoaded;
        [ChoHiddenMember]
        internal event EventHandler<ChoPreviewCommandLineArgEventArgs> BeforeCommandLineArgLoadedInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoPreviewCommandLineArgEventArgs> BeforeCommandLineArgLoaded
        {
            add
            {
                _beforeCommandLineArgLoaded = value;
            }
            remove
            {
                _beforeCommandLineArgLoaded = null;
            }
        }

        #endregion BeforeCommandLineArgLoaded Event

        #region AfterCommandLineArgLoaded Event

        [ChoHiddenMember]
        private EventHandler<ChoCommandLineArgEventArgs> _afterCommandLineArgLoaded;
        [ChoHiddenMember]
        internal event EventHandler<ChoCommandLineArgEventArgs> AfterCommandLineArgLoadedInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoCommandLineArgEventArgs> AfterCommandLineArgLoaded
        {
            add
            {
                _afterCommandLineArgLoaded = value;
            }
            remove
            {
                _afterCommandLineArgLoaded = null;
            }
        }

        #endregion AfterCommandLineArgLoaded Event

        #region CommandLineArgLoadError Event

        [ChoHiddenMember]
        private EventHandler<ChoCommandLineArgErrorEventArgs> _commandLineArgLoadError;
        [ChoHiddenMember]
        internal event EventHandler<ChoCommandLineArgErrorEventArgs> CommandLineArgLoadErrorInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoCommandLineArgErrorEventArgs> CommandLineArgLoadError
        {
            add
            {
                _commandLineArgLoadError = value;
            }
            remove
            {
                _commandLineArgLoadError = null;
            }
        }

        #endregion CommandLineArgLoadError Event

        #region BeforeCommandLineArgObjectLoaded Event

        [ChoHiddenMember]
        private EventHandler<ChoPreviewCommandLineArgObjectEventArgs> _beforeCommandLineArgObjectLoaded;
        [ChoHiddenMember]
        internal event EventHandler<ChoPreviewCommandLineArgObjectEventArgs> BeforeCommandLineArgObjectLoadedInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoPreviewCommandLineArgObjectEventArgs> BeforeCommandLineArgObjectLoaded
        {
            add
            {
                _beforeCommandLineArgObjectLoaded = value;
            }
            remove
            {
                _beforeCommandLineArgObjectLoaded = null;
            }
        }

        #endregion BeforeCommandLineArgObjectLoaded Event

        #region AfterCommandLineArgObjectLoaded Event

        [ChoHiddenMember]
        private EventHandler<ChoCommandLineArgObjectEventArgs> _afterCommandLineArgObjectLoaded;
        [ChoHiddenMember]
        internal event EventHandler<ChoCommandLineArgObjectEventArgs> AfterCommandLineArgObjectLoadedInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoCommandLineArgObjectEventArgs> AfterCommandLineArgObjectLoaded
        {
            add
            {
                _afterCommandLineArgObjectLoaded = value;
            }
            remove
            {
                _afterCommandLineArgObjectLoaded = null;
            }
        }

        #endregion AfterCommandLineArgObjectLoaded Event

        #region CommandLineArgObjectLoadError Event

        [ChoHiddenMember]
        private EventHandler<ChoCommandLineArgObjectErrorEventArgs> _commandLineArgObjectLoadError;
        [ChoHiddenMember]
        internal event EventHandler<ChoCommandLineArgObjectErrorEventArgs> CommandLineArgObjectLoadErrorInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoCommandLineArgObjectErrorEventArgs> CommandLineArgObjectLoadError
        {
            add
            {
                _commandLineArgObjectLoadError = value;
            }
            remove
            {
                _commandLineArgObjectLoadError = null;
            }
        }

        #endregion CommandLineArgObjectLoadError Event

        #region CommandLineArgMemberNotFound Event

        [ChoHiddenMember]
        private EventHandler<ChoCommandLineArgNotFoundEventArgs> _commandLineArgMemberNotFound;
        [ChoHiddenMember]
        internal event EventHandler<ChoCommandLineArgNotFoundEventArgs> CommandLineArgMemberNotFoundInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoCommandLineArgNotFoundEventArgs> CommandLineArgMemberNotFound
        {
            add
            {
                _commandLineArgMemberNotFound = value;
            }
            remove
            {
                _commandLineArgMemberNotFound = null;
            }
        }

        #endregion CommandLineArgMemberNotFound Event

        #region UnrecognizedCommandLineArgFound Event

        [ChoHiddenMember]
        private EventHandler<ChoUnrecognizedCommandLineArgEventArg> _unrecognizedCommandLineArgFound;
        [ChoHiddenMember]
        internal event EventHandler<ChoUnrecognizedCommandLineArgEventArg> UnrecognizedCommandLineArgFoundInternal;
        [ChoHiddenMember]
        private event EventHandler<ChoUnrecognizedCommandLineArgEventArg> UnrecognizedCommandLineArgFound
        {
            add
            {
                _unrecognizedCommandLineArgFound = value;
            }
            remove
            {
                _unrecognizedCommandLineArgFound = null;
            }
        }

        #endregion UnrecognizedCommandLineArgFound Event

        #endregion Events

        #region Constructors

        [ChoHiddenMember]
        public ChoCommandLineArgObject()
        {
            _commandLineArgsObjectAttribute = ChoType.GetAttribute(GetType(), typeof(ChoCommandLineArgObjectAttribute)) as ChoCommandLineArgObjectAttribute;
        
			//Discover and Hook the event handlers
            if (BeforeCommandLineArgLoadedInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoPreviewCommandLineArgEventArgs>(ref BeforeCommandLineArgLoadedInternal, ChoType.GetMethods(GetType(), typeof(ChoBeforeCommandLineArgLoadedHandlerAttribute)), this);
                if (BeforeCommandLineArgLoadedInternal != null && BeforeCommandLineArgLoadedInternal.GetInvocationList().Length > 0)
                    BeforeCommandLineArgLoaded += (EventHandler<ChoPreviewCommandLineArgEventArgs>)BeforeCommandLineArgLoadedInternal.GetInvocationList()[BeforeCommandLineArgLoadedInternal.GetInvocationList().Length - 1];
            }
            if (AfterCommandLineArgLoadedInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoCommandLineArgEventArgs>(ref AfterCommandLineArgLoadedInternal, ChoType.GetMethods(GetType(), typeof(ChoAfterCommandLineArgLoadedHandlerAttribute)), this);
                if (AfterCommandLineArgLoadedInternal != null && AfterCommandLineArgLoadedInternal.GetInvocationList().Length > 0)
                    AfterCommandLineArgLoaded += (EventHandler<ChoCommandLineArgEventArgs>)AfterCommandLineArgLoadedInternal.GetInvocationList()[AfterCommandLineArgLoadedInternal.GetInvocationList().Length - 1];
            }
            if (CommandLineArgLoadErrorInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoCommandLineArgErrorEventArgs>(ref CommandLineArgLoadErrorInternal, ChoType.GetMethods(GetType(), typeof(ChoCommandLineArgLoadErrorHandlerAttribute)), this);
                if (CommandLineArgLoadErrorInternal != null && CommandLineArgLoadErrorInternal.GetInvocationList().Length > 0)
                    CommandLineArgLoadError += (EventHandler<ChoCommandLineArgErrorEventArgs>)CommandLineArgLoadErrorInternal.GetInvocationList()[CommandLineArgLoadErrorInternal.GetInvocationList().Length - 1];
            }

            if (BeforeCommandLineArgObjectLoadedInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoPreviewCommandLineArgObjectEventArgs>(ref BeforeCommandLineArgObjectLoadedInternal, ChoType.GetMethods(GetType(), typeof(ChoBeforeCommandLineArgObjectLoadedHandlerAttribute)), this);
                if (BeforeCommandLineArgObjectLoadedInternal != null && BeforeCommandLineArgObjectLoadedInternal.GetInvocationList().Length > 0)
                    BeforeCommandLineArgObjectLoaded += (EventHandler<ChoPreviewCommandLineArgObjectEventArgs>)BeforeCommandLineArgObjectLoadedInternal.GetInvocationList()[BeforeCommandLineArgObjectLoadedInternal.GetInvocationList().Length - 1];
            }

            if (AfterCommandLineArgObjectLoadedInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoCommandLineArgObjectEventArgs>(ref AfterCommandLineArgObjectLoadedInternal, ChoType.GetMethods(GetType(), typeof(ChoAfterCommandLineArgObjectLoadedHandlerAttribute)), this);
                if (AfterCommandLineArgObjectLoadedInternal != null && AfterCommandLineArgObjectLoadedInternal.GetInvocationList().Length > 0)
                    AfterCommandLineArgObjectLoaded += (EventHandler<ChoCommandLineArgObjectEventArgs>)AfterCommandLineArgObjectLoadedInternal.GetInvocationList()[AfterCommandLineArgObjectLoadedInternal.GetInvocationList().Length - 1];
            }

            if (CommandLineArgObjectLoadErrorInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoCommandLineArgObjectErrorEventArgs>(ref CommandLineArgObjectLoadErrorInternal, ChoType.GetMethods(GetType(), typeof(ChoCommandLineArgObjectLoadErrorHandlerAttribute)), this);
                if (CommandLineArgObjectLoadErrorInternal != null && CommandLineArgObjectLoadErrorInternal.GetInvocationList().Length > 0)
                    CommandLineArgObjectLoadError += (EventHandler<ChoCommandLineArgObjectErrorEventArgs>)CommandLineArgObjectLoadErrorInternal.GetInvocationList()[CommandLineArgObjectLoadErrorInternal.GetInvocationList().Length - 1];
            }

            if (CommandLineArgMemberNotFoundInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoCommandLineArgNotFoundEventArgs>(ref CommandLineArgMemberNotFoundInternal, ChoType.GetMethods(GetType(), typeof(ChoCommandLineArgMemberNotFoundHandlerAttribute)), this);
                if (CommandLineArgMemberNotFoundInternal != null && CommandLineArgMemberNotFoundInternal.GetInvocationList().Length > 0)
                    CommandLineArgMemberNotFound += (EventHandler<ChoCommandLineArgNotFoundEventArgs>)CommandLineArgMemberNotFoundInternal.GetInvocationList()[CommandLineArgMemberNotFoundInternal.GetInvocationList().Length - 1];
            }
            if (UnrecognizedCommandLineArgFoundInternal == null)
            {
                EventHandlerEx.LoadHandlers<ChoUnrecognizedCommandLineArgEventArg>(ref UnrecognizedCommandLineArgFoundInternal, ChoType.GetMethods(GetType(), typeof(ChoUnrecognizedCommandLineArgFoundHandlerAttribute)), this);
                if (UnrecognizedCommandLineArgFoundInternal != null && UnrecognizedCommandLineArgFoundInternal.GetInvocationList().Length > 0)
                    UnrecognizedCommandLineArgFound += (EventHandler<ChoUnrecognizedCommandLineArgEventArg>)UnrecognizedCommandLineArgFoundInternal.GetInvocationList()[UnrecognizedCommandLineArgFoundInternal.GetInvocationList().Length - 1];
            }
        }
        
        #endregion Constructors

        #region Instance Members (Public)

        public virtual string GetUsage()
        {
            return ChoCommandLineArgObjectDirector.GetUsage(this);
        }

        public virtual string AdditionalUsageText
        {
            get { return null; }
        }

        [ChoHiddenMember]
        public void Log(string msg)
        {
            if (_commandLineArgsObjectAttribute == null)
                return;
            _commandLineArgsObjectAttribute.GetMe(GetType()).Log(msg);
        }

        [ChoHiddenMember]
        public void Log(bool condition, string msg)
        {
            if (_commandLineArgsObjectAttribute == null)
                return;
            _commandLineArgsObjectAttribute.GetMe(GetType()).Log(condition, msg);
        }

        #endregion Instance Members (Public)

        #region Object Overrides

        public override string ToString()
        {
            return ChoObject.ToString(this);
        }

        #endregion Object Overrides

        #region Instance Members (Internal)
        
        [ChoHiddenMember]
        internal bool OnBeforeCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
            if (_beforeCommandLineArgObjectLoaded != null)
            {
                ChoPreviewCommandLineArgObjectEventArgs previewCommandLineArgObjectEventArgs = new ChoPreviewCommandLineArgObjectEventArgs(commandLineArgs);
                _beforeCommandLineArgObjectLoaded(this, previewCommandLineArgObjectEventArgs);
                return previewCommandLineArgObjectEventArgs.Cancel;
            }

            return false;
        }

        [ChoHiddenMember]
        internal void OnAfterCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
            if (_afterCommandLineArgObjectLoaded != null)
                _afterCommandLineArgObjectLoaded(this, new ChoCommandLineArgObjectEventArgs(commandLineArgs));
        }

        [ChoHiddenMember]
        internal bool OnCommandLineArgObjectLoadError(string[] commandLineArgs, Exception ex)
        {
            if (_commandLineArgObjectLoadError != null)
            {
                ChoCommandLineArgObjectErrorEventArgs configurationObjectErrorEventArgs = new ChoCommandLineArgObjectErrorEventArgs(commandLineArgs, ex);
                _commandLineArgObjectLoadError(this, configurationObjectErrorEventArgs);
                return configurationObjectErrorEventArgs.Handled;
            }

            return false;
        }

        [ChoHiddenMember]
        internal bool OnBeforeCommandLineArgLoaded(string memberName, object value, object defaultValue, object fallbackValue)
        {
            if (_beforeCommandLineArgLoaded != null)
            {
                ChoPreviewCommandLineArgEventArgs previewCommandLineArgEventArgs = new ChoPreviewCommandLineArgEventArgs(memberName, value, defaultValue, fallbackValue);
                _beforeCommandLineArgLoaded(this, previewCommandLineArgEventArgs);
                return previewCommandLineArgEventArgs.Cancel;
            }

            return false;
        }

        [ChoHiddenMember]
        internal void OnAfterCommandLineArgLoaded(string memberName, object value)
        {
            if (_afterCommandLineArgLoaded != null)
                _afterCommandLineArgLoaded(this, new ChoCommandLineArgEventArgs(memberName, value));
        }

        [ChoHiddenMember]
        internal bool OnCommandLineArgLoadError(string memberName, object unformattedValue, Exception ex)
        {
            if (_commandLineArgLoadError != null)
            {
                ChoCommandLineArgErrorEventArgs configurationObjectMemberErrorEventArgs = new ChoCommandLineArgErrorEventArgs(memberName, unformattedValue, ex);
                _commandLineArgLoadError(this, configurationObjectMemberErrorEventArgs);
                return configurationObjectMemberErrorEventArgs.Handled;
            }

            return false;
        }

        [ChoHiddenMember]
        internal void OnCommandLineArgMemberNotFound(string cmdLineArgSwitch, string cmdLineArgValue)
        {
            if (_commandLineArgMemberNotFound != null)
            {
                ChoCommandLineArgNotFoundEventArgs commandLineArgNotFoundEventArgs = new ChoCommandLineArgNotFoundEventArgs(cmdLineArgSwitch, cmdLineArgValue);
                _commandLineArgMemberNotFound(this, commandLineArgNotFoundEventArgs);
            }
        }

        [ChoHiddenMember]
        internal void OnUnrecognizedCommandLineArgFound(ChoUnrecognizedCommandLineArgEventArg eventArgs)
        {
            if (_unrecognizedCommandLineArgFound != null)
                _unrecognizedCommandLineArgFound(this, eventArgs);
        }

        #endregion Instance Members (Internal)
    }
}
