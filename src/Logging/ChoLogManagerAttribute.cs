namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoLogManagerAttribute : ChoObjectNameableAttribute
    {
        #region Instance Properties

        private Type _logEntryType;
        public Type LogEntryType
        {
            get { return _logEntryType; }
            set
            {
                _logEntryType = value;

                if (value != null)
                    _logEntryData = ChoObjectManagementFactory.CreateInstance<ChoLogEntry>(_logEntryType);
                else
                    _logEntryData = ChoObjectManagementFactory.CreateInstance<ChoLogEntry>(typeof(ChoLogEntry));
            }
        }

        private ChoLogEntry _logEntryData;
        public ChoLogEntry LogEntryData
        {
            get { return _logEntryData; }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoLogManagerAttribute(string name) : base(name)
        {
        }

        #endregion Constructors
    }
}
