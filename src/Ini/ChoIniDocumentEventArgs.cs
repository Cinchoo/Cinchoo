namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoIniDocumentEventArgs : EventArgs
    {
        #region Instance Data Members (Private)

        private ChoIniDocumentState[] _iniDocumentStates;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoIniDocumentEventArgs(ChoIniDocumentState iniDocumentState) : this(new ChoIniDocumentState[] { iniDocumentState })
        {
        }

        internal ChoIniDocumentEventArgs(ChoIniDocumentState[] iniDocumentStates)
        {
            _iniDocumentStates = iniDocumentStates;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public ChoIniDocumentState[] IniDocumentStates
        {
            get { return _iniDocumentStates; }
        }

        #endregion Instance Properties (Public)

        #region Object Overrides

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();

            if (_iniDocumentStates != null)
            {
                bool firstEntry = true;
                foreach (ChoIniDocumentState iniDocumentState in _iniDocumentStates)
                {
                    if (firstEntry)
                    {
                        msg.Append(iniDocumentState.ToString());
                        firstEntry = false;
                    }
                    else
                        msg.AppendFormat("{0}{1}", Environment.NewLine, iniDocumentState.ToString());
                }
            }

            return msg.ToString();
        }

        #endregion Object Overrides
    }

    public class ChoIniDocumentState : EventArgs
    {
        #region Constants

        public const int INVALID_LINE_NO = Int32.MinValue;

        #endregion Constants

        #region Instance Data Members (Private)

        private int _lineNo;
        private string _iniFilePath;
        private Exception _exception;
        private string _message;
        private string _line;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoIniDocumentState(int lineNo, string iniFilePath, Exception exception, string message, string line)
        {
            _lineNo = lineNo;
            _iniFilePath = iniFilePath;
            _exception = exception;
            _message = message;
            _line = line;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public int LineNo
        {
            get { return _lineNo; }
        }

        public string IniFilePath
        {
            get { return _iniFilePath; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string Line
        {
            get { return _line; }
        }

        #endregion Instance Properties (Public)

        #region Object Overrides (Public)

        public override string ToString()
        {
            return String.Format("LineNo: {0}, Path: {1}, Message: {2}, Line: {3}", _lineNo, _iniFilePath, _exception != null ? _exception.Message : _message, _line);
        }

        #endregion Object Overrides (Public)
    }
}
