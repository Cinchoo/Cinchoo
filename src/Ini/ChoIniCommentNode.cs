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

    [DebuggerDisplay("{ToString()}")]
    public sealed class ChoIniCommentNode : ChoIniNode
    {
        #region Instance Data Members (Private)

        private string _commentLine;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoIniCommentNode(ChoIniDocument ownerDocument, string commentLine)
            : this(ownerDocument, commentLine, ownerDocument.FirstAvailableCommentChar)
        {
        }

        internal ChoIniCommentNode(ChoIniDocument ownerDocument, string commentLine, char commentChar)
            : base(ownerDocument)
        {
            Value = commentLine;
            CommentChar = commentChar;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public string Value
        {
            get { return _commentLine; }
            internal set
            {
                if (!String.IsNullOrEmpty(value) && value.IndexOf(Environment.NewLine) >= 0)
                    value = value.Replace(Environment.NewLine, " ");

                if (!OwnerDocument.OnNodeChanging(this, _commentLine, value, ChoIniNodeChangedAction.Value))
                {
                    OwnerDocument.Dirty = true;

                    string oldValue = _commentLine;
                    _commentLine = value;

                    OwnerDocument.OnNodeChanged(this, oldValue, _commentLine, ChoIniNodeChangedAction.Value);
                }
            }
        }

        #endregion Instance Properties (Public)

        #region Object Overrides

        public override string ToString()
        {
            return String.Format("{0}{1}", CommentChar, _commentLine);
        }

        public override bool Comment(char commentChar)
        {
			return true;
        }

        public override bool Uncomment()
        {
			return true;
        }

        public override void Clear()
        {
        }

        #endregion Object Overrides

        #region Object Overrides

        public override bool Equals(ChoIniNode other)
        {
            if (other is ChoIniCommentNode)
                return _commentLine == ((ChoIniCommentNode)other)._commentLine;
            else
                return false;
        }

        #endregion Object Overrides
    }
}
