namespace Cinchoo.Core.Ini
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
    using System.Security.AccessControl;
    using System.Collections.Specialized;

    using Cinchoo.Core;
    using Cinchoo.Core.Threading;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO.IsolatedStorage;

    #endregion NameSpaces

    public abstract class ChoIniNode : ChoSyncDisposableObject<ChoIniNode>, IChoIniNode
    {
        #region Instance Data Members (Private)

        private char _commentChar;
        private ChoIniDocument _ownerDocument;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoIniNode(ChoIniDocument ownerDocument)
        {
            _ownerDocument = ownerDocument;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public abstract override string ToString();
        public abstract bool Comment(char commentChar);
        public abstract bool Uncomment();

        public virtual bool Comment()
        {
            return Comment(_ownerDocument.FirstAvailableCommentChar);
        }

        #endregion Instance Members (Public)

        #region Instance Properties (Protected)

        internal ChoIniDocument OwnerDocument
        {
            get { return _ownerDocument; }
        }

        protected char CommentChar
        {
            get { return _commentChar; }
            set
            {
                if (!ChoIniDocument.IsValidCommentChar(_ownerDocument.CommentChars, value))
                    throw new ChoIniDocumentException(String.Format("Invalid [`{0}`] comment character is passed.", value));

                _commentChar = value;
            }
        }

        protected void ResetCommentChar()
        {
            _commentChar = ChoChar.NUL;
        }

        #endregion Instance Properties (Protected)

        #region Disposeable overrides

        protected override void Dispose(bool finalize)
        {
        }

        #endregion Disposeable overrides

        #region IChoIniNode Members

        public object SyncRoot
        {
            get { return _ownerDocument.SyncRoot; }
        }

        public abstract void Clear();

        #endregion
    }
}
