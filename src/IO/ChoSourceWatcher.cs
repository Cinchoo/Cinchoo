namespace Cinchoo.Core.IO
{
    #region NameSpaces

    using System;
    using System.Diagnostics;

    using Cinchoo.Core.Configuration;

    #endregion NameSpaces
    
    [DebuggerDisplay("Name = {Name}")]
    public class ChoFileSourceChangeWatcher : ChoSyncDisposableObject
    {
        #region Instance Data Members (Private)

        private readonly string _filePath;
        private IChoConfigurationChangeWatcher _configurationChangeWatcher;

        #endregion Instance Data Members (Private)

        #region Events

        public event EventHandler<ChoSourceChangedEventArgs> SourceChanged;

        #endregion Events

        #region Constructor

        public ChoFileSourceChangeWatcher(string filePath)
            : this(filePath, null)
        {
        }

        public ChoFileSourceChangeWatcher(string filePath, string[] otherFileList)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "filePath");

            _filePath = filePath;
            _configurationChangeWatcher = new ChoConfigurationChangeCompositeFileWatcher(_filePath, _filePath, otherFileList);
            _configurationChangeWatcher.SetConfigurationChangedEventHandler(filePath, OnConfigurationChanged);
        }

        #endregion Constructor

        #region Instance Members (Public)

        public void StartWatching()
        {
            ChoGuard.NotDisposed(this);

            if (_configurationChangeWatcher != null)
                _configurationChangeWatcher.StartWatching();
        }

        public void StopWatching()
        {
            ChoGuard.NotDisposed(this);

            if (_configurationChangeWatcher != null)
                _configurationChangeWatcher.StopWatching();
        }

        #endregion Instance Members (Public)

        private void OnConfigurationChanged(object sender, ChoConfigurationChangedEventArgs e)
        {
            SourceChanged.Raise(this, new ChoSourceCompositeChangedEventArgs(e));
        }

        protected override void Dispose(bool finalize)
        {
            if (_configurationChangeWatcher != null)
            {
                _configurationChangeWatcher.StopWatching();
                _configurationChangeWatcher = null;
            }
        }
    }
}
