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

    public sealed class ChoStandardQueuedMsgObject<T> : ChoQueuedMsgServiceObjectBase<T>
    {
        #region Instance Data Members (Private)

        private bool _isQuitService;

        #endregion Instance Data Members (Private)

        #region Constructors

        private ChoStandardQueuedMsgObject(bool isQuitService)
        {
            _isQuitService = isQuitService;
        }

        public ChoStandardQueuedMsgObject(T state)
            : base(state)
        {
        }

        #endregion Constructors

        #region ISyncMsgQObject<ChoIniDocumentState> Members

        public override bool IsQuitServiceMsg
        {
            get { return _isQuitService; }
        }

        #endregion ISyncMsgQObject<ChoIniDocumentState> Members

        #region Shared Members (Public)

        public static ChoStandardQueuedMsgObject<T> New(T msg)
        {
            return new ChoStandardQueuedMsgObject<T>(msg);
        }

        public static ChoStandardQueuedMsgObject<T> QuitMsg
        {
            get { return new ChoStandardQueuedMsgObject<T>(true); }
        }

        #endregion Shared Members (Public)
    }
}
