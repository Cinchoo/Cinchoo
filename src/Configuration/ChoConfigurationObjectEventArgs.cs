using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Configuration
{
    public class ChoPreviewConfigurationObjectEventArgs : ChoConfigurationObjectEventArgs
    {
        #region Instance Data Members (Public)

        public bool Cancel = false;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoPreviewConfigurationObjectEventArgs()
        {
        }

        #endregion Constructors
    }

    public class ChoConfigurationObjectEventArgs : EventArgs
    {
        #region Constructors

        internal ChoConfigurationObjectEventArgs()
        {
        }

        #endregion Constructors
    }

    public class ChoConfigurationObjectErrorEventArgs : ChoConfigurationObjectEventArgs
    {
        #region Instance Data Members (Public)

        public Exception Exception = null;
        public bool Handled = false;
        public bool Dirty = false;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoConfigurationObjectErrorEventArgs(Exception ex)
        {
            Exception = ex;
        }

        #endregion Constructors
    }

    public class ChoPreviewConfigurationObjectMemberEventArgs : ChoConfigurationObjectMemberEventArgs
    {
        #region Instance Data Members (Public)

        public bool Cancel = false;
        public readonly object OriginalState = null;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoPreviewConfigurationObjectMemberEventArgs(string memberName, string propertyName, object state, object originalState)
            : base(memberName, propertyName, state)
        {
            OriginalState = originalState;
        }

        #endregion Constructors
    }

    public class ChoConfigurationObjectMemberErrorEventArgs : ChoConfigurationObjectMemberEventArgs
    {
        #region Instance Data Members (Public)

        public bool Handled = false;
        public readonly Exception Exception;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoConfigurationObjectMemberErrorEventArgs(string memberName, string propertyName, object state, Exception ex)
            : base(memberName, propertyName, state)
        {
            Exception = ex;
        }

        #endregion Constructors
    }

    public class ChoConfigurationObjectMemberEventArgs : EventArgs
    {
        #region Instance Data Members (Public)

        public readonly string MemberName;
        public readonly string PropertyName;
        public readonly object State = null;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoConfigurationObjectMemberEventArgs(string memberName, string propertyName, object state)
        {
            MemberName = memberName;
            PropertyName = propertyName;
            State = state;
        }

        #endregion Constructors
    }
}
