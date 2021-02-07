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
        public readonly object OriginalValue = null;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoPreviewConfigurationObjectMemberEventArgs(string memberName, string propertyName, object value, object originalValue)
            : base(memberName, propertyName, value)
        {
            OriginalValue = originalValue;
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

        internal ChoConfigurationObjectMemberErrorEventArgs(string memberName, string propertyName, object value, Exception ex)
            : base(memberName, propertyName, value)
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
        public readonly object Value = null;

        #endregion Instance Data Members (Public)

        #region Constructors

        internal ChoConfigurationObjectMemberEventArgs(string memberName, string propertyName, object value)
        {
            MemberName = memberName;
            PropertyName = propertyName;
            Value = value;
        }

        #endregion Constructors
    }
}
