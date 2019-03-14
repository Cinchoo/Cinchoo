namespace eSquare.Core.Diagnostics.Logging.LogListeners
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;

    using eSquare.Core.Diagnostics.Logging.Formatters;

    #endregion NameSpaces

    public abstract class ChoLogListener : TraceListener
    {
        #region Instance Data Members (Private)

        private IChoLogFormatter _logFormatter;

        #endregion Instance Data Members (Private)

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoLogListener"/> class.
        /// </summary>
        protected ChoLogListener()
        {
        }

        /// <summary>
        /// Gets or sets the log entry formatter.
        /// </summary>
        public IChoLogFormatter Formatter
        {
            get { return _logFormatter; }
            set { _logFormatter = value; }
        }
    }
}
