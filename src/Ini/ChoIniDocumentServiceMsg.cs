namespace eSquare.Core.Ini
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Web;
    using System.Text;
    using System.Diagnostics;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Collections.Specialized;

    using eSquare.Core;
    using eSquare.Core.Types;
    using eSquare.Core.Services;
    using eSquare.Core.Threading;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.IO.IsolatedStorage;
    using System.Security.AccessControl;

    #endregion NameSpaces

    [DebuggerDisplay("IsQuitMsg = {_isQuitService}, Msg = {_msg}")]
    public struct ChoIniDocumentServiceMsg
    {
        #region Instance Data Members (Private)

        private bool _isQuitService;
        private string _msg;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoIniDocumentServiceMsg(string msg, bool isQuitService)
        {
            _msg = msg;
            _isQuitService = isQuitService;
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static ChoIniDocumentServiceMsg NewQuitServiceMsg()
        {
            return new ChoIniDocumentServiceMsg(null, true);
        }

        #endregion Shared Members (Public)

        #region Instance Properties (Public)

        public bool IsQuitService
        {
            get { return _isQuitService; }
        }

        public string Msg
        {
            get { return _msg; }
        }

        #endregion Instance Properties (Public)
    }
}
