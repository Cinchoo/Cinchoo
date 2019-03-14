namespace Cinchoo.Core.Xml
{
	#region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Text;
    using System.Xml;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Collections.Generic;

	#endregion

	[DebuggerDisplay("Path = {_path}")]
	public class ChoXmlDocument : IDisposable
	{
		#region Constants

        internal const string PathToken = "path";
        internal const string IncludeToken = "cinchoo:include";
        internal const string IncludePrefixToken = "cinchoo";
        public const string CinchoNSToken = "xmlns:cinchoo";
        public const string CinchooNSURI = "http://schemas.cinchoo.com/cinchoo/01/framework";
		internal const string BeginIncludeToken = "DO NOT REMOVE - BEGIN INCLUDE";
		internal const string EndIncludeToken = "DO NOT REMOVE - END INCLUDE";
		internal const string MetaDataToken = "metaData";

		#endregion Constants

		#region Events

		public event EventHandler<ChoXmlDocumentChangingEventArgs> DocumentChanging;
		public event EventHandler<ChoXmlDocumentChangedEventArgs> DocumentChanged;

		#endregion Events

		#region Instance Data Members (Private)

		private readonly string _filePath;
		private readonly string _xml;
		private readonly object _syncRoot = new object();
		private readonly ChoQueuedMsgService<object> _queuedMsgService;
		private readonly ChoTimerService<string> _timerService;
		private readonly object _dirtyLockObject = new object();
		private readonly bool _watchChange = false;
		private readonly bool _readOnly = true;
		private IChoConfigurationChangeWatcher _configurationChangeWatcher;

		private bool _dirty = false;
		private bool _documentLoaded = false;
		private XmlDocument _xmlDocument;
		private ChoNestedDictionary<string, XmlNode> _includeXmlFilePathNodesMap;
		private readonly ChoDictionaryService<string, DateTime> _lastWriteTimeCache = new ChoDictionaryService<string, DateTime>("XmlDocument");

		#endregion Instance Data Members (Private)

		#region Shared Members (Private)
		
		private static readonly NameTable _nameTable = new NameTable();
		private static readonly XmlNamespaceManager _xmlnsManager;

		#endregion Shared Members (Private)

		#region Constructors

		static ChoXmlDocument()
		{
			//Instantiate an XmlNamespaceManager object. 
			_xmlnsManager = new System.Xml.XmlNamespaceManager(_nameTable);

			//Add the namespaces used in books.xml to the XmlNamespaceManager.
			_xmlnsManager.AddNamespace(IncludePrefixToken, CinchooNSURI);
		}

		internal ChoXmlDocument(string filePathOrXml, ChoXmlDocument parentXmlDocument)
			: this(filePathOrXml)
		{
		}

		public ChoXmlDocument(string filePathOrXml) : this(filePathOrXml, false, true)
		{
		}

		public ChoXmlDocument(string filePathOrXml, bool watchChange, bool readOnly)
		{
			ChoGuard.ArgumentNotNullOrEmpty(filePathOrXml, "FilePath or Xml");
			filePathOrXml = filePathOrXml.Trim();
			_watchChange = watchChange;
			_readOnly = readOnly;

			if (filePathOrXml.StartsWith("<"))
				_xml = filePathOrXml;
			else
				_filePath = filePathOrXml;

			if (IsXmlFile)
			{
                if (watchChange || !readOnly)
                {
                    _queuedMsgService = new ChoQueuedMsgService<object>("{0}_{1}".FormatString(_filePath, typeof(ChoXmlDocument).Name),
                        ChoStandardQueuedMsgObject<object>.QuitMsg, false, false,
                        QueueMessageHandler);
                    _timerService = new ChoTimerService<string>(String.Format("{0}_Timer", System.IO.Path.GetFileNameWithoutExtension(_filePath)),
                        OnTimerServiceCallback, null, 5000, false);
                }
			}

			Load();
		}

		public ChoXmlDocument(XmlDocument xmlDocument)
		{
			ChoGuard.ArgumentNotNull(xmlDocument, "XmlDocument");
			xmlDocument = _xmlDocument;

			Load();
		}

		#endregion Constructors

		#region ChoSyncMsgQProcessor Overrides

		private void QueueMessageHandler(IChoQueuedMsgServiceObject<object> msgObject)
		{
			if (msgObject == null) return;

			lock (_syncRoot)
			{
				if (msgObject.State == null)
				{
					if (!_readOnly)
                        SaveInternal();
				}
				else if (msgObject.State is ChoConfigurationCompositeFileChangedEventArgs)
				{
					ChoConfigurationCompositeFileChangedEventArgs e = msgObject.State as ChoConfigurationCompositeFileChangedEventArgs;
					if (e != null)
					{
						try
						{
							ChoXmlDocumentChangingEventArgs xmlDocumentChangingEventArgs = new ChoXmlDocumentChangingEventArgs(_filePath, e.ModifiedIncludeFiles);
							DocumentChanging.Raise(this, xmlDocumentChangingEventArgs);

							if (xmlDocumentChangingEventArgs.IgnoreLoadDocument)
								return;

							_xmlDocument = null;
							Load();

							DocumentChanged.Raise(this, new ChoXmlDocumentChangedEventArgs(_filePath, e.ModifiedIncludeFiles));
						}
						catch
						{
							DocumentLoaded = false;
							throw;
						}
					}
				}

			}
		}

		#endregion ChoSyncMsgQProcessor Overrides

		#region ChoQueuedMsgService Overrides

		internal void Start()
		{
			if (!IsXmlFile) return;

			if (_timerService != null) _timerService.Start();
			if (_queuedMsgService != null) _queuedMsgService.Start();
			if (_configurationChangeWatcher != null) _configurationChangeWatcher.StartWatching();

		}

		bool isStopRequested = false;
		internal void Stop()
		{
			if (!IsXmlFile)
				return;

			lock (_syncRoot)
			{
				if (isStopRequested)
					return;
				else
					isStopRequested = true;
			}
			OnTimerServiceCallback(null);
			if (_timerService != null)
				_timerService.Stop();
			if (_queuedMsgService != null)
				_queuedMsgService.Stop();
			if (_configurationChangeWatcher != null)
				_configurationChangeWatcher.StopWatching();
		}

		#endregion ChoQueuedMsgService Overrides

		#region Instance Members (Private)

		private void OnFileChanged(object sender, ChoConfigurationChangedEventArgs e)
		{
			if (_queuedMsgService != null) _queuedMsgService.Enqueue(ChoStandardQueuedMsgObject<object>.New(e));
		}

		private void Load()
		{
			if (_xmlDocument == null)
			{
				_xmlDocument = new XmlDocument(_nameTable);
                if (IsXmlFile)
                {
                    using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        _xmlDocument.Load(fs);
                }
                else
                    _xmlDocument.LoadXml(_xml);
			}

			LoadDocument();
		}

		private void LoadDocument()
		{
			RefreshDocument();

			Start();

			if (_xmlDocument != null)
			{
				_xmlDocument.NodeChanged += new XmlNodeChangedEventHandler(XmlDocument_NodeChanged);
				_xmlDocument.NodeChanging += new XmlNodeChangedEventHandler(XmlDocument_NodeChanging);
				_xmlDocument.NodeInserted += new XmlNodeChangedEventHandler(XmlDocument_NodeInserted);
				_xmlDocument.NodeInserting += new XmlNodeChangedEventHandler(XmlDocument_NodeInserting);
				_xmlDocument.NodeRemoved += new XmlNodeChangedEventHandler(XmlDocument_NodeRemoved);
				_xmlDocument.NodeRemoving += new XmlNodeChangedEventHandler(XmlDocument_NodeRemoving);
			}

			DocumentLoaded = true;
		}

		private void RefreshDocument()
		{
			if (IsXmlFile)
				_includeXmlFilePathNodesMap = ExpandIncludes(_xmlDocument, _filePath, _xmlnsManager);
			else
				_includeXmlFilePathNodesMap = ExpandIncludes(_xmlDocument, null, _xmlnsManager);

			foreach (string includeFile in IncludeFiles)
				_lastWriteTimeCache.SetValue(includeFile, File.GetLastWriteTime(includeFile));

			if (_watchChange)
			{
				_configurationChangeWatcher = new ChoConfigurationChangeCompositeFileWatcher("ChoXmlDocument", _filePath, IncludeFiles);
				_configurationChangeWatcher.SetConfigurationChangedEventHandler(this, OnFileChanged);
			}
		}

		#endregion Instnace Members (Private)

		#region ChoTimerService Members (Private)

		private void OnTimerServiceCallback(string state)
		{
			if (!Dirty || !IsXmlFile) return;
			Dirty = false;

			if (_queuedMsgService != null) _queuedMsgService.Enqueue(ChoStandardQueuedMsgObject<object>.New(null));
		}

		#endregion ChoTimerService Members (Private)

		#region Instance Properties (Private)

		internal bool IsXmlFile
		{
			get { return !String.IsNullOrEmpty(_filePath); }
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
				lock (_dirtyLockObject)
				{
					return _dirty;
				}
			}
			set
			{
				lock (_dirtyLockObject)
				{
					if (!_documentLoaded) return;

					_dirty = value;
				}
			}
		}

		#endregion Instance Properties (Private)

		#region Create Overloads Shared Members (Public)

		public static ChoXmlDocument Create(string path)
		{
			return Create(path, FileOptions.None);
		}

		public static ChoXmlDocument Create(string path, FileOptions options)
		{
			return Create(path, FileOptions.None, null);
		}

		public static ChoXmlDocument Create(string path, FileOptions options, FileSecurity fileSecurity)
		{
			using (File.Create(path, 0x100, options, fileSecurity))
			{
			}

			return new ChoXmlDocument(path);
		}

		#endregion Create Overloads Shared Members (Public)

		#region Operators Overloading

		public static implicit operator XmlDocument(ChoXmlDocument xmlDocument)
		{
			return xmlDocument == null ? null : xmlDocument._xmlDocument;
		}

		#endregion Operators Overloading

		#region Instance Members (Public)

		internal bool HasIncludeComments
		{
			get
			{
				foreach (XmlNode xmlNode in _xmlDocument.SelectNodes("*//comment()"))
				{
					if (xmlNode is XmlComment && xmlNode.Value.StartsWith(BeginIncludeToken))
						return true;
				}

				return false;
			}
		}

		public void Save()
		{
			if (!Dirty || !IsXmlFile) return;
            Dirty = false;
            if (_queuedMsgService != null)
                _queuedMsgService.Enqueue(ChoStandardQueuedMsgObject<object>.New(null));
            else
                SaveInternal();
		}

        private void SaveInternal()
        {
            ChoNestedDictionary<string, XmlNode> xmlFileList = ContractIncludes(_xmlDocument, _filePath);

            foreach (string fileName in xmlFileList.Keys)
            {
                if (_lastWriteTimeCache.GetValue(fileName) <= File.GetLastWriteTime(fileName))
                    ChoXmlDocument.Save(xmlFileList[fileName], fileName, true);
            }
        }

		#endregion Instance Members (Public)

		#region Instance Properties (Public)

		public XmlDocument XmlDocument
		{
			get { return _xmlDocument; }
		}

		public string[] IncludeFiles
		{
			get 
			{ 
				if (_includeXmlFilePathNodesMap.Length == 0) return new string[] {};

				List<string> nodesMap = new List<string>(_includeXmlFilePathNodesMap.Keys);
				nodesMap.RemoveAt(0);
				return nodesMap.ToArray();
			}
		}

		public ChoNestedDictionary<string, XmlNode> NodesMap
		{
			get { return _includeXmlFilePathNodesMap; }
		}

		#endregion Instance Properties (Public)

		#region Event Handlers

		private void XmlDocument_NodeRemoving(object sender, XmlNodeChangedEventArgs e)
		{
		}

		private void XmlDocument_NodeRemoved(object sender, XmlNodeChangedEventArgs e)
		{
			_dirty = true;
		}

		private void XmlDocument_NodeInserting(object sender, XmlNodeChangedEventArgs e)
		{
		}

		private void XmlDocument_NodeInserted(object sender, XmlNodeChangedEventArgs e)
		{
			_dirty = true;
		}

		private void XmlDocument_NodeChanging(object sender, XmlNodeChangedEventArgs e)
		{
		}

		private void XmlDocument_NodeChanged(object sender, XmlNodeChangedEventArgs e)
		{
			_dirty = true;
		}

		#endregion Event Handlers

		#region Shared Helper Members (Public)

        public static XmlNode GetXmlNode(string nodeName, IEnumerable<XmlNode> xmlNodes)
        {
            if (nodeName.IsNullOrEmpty())
                return null;

            nodeName = nodeName.Trim();

            //Trace.TraceInformation("SectionName: {0}, nodes: {1}".FormatString(sectionName, xmlNodes == null ? 0 : xmlNodes.Count()));

            int pos = nodeName.IndexOf('/');
            if (pos >= 0)
            {
                string sectionGroupName = nodeName.Substring(0, pos);
                nodeName = nodeName.Substring(pos + 1, nodeName.Length - (pos + 1));

                if (sectionGroupName.IsNullOrEmpty())
                    return GetXmlNode(nodeName, xmlNodes);
                else
                {
                    if (xmlNodes != null)
                    {
                        foreach (XmlNode xmlNode in xmlNodes)
                        {
                            if (xmlNode.Name == sectionGroupName)
                                return GetXmlNode(nodeName, xmlNode.ChildNodes.Cast<XmlNode>());
                        }
                    }
                }
            }
            else
            {
                if (xmlNodes != null)
                {
                    foreach (XmlNode xmlNode in xmlNodes)
                    {
                        if (xmlNode.Name == nodeName)
                            return xmlNode;
                    }
                }
            }

            return null;

        }

		public static bool TryLoad(string fileName, out XmlDocument xmlDocument)
		{
			xmlDocument = null;

			try
			{
				xmlDocument = Load(fileName);
				return true;
			}
			catch { }

			return false;
		}

		/// <summary>
		/// Static method that calls XmlDocument.Load() but arranges to open
		/// the file with with FileShared.ReadWrite to keep from blocking others.
		/// </summary>
		public static XmlDocument Load(string filename)
		{
			ChoNestedDictionary<string, XmlNode> includeFileNodesMap = null;

			return Load(filename, out includeFileNodesMap);
		}

		public static XmlDocument Load(string filename, out ChoNestedDictionary<string, XmlNode> includeFileNodesMap)
		{
			XmlDocument doc = new XmlDocument(_nameTable);
			Load(doc, filename);

			includeFileNodesMap = ExpandIncludes(doc.DocumentElement, filename, _xmlnsManager);

			return doc;
		}

		public static void Load(XmlDocument doc, string filename)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentNullException("fileName");

			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (XmlReader reader = XmlReader.Create(stream))
					doc.Load(reader);
			}
		}

		public static string SetInnerXml(XmlNode xmlNode, string innerXml)
		{
			ChoGuard.ArgumentNotNull(xmlNode, "XmlNode");

			XmlDocument doc = xmlNode.OwnerDocument;
			if (doc == null)
				throw new NullReferenceException("Missing Owner document.");

			xmlNode.InnerXml = innerXml;

            return doc.InnerXml.IndentXml();
		}

        public static string AppendToInnerXml(XmlNode xmlNode, string innerXml)
        {
            ChoGuard.ArgumentNotNull(xmlNode, "XmlNode");

            XmlDocument doc = xmlNode.OwnerDocument;
            if (doc == null)
                throw new NullReferenceException("Missing Owner document.");

            xmlNode.InnerXml = xmlNode.InnerXml + innerXml;

            return doc.InnerXml.IndentXml();
        }

		public static string SetInnerXml(string fileName, string xpath, string innerXml)
		{
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("FileName");
			if (!File.Exists(fileName))
				throw new ArgumentException(String.Format("{0} configuration file not exists.", fileName));
			if (String.IsNullOrEmpty(xpath))
				throw new ArgumentNullException("SectionName");

			XmlDocument doc = Load(fileName);

			//Select the cd node with the matching title
			XmlNode configNode = doc.DocumentElement.SelectSingleNode(xpath);
			if (configNode == null)
				throw new NullReferenceException(String.Format("Can't find {0} xpath in the {1} config file.", xpath, fileName));

			return SetInnerXml(configNode, innerXml);
		}
		
		public static string SetInnerXml(XmlNode xmlNode, string xpath, string innerXml)
		{
			ChoGuard.ArgumentNotNull(xmlNode, "XmlNode");

			//Select the cd node with the matching title
			XmlNode configNode = xmlNode.SelectSingleNode(xpath);
			if (configNode == null)
				throw new NullReferenceException(String.Format("Can't find {0} xpath in the XmlNode.", xpath));

			return SetInnerXml(configNode, innerXml);
		}

		public static string SetOuterXml(XmlNode xmlNode, string outerXml)
		{
			return SetNamespaceAwareOuterXml(xmlNode, outerXml, null);
		}
		
		public static string SetNamespaceAwareOuterXml(XmlNode xmlNode, string outerXml, string namespaceURI)
		{
			ChoGuard.ArgumentNotNull(xmlNode, "XmlNode");

			XmlDocument doc = xmlNode.OwnerDocument;
			if (doc == null)
				throw new NullReferenceException("Missing Owner document.");

			//Remove all attributes and elements
			xmlNode.RemoveAll();

			XmlDocument newDoc = new XmlDocument();
			using (XmlTextReader reader = new XmlTextReader(new StringReader(outerXml)))
				newDoc.Load(reader);

			foreach (XmlAttribute attribute in newDoc.DocumentElement.Attributes)
			{
				if (attribute.Name.StartsWith("xmlns:"))
					continue;

				if (namespaceURI.IsNullOrWhiteSpace())
					xmlNode.Attributes.Append(doc.CreateAttribute(attribute.Name)).Value = attribute.Value;
				else
					xmlNode.Attributes.Append(doc.CreateAttribute(attribute.Name, ChoXmlDocument.CinchooNSURI)).Value = attribute.Value;
			}

			xmlNode.InnerXml = newDoc.DocumentElement.InnerXml;

            return doc.OuterXml.IndentXml();
		}

		public static string SetOuterXml(XmlNode xmlNode, string xpath, string outerXml)
		{
			return SetNamespaceAwareOuterXml(xmlNode, xpath, outerXml, null);
		}

		public static string SetNamespaceAwareOuterXml(XmlNode xmlNode, string xpath, string outerXml, string namespaceURI)
		{
			ChoGuard.ArgumentNotNull(xmlNode, "XmlNode");

			//Select the cd node with the matching title
			XmlNode configNode = xmlNode.SelectSingleNode(xpath);
			if (configNode == null)
				throw new NullReferenceException(String.Format("Can't find {0} xpath in the XmlNode.", xpath));

			return SetNamespaceAwareOuterXml(configNode, outerXml, namespaceURI);
		}

		public static string SetOuterXml(string fileName, string xpath, string outerXml)
		{
			return SetNamespaceAwareOuterXml(fileName, xpath, outerXml, null);
		}

		public static string SetNamespaceAwareOuterXml(string fileName, string xpath, string outerXml, string namespaceURI)
		{
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("FileName");
			if (!File.Exists(fileName))
				throw new ArgumentException(String.Format("{0} configuration file not exists.", fileName));
			if (String.IsNullOrEmpty(xpath))
				throw new ArgumentNullException("SectionName");

			XmlDocument doc = Load(fileName);

			//Select the cd node with the matching title
			XmlNode configNode = doc.DocumentElement.SelectSingleNode(xpath);
			if (configNode == null)
				throw new NullReferenceException(String.Format("Can't find {0} xpath in the {1} config file.", xpath, fileName));

			return SetNamespaceAwareOuterXml(configNode, outerXml, namespaceURI);

			////Remove all attributes and elements
			//configNode.RemoveAll();

			//XmlDocument newDoc = new XmlDocument();
			//using (XmlTextReader reader = new XmlTextReader(new StringReader(outerXml)))
			//    newDoc.Load(reader);

			//foreach (XmlAttribute attribute in newDoc.DocumentElement.Attributes)
			//    configNode.Attributes.Append(doc.CreateAttribute(attribute.Name)).Value = attribute.Value;

			//configNode.InnerXml = newDoc.DocumentElement.InnerXml;

			//return IndentXMLString(doc.OuterXml);
		}

		public static void ContractNSave(XmlNode xmlNode, string xmlFilePath)
		{
			ChoNestedDictionary<string, XmlNode> xmlFileList = ContractIncludes(xmlNode, xmlFilePath);

			foreach (string fileName in xmlFileList.Keys)
				ChoXmlDocument.Save(xmlFileList[fileName], fileName, true);
		}

		public static void Save(XmlNode xmlNode, string xmlFilePath, bool indentOutput)
		{
			XmlWriterSettings xws = new XmlWriterSettings();
			xws.Indent = indentOutput;
			xws.NewLineHandling = NewLineHandling.Replace;
			xws.NewLineChars = Environment.NewLine;

			Save(xmlNode, xmlFilePath, xws);
		}

		public static void Save(XmlNode xmlNode, string xmlFilePath, XmlWriterSettings xws)
		{
			ChoGuard.ArgumentNotNull(xmlNode, "XmlDocument");
			ChoGuard.ArgumentNotNullOrEmpty(xmlFilePath, "XmlFilePath");

			if (xmlNode is XmlDocumentFragment)
			{
				xws.OmitXmlDeclaration = true;
				xws.ConformanceLevel = ConformanceLevel.Auto;
				xws.Indent = false;
			}
			using (FileStream fs = File.Create(xmlFilePath))
			{
				// Create a XMLTextWriter that will send its output to a memory stream (file)
				using (XmlWriter xtw = XmlTextWriter.Create(fs, xws))
				{
					// Set the formatting property of the XML Text Writer to indented
					// the text writer is where the indenting will be performed
					//if (indentOutput)
					//    xtw.Formatting = Formatting.Indented;

					// write dom xml to the xmltextwriter
					xmlNode.WriteContentTo(xtw);
					// Flush the contents of the text writer
					// to the memory stream, which is simply a memory file
					xtw.Flush();
				}
			}
		}

		#region Embedded Include File Loading Members

		#region ContractIncludes Overloads
		
		public ChoNestedDictionary<string, XmlNode> ContractIncludes()
		{
			return ContractIncludes(_xmlDocument, _filePath);
		}

		public static ChoNestedDictionary<string, XmlNode> ContractIncludes(XmlNode section)
		{
			return ContractIncludes(section, null);
		}

		public static ChoNestedDictionary<string, XmlNode> ContractIncludes(XmlNode section, string filePath)
		{
			ChoGuard.ArgumentNotNull(section, "XmlNode");

			if (filePath == null)
				filePath = String.Empty;

			ChoNestedDictionary<string, XmlNode> xmlDocumentList = new ChoNestedDictionary<string, XmlNode>();
			xmlDocumentList.Add(filePath, section is XmlDocument ? section as XmlDocument : section.OwnerDocument);

			ContractIncludes(section, filePath, xmlDocumentList);
			return xmlDocumentList;
		}

		private static void ContractIncludes(XmlNode section, string filePath, ChoNestedDictionary<string, XmlNode> xmlDocumentList)
		{
			if (section == null) return;
			section = section is XmlDocument ? ((XmlDocument)section).DocumentElement : section;

			bool beginIncludeTokenFound = false;
			string includeFilePath = String.Empty;
			List<XmlNode> childNodes = new List<XmlNode>();
			XmlNode startCommentNode = null;
			foreach (XmlNode xmlNode in section.SelectNodes("child::node()"))
			{
				if (!beginIncludeTokenFound)
				{
					if (xmlNode is XmlComment && xmlNode.Value.StartsWith(BeginIncludeToken))
					{
						includeFilePath = xmlNode.Value.Replace(BeginIncludeToken, String.Empty).Trim();
						beginIncludeTokenFound = true;
						startCommentNode = xmlNode;
						continue;
					}
					else if (xmlNode is XmlElement)
					{
						ContractIncludes(xmlNode, filePath, xmlDocumentList);
					}
				}
				else
				{
					if (xmlNode is XmlComment && xmlNode.Value.StartsWith(EndIncludeToken) && xmlNode.Value.Replace(EndIncludeToken, String.Empty).Trim() == includeFilePath)
					{
						beginIncludeTokenFound = false;
						XmlNode includeXmlDocument = CreateIncludeDocument(childNodes);

						//Create INCLUDE node
						XmlElement includeElement = section.OwnerDocument.CreateElement(IncludePrefixToken, "include", CinchooNSURI);

						XmlAttribute pathAttribute = section.OwnerDocument.CreateAttribute(PathToken);

						if (Path.IsPathRooted(includeFilePath))
						{
							if (filePath.IsNullOrEmpty() || Path.GetDirectoryName(includeFilePath) != Path.GetDirectoryName(filePath))
								pathAttribute.Value = includeFilePath;
							else
								pathAttribute.Value = Path.GetFileName(includeFilePath);
						}
						else
							pathAttribute.Value = ChoPath.GetRelativePath(Path.GetDirectoryName(filePath), includeFilePath);

						includeElement.Attributes.Append(pathAttribute);
						section.InsertBefore(includeElement, startCommentNode);

						//Remove all the nodes
						section.RemoveChild(startCommentNode);
						foreach (XmlNode xmlChildNode in childNodes)
							section.RemoveChild(xmlChildNode);
						section.RemoveChild(xmlNode);

						ChoNestedDictionary<string, XmlNode> xmlSubDocumentList = new ChoNestedDictionary<string, XmlNode>();
						xmlSubDocumentList.Add(includeFilePath, includeXmlDocument as XmlNode);

						ContractIncludes(includeXmlDocument, filePath, xmlSubDocumentList);

						if (xmlSubDocumentList.Count > 0)
							xmlDocumentList.Add(xmlSubDocumentList);

						childNodes.Clear();

						continue;
					}

					childNodes.Add(xmlNode);
				}
			}
		}

		private static XmlNode CreateIncludeDocument(List<XmlNode> childNodes)
		{
			XmlDocument xmlDocument = new XmlDocument(_nameTable);
			if (childNodes.Count == 0)
				return xmlDocument;
			else if (childNodes.Count == 1)
			{
				xmlDocument.InnerXml = childNodes[0].OuterXml;
				return xmlDocument;
			}
			else
			{
				string nodeText;
				StringBuilder xmlText = new StringBuilder();
				foreach (XmlNode node in childNodes)
				{
					nodeText = node.OuterXml;
					if (nodeText == Environment.NewLine)
					{
						//xmlText.AppendFormat(Environment.NewLine);
					}
					else
						xmlText.AppendFormat("{0}{1}", nodeText, Environment.NewLine);
				}

				XmlDocumentFragment fragment = xmlDocument.CreateDocumentFragment();
				fragment.InnerXml = xmlText.ToString();
				return fragment;
			}
		}

		#endregion ContractIncludes Overloads

		#region ExpandIncludes Overloads

		public static ChoNestedDictionary<string, XmlNode> ExpandIncludes(string configPath, out XmlDocument xmlDocument)
		{
			xmlDocument = new XmlDocument(_nameTable);
			xmlDocument.Load(configPath);

			ChoNestedDictionary<string, XmlNode> parentFileList = new ChoNestedDictionary<string, XmlNode>();

			ExpandIncludes(xmlDocument.DocumentElement, Path.GetDirectoryName(configPath), parentFileList, _xmlnsManager);

			return parentFileList;
		}

		public static ChoNestedDictionary<string, XmlNode> ExpandIncludes(XmlNode section, string baseDirectory, XmlNamespaceManager xmlnsManager)
		{
			ChoNestedDictionary<string, XmlNode> parentFileList = new ChoNestedDictionary<string, XmlNode>();
			parentFileList.Add(baseDirectory, section);

			if (File.Exists(baseDirectory))
				baseDirectory = Path.GetDirectoryName(baseDirectory);

			ExpandIncludes(section, baseDirectory, parentFileList, xmlnsManager);

			return parentFileList;
		}

		#endregion ExpandIncludes Overloads

		#region ExpandIncludes Helper methods (Private)

		private static void ExpandIncludes(XmlNode section, string baseDirectory, ChoNestedDictionary<string, XmlNode> includeFileList, XmlNamespaceManager xmlnsManager)
		{
			XmlNodeList includeNodes = section.SelectNodes(".//cinchoo:include", xmlnsManager);
			if (includeNodes.Count > 0)
			{
				XmlDocument ownerDocument = section is XmlDocument ? section as XmlDocument : section.OwnerDocument;

				ChoNestedDictionary<string, XmlNode> includeSubFileList = new ChoNestedDictionary<string, XmlNode>();
				foreach (XmlElement includeElement in includeNodes)
				{
					if (includeElement == null) continue;

					string includeFileName = includeElement.GetAttribute(PathToken);

					includeFileName = ExpandNow(section, baseDirectory, includeFileList, includeSubFileList, includeElement, includeFileName, xmlnsManager);
				}

				if (includeSubFileList.Length > 0)
					includeFileList.Add(includeSubFileList);
			}
			else if (section is XmlElement && ((XmlElement)section).Name == IncludeToken)
			{
				ChoNestedDictionary<string, XmlNode> includeSubFileList = new ChoNestedDictionary<string, XmlNode>();
				XmlElement element = section as XmlElement;
				string includeFileName = element.Attributes != null && element.Attributes.Count > 0 && element.Attributes[PathToken] != null ? element.Attributes[PathToken].Value : null;

				includeFileName = ExpandNow(section, baseDirectory, includeFileList, includeSubFileList, element, includeFileName, xmlnsManager);

				if (includeSubFileList.Length > 0)
					includeFileList.Add(includeSubFileList);
			}
		}

		private static string ExpandNow(XmlNode section, string baseDirectory, ChoNestedDictionary<string, XmlNode> includeFileList, ChoNestedDictionary<string, XmlNode> includeSubFileList,
			XmlElement includeElement, string includeFileName, XmlNamespaceManager xmlnsManager)
		{
			if (String.IsNullOrEmpty(includeFileName))
			{
				RemoveIncludeNode(includeElement);
				return includeFileName;
			}
			else if (!Path.IsPathRooted(includeFileName) && !String.IsNullOrEmpty(baseDirectory))
				includeFileName = Path.Combine(baseDirectory, includeFileName);

			//if (!File.Exists(includeFileName))
			//    RemoveIncludeNode(section, includeElement);

			if (IsCircularFileExists(includeFileList, includeFileName))
				throw new ChoXmlDocumentException(String.Format("Circular reference encountered on the {0} file.", baseDirectory));

			XmlNode[] newNodes = InsertNodes(includeElement, includeFileName, ExtractNodes(includeElement, includeFileName));
			if (newNodes != null)
			{
				foreach (XmlNode newNode in newNodes)
				{
					if (!includeSubFileList.ContainsKey(includeFileName))
						includeSubFileList.Add(includeFileName, newNode);
					ExpandIncludes(newNode, baseDirectory, includeSubFileList, xmlnsManager);
				}
			}
			//includeSubFileList.Add(includeFileName, includeElement);
			return includeFileName;
		}

		private static void RemoveIncludeNode(XmlElement includeElement)
		{
			includeElement.ParentNode.RemoveChild(includeElement);
		}

		private static XmlNode[] InsertNodes(XmlNode includeElement, string includeFileName, IEnumerable nodes)
		{
			XmlDocument ownerDocument = includeElement is XmlDocument ? includeElement as XmlDocument : includeElement.OwnerDocument;
			
			bool isFirstChild = true;
			XmlNode lastChild = null;
			XmlNode commentNode = null;
			XmlNode parentNode = includeElement.ParentNode;
			List<XmlNode> newNodes = new List<XmlNode>();
			foreach (XmlNode newNode in nodes)
			{
				if (newNode.NodeType == XmlNodeType.Whitespace) continue;

				newNodes.Add(newNode);
				if (isFirstChild)
				{
					isFirstChild = false;
					lastChild = newNode;
					parentNode.ReplaceChild(newNode, includeElement);
					commentNode = parentNode.InsertBefore(ownerDocument.CreateComment(String.Format(String.Format("{0} {1}", BeginIncludeToken, includeFileName))), newNode);
				}
				else
				{
					parentNode.InsertAfter(newNode, lastChild);
					lastChild = newNode;
				}
			}

			if (lastChild != null)
			{
				//if (lastChild.Name != IncludeToken)
					parentNode.InsertAfter(ownerDocument.CreateComment(String.Format(String.Format("{0} {1}", EndIncludeToken, includeFileName))), lastChild);
			//    else
			//        parentNode.AppendChild(ownerDocument.CreateComment(String.Format(String.Format("{0} {1}", EndIncludeToken, includeFileName))));
			}
			return newNodes.ToArray();
		}

		private static IEnumerable ExtractNodes(XmlNode element, string includeFileName)
		{
			XmlDocument ownerDocument = element is XmlDocument ? element as XmlDocument : element.OwnerDocument;

			XmlDocumentFragment fragment = ownerDocument.CreateDocumentFragment();

			try
			{
				//XmlDocument includeDoc = ChoXmlDocument.Load(includeFileName);
				XmlDocument includeDoc = new XmlDocument(_nameTable);
				includeDoc.Load(includeFileName);

				foreach (XmlAttribute attr in element.Attributes)
				{
					if (attr.Name != PathToken
						&& attr.Name != CinchoNSToken)
						includeDoc.DocumentElement.SetAttribute(attr.Name, attr.Value);
				}

				fragment.InnerXml = includeDoc.InnerXml;

				return new XmlNode[] { fragment.SelectSingleNode("/*") };
			}
			catch (XmlException)
			{
				try
				{
					using (StreamReader sr = File.OpenText(includeFileName))
						fragment.InnerXml = sr.ReadToEnd();

					return fragment.SelectNodes("child::node()"/*"/*|/comment()"*/);
				}
				catch (Exception innerEx)
				{
					throw new ChoXmlDocumentException(String.Format("{0}: Error loading xml file.", includeFileName), innerEx);
				}
			}
			catch (Exception ex)
			{
				throw new ChoXmlDocumentException(String.Format("{0}: Error loading xml file.", includeFileName), ex);
			}
		}

		private static bool IsCircularFileExists(ChoNestedDictionary<string, XmlNode> parentFileList, string xmlFilePath)
		{
			return parentFileList.ContainsKey(xmlFilePath); //.Exists(delegate(string item) { return String.Compare(item, xmlFilePath, true) == 0; });
		}

		#endregion ExpandIncludes Helper methods (Private)

		#endregion Embedded Include File Loading Members

		#endregion Shared Helper Members (Public)

		#region Shared Members (Public)

		public static XmlNamespaceManager NameSpaceManager
		{
			get { return _xmlnsManager; }
		}
        
        public static void CreateXmlFileIfEmpty(string appConfigPath)
        {
            CreateXmlFileIfEmpty(appConfigPath, "configuration");
        }

        public static void CreateXmlFileIfEmpty(string appConfigPath, string rootElementName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(rootElementName, "RootElementName");
            
            if (!File.Exists(appConfigPath))
            {
                ChoDirectory.CreateDirectoryFromFilePath(appConfigPath);

                using (StreamWriter sw = File.CreateText(appConfigPath))
                {
                }
            }

            bool hasRoot = false;
            using (XmlReader xmlReader = XmlReader.Create(appConfigPath))
            {
                try
                {
                    if (xmlReader.MoveToContent() == XmlNodeType.Element)
                        hasRoot = true;
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Root element is missing.")
                        throw;
                }
            }

            //string allText = File.ReadAllText(appConfigPath);
            if (!hasRoot) //allText.IsNullOrWhiteSpace())
            {
                File.WriteAllText(appConfigPath, @"<?xml version=""1.0"" encoding=""utf-8"" ?>{0}<{1}/>".FormatString(Environment.NewLine, rootElementName));
            }
        }
		#endregion Shared Members (Public)

		#region ChoSyncDisposableObject Overrides

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

		protected void Dispose(bool finalize)
		{
			Save();
			Stop();
		}

		#endregion ChoSyncDisposableObject Overrides

        #region Finalizer

        ~ChoXmlDocument()
        {
            Dispose(true);
        }

        #endregion Finalizer
    }
}
