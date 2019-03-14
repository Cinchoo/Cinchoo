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

	[DebuggerDisplay("Name={_name}")]
    public sealed class ChoIniSectionNode : ChoIniNode, IEnumerable<ChoIniNode>, IChoIniNodesContainer
    {
        #region Instance Data Members (Private)

        private string _name;

		private readonly List<ChoIniNode> _iniNodes = new List<ChoIniNode>();
		private ChoIniCommentNode _inlineCommentNode = null;
        private ChoIniSectionNode _parentIniSectionNode = null;

		#endregion Instance Data Members (Private)

		#region Constructors

		internal ChoIniSectionNode(ChoIniDocument ownerDocument, string name)
			: this(ownerDocument, name, null)
		{
		}

		internal ChoIniSectionNode(ChoIniDocument ownerDocument, string name, ChoIniCommentNode inlineCommentNode)
			: base(ownerDocument)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			_name = name != null ? name.Trim() : null;
			_inlineCommentNode = inlineCommentNode;
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public string Name
		{
			get { return _name; }
		}

		#endregion Instance Properties (Public)

		#region Instance Properties (Internal)

		internal int Count
		{
			get
			{
				return _iniNodes.Count;
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

		#region Indexers

		public string this[string name]
		{
			get
			{
				ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

				ChoIniNameValueNode nameValueNode;
				if (TryGetNameValueNode(name, out nameValueNode))
					return nameValueNode.Value;
				else
					return null;
			}
			set
			{
				ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

				ChoIniNameValueNode nameValueNode;
				if (TryGetNameValueNode(name, out nameValueNode))
					nameValueNode.Value = value;
				else
					throw new ChoIniDocumentException("No INI section '{0}' found.".FormatString(name));
			}
		}

		#endregion Indexers

		#region Instance Members (Internal)

        internal bool IsRootSection
        {
            get
            {
                return _name == ChoIniDocument.ROOT_SECTION_NAME;
            }
        }

		internal ChoIniNode this[int index]
		{
			get
			{
				return _iniNodes[index];
			}
		}

		internal ChoIniNode AddIniNode(ChoIniNode iniNode)
		{
			if (!OwnerDocument.OnNodeInserting(this, iniNode))
			{
				OwnerDocument.Dirty = true;
				_iniNodes.Add(iniNode);
				OwnerDocument.OnNodeInserted(this, iniNode);

				return iniNode;
			}

			return null;
		}

		internal bool InsertBeforeIniNode(int index, ChoIniNode iniNode)
		{
			if (!OwnerDocument.OnNodeInserting(this, iniNode))
			{
				OwnerDocument.Dirty = true;
				if (index - 1 < 0)
					_iniNodes.Insert(0, iniNode);
				else
					_iniNodes.Insert(index - 1, iniNode);

				OwnerDocument.OnNodeInserted(this, iniNode);
				return true;
			}

			return false;
		}

		internal bool InsertAfterIniNode(int index, ChoIniNode iniNode)
		{
			if (!OwnerDocument.OnNodeInserting(this, iniNode))
			{
				OwnerDocument.Dirty = true;
				if (index + 1 >= _iniNodes.Count)
					_iniNodes.Add(iniNode);
				else
					_iniNodes.Insert(index + 1, iniNode);

				OwnerDocument.OnNodeInserted(this, iniNode);
				return true;
			}

			return false;
		}

		internal int GetIndex(ChoIniNode iniNode)
		{
			return _iniNodes.IndexOf(iniNode);
		}

		internal void ClearNodes()
		{
			OwnerDocument.Dirty = true;
			_iniNodes.Clear();
		}

        internal bool AddSection(ChoIniSectionNode iniSectionNode)
        {
            ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

            if (Contains(iniSectionNode.Name))
                throw new ChoIniDocumentException(String.Format("Failed to add duplicate '{0}' section. Please check in the INI document and its sub-documents.", iniSectionNode.Name));

            iniSectionNode._parentIniSectionNode = this;
            return AddIniNode(iniSectionNode) != null;
        }

        internal bool DeleteSection(ChoIniSectionNode section)
        {
            return Remove(section);
        }

        internal bool CommentSection(ChoIniSectionNode section)
        {
            return section.Comment();
        }

        internal void UncommentSection(ChoIniSectionNode section)
        {
            section.Uncomment();
        }

		#endregion Instance Members (Internal)

		#region Instance Members (Public)

        public bool Delete()
        {
            if (_parentIniSectionNode != null)
                return _parentIniSectionNode.Remove(this);
            else
                return false;
        }

		#region InlineComment Members

		public ChoIniCommentNode AddInlineComment(string inlineComment)
		{
            ChoIniCommentNode inlineCommentNode = new ChoIniCommentNode(OwnerDocument, inlineComment);

            if (!OwnerDocument.OnNodeInserting(this, inlineCommentNode))
			{
				OwnerDocument.Dirty = true;
				_inlineCommentNode = inlineCommentNode;
				OwnerDocument.OnNodeInserted(this, inlineCommentNode);
			}
			return _inlineCommentNode;
		}

		public bool RemoveInlineComment()
		{
			ChoIniCommentNode inlineCommentNode = _inlineCommentNode;
			if (!OwnerDocument.OnNodeRemoving(this, _inlineCommentNode))
			{
				OwnerDocument.Dirty = true;
				_inlineCommentNode = null;
				OwnerDocument.OnNodeRemoved(this, inlineCommentNode);

				return true;
			}
			else
				return false;
		}

        public bool ReplaceInlineComment(string inlineComment)
		{
            ChoIniCommentNode inlineCommentNode = new ChoIniCommentNode(OwnerDocument, inlineComment);
			if (!OwnerDocument.OnNodeChanging(this, _inlineCommentNode, inlineCommentNode, ChoIniNodeChangedAction.ReplaceNode))
			{
				OwnerDocument.Dirty = true;
				object oldValue = _inlineCommentNode;
				_inlineCommentNode = inlineCommentNode;
				OwnerDocument.OnNodeChanged(this, oldValue, _inlineCommentNode, ChoIniNodeChangedAction.ReplaceNode);
				return true;
			}
			else
				return false;
		}

		#endregion InlineComment Members

		public NameValueCollection ToNameValueCollection()
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (ChoIniNode iniNode in this)
			{
				if (!(iniNode is ChoIniNameValueNode))
					continue;
				nameValueCollection.Add(((ChoIniNameValueNode)iniNode).Name, ((ChoIniNameValueNode)iniNode).Value);
			}

			return nameValueCollection;
		}

		public ChoIniCommentNode GetCommentNode(string commentLine)
		{
            ChoIniCommentNode commentNode = null;
            if (TryGetCommentNode(commentLine, out commentNode))
                return commentNode;
            else
			    throw new ChoIniDocumentException(String.Format("Can't find comment node. [Comment: {0}]", commentLine));
		}

        public bool TryGetCommentNode(string commentLine, out ChoIniCommentNode commentNode)
        {
            commentNode = null;
            foreach (ChoIniNode iniNode in _iniNodes)
            {
                if (!(iniNode is ChoIniCommentNode))
                    continue;
                if (((ChoIniCommentNode)iniNode).Value == commentLine)
                {
                    commentNode = iniNode as ChoIniCommentNode;
                    return true;
                }
            }
            return false;
        }

		public ChoIniNameValueNode GetNameValueNode(string name)
		{
            ChoIniNameValueNode nameValueNode = null;
            if (TryGetNameValueNode(name, out nameValueNode))
                return nameValueNode;
            else
    			throw new ChoIniDocumentException(String.Format("Can't find namevalue node. [Name: {0}]", name));
		}

		public bool TryGetNameValueNode(string name, out ChoIniNameValueNode nameValueNode)
		{
			nameValueNode = null;
			foreach (ChoIniNode iniNode in _iniNodes)
			{
				if (!(iniNode is ChoIniNameValueNode))
					continue;
				if (String.Compare(((ChoIniNameValueNode)iniNode).Name, name, false) == 0)
				{
					nameValueNode = iniNode as ChoIniNameValueNode;
					return true;
				}
			}
			return false;
		}

		public bool ContainsNameValueNode(string name)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			ChoIniNameValueNode nameValueNode;
			return TryGetNameValueNode(name, out nameValueNode);
		}

		public string GetValue(string name)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			ChoIniNameValueNode nameValueNode = GetNameValueNode(name);
			return nameValueNode != null ? nameValueNode.Value : null;
		}

		public bool TryGetValue(string name, out string value)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			value = null;
			ChoIniNameValueNode nameValueNode = GetNameValueNode(name);
			value = nameValueNode != null ? nameValueNode.Value : null;
			return nameValueNode != null;
		}

		public ChoIniNameValueNode AddNameValueNode(string name, string value)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			//Check for duplicate name value node
			if (ContainsNameValueNode(name))
                throw new ChoIniDocumentException(String.Format("Failed to add `{0}` namevalue node. Found duplicate node.", name));
			
			ChoIniNameValueNode nameValueNode = new ChoIniNameValueNode(OwnerDocument, name, value);
			return AddIniNode(nameValueNode) as ChoIniNameValueNode;
		}

		public bool RemoveNameValueNode(string name)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			ChoIniNameValueNode nameValueNode = GetNameValueNode(name);
			return Remove(nameValueNode);
		}

		public bool CommentNameValueNode(string name)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			ChoIniNameValueNode nameValueNode = GetNameValueNode(name);
			return nameValueNode.Comment();
		}

		public ChoIniCommentNode AddCommentNode(string commentLine)
		{
			ChoIniCommentNode commentNode = new ChoIniCommentNode(OwnerDocument, commentLine);
			return AddIniNode(commentNode) as ChoIniCommentNode;
		}

		public bool RemoveCommentNode(string commentLine)
		{
			ChoIniCommentNode commentNode = GetCommentNode(commentLine);
			return Remove(commentNode);
		}

		public bool ReplaceCommentNode(string origCommentLine, string newCommentLine)
		{
			ChoIniCommentNode newCommentNode = new ChoIniCommentNode(OwnerDocument, newCommentLine);
			ChoIniCommentNode commentNode = GetCommentNode(origCommentLine);
			if (!OwnerDocument.OnNodeChanging(this, commentNode, newCommentNode, ChoIniNodeChangedAction.ReplaceNode))
			{
				OwnerDocument.Dirty = true;
				object oldValue = commentNode;
				commentNode.Value = newCommentLine;
				OwnerDocument.OnNodeChanged(this, oldValue, newCommentNode, ChoIniNodeChangedAction.ReplaceNode);
				return true;
			}
			else
				return false;
		}

		#endregion Instance Members (Public)

		#region IEnumerable<ChoIniNode> Members

		public IEnumerator<ChoIniNode> GetEnumerator()
		{
			return _iniNodes.GetEnumerator();
		}

		#endregion

		#region Keys Members

		public IEnumerable<string> Keys
		{
			get
			{
				foreach (ChoIniNode iniNode in this)
				{
					if (iniNode is ChoIniNameValueNode)
						yield return ((ChoIniNameValueNode)iniNode).Name;
				}
			}
		}

		public IEnumerable<KeyValuePair<string, string>> KeyValues
		{
			get
			{
				foreach (ChoIniNode iniNode in this)
				{
					if (iniNode is ChoIniNameValueNode)
						yield return new KeyValuePair<string, string>(((ChoIniNameValueNode)iniNode).Name, ((ChoIniNameValueNode)iniNode).Value);
				}
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _iniNodes.GetEnumerator();
		}

		#endregion

		#region ChoIniNode Overrides

		public override string ToString()
		{
			StringBuilder msg = new StringBuilder();
			if (CommentChar != ChoChar.NUL)
				msg.AppendFormat("{0}[{1}] {2}{3}", CommentChar, GetName(), InlineCommentString, Environment.NewLine);
			else
                msg.AppendFormat("[{0}] {1}{2}", GetName(), InlineCommentString, Environment.NewLine);

			foreach (ChoIniNode iniNode in _iniNodes)
				msg.AppendFormat("{0}{1}", iniNode.ToString(), Environment.NewLine);

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
				foreach (ChoIniNode iniNode in _iniNodes)
					iniNode.Comment(commentChar);
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
				foreach (ChoIniNode iniNode in _iniNodes)
					iniNode.Uncomment();
				OwnerDocument.OnNodeChanged(this, null, null, ChoIniNodeChangedAction.Uncomment);

				return true;
			}

			return false;
		}

		public override void Clear()
		{
			OwnerDocument.Dirty = true;
			_iniNodes.Clear();
			_inlineCommentNode = null;
		}

		#endregion ChoIniNode Overrides

		#region Object Overrides

		public override bool Equals(ChoIniNode other)
		{
			if (other is ChoIniSectionNode)
				return _name == ((ChoIniSectionNode)other)._name;
			else
				return false;
		}

		#endregion Object Overrides

        #region Section Manipulation Methods

        public bool Contains(string sectionName)
        {
            return GetSection(sectionName) != null;
        }

        public ChoIniSectionNode GetSection(string sectionName)
        {
            foreach (ChoIniNode iniNode in _iniNodes)
            {
                if (iniNode is ChoIniSectionNode)
                {
                    if (String.Compare(((ChoIniSectionNode)iniNode).Name, sectionName, false) == 0)
                        return iniNode as ChoIniSectionNode;
                }
                else if (iniNode is ChoIniIncludeFileNode)
                {
                    ChoIniSectionNode sectionNode = ((ChoIniIncludeFileNode)iniNode).GetSection(sectionName);
                    if (sectionNode != null)
                        return sectionNode;
                }
            }

            return null;
        }

        public bool TryGetSection(string sectionName, out ChoIniSectionNode section)
        {
            section = GetSection(sectionName);

            return section != null;
        }

        public ChoIniSectionNode AddSection(string sectionName)
        {
            if (Contains(sectionName))
                throw new ChoIniDocumentException(String.Format("Failed to add `{0}` section. Found duplicate node. Please check in the INI document and its sub-documents.", sectionName));

            ChoIniSectionNode iniSection = new ChoIniSectionNode(this.OwnerDocument, sectionName);
            iniSection._parentIniSectionNode = this;
            return AddIniNode(iniSection) as ChoIniSectionNode;
        }

        //public bool InsertBeforeSection(string sectionName, ChoIniSectionNode iniSectionNode)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
        //    ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

        //    ChoIniSectionNode insertBeforeSection = null;

        //    if (!TryGetSection(sectionName, out insertBeforeSection))
        //        throw new ChoIniDocumentException(String.Format("Can't find {0} section.", sectionName));
        //    else
        //        return InsertBeforeIniNode(GetIndex(insertBeforeSection), iniSectionNode);
        //}

        //public bool InsertAfterSection(string sectionName, ChoIniSectionNode iniSectionNode)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
        //    ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

        //    ChoIniSectionNode insertBeforeSection = null;

        //    if (!TryGetSection(sectionName, out insertBeforeSection))
        //        throw new ChoIniDocumentException(String.Format("Can't find {0} section.", sectionName));
        //    else
        //        return InsertBeforeIniNode(GetIndex(insertBeforeSection), iniSectionNode);
        //}

        //public ChoIniSectionNode CreateSection(string sectionName)
        //{
        //    if (Contains(sectionName))
        //        throw new ChoIniDocumentException(String.Format("Duplicate `{0}` section found. Please check in the INI document and its sub-documents.", sectionName));

        //    ChoIniSectionNode iniSection = new ChoIniSectionNode(this.OwnerDocument, sectionName);
        //    iniSection._parentIniSectionNode = this;
        //    return iniSection;
        //}

        public bool DeleteSection(string sectionName)
        {
            ChoIniSectionNode foundSection = GetSection(sectionName);

            if (foundSection == null)
                throw new ChoIniDocumentException(String.Format("Can't find `{0}` section.", sectionName));
            else
                return DeleteSection(foundSection);
        }

        public bool CommentSection(string sectionName)
        {
            ChoIniSectionNode foundSection = GetSection(sectionName);

            if (foundSection == null)
                throw new ChoIniDocumentException(String.Format("Can't find `{0}` section.", sectionName));
            else
                return CommentSection(foundSection);
        }

        public void UncommentSection(string sectionName)
        {
            ChoIniSectionNode foundSection = GetSection(sectionName);

            if (foundSection == null)
                throw new ChoIniDocumentException(String.Format("Can't find `{0}` section.", sectionName));
            else
                UncommentSection(foundSection);
        }

        #endregion Section Manipulation Methods

        #region Instance Members (Private)

        private string GetName()
        {
            StringBuilder name = new StringBuilder();

            ChoIniSectionNode iniSectionNode = this;

            name.Append(_name);
            while (iniSectionNode._parentIniSectionNode != null)
            {
                if (iniSectionNode._parentIniSectionNode.IsRootSection)
                    break;

                name.Insert(0, '/');
                name.Insert(0, iniSectionNode._parentIniSectionNode._name);
                iniSectionNode = iniSectionNode._parentIniSectionNode;
            }

            return name.ToString();
        }

        #endregion Instance Members (Private)

        #region IChoIniNodesContainer Overrides

        public ChoIniNewLineNode AppendNewLine()
        {
            ChoIniNewLineNode iniNewLineNode = new ChoIniNewLineNode(this.OwnerDocument);
            _iniNodes.Add(iniNewLineNode);
            return iniNewLineNode;
        }

        //public bool Remove(ChoIniNode iniNode)
        //{
        //    if (iniNode == null)
        //        return false;

        //    if (_iniNodes.Contains(iniNode))
        //    {
        //        _iniNodes.Remove(iniNode);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public bool Remove(ChoIniNode iniNode)
        {
            ChoGuard.ArgumentNotNull(iniNode, "Node");

            int index = _iniNodes.FindIndex((node) => node == iniNode);
            if (index < 0)
                throw new ChoIniDocumentException(String.Format("Can't find node. [Node: {0}]", iniNode, iniNode.ToString()));

            if (!OwnerDocument.OnNodeRemoving(this, iniNode))
            {
                _iniNodes.RemoveAt(index);
                OwnerDocument.Dirty = true;
                OwnerDocument.OnNodeRemoved(this, iniNode);
                return true;
            }

            return false;
        }

        #endregion IChoIniNodesContainer Overrides
    }
}
