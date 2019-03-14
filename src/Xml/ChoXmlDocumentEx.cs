namespace eSquare.Core.Xml
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Xml.XPath;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using eSquare.Core.Exceptions;
    using eSquare.Core.Services;
    using System.Security.AccessControl;

    #endregion

    [DebuggerDisplay("Path = {_path}")]
    public class ChoXmlDocument : ChoSyncDisposableObject
    {
        #region Constants

        internal const string PathToken = "__path__";

        #endregion Constants

        #region Events

        //public event EventHandler<ChoXmlDocumentEventArgs> IgnoredEntry;
        //public event EventHandler<ChoXmlDocumentEventArgs> ErrorFound;

        #endregion Events

        #region Instance Data Members (Private)

        private readonly string _filePath;
        private readonly string _xml;
        private readonly object _syncRoot = new object();
        private readonly bool _ignoreInvalidNode = true;
        private readonly ChoQueuedMsgService<ChoStandardQueuedMsg> _queuedMsgService;
        private readonly ChoTimerService<string> _timerService;
        private readonly Regex _includeXmlFileRegEx;
        private readonly ChoXmlDocument _parentXmlDocument;
        private readonly ChoXmlDocument _ultimateParentXmlDocument;
        private readonly object _dirtyLockObject = new object();

        private bool _isInitialized = false;
        private bool _dirty = false;
        private bool _documentLoaded = false;
        private int _lineNo;
        private List<string> _allXmlFilePaths;
        private XmlDocument _xmlDocument;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoXmlDocument(string filePathOrXml, ChoXmlDocument parentXmlDocument)
            : this(filePathOrXml)
        {
            ////_syncRoot = parentXmlDocument.SyncRoot;
            //_ultimateParentXmlDocument = _parentXmlDocument = parentXmlDocument;

            //while (_ultimateParentXmlDocument.ParentXmlDocument != _ultimateParentXmlDocument)
            //    _ultimateParentXmlDocument = _ultimateParentXmlDocument.ParentXmlDocument;
        }

        public ChoXmlDocument(string filePathOrXml)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePathOrXml, "FilePath or Xml");
            filePathOrXml = filePathOrXml.Trim();

            if (filePathOrXml.StartsWith("<"))
                _xml = filePathOrXml;
            else
                _filePath = filePathOrXml;

            if (IsXmlFile)
            {
                //_includeXmlFileRegEx = new Regex(String.Format(@"^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*[{1}](?<{2}>.*)$|^\s*\[INCLUDE\(""(?<{0}>[A-Za-z0-9:\\\._\s*]+)""\)\]\s*$", INI_INCLUDE_FILE_FILEPATH_TOKEN, _commentChars, INI_INCLUDE_FILE_COMMENTS_TOKEN),
                //    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
                _queuedMsgService = new ChoQueuedMsgService<ChoStandardQueuedMsg>(new ChoStandardQueuedMsgObject().QuitServiceMsg, false,
                    new ChoQueuedMsgService<ChoStandardQueuedMsg>.ChoQMessageHandler(QueueMessageHandler));
                _timerService = new ChoTimerService<string>(String.Format("{0}_Timer", System.IO.Path.GetFileNameWithoutExtension(_filePath)),
                    new ChoTimerService<string>.ChoTimerServiceCallback(OnTimerServiceCallback), null, 1000, 5000, false);
            }
        }

        public ChoXmlDocument(XmlDocument xmlDocument)
        {
            ChoGuard.ArgumentNotNull(xmlDocument, "XmlDocument");
        }

        #endregion Constructors

        #region ChoSyncMsgQProcessor Overrides

        private void QueueMessageHandler(IChoQueuedMsgServiceObject<ChoStandardQueuedMsg> msgObject)
        {
            if (msgObject == null || !ChoGuard.IsArgumentNotNullOrEmpty(msgObject.State)) return;

            lock (_syncRoot)
            {
                //File.WriteAllText(Path, msgObject.State.Msg);
            }
        }

        #endregion ChoSyncMsgQProcessor Overrides

        #region ChoQueuedMsgService Overrides

        internal void Start()
        {
            _timerService.Start();
            _queuedMsgService.Start();
        }

        internal void Stop()
        {
            lock (_syncRoot)
            {
                OnTimerServiceCallback(null);
                _timerService.Stop();
                _queuedMsgService.Stop();
            }
        }

        #endregion ChoQueuedMsgService Overrides

        #region Instance Members (Private)

        private void Load()
        {
            _xmlDocument = new XmlDocument();

            if (IsXmlFile)
                _xmlDocument.Load(_filePath);
            else
                _xmlDocument.LoadXml(_xml);

            ChoXmlDocument.ExpandIncludes(_filePath, out _xmlDocument);

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

        #endregion Instnace Members (Private)

        private void OnTimerServiceCallback(string state)
        {
            if (!Dirty) return;
            Dirty = false;

            _queuedMsgService.Enqueue(ChoStandardQueuedMsgObject.New(ToString()));
        }

        #region Instance Properties (Private)

        internal bool IsXmlFile
        {
            get { return String.IsNullOrEmpty(_filePath); }
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

        #region Instance Properties (Public)

        public XmlDocument XmlDocument
        {
            get { return _xmlDocument; }
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
            string[] includeFileList;

            return Load(filename, out includeFileList);
        }

        public static XmlDocument Load(string filename, out string[] includeFileList)
        {
            XmlDocument doc = new XmlDocument();
            Load(doc, filename);

            includeFileList = ExpandIncludes(doc, filename);

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

        public static string SetInnerXml(string fileName, string sectionName, string innerXml)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("FileName");
            if (!File.Exists(fileName))
                throw new ArgumentException(String.Format("{0} configuration file not exists.", fileName));
            if (String.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("SectionName");

            XmlDocument doc = Load(fileName);

            //Select the cd node with the matching title
            XmlNode configNode = doc.DocumentElement.SelectSingleNode(String.Format("//{0}", sectionName));
            if (configNode == null)
                throw new NullReferenceException(String.Format("Can't find {0} section in the {1} config file.", sectionName, fileName));

            int index = innerXml.IndexOf(String.Format("<{0}", sectionName));
            if (index == -1)
                configNode.InnerXml = innerXml == null ? String.Empty : innerXml;
            else
                configNode.InnerXml = innerXml == null ? String.Empty : innerXml.Substring(index);

            return IndentXMLString(doc.InnerXml);
        }

        public static string SetOuterXml(string fileName, string sectionName, string outerXml)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("FileName");
            if (!File.Exists(fileName))
                throw new ArgumentException(String.Format("{0} configuration file not exists.", fileName));
            if (String.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("SectionName");

            XmlDocument doc = Load(fileName);

            //Select the cd node with the matching title
            XmlNode configNode = doc.DocumentElement.SelectSingleNode(String.Format("//{0}", sectionName));
            if (configNode == null)
                throw new NullReferenceException(String.Format("Can't find {0} section in the {1} config file.", sectionName, fileName));

            //Remove all attributes and elements
            configNode.RemoveAll();

            XmlDocument newDoc = new XmlDocument();
            using (XmlTextReader reader = new XmlTextReader(new StringReader(outerXml)))
                newDoc.Load(reader);

            XmlNode newConfigNode = newDoc.DocumentElement.SelectSingleNode(String.Format("//{0}", sectionName));
            if (newConfigNode == null)
                throw new NullReferenceException(String.Format("Can't find {0} section in the input xml.", sectionName));

            foreach (XmlAttribute attribute in newConfigNode.Attributes)
                configNode.Attributes.Append(doc.CreateAttribute(attribute.Name)).Value = attribute.Value;

            configNode.InnerXml = newConfigNode.InnerXml;

            return IndentXMLString(doc.OuterXml);
        }

        public static string IndentXMLString(string xml)
        {
            XmlDocument doc = new XmlDocument();

            // Load the unformatted XML text string into an instance 
            // of the XML Document Object Model (DOM)
            doc.LoadXml(xml);

            return IndentXMLString(doc);
        }

        public static string IndentXMLString(XmlDocument doc)
        {
            MemoryStream ms = new MemoryStream();
            // Create a XMLTextWriter that will send its output to a memory stream (file)
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);

            // Set the formatting property of the XML Text Writer to indented
            // the text writer is where the indenting will be performed
            xtw.Formatting = Formatting.Indented;

            // write dom xml to the xmltextwriter
            doc.WriteContentTo(xtw);
            // Flush the contents of the text writer
            // to the memory stream, which is simply a memory file
            xtw.Flush();

            // set to start of the memory stream (file)
            ms.Seek(0, SeekOrigin.Begin);
            // create a reader to read the contents of 
            // the memory stream (file)
            StreamReader sr = new StreamReader(ms);
            // return the formatted string to caller
            return sr.ReadToEnd();
        }

        public static void Save(XmlDocument xmlDocument, string xmlFilePath, bool indentOutput)
        {
            ChoGuard.ArgumentNotNull(xmlDocument, "XmlDocument");
            ChoGuard.ArgumentNotNullOrEmpty(xmlFilePath, "XmlFilePath");

            if (!indentOutput)
                xmlDocument.Save(xmlFilePath);
            else
            {
                using (FileStream fs = File.Create(xmlFilePath))
                {
                    // Create a XMLTextWriter that will send its output to a memory stream (file)
                    XmlTextWriter xtw = new XmlTextWriter(fs, Encoding.Unicode);

                    // Set the formatting property of the XML Text Writer to indented
                    // the text writer is where the indenting will be performed
                    xtw.Formatting = Formatting.Indented;

                    // write dom xml to the xmltextwriter
                    xmlDocument.WriteContentTo(xtw);
                    // Flush the contents of the text writer
                    // to the memory stream, which is simply a memory file
                    xtw.Flush();
                }
            }
        }

        #region Embedded Include File Loading Members

        public static string[] ExpandIncludes(string configPath, out XmlDocument xmlDocument)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.Load(configPath);

            return ExpandIncludes(xmlDocument.DocumentElement, configPath);
        }

        public static string[] ExpandIncludes(XmlNode section, string baseDirectory)
        {
            List<string> parentFileList = new List<string>();

            return ExpandIncludes(section, baseDirectory, parentFileList);
        }

        private static string[] ExpandIncludes(XmlNode section, string baseDirectory, List<string> parentFileList)
        {
            if (!String.IsNullOrEmpty(baseDirectory) && !parentFileList.Contains(baseDirectory) && !String.IsNullOrEmpty(Path.GetFileName(baseDirectory)))
                parentFileList.Add(baseDirectory);

            if (!String.IsNullOrEmpty(baseDirectory))
                baseDirectory = Path.GetDirectoryName(baseDirectory);

            List<string> includeFileList = new List<string>();

            XmlNodeList includeNodeList = section.SelectNodes(String.Format("//include[@{0}]", PathToken));
            foreach (XmlElement element in includeNodeList)
            {
                string includeFileName = element.GetAttribute(PathToken);
                //if (String.IsNullOrEmpty(includeFileName)) continue;
                if (!String.IsNullOrEmpty(baseDirectory) && !Path.IsPathRooted(includeFileName))
                    includeFileName = Path.Combine(baseDirectory, includeFileName);

                if (!String.IsNullOrEmpty(baseDirectory) && IsCircularFileExists(parentFileList, includeFileName))
                    throw new ChoXmlDocumentException(String.Format("Circular reference encountered on the {0} file.", baseDirectory));

                if (!includeFileList.Contains(includeFileName))
                    includeFileList.Add(includeFileName);

                XmlDocument ownerDocment = section is XmlDocument ? section as XmlDocument : section.OwnerDocument;
                XmlDocumentFragment fragment = ownerDocment.CreateDocumentFragment();

                try
                {
                    XmlDocument includeDoc = ChoXmlDocument.Load(includeFileName);

                    foreach (XmlAttribute attr in element.Attributes)
                    {
                        if (attr.Name != PathToken)
                            includeDoc.DocumentElement.SetAttribute(attr.Name, attr.Value);
                    }

                    fragment.InnerXml = includeDoc.InnerXml;

                    XmlElement xmlSubElement = (XmlElement)fragment.SelectSingleNode("/*");

                    string[] innerIncludeFileList = ExpandIncludes(xmlSubElement, baseDirectory, parentFileList);
                    foreach (string innerIncludeFile in innerIncludeFileList)
                    {
                        if (!includeFileList.Contains(includeFileName))
                            includeFileList.Add(innerIncludeFile);
                    }

                    element.ParentNode.ReplaceChild(xmlSubElement, element);
                    xmlSubElement.ParentNode.InsertBefore(xmlSubElement.OwnerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - BEGIN INCLUDE {0}", includeFileName))),
                        xmlSubElement);
                    xmlSubElement.ParentNode.AppendChild(xmlSubElement.OwnerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - END INCLUDE {0}", includeFileName))));
                }
                catch (XmlException)
                {
                    try
                    {
                        using (StreamReader sr = File.OpenText(includeFileName))
                            fragment.InnerXml = sr.ReadToEnd();

                        XmlNode parentNode = element.ParentNode;

                        parentNode.RemoveChild(element);

                        bool isFirstChild = true;
                        XmlNode lastChild = null;
                        foreach (XmlNode xmlNode in fragment.SelectNodes("/*|//comment()"))
                        {
                            if (xmlNode == null) continue;

                            if (xmlNode is XmlElement && xmlNode.Name == "include")
                            {
                                string[] innerIncludeFileList = ExpandIncludes(xmlNode, includeFileName, parentFileList); //, false);
                                foreach (string innerIncludeFile in innerIncludeFileList)
                                {
                                    if (!includeFileList.Contains(includeFileName))
                                        includeFileList.Add(innerIncludeFile);
                                }
                            }

                            parentNode.AppendChild(xmlNode);
                            if (isFirstChild)
                            {
                                parentNode.InsertBefore(parentNode.OwnerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - BEGIN INCLUDE {0}", includeFileName))),
                                    xmlNode);
                                isFirstChild = false;
                            }
                            lastChild = xmlNode;
                        }
                        parentNode.InsertAfter(parentNode.OwnerDocument.CreateComment(String.Format(String.Format("DO NOT REMOVE - END INCLUDE {0}", includeFileName))),
                            lastChild);
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

            return includeFileList.ToArray();
        }

        private static bool IsCircularFileExists(List<string> parentFileList, string xmlFilePath)
        {
            return parentFileList.Exists(delegate(string item) { return String.Compare(item, xmlFilePath, true) == 0; });
        }

        #endregion Embedded Include File Loading Members

        #endregion Shared Helper Members (Public)

        protected override void Dispose(bool finalize)
        {
        }
    }
}
