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

    [DebuggerDisplay("Path={DisplayPath}")]
    public sealed class ChoIniIncludeFileNode : ChoIniNode, IChoIniDocument, IChoIniNodesContainer
    {
        #region Instance Data Members (Private)

        private readonly string _filePath;
        private readonly List<ChoIniNewLineNode> _iniNewLineNodes = new List<ChoIniNewLineNode>();
        private readonly ChoIniDocument _iniDocument;
        //private ChoIniIncludeFileNode _parentIniIncludeFileNode = null;
        private ChoIniCommentNode _inlineCommentNode = null;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoIniIncludeFileNode(ChoIniDocument ownerDocument, string filePath)
            : this(ownerDocument, filePath, (string)null)
        {
        }

        internal ChoIniIncludeFileNode(ChoIniDocument ownerDocument, string filePath, string commentLine, Stream stream = null)
            : this(ownerDocument, filePath, String.IsNullOrEmpty(commentLine) ? null : new ChoIniCommentNode(ownerDocument, commentLine), stream)
        {
        }

        internal ChoIniIncludeFileNode(ChoIniDocument ownerDocument, string filePath, ChoIniCommentNode inlineCommentNode, Stream stream = null)
            : base(ownerDocument)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "Ini Include File Path");

            _filePath = filePath;
            _inlineCommentNode = inlineCommentNode;

            ChoIniDocument iniDocument = OwnerDocument;
            while (true)
            {
                if (iniDocument == null)
                    break;

                if (_filePath == iniDocument.Path)
                    throw new ChoIniDocumentException(String.Format("Can't include {0} document, it is already included in the include chain of documents.", _filePath));

                iniDocument = iniDocument.ParentIniDocument;
            }

            _iniDocument = new ChoIniDocument(_filePath, ownerDocument);
        }

        #endregion Constructors}

        #region ChoIniNode Overrides

        internal void ClearNSave()
        {
            _iniNewLineNodes.Clear();
            _iniDocument.ClearNSave();
        }

        internal void Save()
        {
            if (_iniDocument != null)
                _iniDocument.Save();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            if (CommentChar != ChoChar.NUL)
                msg.AppendFormat(@"{0}[INCLUDE(""{1}"")] {2}{3}", CommentChar, _filePath, InlineCommentString, Environment.NewLine);
            else
                msg.AppendFormat(@"[INCLUDE(""{0}"")] {1}{2}", _filePath, InlineCommentString, Environment.NewLine);

            foreach (ChoIniNewLineNode iniNewLineNode in _iniNewLineNodes)
                msg.AppendFormat("{0}{1}", iniNewLineNode.ToString(), Environment.NewLine);

            return msg.ToString();
        }

        public override bool Comment(char commentChar)
        {
            if (CommentChar != ChoChar.NUL)
                return false;

            if (!OwnerDocument.OnNodeChanging(this, null, null, ChoIniNodeChangedAction.Comment))
            {
                OwnerDocument.Dirty = true;

                CommentChar = commentChar;
                if (_iniDocument != null)
                    _iniDocument.Comment(commentChar);

                OwnerDocument.OnNodeChanged(this, null, null, ChoIniNodeChangedAction.Comment);

                return true;
            }

            return false;
        }

        public override bool Uncomment()
        {
            if (CommentChar == ChoChar.NUL)
                return false;

            if (!OwnerDocument.OnNodeChanging(this, null, null, ChoIniNodeChangedAction.Uncomment))
            {
                OwnerDocument.Dirty = true;

                ResetCommentChar();
                if (_iniDocument != null)
                    _iniDocument.Uncomment();

                OwnerDocument.OnNodeChanged(this, null, null, ChoIniNodeChangedAction.Uncomment);

                return true;
            }

            return false;
        }

        #endregion ChoIniNode Overrides

        #region Disposeable overrides

        protected override void Dispose(bool finalize)
        {
			if (_iniDocument != null) _iniDocument.Dispose();
        }

        #endregion Disposeable overrides

        #region Indexers

        public ChoIniSectionNode this[string sectionName]
        {
            get
            {
                ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
                return _iniDocument != null ? _iniDocument[sectionName] : null;
            }
        }

        #endregion Indexers

        #region Instance Members (Internal)

        internal new void Close()
        {
            _iniDocument.Close();
        }

		internal bool ContainsIniIncludeFile(string filePath)
		{
			if (_iniDocument != null)
			{
				if (_iniDocument.Path == filePath)
					return true;

				return _iniDocument.ContainsIniIncludeFileNode(filePath);
			}
			else
				return false;
		}

        #endregion Instance Members (Internal)

        #region IChoIniDocument Members

        public ChoIniNewLineNode AddNewLineToHeadingComments()
        {
            return _iniDocument.AddNewLineToHeadingComments();
        }

        public bool CommentSection(string sectionName)
        {
            return _iniDocument.CommentSection(sectionName);
        }

        public bool UncommentSection(string sectionName)
        {
            return _iniDocument.UncommentSection(sectionName);
        }

        public bool ContainsSection(string sectionName)
        {
            return _iniDocument.ContainsSection(sectionName);
        }

        public ChoIniCommentNode CreateComment(string comment, char commentChar)
        {
            return _iniDocument.CreateComment(comment, commentChar);
        }

        public ChoIniCommentNode CreateCommentNode(string comment)
        {
            return _iniDocument.CreateCommentNode(comment);
        }

        public ChoIniCommentNode AddHeadingComment(string commentLine)
        {
            return _iniDocument.AddHeadingComment(commentLine);
        }

        public ChoIniCommentNode[] AddHeadingComments(string[] commentLines)
        {
            return _iniDocument.AddHeadingComments(commentLines);
        }

        public ChoIniNameValueNode CreateNameValueNode(string name, string value)
        {
            return _iniDocument.CreateNameValueNode(name, value);
        }

        public ChoIniNewLineNode CreateNewLine()
        {
            return _iniDocument.CreateNewLine();
        }

        public bool RemoveSection(string sectionName)
        {
            return _iniDocument.RemoveSection(sectionName);
        }

        public IEnumerator<ChoIniNode> GetEnumerator()
        {
            return _iniDocument.GetEnumerator();
        }

        public ChoIniNode[] GetHeadingComments()
        {
            return _iniDocument.GetHeadingComments();
        }

        public ChoIniSectionNode GetSection(string sectionName)
        {
            return _iniDocument.GetSection(sectionName);
        }

        public bool RemoveHeadingComments()
        {
            return _iniDocument.RemoveHeadingComments();
        }

        public bool TryGetSection(string sectionName, out ChoIniSectionNode section)
        {
            return _iniDocument.TryGetSection(sectionName, out section);
        }

        public string Path
        {
            get { return _iniDocument.Path; }
        }

        public string[] AllIniFilePaths
        {
            get
            {
                return _iniDocument.AllIniFilePaths;
            }
        }

        #endregion

        #region Sections Members

        internal IEnumerable<ChoIniSectionNode> Sections
        {
            get
            {
                return _iniDocument.Sections;
            }
        }

        #endregion

        #region Instance Properties (Internal)

        internal string DisplayPath
        {
            get
            {
                if (_iniDocument == null)
                    return _filePath;
                else
                {
                    string displayPath = _filePath;
                    ChoIniDocument iniDocument = _iniDocument;
                    while (true)
                    {
                        if (iniDocument.ParentIniDocument == iniDocument)
                            break;
                        else
                        {
                            displayPath = String.Format("{0}|{1}", _iniDocument.ParentIniDocument.Path, displayPath);
                            iniDocument = iniDocument.ParentIniDocument;
                        }
                    }
                    return displayPath;
                }
            }
        }

        internal string InlineCommentString
        {
            get
            {
                return _inlineCommentNode != null ? _inlineCommentNode.ToString() : String.Empty;
            }
        }

        #endregion Instance Properties (Internal)

        #region Instance Members (Public)

        public ChoIniNewLineNode AddNewLineNode()
        {
            ChoIniNewLineNode newLineNode = new ChoIniNewLineNode(OwnerDocument);
            if (!OwnerDocument.OnNodeInserting(this, newLineNode))
            {
                OwnerDocument.Dirty = true;
                _iniNewLineNodes.Add(newLineNode);
                OwnerDocument.OnNodeInserted(this, newLineNode);

                return newLineNode;
            }

            return null;
        }

        #endregion Instance Members (Public)

        #region InlineComment Members

        public ChoIniCommentNode AddInlineCommentNode(string inlineComment)
        {
            return AddInlineCommentNode(new ChoIniCommentNode(OwnerDocument, inlineComment));
        }

        public ChoIniCommentNode AddInlineCommentNode(ChoIniCommentNode inlineCommentNode)
        {
            if (!OwnerDocument.OnNodeInserting(this, inlineCommentNode))
            {
                OwnerDocument.Dirty = true;
                _inlineCommentNode = inlineCommentNode;
                OwnerDocument.OnNodeInserted(this, inlineCommentNode);

                return _inlineCommentNode;
            }

            return null;
        }

        public bool RemoveInlineComment()
        {
            if (!OwnerDocument.OnNodeRemoving(this, _inlineCommentNode))
            {
                OwnerDocument.Dirty = true;
                ChoIniCommentNode oldValue = _inlineCommentNode;
                _inlineCommentNode = null;
                OwnerDocument.OnNodeRemoved(this, oldValue);
            }
            else
                return false;

            return true;
        }

        public bool ReplaceInlineComment(string commentLine)
        {
            return ReplaceInlineComment(new ChoIniCommentNode(OwnerDocument, commentLine));
        }

        public bool ReplaceInlineComment(ChoIniCommentNode inlineCommentNode)
        {
            if (!OwnerDocument.OnNodeChanging(this, _inlineCommentNode, inlineCommentNode, ChoIniNodeChangedAction.ReplaceNode))
            {
                OwnerDocument.Dirty = true;
                ChoIniCommentNode oldValue = _inlineCommentNode;
                _inlineCommentNode = inlineCommentNode;
                OwnerDocument.OnNodeChanged(this, oldValue, _inlineCommentNode, ChoIniNodeChangedAction.ReplaceNode);
            }
            else
                return false;

            return true;
        }

        #endregion InlineComment Members

		#region IChoIniDocument Members

        public override void Clear()
        {
            OwnerDocument.Dirty = true;
            _iniNewLineNodes.Clear();
            _iniDocument.Clear();
        }

		public ChoIniIncludeFileNode AddIniIncludeFileNode(string filePath)
		{
			return _iniDocument.AddIniIncludeFileNode(filePath);
		}

		public ChoIniIncludeFileNode AddIniIncludeFileNode(string filePath, string comments)
		{
			return _iniDocument.AddIniIncludeFileNode(filePath, comments);
		}

		public bool ContainsIniIncludeFileNode(string filePath)
		{
			return _iniDocument.ContainsIniIncludeFileNode(filePath);
		}

		public ChoIniIncludeFileNode GetIniIncludeFileNode(string filePath)
		{
			ChoIniIncludeFileNode iniIncludeFileNode = null;
			TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode);
			return iniIncludeFileNode;
		}

		public bool TryGetIniIncludeFileNode(string filePath, out ChoIniIncludeFileNode iniIncludeFileNode)
		{
			iniIncludeFileNode = null;

			if (_iniDocument != null)
			{
				if (_iniDocument.Path == filePath)
				{
					iniIncludeFileNode = this;
					return true;
				}
				else
					return _iniDocument.TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode);
			}
			else
				return false;
		}

		public bool RemoveIniIncludeFileNode(string filePath)
		{
			return _iniDocument.RemoveIniIncludeFileNode(filePath);
		}

		public bool CommentIniIncludeFileNode(string filePath)
		{
			return _iniDocument.CommentIniIncludeFileNode(filePath);
		}

		public bool UncommentIniIncludeFileNode(string filePath)
		{
			return _iniDocument.UncommentIniIncludeFileNode(filePath);
		}

        public ChoIniSectionNode AddSection(string sectionName)
		{
			return _iniDocument.AddSection(sectionName);
		}

		#endregion

        #region Object Overrides

        public override bool Equals(ChoIniNode other)
        {
            if (other is ChoIniIncludeFileNode)
                return _filePath == ((ChoIniIncludeFileNode)other)._filePath;
            else
                return false;
        }

        #endregion Object Overrides

        #region IChoIniNodesContainer Overrides

        public bool Remove(ChoIniNode node)
        {
            return _iniDocument.Remove(node);
        }

        public ChoIniNewLineNode AppendNewLine()
        {
            return _iniDocument.AppendNewLine();
        }

        #endregion IChoIniNodesContainer Overrides
    }
}
