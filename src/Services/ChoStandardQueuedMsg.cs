namespace Cinchoo.Core.Services
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

    using Cinchoo.Core;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Threading;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO.IsolatedStorage;
    using System.Security.AccessControl;

    #endregion NameSpaces

    [DebuggerDisplay("IsQuitMsg = {_isQuitService}, Msg = {_msg}")]
    public struct ChoStandardQueuedMsg<T>
    {
        #region Instance Data Members (Private)

        private bool _isQuitService;
        private T _msg;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoStandardQueuedMsg(T msg, bool isQuitService)
        {
            _msg = msg;
            _isQuitService = isQuitService;
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static ChoStandardQueuedMsg<T> NewQuitServiceMsg()
        {
            return new ChoStandardQueuedMsg<T>(default(T), true);
        }

        #endregion Shared Members (Public)

        #region Instance Properties (Public)

        public bool IsQuitService
        {
            get { return _isQuitService; }
        }

        public T Msg
        {
            get { return _msg; }
        }

        #endregion Instance Properties (Public)
    }
}
