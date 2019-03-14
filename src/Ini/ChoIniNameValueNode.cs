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

    [DebuggerDisplay("{_name} = {_value}")]
    public sealed class ChoIniNameValueNode : ChoIniNode
    {
        #region Instance Data Members (Private)

        private char _nameValueSeperator;
        private string _name = null;
        private string _rawValue = null;
        private string _value = null;
        private ChoIniCommentNode _inlineCommentNode = null;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoIniNameValueNode(ChoIniDocument ownerDocument, string name, string value)
            : this(ownerDocument, name, value, ownerDocument.NameValueSeperator, null)
        {
        }

        internal ChoIniNameValueNode(ChoIniDocument ownerDocument, string name, string value, char nameValueSeperator)
            : this(ownerDocument, name, value, nameValueSeperator, null)
        {
        }

        internal ChoIniNameValueNode(ChoIniDocument ownerDocument, string name, string value, char nameValueSeperator, ChoIniCommentNode inlineCommentNode)
            : base(ownerDocument)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            //Check for valid delimiter
            if (ownerDocument.NameValueSeperator != nameValueSeperator)
                throw new ChoIniDocumentException(String.Format("Invalid NameValue Seperator [{0}] passed.", nameValueSeperator));

            _nameValueSeperator = nameValueSeperator;
            _name = name.Trim();
            if (!ownerDocument.IgnoreValueWhiteSpaces)
                _rawValue = value;
            else
                _rawValue = value != null ? value.Trim() : null;
            
            _inlineCommentNode = inlineCommentNode;
            NormalizeValue();
        }

        private void NormalizeValue()
        {
            if (_rawValue.IsNull())
                return;

            _value = _rawValue;
            string trimValue = _value.Trim();
            if (trimValue.StartsWith("\""))
            {
                _value = _value.Remove(_value.IndexOf("\""), 1);
                int lastQuotesIndex = _value.LastIndexOf("\"");
                if (lastQuotesIndex >= 0)
                    _value = _value.Remove(lastQuotesIndex, 1);
            }
            else
            {
                bool first = true;
                StringBuilder valueBuilder = new StringBuilder();
                string lValue = null;
                foreach (string value in _value.Split(Environment.NewLine))
                {
                    trimValue = value.Trim();
                    if (trimValue.EndsWith("\\"))
                        lValue = value.Remove(value.IndexOf("\\"), 1);
                    else
                        lValue = value;

                    if (first)
                    {
                        first = false;
                        valueBuilder.Append(lValue);
                    }
                    else
                        valueBuilder.Append(Environment.NewLine + lValue);
                }
                _value = valueBuilder.ToString();
            }
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
            set 
            {
                RawValue = value;
            }
        }

        public string RawValue
        {
            get { return _rawValue; }
            set
            {
                if (!OwnerDocument.OnNodeChanging(this, _rawValue, value, ChoIniNodeChangedAction.Value))
                {
                    OwnerDocument.Dirty = true;
                    string oldValue = _rawValue;
                    _rawValue = value;
                    OwnerDocument.OnNodeChanged(this, oldValue, _rawValue, ChoIniNodeChangedAction.Value);
                }

                NormalizeValue();
            }
        }

        #endregion Instance Properties (Public)

        #region Instance Properties (Internal)

        internal string InlineCommentString
        {
            get
            {
                return _inlineCommentNode != null ? _inlineCommentNode.ToString() : String.Empty;
            }
        }

        #endregion Instance Propeties (Internal)

        #region Instance Members (Public)

        public ChoIniCommentNode CreateOrReplaceInlineCommentNode(string inlineComment)
        {
            return CreateOrReplaceInlineCommentNode(new ChoIniCommentNode(OwnerDocument, inlineComment));
        }

        public ChoIniCommentNode CreateOrReplaceInlineCommentNode(ChoIniCommentNode inlineCommentNode)
        {
            _inlineCommentNode = inlineCommentNode;
            return _inlineCommentNode;
        }

        public void RemoveInlineComment()
        {
            _inlineCommentNode = null;
        }

        public void ReplaceInlineComment(string commentLine)
        {
            ReplaceInlineComment(new ChoIniCommentNode(OwnerDocument, commentLine));
        }

        public void ReplaceInlineComment(ChoIniCommentNode InlineComment)
        {
            _inlineCommentNode = InlineComment;
        }

        public override void Clear()
        {
        }

        #endregion Instance Members (Public)

        #region ChoIniNode Overrides

        public override string ToString()
        {
            string inlineCommentString = InlineCommentString;
            if (CommentChar == ChoChar.NUL)
            {
                if (_value == null)
                {
                    if (!String.IsNullOrEmpty(inlineCommentString))
                        return String.Format("{0} {1}", _name, inlineCommentString);
                    else
                        return String.Format("{0}", _name);
                }
                else
                {
                    if (!String.IsNullOrEmpty(inlineCommentString))
                        return String.Format("{0}{1}{2} {3}", _name, _nameValueSeperator, _value, inlineCommentString);
                    else
                        return String.Format("{0}{1}{2}", _name, _nameValueSeperator, _value);
                }
            }
            else
            {
                if (_value == null)
                {
                    if (!String.IsNullOrEmpty(inlineCommentString))
                        return String.Format("{0} {1} {2}", CommentChar, _name, inlineCommentString);
                    else
                        return String.Format("{0} {1}", CommentChar, _name);
                }
                else
                {
                    if (!String.IsNullOrEmpty(inlineCommentString))
                        return String.Format("{0}{1}{2}{3} {4}", CommentChar, _name, _nameValueSeperator, _value, inlineCommentString);
                    else
                        return String.Format("{0}{1}{2}{3}", CommentChar, _name, _nameValueSeperator, _value);
                }
            }
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

        #endregion ChoIniNode Overrides

        #region Object Overrides

        public override bool Equals(ChoIniNode other)
        {
            if (other is ChoIniNameValueNode)
                return _name == ((ChoIniNameValueNode)other)._name;
            else
                return false;
        }

        #endregion Object Overrides
    }
}
