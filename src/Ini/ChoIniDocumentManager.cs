namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.IO;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public static class ChoIniDocumentManager
    {
        #region Shared Data Members (Private)

        private static ChoDictionary<string, ChoIniDocument> _iniDocuments = ChoDictionary<string, ChoIniDocument>.Synchronized(new ChoDictionary<string, ChoIniDocument>());
        private static ChoDictionary<string, DateTime> _iniDocumentsLastModifiedDateTimes = new ChoDictionary<string, DateTime>();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static ChoIniDocument GetDocument(string filePath)
        {
            lock (_iniDocuments.SyncRoot)
            {
                if (IsModified(filePath) && _iniDocuments.ContainsKey(filePath))
                    _iniDocuments.Remove(filePath);

                return LoadDocument(filePath);
            }
        }

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		private static ChoIniDocument LoadDocument(string filePath)
        {
            lock (_iniDocuments.SyncRoot)
            {
                if (!_iniDocuments.ContainsKey(filePath))
                    _iniDocuments.Add(filePath, OpenNLoadDocument(filePath));
                else if (_iniDocuments[filePath].IsDisposed)
                    _iniDocuments[filePath] = ChoIniDocument.Load(filePath);

                return _iniDocuments[filePath];
            }
        }

        private static ChoIniDocument OpenNLoadDocument(string filePath)
        {
            ChoIniDocument iniDocument = ChoIniDocument.Load(filePath);
            iniDocument.TextIgnored += new EventHandler<ChoIniDocumentEventArgs>(iniDocument_IgnoredEntry);
            iniDocument.TextErrorFound += new EventHandler<ChoIniDocumentEventArgs>(iniDocument_ErrorFound);

            return iniDocument;
        }

        private static void iniDocument_ErrorFound(object sender, ChoIniDocumentEventArgs e)
        {
            if (e == null || e.IniDocumentStates == null) return;

            foreach (ChoIniDocumentState iniDocumentState in e.IniDocumentStates)
            {
                ChoProfile.RegisterIfNotExists(iniDocumentState.IniFilePath, new ChoBufferProfileEx(ChoFileProfileSettings.GetFullPath(ChoReservedDirectoryName.Others,
                    ChoPath.AddExtension(Path.GetFileName(iniDocumentState.IniFilePath), ChoReservedFileExt.Err)),
                    "Error found entries are..."));
                ChoProfile.GetContext(iniDocumentState.IniFilePath).AppendLine(iniDocumentState.ToString());
            }
        }

        private static void iniDocument_IgnoredEntry(object sender, ChoIniDocumentEventArgs e)
        {
            if (e == null || e.IniDocumentStates == null) return;

            foreach (ChoIniDocumentState iniDocumentState in e.IniDocumentStates)
            {
                ChoProfile.RegisterIfNotExists(iniDocumentState.IniFilePath, new ChoBufferProfileEx(ChoFileProfileSettings.GetFullPath(ChoReservedDirectoryName.Others,
                    ChoPath.AddExtension(Path.GetFileName(iniDocumentState.IniFilePath), ChoReservedFileExt.Ignore)),
                    "Entries Ignored are..."));
                ChoProfile.GetContext(iniDocumentState.IniFilePath).AppendLine(iniDocumentState.ToString());
            }
        }

        private static bool IsModified(string filePath)
        {
            DateTime lastModifiedDateTime = DateTime.MinValue;

            if (_iniDocumentsLastModifiedDateTimes.ContainsKey(filePath))
                lastModifiedDateTime = _iniDocumentsLastModifiedDateTimes[filePath];
            else
                _iniDocumentsLastModifiedDateTimes.Add(filePath, lastModifiedDateTime);

            FileInfo fileInfo = new FileInfo(filePath);
            if (lastModifiedDateTime < fileInfo.LastWriteTimeUtc)
            {
                _iniDocumentsLastModifiedDateTimes[filePath] = fileInfo.LastWriteTimeUtc;
                return true;
            }

            return false;
        }

        [ChoAppDomainUnloadMethod("Closing all INI documents...")]
        private static void DisposeAll()
        {
            foreach (ChoIniDocument iniDocument in _iniDocuments.ToValuesArray())
            {
                if (iniDocument == null) continue;
                iniDocument.Dispose();
            }
            _iniDocuments.Clear();
        }
        
		#endregion Shared Members (Public)
    }
}
