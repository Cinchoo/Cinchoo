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

        #endregion Instance Data Members

        public ChoUnknownProperyEventArgs(string propertyName, string format)
        {
            PropertyName = propertyName;
            Format = format;
        }
    }
}
