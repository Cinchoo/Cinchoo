namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	/// <summary>
	/// <para>Provides a way to watch for changes to configuration in storage.</para>
	/// </summary>
	public interface IChoConfigurationChangeWatcher : IDisposable
	{
		/// <summary>
		/// Event raised when the underlying persistence mechanism for configuration notices that
		/// the persistent representation of configuration information has changed.
		/// </summary>
		//event ChoConfigurationChangedEventHandler ConfigurationChanged;

		void SetConfigurationChangedEventHandler(object key, ChoConfigurationChangedEventHandler ConfigurationChanged);

		/// <summary>
		/// When implemented by a subclass, starts the object watching for configuration changes
		/// </summary>
		void StartWatching();

		/// <summary>
		/// When implemented by a subclass, stops the object from watching for configuration changes
		/// </summary>
		void StopWatching();

		/// <summary>
		/// When implemented by a subclass, stops the object from watching for configuration changes
		/// </summary>
		void RestartWatching();

		void ResetWatching();

		/// <summary>
		/// When implemented by a subclass, returns the section name that is being watched.
		/// </summary>
		string SectionName { get; }

		void OnConfigurationChanged();

		ChoConfigurationChangedEventArgs EventData { get; }
	}
}
