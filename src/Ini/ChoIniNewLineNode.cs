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

    [DebuggerDisplay("NewLine")]
    public sealed class ChoIniNewLineNode : ChoIniNode
    {
        #region Constructors

        internal ChoIniNewLineNode(ChoIniDocument ownerDocument)
            : base(ownerDocument)
        {
        }

        #endregion Constructors

        #region ChoIniNode Overrides

        public override string ToString()
        {
            if (CommentChar == ChoChar.NUL)
                return String.Empty;
            else
                return CommentChar.ToString();
        }

        public override bool Comment(char commentChar)
        {
			if (!OwnerDocument.OnNodeChanging(this, null, null, ChoIniNodeChangedAction.Comment))
			{
				OwnerDocument.Dirty = true;
				CommentChar = commentChar;
				OwnerDocument.OnNodeChanged(this, null, null, ChoIniNodeChangedAction.Comment);

				return true;
			}

			return false;
		}

        public override bool Uncomment()
        {
			if (!OwnerDocument.OnNodeChanging(this, null, null, ChoIniNodeChangedAction.Uncomment))
			{
				OwnerDocument.Dirty = true;
				ResetCommentChar();
				OwnerDocument.OnNodeChanged(this, null, null, ChoIniNodeChangedAction.Uncomment);

				return true;
			}

			return false;
		}

        public override void Clear()
        {
        }

        #endregion ChoIniNode Overrides

        #region Object Overrides

        public override bool Equals(ChoIniNode other)
        {
            if (other is ChoIniNode)
                return GetHashCode() == ((ChoIniNode)other).GetHashCode();
            else
                return false;
        }

        #endregion Object Overrides
    }
}
