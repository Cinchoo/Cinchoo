namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	/// <summary>
	/// </summary>
	[Serializable]
	public class ChoConfigurationStateChangedEventArgs : EventArgs
	{
		private readonly string _configurationSectionName;
		private readonly object _cmdInstruction;
		private readonly object _tag;

		/// <summary>
		/// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the section name</para>
		/// </summary>
		/// <param name="configurationSectionName"><para>The section name of the changes.</para></param>
		public ChoConfigurationStateChangedEventArgs(string configurationSectionName, object cmdInstruction, object tag)
		{
			this._configurationSectionName = configurationSectionName;
			_cmdInstruction = cmdInstruction;
			_tag = tag;
		}

		public object Tag
		{
			get { return _tag; }
		}

		public object CmdInstruction
		{
			get { return _cmdInstruction; }
		}

		/// <summary>
		/// <para>Gets the section name where the changes occurred.</para>
		/// </summary>
		/// <value>
		/// <para>The section name where the changes occurred.</para>
		/// </value>
		public string SectionName
		{
			get { return _configurationSectionName; }
		}
	}
}
