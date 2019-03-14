namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Web;
    using System.Xml.XPath;
    using System.Globalization;
    using System.Configuration;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using eSquare.Core.IO;
    using eSquare.Core.Properties;
    using eSquare.Core.Xml.Serialization;
    using eSquare.Core.Configuration.Sections;

    #endregion NameSpaces

    public static class ChoConfigFilesMapper
    {
        #region Shared Data Members

        private static StringDictionary _files = new StringDictionary();

        #endregion

        #region Shared Members (Public)

        public static string GetFullPath(string name, string path)
        {
            if (path == null || path.Trim().Length == 0) return path;

            path = path.Trim();

            if (_files[name] != null)
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_files[name]), path);
            else
                return ChoPath.GetFullPath(path);
        }

        public static bool Contains(string name)
        {
            return _files.ContainsKey(name);
        }

        public static void Add(string name, string location)
        {
            if (Contains(name)) return;

            _files.Add(name, location);
        }

        #endregion
    }
}
