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

    public sealed class ChoIniDocumentMsgObject : ChoQueuedMsgServiceObjectBase<ChoStandardQueuedMsg>
    {
        #region Constructors

        public ChoIniDocumentMsgObject()
        {
        }

        public ChoIniDocumentMsgObject(ChoStandardQueuedMsg state)
            : base(state)
        {
        }

        #endregion Constructors

        #region ISyncMsgQObject<ChoIniDocumentState> Members

        public override bool IsQuitServiceMsg
        {
            get { return State.IsQuitService; }
        }

        public override IChoQueuedMsgServiceObject<ChoStandardQueuedMsg> QuitServiceMsg
        {
            get { return new ChoIniDocumentMsgObject(ChoStandardQueuedMsg.NewQuitServiceMsg()); }
        }

        #endregion ISyncMsgQObject<ChoIniDocumentState> Members

        #region Shared Members (Public)

        public static ChoIniDocumentMsgObject New(string msg)
        {
            return New(msg, false);
        }

        public static ChoIniDocumentMsgObject New(string msg, bool isQuitService)
        {
            return new ChoIniDocumentMsgObject(new ChoStandardQueuedMsg(msg, isQuitService));
        }

        #endregion Shared Members (Public)
    }
}
