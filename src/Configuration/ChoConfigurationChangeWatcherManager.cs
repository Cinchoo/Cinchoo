namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Collections.Generic;

    using eSquare.Core.Diagnostics;
    using eSquare.Core.Configuration.Storage;
    using eSquare.Core.Diagnostics.Attributes;

    #endregion NameSpaces

    /// <summary>
    /// Manages the configuration file watchers for a collection of configuration sections.
    /// </summary>
    [ChoStreamProfile("Configuration Source Watchers", FileNameFromTypeFullName=typeof(ChoConfigurationChangeWatcherManager), Extension = ChoExt.Log, Mode = ChoProfileMode.Truncate)]
    public static class ChoConfigurationChangeWatcherManager
    {
        private static readonly List<IChoConfigurationChangeWatcher> _configSourceWatcherMapping = new List<IChoConfigurationChangeWatcher>();

        /// <summary>
        /// Event to notify when configuration changes.
        /// </summary>
        //public static event ChoConfigurationChangedEventHandler ConfigurationChanged;

        ///<summary>
        /// Determines if the configuration source is being watched.
        ///</summary>
        ///<param name="configSource">
        /// The configuration source.
        /// </param>
        ///<returns>
        /// true if the source is being watched; otherwise, false.
        /// </returns>
        public static bool IsWatchingConfigSource(IChoConfigurationChangeWatcher configSourceWatcher)
        {
            return _configSourceWatcherMapping.Contains(configSourceWatcher);
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments.</param>
        public static void OnConfigurationChanged(object sender,
                                           ChoConfigurationChangedEventArgs args)
        {
            foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in _configSourceWatcherMapping)
            {
                if (configurationChangeWatcher == null) continue;
                configurationChangeWatcher.OnConfigurationChanged();
            }

            //if (ConfigurationChanged != null)
            //{
            //    ConfigurationChanged(sender, args);
            //}
        }

        /// <summary>
        /// Removes a watcher for the configuration source.
        /// </summary>
        /// <param name="configSource">
        /// The source to remove the watcher.
        /// </param>
        public static void RemoveWatcherForConfigSource(IChoConfigurationChangeWatcher configSourceWatcher)
        {
            if (_configSourceWatcherMapping.Contains(configSourceWatcher))
            {
                _configSourceWatcherMapping.Remove(configSourceWatcher);
                configSourceWatcher.Dispose();
                
                ChoProfile.WriteLine(String.Format("Removing Watcher: [{0}]", configSourceWatcher.EventData.ToString()));
            }
        }

        /// <summary>
        /// Sets a watcher for a configuration source.
        /// </summary>
        /// <param name="configSource">
        /// The configuration source to watch.
        /// </param>
        public static void SetWatcherForConfigSource(IChoConfigurationChangeWatcher configSourceWatcher)
        {
            if (!IsWatchingConfigSource(configSourceWatcher))
            {
                ////configSourceWatcher.ConfigurationChanged += OnConfigurationChanged;

                _configSourceWatcherMapping.Add(configSourceWatcher);

                ChoProfile.WriteLine(String.Format("Setting Watcher: [{0}]", configSourceWatcher.EventData.ToString()));
            }
        }

        //public static void SetWatcherForConfigSource(IChoConfigurationChangeWatcher configSourceWatcher, ChoConfigurationChangedEventHandler configurationChangedEventHandler)
        //{
        //    ChoGuard.ArgumentNotNull(configSourceWatcher, "ConfigSourceWatcher");
        //    ChoGuard.ArgumentNotNull(configurationChangedEventHandler, "ConfigurationChangedEventHandler");

        //    SetWatcherForConfigSource(configSourceWatcher);
        //    //ConfigurationChanged += configurationChangedEventHandler;
        //}

        //public static void SetWatcherForConfigSource(IChoConfigurationChangeWatcher[] configSourceWatchers, ChoConfigurationChangedEventHandler configurationChangedEventHandler)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(configSourceWatchers, "ConfigSourceWatchers");
        //    ChoGuard.ArgumentNotNull(configurationChangedEventHandler, "ConfigurationChangedEventHandler");

        //    foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configSourceWatchers)
        //        SetWatcherForConfigSource(configSourceWatcher);

        //    ConfigurationChanged += configurationChangedEventHandler;
        //}
    }
}
