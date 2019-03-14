namespace Cinchoo.Core.Ini
{
	#region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Security.AccessControl;
    using System.Text;
    using System.Text.RegularExpressions;
    using Cinchoo.Core;

    #endregion NameSpaces

    // ignore leading and trailing whitespace around the parameter value; others consider all characters following the equals sign (including whitespace) to be part of the value.
	[DebuggerDisplay("Path = {_path}")]
	public sealed class ChoIniDocument : ChoSyncDisposableObject, IEnumerable<ChoIniNode>, IChoIniDocument, IChoIniNode
	{
		#region Constants

        private const string DEFAULT_COMMENT_CHARS = ";#";
        private const char DEFAULT_NAME_VALUE_DELIMITER = '=';
		private const string ROOT_SECTION_NAME = "${{ROOT}}";
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

		private readonly string _path;
		private readonly char _nameValueDelimiter = DEFAULT_NAME_VALUE_DELIMITER;
		private readonly string _commentChars = DEFAULT_COMMENT_CHARS;
		private readonly object _syncRoot;
		private readonly bool _ignoreInvalidNode = true;
		private readonly ChoIniSectionNode _rootSection;
		private readonly List<ChoIniNode> _headings = new List<ChoIniNode>();
		private readonly bool _ignoreValueWhiteSpaces = false;
		private readonly FileMode _fileMode = FileMode.OpenOrCreate;
		private readonly Regex _includeIniFileRegEx;
		private readonly ChoIniDocument _parentIniDocument;
		private readonly ChoIniDocument _ultimateParentIniDocument;
		private readonly object _dirtyLockObject;

        private Stream _stream;
		private bool _isInitialized = false;
		private bool _dirty = false;
		private bool _documentLoaded = false;
		private List<ChoIniIncludeFileNode> _allIniIncludeFileNodes;
		private List<string> _allIniFilePaths;
		private int _lineNo;

        //private FileSecurity _fileSecurity;
        //private FileOptions _fileOptions = FileOptions.None;
        //private int _bufferSize = 0x1000;

		#endregion Instance Data Members (Private)

		#region Constructors

		internal ChoIniDocument(string path, ChoIniDocument parentIniDocument)
			: this(path, parentIniDocument.NameValueDelimiter, parentIniDocument.CommentChars, parentIniDocument.IgnoreValueWhiteSpaces, parentIniDocument.FileMode)
		{
			_syncRoot = parentIniDocument.SyncRoot;
			_ultimateParentIniDocument = _parentIniDocument = parentIniDocument;

			while (_ultimateParentIniDocument.ParentIniDocument != _ultimateParentIniDocument)
				_ultimateParentIniDocument = _ultimateParentIniDocument.ParentIniDocument;
		}

        internal ChoIniDocument(string path)
            : this(path, new ChoIniSettings().NameValueDelimiter, new ChoIniSettings().CommantChars, false, FileMode.OpenOrCreate)
        {
        }

        internal ChoIniDocument(string path, FileMode fileMode)
            : this(path, new ChoIniSettings().NameValueDelimiter, new ChoIniSettings().CommantChars, false, fileMode)
        {
        }

        internal ChoIniDocument(string path, char nameValueDelimiter, string commentChars, bool ignoreValueWhiteSpaces, FileMode fileMode)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "FilePath");
            ChoGuard.ArgumentNotNullOrEmpty(nameValueDelimiter, "NameValueDelimiter");
            ChoGuard.ArgumentNotNullOrEmpty(commentChars, "CommentChars");

            _ultimateParentIniDocument = _parentIniDocument = this;
            _dirtyLockObject = new object();
            _syncRoot = new object();
            _nameValueDelimiter = nameValueDelimiter;
            _commentChars = commentChars;
            _ignoreValueWhiteSpaces = ignoreValueWhiteSpaces;
            _includeIniFileRegEx = new Regex(String.Format(@"^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*[{1}](?<{2}>.*)$|^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*$", INI_INCLUDE_FILE_FILEPATH_TOKEN, _commentChars, INI_INCLUDE_FILE_COMMENTS_TOKEN),
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _rootSection = new ChoIniSectionNode(this, ROOT_SECTION_NAME);

            _path = path;
            _fileMode = fileMode;
        }

        private ChoIniDocument(char nameValueDelimiter = '\0', string commentChars = null, bool ignoreValueWhiteSpaces = false)
        {
            if (nameValueDelimiter == '\0')
                nameValueDelimiter = new ChoIniSettings().NameValueDelimiter;

            if (commentChars == null)
                commentChars = new ChoIniSettings().CommantChars;

            _ultimateParentIniDocument = _parentIniDocument = this;
            _dirtyLockObject = new object();
            _syncRoot = new object();
            _nameValueDelimiter = nameValueDelimiter;
            _commentChars = commentChars;
            _ignoreValueWhiteSpaces = ignoreValueWhiteSpaces;
            _includeIniFileRegEx = new Regex(String.Format(@"^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*[{1}](?<{2}>.*)$|^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*$", INI_INCLUDE_FILE_FILEPATH_TOKEN, _commentChars, INI_INCLUDE_FILE_COMMENTS_TOKEN),
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _rootSection = new ChoIniSectionNode(this, ROOT_SECTION_NAME);
        }

		#endregion Constructors

        #region Indexers

        public string this[string key]
        {
            get { return null; }
            set { }
        }

        #endregion Indexers

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

		private void Initialize()
		{
			Initialize(0x1000, FileOptions.None, null);
		}

		private void Initialize(int bufferSize, FileOptions fileOptions, FileSecurity fileSecurity)
		{
			ChoGuard.NotDisposed(this);

			if (_isInitialized || _stream != null) return;

			lock (_syncRoot)
			{
				try
				{
					_allIniIncludeFileNodes = null;
					_allIniFilePaths = null;

					OpenOrCreateIniFile(_fileMode, bufferSize, fileOptions, fileSecurity);
					Load();
				}
				finally
				{
					_isInitialized = true;
					_stream = null;
				}
			}
		}

		private void OpenOrCreateIniFile(FileMode fileMode)
		{
			OpenOrCreateIniFile(fileMode, 0x1000, FileOptions.None, null);
		}

		private void OpenOrCreateIniFile(FileMode fileMode, int bufferSize, FileOptions fileOptions, FileSecurity fileSecurity)
		{
			if (_stream != null)
				_stream.Close();

            //_fileOptions = fileOptions;
            //_fileSecurity = fileSecurity;
            //_bufferSize = bufferSize;

			_stream = OpenOrCreateIniFile(_path, fileMode, bufferSize, fileOptions, fileSecurity);
		}

		private FileStream OpenOrCreateIniFile(string filePath, FileMode fileMode, int bufferSize, FileOptions fileOptions, FileSecurity fileSecurity)
		{
			if (fileSecurity == null)
				return new FileStream(filePath, fileMode, FileAccess.ReadWrite, FileShare.None, bufferSize, fileOptions);
			else
				return new FileStream(filePath, fileMode, FileSystemRights.Read | FileSystemRights.Write, FileShare.None, bufferSize, fileOptions, fileSecurity);
		}

		private string ReadSections(string line, StreamReader sr)
		{
			if (line == null) return null;

			//Check if this is include file section
			Match match = _includeIniFileRegEx.Match(line);
			if (match.Success)
			{
				ChoIniIncludeFileNode iniIncludeFileNode = null;
				try
				{
					iniIncludeFileNode = CreateIniIncludeFileNode(match.Groups[INI_INCLUDE_FILE_FILEPATH_TOKEN].ToString(),
						match.Groups[INI_INCLUDE_FILE_COMMENTS_TOKEN].ToString());
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
				return line;
			}
			else
			{
				ChoIniSectionNode currentSection = null;
				try
				{
					currentSection = ExtractAndCreateIniSection(line);
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
					if (currentSection != null)
						_rootSection.AddIniNode(currentSection);

					//Add any new lines to this node
					while ((line = sr.ReadLine()) != null)
					{
						_lineNo++;

						line = line.Trim();

						if (line.Length == 0) //WhiteSpace
						{
							if (currentSection != null)
								currentSection.AddNewLineNode();
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
								if (equalPos < 0) // Name only
									currentSection.AddIniNode(CreateNameValueNode(line, null));
								else
									currentSection.AddIniNode(CreateNameValueNode(line.Substring(0, equalPos), line.Substring(equalPos + 1)));
							}
							else
								OnTextIgnored(line);
						}
					}
				}
				return line;
			}
		}

		private string ReadHeadings(StreamReader sr)
		{
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				_lineNo++;

				line = line.Trim();

				if (line.Length == 0) //WhiteSpace
					AppendHeadingNewLine();
                else if (IsValidCommentChar(_commentChars, line[0])) //Comment
					AppendHeadingComment(new ChoIniCommentNode(this, line.Substring(1), line[0]));
				else if (line[0] == '[') //Section
					return line;
				else
					OnTextIgnored(line);
			}

			return null;
		}

		private void Load()
		{
			_rootSection.ClearNodes();

			ChoIniSectionNode currentSection = _rootSection;

			_lineNo = 0;
			//Load the sections into memory
			if (_stream != null)
			{
				using (StreamReader sr = new StreamReader(_stream))
				{
					string line;
					line = ReadHeadings(sr);
					while ((line = ReadSections(line, sr)) != null) ;
				}
			}

			DocumentLoaded = true;
		}

		private ChoIniSectionNode ExtractAndCreateIniSection(string line)
		{
			int commentPos = line.IndexOfAny(_commentChars.ToCharArray());

			string commentLine = commentPos >= 0 ? line.Substring(commentPos).Trim() : null;
			string sectionName = commentPos >= 0 ? line.Substring(0, commentPos).Trim() : line;

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
				ChoIniCommentNode commentNode = null;

				if (commentLine != null && commentLine.Length > 0)
				{
					if (commentLine.Length == 1)
						commentNode = new ChoIniCommentNode(this, String.Empty, commentLine[0]);
					else
						commentNode = new ChoIniCommentNode(this, commentLine.Substring(1), commentLine[0]);
				}

				ChoIniSectionNode iniSectionNode = CreateSection(sectionName.Substring(1, sectionName.Length - 2));
				iniSectionNode.AddInlineCommentNode(commentNode);

				return iniSectionNode;
			}

			return null;
		}

		private IEnumerable<ChoIniIncludeFileNode> GetAllIniIncludeFileNodes()
		{
			lock (_syncRoot)
			{
				if (_allIniIncludeFileNodes == null)
				{
					_allIniIncludeFileNodes = new List<ChoIniIncludeFileNode>();
					foreach (ChoIniNode iniNode in _rootSection)
					{
						if (iniNode is ChoIniIncludeFileNode)
							_allIniIncludeFileNodes.Add(iniNode as ChoIniIncludeFileNode);
					}
				}
				return _allIniIncludeFileNodes;
			}
		}

		#endregion Instance Members (Private)

		#region Instance Properties (Internal)

        //internal FileSecurity FileSecurity
        //{
        //    get { return _fileSecurity; }
        //}

        //internal FileOptions FileOptions
        //{
        //    get { return _fileOptions; }
        //}

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

		#region Create Overloads Shared Members (Public)

        public static ChoIniDocument Parse(string text)
        {
            ChoIniDocument iniDoc = new ChoIniDocument();

            return iniDoc;
        }

        #region Load Overloads

        public static ChoIniDocument Load(Stream stream)
        {
            return null;
        }

        public static ChoIniDocument Load(TextReader reader)
        {
            return null;
        }

        public static ChoIniDocument Load(string path)
		{
            return Load(path, 0x1000, FileOptions.None);
		}

        #endregion Load Overloads

        #endregion Create Overloads Shared Members (Public)

        #region Instance Members (Public)

        #region Other Members (Public)

        #region Save Overloads

        public void Save(Stream stream)
        {
            lock (_syncRoot)
            {
                foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
                    iniIncludeFileNode.Save();

                File.WriteAllText(Path, ToString());

                Dirty = false;
            }
        }

        public void Save(string fileName)
        {
            lock (_syncRoot)
            {
                foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
                    iniIncludeFileNode.Save();

                File.WriteAllText(Path, ToString());

                Dirty = false;
            }
        }

        public void Save(TextWriter textWriter)
        {
            lock (_syncRoot)
            {
                foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
                    iniIncludeFileNode.Save();

                File.WriteAllText(Path, ToString());

                Dirty = false;
            }
        }

        public void Save()
		{
			lock (_syncRoot)
			{
				foreach (ChoIniIncludeFileNode iniIncludeFileNode in GetAllIniIncludeFileNodes())
					iniIncludeFileNode.Save();
				
				File.WriteAllText(Path, ToString());

				Dirty = false;
			}
		}

        #endregion Save Overloads

        public void Refresh()
		{
			ChoGuard.NotDisposed(this);

			_isInitialized = false;
			Initialize();
		}

		public void AppendNewLine()
		{
			Initialize();

			lock (_syncRoot)
			{
				//Add NewLine to the last section, if it dont have one
				if (_rootSection.Count > 0)
				{
					ChoIniSectionNode lastSection = _rootSection[_rootSection.Count - 1] as ChoIniSectionNode;
					lastSection.AddIniNode(CreateNewLine());
				}
				else
					AppendHeadingNewLine();
			}
		}

		#endregion Other Members (Public)

		#region Section Manipulation Methods

		public bool Contains(string sectionName)
		{
			return GetSection(sectionName) != null;
		}

		public ChoIniSectionNode GetSection(string sectionName)
		{
			Initialize();

			lock (_syncRoot)
			{
				foreach (ChoIniNode iniNode in _rootSection)
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
		}

		public bool TryGetSection(string sectionName, out ChoIniSectionNode section)
		{
			section = GetSection(sectionName);

			return section != null;
		}

		public bool AddSection(string sectionName)
		{
			lock (_syncRoot)
			{
				if (Contains(sectionName))
					throw new ChoIniDocumentException(String.Format("Duplicate `{0}` section found. Please check in the INI document and its sub-documents.", sectionName));

				ChoIniSectionNode iniSection = new ChoIniSectionNode(this, sectionName);
				return _rootSection.AddIniNode(iniSection) != null;
			}
		}

		public bool AddSection(ChoIniSectionNode iniSectionNode)
		{
			ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

			lock (_syncRoot)
			{
				if (Contains(iniSectionNode.Name))
					throw new ChoIniDocumentException(String.Format("Duplicate `{0}` section found. Please check in the INI document and its sub-documents.", iniSectionNode.Name));

				return _rootSection.AddIniNode(iniSectionNode) != null;
			}
		}

		public bool InsertBeforeSection(string sectionName, ChoIniSectionNode iniSectionNode)
		{
			ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
			ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

			lock (_syncRoot)
			{
				ChoIniSectionNode insertBeforeSection = null;

				if (!TryGetSection(sectionName, out insertBeforeSection))
					throw new ChoIniDocumentException(String.Format("Can't find {0} section.", sectionName));
				else
					return _rootSection.InsertBeforeIniNode(_rootSection.GetIndex(insertBeforeSection), iniSectionNode);
			}
		}

		public bool InsertAfterSection(string sectionName, ChoIniSectionNode iniSectionNode)
		{
			ChoGuard.ArgumentNotNullOrEmpty(sectionName, "SectionName");
			ChoGuard.ArgumentNotNullOrEmpty(iniSectionNode, "IniSectionNode");

			lock (_syncRoot)
			{
				ChoIniSectionNode insertBeforeSection = null;

				if (!TryGetSection(sectionName, out insertBeforeSection))
					throw new ChoIniDocumentException(String.Format("Can't find {0} section.", sectionName));
				else
					return _rootSection.InsertBeforeIniNode(_rootSection.GetIndex(insertBeforeSection), iniSectionNode);
			}
		}

		public ChoIniSectionNode CreateSection(string sectionName)
		{
			if (Contains(sectionName))
				throw new ChoIniDocumentException(String.Format("Duplicate `{0}` section found. Please check in the INI document and its sub-documents.", sectionName));

			return new ChoIniSectionNode(this, sectionName);
		}

		public bool DeleteSection(string sectionName)
		{
			lock (_syncRoot)
			{
				ChoIniSectionNode foundSection = GetSection(sectionName);

				if (foundSection == null)
					throw new ChoIniDocumentException(String.Format("Can't find `{0}` section.", sectionName));
				else
					return DeleteSection(foundSection);
			}
		}

		public bool DeleteSection(ChoIniSectionNode section)
		{
			Initialize();

			return _rootSection.RemoveNode(section);
		}

		public bool CommentSection(string sectionName)
		{
			ChoIniSectionNode foundSection = GetSection(sectionName);

			if (foundSection == null)
				throw new ChoIniDocumentException(String.Format("Can't find `{0}` section.", sectionName));
			else
				return CommentSection(foundSection);
		}

		public bool CommentSection(ChoIniSectionNode section)
		{
			Initialize();
			return section.Comment();
		}

		public void UncommentSection(ChoIniSectionNode section)
		{
			Initialize();
			section.Uncomment();
		}
		
		#endregion Section Manipulation Methods

		#region Comments Manipulation Methods

		public ChoIniNode[] GetHeadingComments()
		{
			Initialize();

			lock (_syncRoot)
			{
				return _headings.ToArray();
			}
		}

		public bool CreateHeadingComment(string commentLine)
		{
			return CreateHeadingComments(new string[] { commentLine });
		}

		public bool CreateHeadingComments(string[] commentLines)
		{
			if (!ChoGuard.IsArgumentNotNullOrEmpty(commentLines))
				return true;

			Initialize();

			lock (_syncRoot)
			{
				if (!OnNodeInserting(this, commentLines))
				{
					RemoveHeadingComments();
					foreach (string commentLine in commentLines)
					{
						ChoIniCommentNode iniCommentNode = new ChoIniCommentNode(this, commentLine, FirstAvailableCommentChar);
						Dirty = true;
						_headings.Add(iniCommentNode);
					}
					OnNodeInserted(this, commentLines);
				}
				else
					return false;
			}

			return true;
		}

		public bool AppendHeadingComment(string commentLine)
		{
			return AppendHeadingComments(new string[] { commentLine });
		}

		public bool AppendHeadingComments(string[] commentLines)
		{
			if (!ChoGuard.IsArgumentNotNullOrEmpty(commentLines)) return true;

			Initialize();

			lock (_syncRoot)
			{
				if (!OnNodeInserting(this, commentLines))
				{
					foreach (string commentLine in commentLines)
					{
						ChoIniCommentNode iniCommentNode = new ChoIniCommentNode(this, commentLine, FirstAvailableCommentChar);
						Dirty = true;
						_headings.Add(iniCommentNode);
					}
					OnNodeInserted(this, commentLines);
				}
				else
					return false;
			}

			return true;
		}

		public bool AppendHeadingComment(ChoIniCommentNode comment)
		{
			ChoGuard.ArgumentNotNull(comment, "Comment");

			return AppendHeadingComments(new ChoIniCommentNode[] { comment });
		}

		public bool AppendHeadingComments(ChoIniCommentNode[] comments)
		{
			if (!ChoGuard.IsArgumentNotNullOrEmpty(comments))
				return true;

			Initialize();

			lock (_syncRoot)
			{
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
			}

			return true;
		}

		public bool AppendHeadingNewLine()
		{
			Initialize();

			lock (_syncRoot)
			{
				ChoIniNewLineNode iniNewLineNode = new ChoIniNewLineNode(this);
				if (!OnNodeInserting(this, iniNewLineNode))
				{
					Dirty = true;
					_headings.Add(iniNewLineNode);
					OnNodeInserted(this, iniNewLineNode);
				}
				else
					return false;
			}

			return true;
		}

		public bool RemoveHeadingComments()
		{
			Initialize();

			lock (_syncRoot)
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
			}

			return true;
		}

		#endregion Comments Manipulation Methods

		#region IncludeFile Manipulation Methods

		public ChoIniIncludeFileNode CreateIniIncludeFileNode(string filePath)
		{
			return CreateIniIncludeFileNode(filePath, null);
		}

		public ChoIniIncludeFileNode CreateIniIncludeFileNode(string filePath, string comments)
		{
			Initialize();

			using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Create)) //, this._bufferSize, this._fileOptions, this._fileSecurity))
			{
			}

			ChoIniIncludeFileNode iniIncludeFileNode = new ChoIniIncludeFileNode(this, filePath, comments);
			//iniIncludeFileNode.Refresh();

			return iniIncludeFileNode;
		}

		public bool ContainsIniIncludeFileNode(string filePath)
		{
			ChoGuard.ArgumentNotNull(filePath, "FilePath");
			Initialize();

			lock (_syncRoot)
			{
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
			}

			return false;
		}

		public ChoIniIncludeFileNode GetIniIncludeFileNode(string filePath)
		{
			ChoGuard.ArgumentNotNull(filePath, "FilePath");
			Initialize();

			lock (_syncRoot)
			{
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
			}

			return null;
		}

		public bool TryGetIniIncludeFileNode(string filePath, out ChoIniIncludeFileNode iniIncludeFileNode)
		{
			iniIncludeFileNode = GetIniIncludeFileNode(filePath);

			return iniIncludeFileNode != null;
		}

		public bool AddIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
		{
			ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");
			Initialize();

			return _rootSection.AddIniNode(iniIncludeFileNode) != null;
		}

		public bool InsertIniIncludeFileNodeBefore(int index, ChoIniIncludeFileNode iniIncludeFileNode)
		{
			if (index < 0)
				throw new IndexOutOfRangeException("Index should be positive.");

			ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");
			Initialize();

			return _rootSection.InsertBeforeIniNode(index, iniIncludeFileNode);
		}

		public bool InsertIniIncludeFileNodeAfter(int index, ChoIniIncludeFileNode iniIncludeFileNode)
		{
			if (index < 0)
				throw new IndexOutOfRangeException("Index should be positive.");

			ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");
			Initialize();

			return _rootSection.InsertAfterIniNode(index, iniIncludeFileNode);
		}
		
		public bool DeleteIniIncludeFileNode(string filePath)
		{
			ChoIniIncludeFileNode iniIncludeFileNode = null;

			lock (_syncRoot)
			{
				if (!TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode))
					throw new ChoIniDocumentException("Can't find '{0}' include ini node.".FormatString(filePath));

				return DeleteIniIncludeFileNode(iniIncludeFileNode);
			}
		}

		public bool DeleteIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
		{
			ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");

			lock (_syncRoot)
			{
				if (ContainsIniIncludeFileNode(iniIncludeFileNode.Path))
					throw new ChoIniDocumentException("Can't find '{0}' include node.".FormatString(iniIncludeFileNode.Path));

				foreach (ChoIniNode iniNode in _rootSection)
				{
					if (iniNode is ChoIniIncludeFileNode)
					{
						ChoIniIncludeFileNode iniChildIncludeFileNode = iniNode as ChoIniIncludeFileNode;
						if (iniChildIncludeFileNode.Path == iniIncludeFileNode.Path)
							return _rootSection.RemoveNode(iniIncludeFileNode);
						else
							return iniIncludeFileNode.DeleteIniIncludeFileNode(iniChildIncludeFileNode.Path);
					}
				}
			}

			return false;
		}

		public bool CommentIniIncludeFileNode(string filePath)
		{
			ChoIniIncludeFileNode iniIncludeFileNode = null;

			lock (_syncRoot)
			{
				if (!TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode))
					throw new ChoIniDocumentException("Can't find '{0}' include ini node.".FormatString(filePath));

				return CommentIniIncludeFileNode(iniIncludeFileNode);
			}
		}

		public bool CommentIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
		{
			ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");
			Initialize();

			return iniIncludeFileNode.Comment();
		}

		public bool UncommentIniIncludeFileNode(string filePath)
		{
			ChoIniIncludeFileNode iniIncludeFileNode = null;

			lock (_syncRoot)
			{
				if (!TryGetIniIncludeFileNode(filePath, out iniIncludeFileNode))
					throw new ChoIniDocumentException("Can't find '{0}' include ini node.".FormatString(filePath));

				return UncommentIniIncludeFileNode(iniIncludeFileNode);
			}
		}

		public bool UncommentIniIncludeFileNode(ChoIniIncludeFileNode iniIncludeFileNode)
		{
			ChoGuard.ArgumentNotNull(iniIncludeFileNode, "IniIncludeFileNode");
			Initialize();

			return iniIncludeFileNode.Uncomment();
		}

		#endregion IncludeFile Manipulation Methods

		#region Factory Members (Public)

		public ChoIniNameValueNode CreateNameValueNode(string name, string value)
		{
            return CreateNameValueNode(name, value, _nameValueDelimiter);
		}

		public ChoIniNameValueNode CreateNameValueNode(string name, string value, char nameValueDelimiter)
		{
			Initialize();
			return new ChoIniNameValueNode(this, name, value, nameValueDelimiter);
		}

		public ChoIniCommentNode CreateCommentNode(string comment)
		{
			return CreateComment(comment, FirstAvailableCommentChar);
		}

		public ChoIniCommentNode CreateComment(string comment, char commentChar)
		{
			Initialize();
			return new ChoIniCommentNode(this, comment, commentChar);
		}

		public ChoIniNewLineNode CreateNewLine()
		{
			Initialize();
			return new ChoIniNewLineNode(this);
		}

		#endregion Factory Members (Internal)

		#endregion Instance Members (Public)

		#region Instance Properties (Public)

		public string[] AllIniFilePaths
		{
			get 
			{
				lock (_syncRoot)
				{
					Initialize();

					if (_allIniFilePaths == null)
					{
						_allIniFilePaths = new List<string>();
						_allIniFilePaths.AddRange(GetAllIncludeFilePaths(this));
					}
					return _allIniFilePaths.ToArray();
				}
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

		public FileMode FileMode
		{
			get { return _fileMode; }
		}

		public char NameValueDelimiter
		{
			get { return _nameValueDelimiter; }
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

        private static ChoIniDocument Load(string path, int bufferSize, FileOptions options)
        {
            return Load(path, 0x1000, FileOptions.None, null);
        }

        private static ChoIniDocument Load(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            ChoIniDocument iniFile = new ChoIniDocument(path);
            try
            {
                iniFile.OpenOrCreateIniFile(FileMode.Open, bufferSize, options, fileSecurity);
                return iniFile;
            }
            finally
            {
                if (iniFile._stream != null)
                    iniFile._stream.Close();
                iniFile._stream = null;
            }
        }

		internal static string[] GetAllIncludeFilePaths(IChoIniDocument iniDocument)
		{
			List<string> iniFilePaths = new List<string>();
			iniFilePaths.Add(iniDocument.Path);

			foreach (ChoIniNode iniNode in iniDocument)
			{
				if (iniNode is ChoIniIncludeFileNode)
					iniFilePaths.AddRange(((IChoIniDocument)iniNode).AllIniFilePaths);
			}
			return iniFilePaths.ToArray();
		}

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
			Initialize();
			return _rootSection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			Initialize();
			return _rootSection.GetEnumerator();
		}

		#endregion

		#region Object Overrides

		public override string ToString()
		{
			lock (_syncRoot)
			{
				Initialize();

				StringBuilder msg = new StringBuilder();

				foreach (ChoIniNode iniNode in _headings)
					msg.AppendFormat("{0}{1}", iniNode.ToString(), Environment.NewLine);

				foreach (ChoIniNode iniNode in _rootSection)
					msg.AppendFormat("{0}", iniNode.ToString());

				return msg.ToString();
			}
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
			lock (_syncRoot)
			{
				Initialize();

				foreach (ChoIniNode iniNode in _rootSection)
				{
					if (!iniNode.Comment(commentChar))
						return false;
				}
			}

			return true;
		}

		public bool Uncomment()
		{
			lock (_syncRoot)
			{
				Initialize();

				foreach (ChoIniNode iniNode in _rootSection)
				{
					if (!iniNode.Uncomment())
						return false;
				}
			}

			return true;
		}

		#endregion
	}
}
