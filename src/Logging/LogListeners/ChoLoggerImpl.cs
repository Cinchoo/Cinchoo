namespace eSquare.Core.Diagnostics.Logging.Loggers
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public abstract class ChoLoggerImpl : IChoLoggerManager
    {
        #region Instance Data Members (Private)

        private string _msgPattern;

        #endregion Instance Data Members (Private)

        #region IChoLogger Members

        public string MsgPattern
        {
            get { return _msgPattern; }
            set { _msgPattern = value; }
        }

        public abstract void Log(object value);

        protected object Format(object value)
        {
            if (!(value is string)) return value;

            //TODO: Search and replace pattern keys in the object and return the resultant value
            return value;
        }

        #endregion
    }
}
