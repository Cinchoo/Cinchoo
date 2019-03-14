namespace Cinchoo.Core.IO
{
	#region NameSpaces
	
	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces
	
    /// </summary>
	/// <param name="sender">
	/// <para>The Source of the event.</para>
	/// </param>
	/// <param name="e">
	/// <para>A <see cref="SourceChangedEventArgs"/> that contains the event data.</para>
	/// </param>
	public delegate void ChoSourceChangedEventHandler(object sender, ChoSourceChangedEventArgs e);

	/// <summary>
	/// </summary>
	[Serializable]
	public class ChoSourceChangedEventArgs : EventArgs
	{
		private readonly string _name;
        private readonly DateTime _lastUpdatedTimeStamp;

		/// <summary>
		/// <para>Initialize a new instance of the <see cref="SourceChangingEventArgs"/> class with the section name</para>
		/// </summary
		/// <param name="SourceSectionName"><para>The section name of the changes.</para></param>
        public ChoSourceChangedEventArgs(string name, DateTime lastUpdatedTimeStamp)
		{
			this._name = name;
            _lastUpdatedTimeStamp = lastUpdatedTimeStamp;
		}

		/// <summary>
		/// <para>Gets the section name where the changes occurred.</para>
		/// </summary>
		/// <value>
		/// <para>The section name where the changes occurred.</para>
		/// </value>
		public string Name
		{
			get { return _name; }
		}

        public DateTime LastUpdatedTimeStamp
        {
            get { return _lastUpdatedTimeStamp; }
        }
    }
}
