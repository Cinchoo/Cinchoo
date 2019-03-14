namespace Cinchoo.Core.Ini
{
	#region NameSpaces

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Security.AccessControl;
    using System.Text;
    using System.Text.RegularExpressions;
    using Cinchoo.Core;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    #region ChoIniLoadOptions Class

    public class ChoIniLoadOptions
    {
        #region Shared Data Members (Public)

        public static readonly ChoIniLoadOptions Default = new ChoIniLoadOptions();

        #endregion Shared Data Members (Public)

        #region Instance Data Members (Public)

        public char NameValueSeperator;
        public string CommentChars;
        public bool IgnoreValueWhiteSpaces;

        #endregion Instance Data Members (Public)

        #region Constructors

        public ChoIniLoadOptions()
        {
            NameValueSeperator = ChoIniSettings.Me.NameValueSeperator;
            CommentChars = ChoIniSettings.Me.CommentChars;
            IgnoreValueWhiteSpaces = ChoIniSettings.Me.IgnoreValueWhiteSpaces;
        }
        
        #endregion Constructors
    }

    #endregion ChoIniLoadOptions Class

    // ignore leading and trailing whitespace around the parameter value; others consider all characters following the equals sign (including whitespace) to be part of the value.
	[DebuggerDisplay("Path = {_path}")]
    public sealed class ChoIniDocument : ChoSyncDisposableObject, IEnumerable<ChoIniNode>, IChoIniDocument, IChoIniNode, IChoIniNodesContainer
	{
		#region Constants

        private const string DEFAULT_COMMENT_CHARS = ";#";
        private const char DEFAULT_NAME_VALUE_DELIMITER = '=';
		internal const string ROOT_SECTION_NAME = "${{ROOT}}";
		private const string INI_INCLUDE_FILE_FILEPATH_TOKEN = "filePath";
		private const string INI_INCLUDE_FILE_COMMENTS_TOKEN = "comments";

		#endregion Constants

		#region Events

		public event EventHandler<ChoIniDocumentEventArgs> TextIgnored;
		public event EventHandler<ChoIniDocumentEventArgs> TextErrorFound;

		// Summary:
		//     Occurs when the content of IniNode belonging to this document
		//     has been changed.
		public event EventHandler<ChoIniNodeChangedEventArgs> NodeChanged;
		//
		// Summary:
		//     Occurs when the content of IniNode belonging to this document
		//     is about to be changed.
		public event EventHandler<ChoIniNodeChangingEventArgs> NodeChanging;
		//
		// Summary:
		//     Occurs when a node belonging to this document has been inserted into it.
		public event EventHandler<ChoIniNodeInsertedEventArgs> NodeInserted;
		//
		// Summary:
		//     Occurs when a node belonging to this document is about to be inserted it.
		public event EventHandler<ChoIniNodeInsertingEventArgs> NodeInserting;
		//
		// Summary:
		//     Occurs when a node belonging to this document has been removed from its parent.
		public event EventHandler<ChoIniNodeRemovedEventArgs> NodeRemoved;
		//
		// Summary:
		//     Occurs when a node belonging to this document is about to be removed from
		//     the document.
		public event EventHandler<ChoIniNodeRemovingEventArgs> NodeRemoving;

		#endregion Events

		#region Instance Data Members (Private)

        private object _syncRoot;
        
        private readonly object _dirtyLockObject = new object();
        private readonly bool _ignoreInvalidNode = true;
        private readonly List<ChoIniNode> _headings = new List<ChoIniNode>();
        private readonly string _path;
		
        private char _nameValueSeperator = DEFAULT_NAME_VALUE_DELIMITER;
		private string _commentChars = DEFAULT_COMMENT_CHARS;
		private ChoIniSectionNode _rootSection;
		private bool _ignoreValueWhiteSpaces = false;
		private Regex _includeIniFileRegEx;
		private ChoIniDocument _parentIniDocument;
		private ChoIniDocument _ultimateParentIniDocument;
        private ChoIniLoadOptions _loadOptions;

        private Stream _stream;
		private bool _dirty = false;
		private bool _documentLoaded = false;
		private int _lineNo;

		#endregion Instance Data Members (Private)

		#region Constructors

        internal ChoIniDocument(string path, ChoIniDocument parentIniDocument)
        {
            _syncRoot = parentIniDocument.SyncRoot;
            _parentIniDocument = parentIniDocument;
            _ultimateParentIniDocument = parentIniDocument.UltimateParentIniDocument;

            _path = ChoPath.GetFullPath(path);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_path));
            _stream = new FileStream(ChoPath.GetFullPath(_path), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            LoadIni(new ChoNoCloseStreamReader(_stream), parentIniDocument, _ultimateParentIniDocument, _syncRoot, parentIniDocument._loadOptions);
        }

        internal ChoIniDocument(string path, ChoIniLoadOptions loadOptions = null)
        {
            _path = ChoPath.GetFullPath(path);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_path));
            _stream = new FileStream(ChoPath.GetFullPath(_path), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            LoadIni(new ChoNoCloseStreamReader(_stream), null, this, new object(), loadOptions);
        }

        private ChoIniDocument(Stream stream, ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
        {
            _stream = stream;
            LoadIni(new ChoNoCloseStreamReader(stream), null, this, new object(), loadOptions);
        }

        private ChoIniDocument(TextReader textReader, ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
        {
            LoadIni(textReader, null, this, new object(), loadOptions);
        }

		#endregion Constructors

        #region Instance Members (Internal)

        internal void OnTextIgnored(string line)
		{
			if (UltimateParentIniDocument.TextIgnored != null)
				UltimateParentIniDocument.TextIgnored(this, new ChoIniDocumentEventArgs(new ChoIniDocumentState(_lineNo, Path, null, null, line)));
		}

		internal void OnTextErrorFound(string line, Exception ex)
		{
			OnTextIgnored(line);
			if (UltimateParentIniDocument.TextErrorFound != null)
				UltimateParentIniDocument.TextErrorFound(this, new ChoIniDocumentEventArgs(new ChoIniDocumentState(_lineNo, Path, ex, null, line)));
		}

		internal void OnNodeChanged(IChoIniNode iniNode, object oldValue, object newValue, ChoIniNodeChangedAction action)
		{
			if (!_documentLoaded) return;
			if (UltimateParentIniDocument.NodeChanged != null)
				UltimateParentIniDocument.NodeChanged(this, new ChoIniNodeChangedEventArgs(iniNode, oldValue, newValue, action));
		}

		internal bool OnNodeChanging(IChoIniNode iniNode, object oldValue, object newValue, ChoIniNodeChangedAction action)
		{
			if (!_documentLoaded) return false;
			if (UltimateParentIniDocument.NodeChanging != null)
			{
				ChoIniNodeChangingEventArgs iniNodeChangingEventArgs = new ChoIniNodeChangingEventArgs(iniNode, oldValue, newValue, action);
				UltimateParentIniDocument.NodeChanging(this, iniNodeChangingEventArgs);
				return iniNodeChangingEventArgs.Cancel;
			}

			return false;
		}

		internal void OnNodeInserted(IChoIniNode iniNode, object newValue)
		{
			if (!_documentLoaded) return;
			if (UltimateParentIniDocument.NodeInserted != null)
				UltimateParentIniDocument.NodeInserted(this, new ChoIniNodeInsertedEventArgs(iniNode, newValue));
		}

		internal bool OnNodeInserting(IChoIniNode iniNode, object newValue)
		{
			if (!_documentLoaded) return false;
			if (UltimateParentIniDocument.NodeInserting != null)
			{
				ChoIniNodeInsertingEventArgs iniNodeInsertingEventArgs = new ChoIniNodeInsertingEventArgs(iniNode, newValue);
				UltimateParentIniDocument.NodeInserting(this, iniNodeInsertingEventArgs);
				return iniNodeInsertingEventArgs.Cancel;
			}

			return false;
		}

		internal void OnNodeRemoved(IChoIniNode iniNode, object oldValue)
		{
			if (!_documentLoaded) return;
			if (UltimateParentIniDocument.NodeRemoved != null)
				UltimateParentIniDocument.NodeRemoved(this, new ChoIniNodeRemovedEventArgs(iniNode, oldValue));
		}

		internal bool OnNodeRemoving(IChoIniNode iniNode, object oldValue)
		{
			if (!_documentLoaded) return false;
			if (UltimateParentIniDocument.NodeRemoving != null)
			{
				ChoIniNodeRemovingEventArgs iniNodeRemovingEventArgs = new ChoIniNodeRemovingEventArgs(iniNode, oldValue);
				UltimateParentIniDocument.NodeRemoving(this, iniNodeRemovingEventArgs);
				return iniNodeRemovingEventArgs.Cancel;
			}

			return false;
		}

		#endregion Instance Members (Internal)

		#region Instance Members (Private)

        private bool CommentIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
        {
            ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");

            return iniIncludeFileNode.Comment();
        }

        private bool UncommentIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
        {
            ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");

            return iniIncludeFileNode.Uncomment();
        }

        private bool DeleteIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
        {
            ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");

            if (!ContainsIniIncludeFileNode(iniIncludeFileNode.Path))
                throw new ChoIniDocumentException("Can't find '{0}' include node.".FormatString(iniIncludeFileNode.Path));

            foreach (ChoIniNode iniNode in _rootSection)
            {
                if (iniNode is ChoIniIncludeFileNode)
                {
                    ChoIniIncludeFileNode iniChildIncludeFileNode = iniNode as ChoIniIncludeFileNode;
                    if (iniChildIncludeFileNode.Path == iniIncludeFileNode.Path)
                        return _rootSection.Remove(iniIncludeFileNode);
                    else
                        return iniIncludeFileNode.RemoveIniIncludeFileNode(iniChildIncludeFileNode.Path);
                }
            }

            return false;
        }

        private bool DeleteSection(ChoIniSectionNode section)
        {
            return section.Delete(); // _rootSection.RemoveNode(section);
        }

        private bool CommentSection(ChoIniSectionNode section)
        {
            return section.Comment();
        }

        private bool UncommentSection(ChoIniSectionNode section)
        {
            return section.Uncomment();
        }

        internal bool AppendHeadingComment(ChoIniCommentNode comment)
        {
            ChoGuard.ArgumentNotNull(comment, "Comment");

            return AppendHeadingComments(new ChoIniCommentNode[] { comment });
        }

        internal bool AppendHeadingComments(ChoIniCommentNode[] comments)
        {
            if (!ChoGuard.IsArgumentNotNullOrEmpty(comments))
                return true;

            if (!OnNodeInserting(this, comments))
            {
                foreach (ChoIniCommentNode iniCommentNode in comments)
                {
                    if (iniCommentNode == null)
                        continue;

                    Dirty = true;
                    _headings.Add(iniCommentNode);
                }
                OnNodeInserted(this, comments);
            }
            else
                return false;

            return true;
        }

		private string ReadSections(string line, TextReader sr)
		{
			if (line == null) return null;

			//Check if this is include file section
			Match match = _includeIniFileRegEx.Match(line);
			if (match.Success)
			{
                if (Path.IsNullOrWhiteSpace())
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line[0] == '[') //Section
                            break;
                    }
                }
                else
                {
                    ChoIniIncludeFileNode iniIncludeFileNode = null;
                    try
                    {
                        iniIncludeFileNode = AddIniIncludeFileNode(match.Groups[INI_INCLUDE_FILE_FILEPATH_TOKEN].ToString(),
                            match.Groups[INI_INCLUDE_FILE_COMMENTS_TOKEN].ToString());
                    }
                    catch (Exception ex)
                    {
                        if (_ignoreInvalidNode)
                        {
                            OnTextErrorFound(line, ex);
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line[0] == '[') //Section
                                    break;
                            }
                        }
                        else
                            throw;
                    }
                    finally
                    {
                        if (iniIncludeFileNode != null)
                            _rootSection.AddIniNode(iniIncludeFileNode);

                        //Add any new lines to this node
                        while ((line = sr.ReadLine()) != null)
                        {
                            _lineNo++;

                            line = line.Trim();

                            if (line.Length == 0) //WhiteSpace
                            {
                                if (iniIncludeFileNode != null)
                                    iniIncludeFileNode.AddNewLineNode();
                                else
                                    OnTextIgnored(line);
                            }
                            else if (line[0] == '[') //Section
                                break;
                            else
                            {
                                if (iniIncludeFileNode != null)
                                    OnTextIgnored(line);
                            }

                        }
                    }
                }
				return line;
			}
			else
			{
				ChoIniSectionNode currentSection = null;
				try
				{
					currentSection = ExtractAndCreateIniSectionEx(line);
				}
				catch (Exception ex)
				{
					if (_ignoreInvalidNode)
						OnTextErrorFound(line, ex);
					else
						throw;
				}
				finally
				{
                    //if (currentSection != null)
                    //    _rootSection.AddIniNode(currentSection);

					//Add any new lines to this node
					while ((line = sr.ReadLine()) != null)
					{
						_lineNo++;

						line = line.Trim();

						if (line.Length == 0) //WhiteSpace
						{
							if (currentSection != null)
								currentSection.AppendNewLine();
							else
								OnTextIgnored(line);
						}
                        else if (IsValidCommentChar(_commentChars, line[0])) //Comment
						{
							if (currentSection != null)
								currentSection.AddIniNode(CreateComment(line.Substring(1), line[0]));
							else
								OnTextIgnored(line);
						}
						else if (line[0] == '[') //Section
							break;
						else // Name = Value / Name
						{
							if (currentSection != null)
							{
								int equalPos = line.IndexOf('=');
                                if (equalPos < 0) // No name value pair
                                {
                                    string value = null;
                                    string inlineComment = null;
                                    ExtractCommentFromLine(ref line, out inlineComment);

                                    string name = line;
                                    if (inlineComment.IsNullOrWhiteSpace())
                                        currentSection.AddIniNode(CreateNameValueNode(name, value));
                                    else
                                        currentSection.AddIniNode(CreateNameValueNode(name, value, NewIniCommentLineNode(inlineComment)));
                                }
                                else
                                {
                                    string name = line.Substring(0, equalPos);
                                    string value = line.Substring(equalPos + 1);
                                    string inlineComment = null;

                                    if (name.IsNullOrWhiteSpace())
                                    {
                                        OnTextIgnored(line);
                                        continue;
                                    }
                                    else if (!value.IsNullOrEmpty())
                                    {
                                        if (_ignoreValueWhiteSpaces)
                                            value = value.Trim();

                                        string trimValue = value.Trim();
                                        if (trimValue.EndsWith("\\"))
                                        {
                                            StringBuilder valueBuilder = new StringBuilder(value);
                                            while ((line = sr.ReadLine()) != null)
                                            {
                                                string trimEndLine = line.TrimEnd();
                                                if (trimEndLine.EndsWith("\\"))
                                                    valueBuilder.Append(Environment.NewLine + line);
                                                else
                                                {
                                                    ExtractCommentFromLine(ref line, out inlineComment);
                                                    valueBuilder.Append(Environment.NewLine + line);
                                                    value = valueBuilder.ToString();
                                                    break;
                                                }
                                            }
                                        }
                                        else if (trimValue.StartsWith("\""))
                                        {
                                            StringBuilder valueBuilder = new StringBuilder(value);
                                            if (trimValue.LastIndexOf('"') != trimValue.Length - 1)
                                            {
                                                while ((line = sr.ReadLine()) != null)
                                                {
                                                    string trimEndLine = line.TrimEnd();
                                                    if (!trimEndLine.Contains("\""))
                                                        valueBuilder.Append(Environment.NewLine + line);
                                                    else
                                                    {
                                                        ExtractCommentFromLine(ref line, out inlineComment);
                                                        valueBuilder.Append(Environment.NewLine + line);
                                                        value = valueBuilder.ToString();
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ExtractCommentFromLine(ref value, out inlineComment);
                                        }

                                    }

                                    if (inlineComment.IsNullOrWhiteSpace())
                                        currentSection.AddIniNode(CreateNameValueNode(name, value));
                                    else
                                        currentSection.AddIniNode(CreateNameValueNode(name, value, NewIniCommentLineNode(inlineComment)));
                                }
							}
							else
								OnTextIgnored(line);
						}
					}
				}
				return line;
			}
		}

		private string ReadHeadings(TextReader sr)
		{
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				_lineNo++;

				line = line.Trim();

				if (line.Length == 0) //WhiteSpace
					AddNewLineToHeadingComments();
                else if (IsValidCommentChar(_commentChars, line[0])) //Comment
					AppendHeadingComment(new ChoIniCommentNode(this, line.Substring(1), line[0]));
				else if (line[0] == '[') //Section
					return line;
				else
					OnTextIgnored(line);
			}

			return null;
		}

        private void LoadIni(TextReader textReader, ChoIniDocument parentIniDocument,
            ChoIniDocument ultimateParentIniDocument,
            object syncRoot,
            ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
        {
            if (loadOptions == default(ChoIniLoadOptions))
                _loadOptions = ChoIniLoadOptions.Default;
            else
                _loadOptions = loadOptions;

            _nameValueSeperator = _loadOptions.NameValueSeperator;
            _commentChars = _loadOptions.CommentChars;
            _ignoreValueWhiteSpaces = _loadOptions.IgnoreValueWhiteSpaces;

            _parentIniDocument = parentIniDocument;
            _ultimateParentIniDocument = ultimateParentIniDocument;
            _syncRoot = syncRoot;
            _includeIniFileRegEx = new Regex(String.Format(@"^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*[{1}](?<{2}>.*)$|^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*$", INI_INCLUDE_FILE_FILEPATH_TOKEN, _commentChars, INI_INCLUDE_FILE_COMMENTS_TOKEN),
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _rootSection = new ChoIniSectionNode(this, ROOT_SECTION_NAME);

            _rootSection.ClearNodes();

            ChoIniSectionNode currentSection = _rootSection;

            _lineNo = 0;
            //Load the sections into memory
            string line;
            line = ReadHeadings(textReader);
            while ((line = ReadSections(line, textReader)) != null)
                ;

            DocumentLoaded = true;
        }

		private ChoIniSectionNode ExtractAndCreateIniSection(string line)
		{
            string commentLine;
            ExtractCommentFromLine(ref line, out commentLine);

            string sectionName = line.Trim();
			if (sectionName.Length == 1
				|| sectionName.IndexOf('[', 1) > 0
				|| sectionName.Split(']').Length > 2
				|| !sectionName.EndsWith("]"))
			{
				if (!_ignoreInvalidNode)
					throw new ChoIniDocumentException(String.Format("Section [{0}] found with unmatching brackets.", sectionName));
				else
					OnTextErrorFound(line, new ChoIniDocumentException(String.Format("Section [{0}] found with unmatching brackets.", sectionName)));
			}
			else
			{
				ChoIniSectionNode iniSectionNode = AddSection(sectionName.Substring(1, sectionName.Length - 2));
                if (commentLine != null && commentLine.Length > 0)
                    iniSectionNode.AddCommentNode(commentLine);

				return iniSectionNode;
			}

			return null;
		}

        private ChoIniSectionNode ExtractAndCreateIniSectionEx(string line)
        {
            string commentLine;
            ExtractCommentFromLine(ref line, out commentLine);

            string sectionName = line.Trim();
            if (sectionName.Length == 1
                || sectionName.IndexOf('[', 1) > 0
                || sectionName.Split(']').Length > 2
                || !sectionName.EndsWith("]"))
            {
                if (!_ignoreInvalidNode)
                    throw new ChoIniDocumentException(String.Format("Section [{0}] found with unmatching brackets.", sectionName));
                else
                    OnTextErrorFound(line, new ChoIniDocumentException(String.Format("Section [{0}] found with unmatching brackets.", sectionName)));
            }
            else
            {
                ChoIniSectionNode iniSectionNode = null;
                if (sectionName.IndexOf('/') < 0)
                    iniSectionNode = AddSection(sectionName.Substring(1, sectionName.Length - 2));
                else
                    iniSectionNode = CreateNestedIniSection(sectionName.Substring(1, sectionName.Length - 2));

                if (iniSectionNode != null)
                {
                    if (commentLine != null && commentLine.Length > 0)
                        iniSectionNode.AddCommentNode(commentLine);
                }

                return iniSectionNode;
            }

            return null;
        }

        private ChoIniSectionNode CreateNestedIniSection(string nestedSectionName) 
        {
            ChoIniSectionNode parentIniSectionNode = _rootSection;

            foreach (string sectionName in nestedSectionName.SplitNTrim('/'))
            {
                ChoIniSectionNode iniSectionNode = parentIniSectionNode.GetSection(sectionName);
                if (iniSectionNode == null)
                    iniSectionNode = new ChoIniSectionNode(this, sectionName);
                else
                {
                    parentIniSectionNode = iniSectionNode;
                    continue;
                }

                if (iniSectionNode != null)
                {
                    parentIniSectionNode.AddSection(iniSectionNode);
                    parentIniSectionNode = iniSectionNode;
                }
                else
                    break;
            }

            return parentIniSectionNode;
        }

        private ChoIniCommentNode NewIniCommentLineNode(string commentLine)
        {
            ChoIniCommentNode commentNode = null;

            if (commentLine.Length == 1)
                commentNode = new ChoIniCommentNode(this, String.Empty, commentLine[0]);
            else
                commentNode = new ChoIniCommentNode(this, commentLine.Substring(1), commentLine[0]);
            return commentNode;
        }

        private void ExtractCommentFromLine(ref string line, out string commentLine)
        {
            int commentPos = line.IndexOfAny(_commentChars.ToCharArray());

            commentLine = commentPos >= 0 ? line.Substring(commentPos) : null;
            line = commentPos >= 0 ? line.Substring(0, commentPos) : line;
        }

        private IEnumerable<ChoIniIncludeFileNode> GetAllIniIncludeFileNodes()
        {
            if (_rootSection != null)
            {
                foreach (ChoIniNode iniNode in _rootSection)
                {
                    if (iniNode is ChoIniIncludeFileNode)
                        yield return iniNode as ChoIniIncludeFileNode;
                }
            }
        }

		#endregion Instance Members (Private)

		#region Instance Properties (Internal)

		internal ChoIniDocument ParentIniDocument
		{
			get { return _parentIniDocument; }
		}

		internal ChoIniDocument UltimateParentIniDocument
		{
			get { return _ultimateParentIniDocument; }
		}

		internal char FirstAvailableCommentChar
		{
			get
			{
				if (String.IsNullOrEmpty(_commentChars))
					return DEFAULT_COMMENT_CHARS[0];
				else
					return _commentChars[0];
			}
		}

		internal bool DocumentLoaded
		{
			get { return _documentLoaded; }
			set
			{
				_documentLoaded = value;
				if (value)
					Dirty = false;
			}
		}

		internal bool Dirty
		{
			get 
			{
				lock (UltimateParentIniDocument._dirtyLockObject) 
				{ 
					return UltimateParentIniDocument._dirty;
				} 
			}
			set 
			{
				lock (UltimateParentIniDocument._dirtyLockObject)
				{
					if (!UltimateParentIniDocument._documentLoaded)
						return;

					UltimateParentIniDocument._dirty = value;
				}
			}
		}

		#endregion Instance Properties (Internal)

		#region Parse Overloads (Public)

        public static ChoIniDocument Parse(string text, ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
        {
            return new ChoIniDocument(new StringReader(text), loadOptions);
        }

        #endregion Parse Overloads (Public)

        #region Load Overloads

        public static ChoIniDocument Load(Stream stream, ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
        {
            return new ChoIniDocument(stream, loadOptions);
        }

        public static ChoIniDocument Load(TextReader reader, ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
        {
            return new ChoIniDocument(reader, loadOptions);
        }

        public static ChoIniDocument Load(string path, ChoIniLoadOptions loadOptions = default(ChoIniLoadOptions))
		{
            return new ChoIniDocument(path, loadOptions);
		}

        #endregion Load Overloads

        #region Clear Method

        public static void Clean(string path)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");

            using (ChoIniDocument doc = ChoIniDocument.Load(path))
            {
                doc.ClearNSave();
            }
        }

        #endregion Clear Method

        #region Instance Members (Public)

        #region Save Overloads

        //public void Save(Stream stream)
        //{
        //    foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
        //        iniIncludeFileNode.Save();

        //    if (stream != null)
        //    {
        //        _stream.Position = 0;
        //        _stream.SetLength(0);
        //        using (ChoNoCloseStreamWriter sw = new ChoNoCloseStreamWriter(stream))
        //        {
        //            sw.Write(ToString());
        //            sw.Flush();
        //        }
        //    }

        //    Dirty = false;
        //}

        //public void Save(TextWriter textWriter)
        //{
        //    foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
        //        iniIncludeFileNode.Save();

        //    textWriter.Write(ToString());

        //    Dirty = false;
        //}

        //public void Save(string path)
        //{
        //    foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
        //        iniIncludeFileNode.Save();

        //    using (StreamWriter sw = new StreamWriter(path))
        //    {
        //        sw.Write(ToString());
        //        sw.Flush();
        //    }

        //    Dirty = false;
        //}

        public void Save()
        {
            foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
                iniIncludeFileNode.Save();

            if (_stream != null)
            {
                _stream.Position = 0;
                _stream.SetLength(0);
                using (ChoNoCloseStreamWriter sw = new ChoNoCloseStreamWriter(_stream))
                {
                    sw.Write(ToString());
                    sw.Flush();
                }
            }

            Dirty = false;
        }

        #endregion Save Overloads

        #region IChoIniNodesContainer Overrides

        public ChoIniNewLineNode AppendNewLine()
        {
            //Add NewLine to the last section, if it dont have one
            if (_rootSection.Count > 0)
            {
                ChoIniSectionNode lastSection = _rootSection[_rootSection.Count - 1] as ChoIniSectionNode;
                return lastSection.AddIniNode(CreateNewLine()) as ChoIniNewLineNode;
            }
            else
                return AddNewLineToHeadingComments();
        }

        public bool Remove(ChoIniNode iniNode)
        {
            ChoGuard.ArgumentNotNull(iniNode, "Node");

            return _rootSection.Remove(iniNode);
        }

        #endregion IChoIniNodesContainer Overrides

		#region Section Manipulation Methods

        public ChoIniSectionNode GetSection(string sectionName)
        {
            bool found = false;
            ChoIniSectionNode iniSectionNode = _rootSection;
            foreach (string splitSectionName in sectionName.Split('/'))
            {
                found = false;
                foreach (ChoIniNode iniNode in iniSectionNode)
                {
                    if (iniNode is ChoIniSectionNode)
                    {
                        if (String.Compare(((ChoIniSectionNode)iniNode).Name, splitSectionName, false) == 0)
                        {
                            iniSectionNode = iniNode as ChoIniSectionNode;
                            found = true;
                            break;
                        }
                    }
                    else if (iniNode is ChoIniIncludeFileNode)
                    {
                        ChoIniSectionNode sectionNode = ((ChoIniIncludeFileNode)iniNode).GetSection(splitSectionName);
                        if (sectionNode != null)
                        {
                            iniSectionNode = sectionNode;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    iniSectionNode = null;
                    break;
                }
            }

            return iniSectionNode == _rootSection ? null : iniSectionNode;
        }

		public bool ContainsSection(string sectionName)
		{
			return GetSection(sectionName) != null;
		}

        //public ChoIniSectionNode GetSection(string sectionName)
        //{
        //    foreach (ChoIniNode iniNode in _rootSection)
        //    {
        //        if (iniNode is ChoIniSectionNode)
        //        {
        //            if (String.Compare(((ChoIniSectionNode)iniNode).Name, sectionName, false) == 0)
        //                return iniNode as ChoIniSectionNode;
        //        }
        //        else if (iniNode is ChoIniIncludeFileNode)
        //        {
        //            ChoIniSectionNode sectionNode = ((ChoIniIncludeFileNode)iniNode).GetSection(sectionName);
        //            if (sectionNode != null)
        //                return sectionNode;
        //        }
        //    }

        //    return null;
        //}

		public bool TryGetSection(string sectionName, out ChoIniSectionNode section)
		{
			section = GetSection(sectionName);

			return section != null;
		}

        public ChoIniSectionNode AddSection(string sectionName)
		{
			if (ContainsSection(sectionName))
				throw new ChoIniDocumentException(String.Format("Failed to add '{0}' section. Found duplicate section. Please check in the INI document and its sub-documents.", sectionName));

			ChoIniSectionNode iniSection = new ChoIniSectionNode(this, sectionName);
            _rootSection.AddIniNode(iniSection);
            return iniSection;
		}

        //public bool AddSection(ChoIniSectionNode iniSectionNode)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

        //    if (Contains(iniSectionNode.Name))
        //        throw new ChoIniDocumentException(String.Format("Duplicate `{0}` section found. Please check in the INI document and its sub-documents.", iniSectionNode.Name));

        //    return _rootSection.AddIniNode(iniSectionNode) != null;
        //}

        //public bool InsertBeforeSection(string sectionName, ChoIniSectionNode iniSectionNode)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
        //    ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

        //    ChoIniSectionNode insertBeforeSection = null;

        //    if (!TryGetSection(sectionName, out insertBeforeSection))
        //        throw new ChoIniDocumentException(String.Format("Can't find {0} section.", sectionName));
        //    else
        //        return _rootSection.InsertBeforeIniNode(_rootSection.GetIndex(insertBeforeSection), iniSectionNode);
        //}

        //public bool InsertAfterSection(string sectionName, ChoIniSectionNode iniSectionNode)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
        //    ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

        //    ChoIniSectionNode insertBeforeSection = null;

        //    if (!TryGetSection(sectionName, out insertBeforeSection))
        //        throw new ChoIniDocumentException(String.Format("Can't find {0} section.", sectionName));
        //    else
        //        return _rootSection.InsertBeforeIniNode(_rootSection.GetIndex(insertBeforeSection), iniSectionNode);
        //}

        //public ChoIniSectionNode CreateSection(string sectionName)
        //{
        //    if (Contains(sectionName))
        //        throw new ChoIniDocumentException(String.Format("Duplicate `{0}` section found. Please check in the INI document and its sub-documents.", sectionName));

        //    return new ChoIniSectionNode(this, sectionName);
        //}

        public bool RemoveSection(string sectionName)
        {
            ChoIniSectionNode foundSection = GetSection(sectionName);

            if (foundSection == null)
                throw new ChoIniDocumentException(String.Format("Failed to find `{0}` section to remove.", sectionName));
            else
                return DeleteSection(foundSection);
        }

		public bool CommentSection(string sectionName)
		{
			ChoIniSectionNode foundSection = GetSection(sectionName);

			if (foundSection == null)
                throw new ChoIniDocumentException(String.Format("Failed to find `{0}` section to comment.", sectionName));
			else
				return CommentSection(foundSection);
		}

        public bool UncommentSection(string sectionName)
        {
            ChoIniSectionNode foundSection = GetSection(sectionName);

            if (foundSection == null)
                throw new ChoIniDocumentException(String.Format("Can't find `{0}` section.", sectionName));
            else
                return UncommentSection(foundSection);
        }
		
		#endregion Section Manipulation Methods

		#region Comments Manipulation Methods

        public ChoIniNode[] GetHeadingComments()
        {
            return _headings.ToArray();
        }

        public ChoIniCommentNode AddHeadingComment(string commentLine)
		{
			return AddHeadingComments(new string[] { commentLine }).FirstOrDefault();
		}

        public ChoIniCommentNode[] AddHeadingComments(string[] commentLines)
        {
            if (!ChoGuard.IsArgumentNotNullOrEmpty(commentLines))
                return null;

            List<ChoIniCommentNode> iniCommentNodes = new List<ChoIniCommentNode>();
            if (!OnNodeInserting(this, commentLines))
            {
                //RemoveHeadingComments();
                foreach (string commentLine in commentLines)
                {
                    ChoIniCommentNode iniCommentNode = new ChoIniCommentNode(this, commentLine, FirstAvailableCommentChar);
                    Dirty = true;
                    _headings.Add(iniCommentNode);
                    iniCommentNodes.Add(iniCommentNode);
                }
                OnNodeInserted(this, commentLines);
            }
            else
                return null;

            return iniCommentNodes.ToArray();
        }

        public ChoIniNewLineNode AddNewLineToHeadingComments()
        {
            ChoIniNewLineNode iniNewLineNode = new ChoIniNewLineNode(this);
            if (!OnNodeInserting(this, iniNewLineNode))
            {
                Dirty = true;
                _headings.Add(iniNewLineNode);
                OnNodeInserted(this, iniNewLineNode);
            }
            else
                return null;

            return iniNewLineNode;
        }

        public bool RemoveHeadingComments()
        {
            ChoIniNode[] headings = _headings.ToArray();
            if (!OnNodeRemoving(this, headings))
            {
                Dirty = true;
                _headings.Clear();
                OnNodeRemoved(this, headings);
            }
            else
                return false;

            return true;
        }

		#endregion Comments Manipulation Methods

		#region IncludeFile Manipulation Methods

        #region AddIniIncludeFileNode Overloads

        public ChoIniIncludeFileNode AddIniIncludeFileNode(string filePath)
		{
            return AddIniIncludeFileNode(filePath, null);
		}

		public ChoIniIncludeFileNode AddIniIncludeFileNode(string filePath, string comments)
		{
            if (_path.IsNullOrWhiteSpace())
                throw new ChoIniDocumentException("Can't include nested INI nodes to INI document which don't have path attached to it.");

            ChoIniIncludeFileNode iniIncludeFileNode = new ChoIniIncludeFileNode(this, GetFullPath(filePath), comments);
            _rootSection.AddIniNode(iniIncludeFileNode);
            return iniIncludeFileNode;
        }

        #endregion AddIniIncludeFileNode Overloads

        public bool ContainsIniIncludeFileNode(string filePath)
        {
            ChoGuard.ArgumentNotNull(filePath, "FilePath");

            filePath = GetFullPath(filePath);
            foreach (ChoIniNode iniNode in _rootSection)
            {
                if (iniNode is ChoIniIncludeFileNode)
                {
                    ChoIniIncludeFileNode iniIncludeFileNode = iniNode as ChoIniIncludeFileNode;
                    if (iniIncludeFileNode.Path == filePath)
                        return true;
                    else
                        return iniIncludeFileNode.ContainsIniIncludeFile(filePath);
                }
            }

            return false;
        }

        public ChoIniIncludeFileNode GetIniIncludeFileNode(string filePath)
        {
            ChoGuard.ArgumentNotNull(filePath, "FilePath");

            filePath = GetFullPath(filePath);
            foreach (ChoIniNode iniNode in _rootSection)
            {
                if (iniNode is ChoIniIncludeFileNode)
                {
                    ChoIniIncludeFileNode iniIncludeFileNode = iniNode as ChoIniIncludeFileNode;
                    if (iniIncludeFileNode.Path == filePath)
                        return iniIncludeFileNode;
                    else
                        return iniIncludeFileNode.GetIniIncludeFileNode(filePath);
                }
            }

            return null;
        }

		public bool TryGetIniIncludeFileNode(string filePath, out ChoIniIncludeFileNode iniIncludeFileNode)
		{
			iniIncludeFileNode = GetIniIncludeFileNode(filePath);

			return iniIncludeFileNode != null;
		}

        //public bool InsertIniIncludeFileNodeBefore(int index, ChoIniIncludeFileNode iniIncludeFileNode)
        //{
        //    if (index < 0)
        //        throw new IndexOutOfRangeException("Index should be positive.");

        //    ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");

        //    return _rootSection.InsertBeforeIniNode(index, iniIncludeFileNode);
        //}

        //public bool InsertIniIncludeFileNodeAfter(int index, ChoIniIncludeFileNode iniIncludeFileNode)
        //{
        //    if (index < 0)
        //        throw new IndexOutOfRangeException("Index should be positive.");

        //    ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");

        //    return _rootSection.InsertAfterIniNode(index, iniIncludeFileNode);
        //}

        public bool RemoveIniIncludeFileNode(string filePath)
        {
            ChoIniIncludeFileNode iniIncludeFileNode = null;

            filePath = GetFullPath(filePath);
            if (!TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode))
                throw new ChoIniDocumentException("Can't find '{0}' include ini node.".FormatString(filePath));

            return DeleteIniIncludeFileNode(iniIncludeFileNode);
        }

        public bool CommentIniIncludeFileNode(string filePath)
        {
            ChoIniIncludeFileNode iniIncludeFileNode = null;

            filePath = GetFullPath(filePath);
            if (!TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode))
                throw new ChoIniDocumentException("Can't find '{0}' include ini node.".FormatString(filePath));

            return CommentIniIncludeFileNode(iniIncludeFileNode);
        }

        public bool UncommentIniIncludeFileNode(string filePath)
        {
            ChoIniIncludeFileNode iniIncludeFileNode = null;

            filePath = GetFullPath(filePath);
            if (!TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode))
                throw new ChoIniDocumentException("Can't find '{0}' include ini node.".FormatString(filePath));

            return UncommentIniIncludeFileNode(iniIncludeFileNode);
        }

		#endregion IncludeFile Manipulation Methods

		#endregion Instance Members (Public)

		#region Instance Properties (Public)

		public string[] AllIniFilePaths
		{
            get
            {
                List<string> iniFilePaths = new List<string>();
                iniFilePaths.Add(Path);

                foreach (ChoIniNode iniNode in this)
                {
                    if (iniNode is ChoIniIncludeFileNode)
                        iniFilePaths.AddRange(((IChoIniDocument)iniNode).AllIniFilePaths);
                }
                return iniFilePaths.ToArray();
            }
        }

		public string Path
		{
			get { return _path; }
		}

		public bool IgnoreValueWhiteSpaces
		{
			get { return _ignoreValueWhiteSpaces; }
		}

		public char NameValueSeperator
		{
			get { return _nameValueSeperator; }
		}

		public string CommentChars
		{
			get { return _commentChars; }
		}

		public bool IgnoreInvalidNode
		{
			get { return _ignoreInvalidNode; }
		}

		#endregion Instance Properties (Public)

		#region Shared Members (Internal)

		internal static bool IsValidCommentChar(string commentChars, char commentChar)
		{
			foreach (char definedCommentChar in commentChars.ToCharArray())
			{
				if (definedCommentChar == commentChar) return true;
			}

			return false;
		}

		#endregion Shared Members (Internal)

		#region ChoSyncDisposableObject Overrides

		protected override void Dispose(bool finalize)
		{
			foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
				iniIncludeFileNode.Dispose();

			try
			{
				if (_stream != null)
				{
					_stream.Dispose();
					_stream = null;
				}
			}
			finally
			{
				IsDisposed = true;
			}
		}

		#endregion ChoSyncDisposableObject Overrides

		#region IEnumerable<ChoIniNode> Members

		public IEnumerator<ChoIniNode> GetEnumerator()
		{
			return _rootSection.GetEnumerator();
		}

		#endregion

        #region Sections Members

        public IEnumerable<ChoIniSectionNode> Sections
        {
            get
            {
                foreach (ChoIniNode iniNode in this)
                {
                    if (iniNode is ChoIniSectionNode)
                        yield return (ChoIniSectionNode)iniNode;
                    else if (iniNode is ChoIniIncludeFileNode)
                    {
                        foreach (ChoIniSectionNode iniSectionNode in ((ChoIniIncludeFileNode)iniNode).Sections)
                            yield return iniSectionNode;
                    }
                }
            }
        }

        #endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _rootSection.GetEnumerator();
		}

		#endregion

		#region Object Overrides

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();

            foreach (ChoIniNode iniNode in _headings)
                msg.AppendFormat("{0}{1}", iniNode.ToString(), Environment.NewLine);

            foreach (ChoIniNode iniNode in _rootSection)
                msg.AppendFormat("{0}", iniNode.ToString());

            return msg.ToString();
        }

		#endregion Object Overrides

		#region IChoIniNode Members
		
		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		public bool Comment()
		{
			return Comment(FirstAvailableCommentChar);
		}

        public bool Comment(char commentChar)
        {
            foreach (ChoIniNode iniNode in _rootSection)
            {
                if (!iniNode.Comment())
                    return false;
            }

            return true;
        }

        public bool Uncomment()
        {
            foreach (ChoIniNode iniNode in _rootSection)
            {
                if (!iniNode.Uncomment())
                    return false;
            }

            return true;
        }

        public void Clear()
        {
            foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
            {
                iniIncludeFileNode.Clear();
            }

            _headings.Clear();
            _rootSection.Clear();
        }

		#endregion

        #region Instance Members (Internal)

        internal void ClearNSave()
        {
            foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
            {
                iniIncludeFileNode.ClearNSave();
            }

            Clear();
            Save();
        }

        internal string GetFullPath(string path)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");

            if (System.IO.Path.IsPathRooted(path))
                return path;

            if (_path.IsNullOrEmpty())
                return ChoPath.GetFullPath(path);

            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_path), path);
        }

        #endregion Instance Members (Internal)

        #region Indexers

        public ChoIniSectionNode this[string sectionName]
        {
            get
            {
                ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
                return GetSection(sectionName);
            }
        }

        #endregion Indexers

        #region Factory Members (Internal)

        internal ChoIniNameValueNode CreateNameValueNode(string name, string value)
        {
            return CreateNameValueNode(name, value, _nameValueSeperator);
        }

        internal ChoIniNameValueNode CreateNameValueNode(string name, string value, ChoIniCommentNode inlineCommentNode)
        {
            ChoIniNameValueNode iniNameValueNode = CreateNameValueNode(name, value, _nameValueSeperator);
            iniNameValueNode.CreateOrReplaceInlineCommentNode(inlineCommentNode);
            return iniNameValueNode;
        }

        internal ChoIniNameValueNode CreateNameValueNode(string name, string value, string inlineComment)
        {
            ChoIniNameValueNode iniNameValueNode = CreateNameValueNode(name, value, _nameValueSeperator);
            iniNameValueNode.CreateOrReplaceInlineCommentNode(inlineComment);
            return iniNameValueNode;
        }

        internal ChoIniNameValueNode CreateNameValueNode(string name, string value, char nameValueSeperator)
        {
            return new ChoIniNameValueNode(this, name, value, nameValueSeperator);
        }

        internal ChoIniCommentNode CreateCommentNode(string comment)
        {
            return CreateComment(comment, FirstAvailableCommentChar);
        }

        internal ChoIniCommentNode CreateComment(string comment, char commentChar)
        {
            return new ChoIniCommentNode(this, comment, commentChar);
        }

        internal ChoIniNewLineNode CreateNewLine()
        {
            return new ChoIniNewLineNode(this);
        }

        #endregion Factory Members (Internal)

        public bool Remove(IChoIniNode node)
        {
            throw new NotImplementedException();
        }
    }
}
