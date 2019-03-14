namespace Cinchoo.Core.IO
{
    #region NameSpaces

    using System.IO;

    #endregion NameSpaces

    public class ChoFileSystemWatcher : FileSystemWatcher
    {
        #region Instance Data Members (Private)

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        #endregion Instance Data Members (Private)

        #region Constructors

        protected ChoFileSystemWatcher(bool trash)
        {
        }

        public ChoFileSystemWatcher(string name)
        {
            ChoGuard.ArgumentNotNull(name, "Name");
            _name = name;

        }

        #endregion Constructors

        #region Shared Members (Public)

        public static ChoFileSystemWatcher AutoFix(ChoFileSystemWatcher fileSystemWatcher)
        {
            ChoGuard.ArgumentNotNull(fileSystemWatcher, "FileSystemWatcher");
            return new ChoAutoFixFileSystemWatcher(fileSystemWatcher);
        }

        #endregion Shared Members (Public)

        private class ChoAutoFixFileSystemWatcher : ChoFileSystemWatcher
        {
            #region Instance Data Members (Private)

            private ChoFileSystemWatcher _fileSystemWatcher;

            #endregion Instance Data Members (Private)

            #region Constructors

            public ChoAutoFixFileSystemWatcher(ChoFileSystemWatcher fileSystemWatcher) : base(true)
            {
                _fileSystemWatcher = fileSystemWatcher;
                _fileSystemWatcher.Error += new ErrorEventHandler(FileSystemWatcherError);
            }

            #endregion Constructors

            #region Instance Members (Private)

            private void FileSystemWatcherError(object sender, ErrorEventArgs e)
            {
            }

            #endregion Instance Members (Private)
        }
    }
}
