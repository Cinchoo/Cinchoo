namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public class ChoUnknownProperyEventArgs : EventArgs
    {
        #region Instance Data Members

        public readonly string PropertyName;
        public readonly string Format;
        public string PropertyValue;
        public bool Resolved;
        public object Context;
        public bool HelpTextRequested;
        public readonly Dictionary<string, string> HelpTextDict = new Dictionary<string,string>();

        #endregion Instance Data Members

        internal ChoUnknownProperyEventArgs()
        {
            HelpTextRequested = true;
        }

        public ChoUnknownProperyEventArgs(string propertyName, string format)
        {
            PropertyName = propertyName;
            Format = format;
        }
    }

    public class ChoObjectFormaterEventArgs : EventArgs
    {
        #region Instance Data Members

        public readonly Type ObjectType;
        public ICustomFormatter Formatter;
        public bool Resolved;

        #endregion Instance Data Members

        internal ChoObjectFormaterEventArgs(Type objectType)
        {
            objectType = ObjectType;
        }
    }
}
