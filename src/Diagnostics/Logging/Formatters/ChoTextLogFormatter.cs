namespace eSquare.Core.Diagnostics.Logging.Formatters
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;

    #endregion NameSpaces
    
    [ChoLogFormatterAttribute("ChoTextFormatter")]
    public class ChoTextLogFormatter : IChoLogFormatter
    {
        #region Instance Data Members (Private)

        private string template;

        #endregion Instance Data Members (Private)

        #region Constants

        private const string defaultMsgTemplate = "{timestamp} {message} {newline}";

        private const string timeStampToken = "{timestamp}";
        private const string messageToken = "{message}";
        private const string severityToken = "{severity}";

        private const string NewLineToken = "{newline}";
        private const string TabToken = "{tab}";

        #endregion Constants

        #region Constructors

        public ChoTextLogFormatter(string template)
		{
            if (!string.IsNullOrEmpty(template))
                this.template = template;
            else
                this.template = defaultMsgTemplate;
		}

        public ChoTextLogFormatter()
            : this(defaultMsgTemplate)
		{
		}

        #endregion Constructors

        #region IChoLogFormatter Members

        public string Format(ChoLogEntry logMessage)
        {
            StringBuilder msg = new StringBuilder(template);

            msg.Replace(timeStampToken, logMessage.TimeStamp.ToShortDateString());
            msg.Replace(messageToken, logMessage.Message);
            msg.Replace(severityToken, logMessage.Severity.ToString());

            msg.Replace(NewLineToken, Environment.NewLine);
            msg.Replace(TabToken, "\t");

            return msg.ToString();
        }

        #endregion
    }
}
