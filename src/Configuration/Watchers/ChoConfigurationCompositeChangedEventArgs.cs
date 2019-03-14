namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;

	using Cinchoo.Core.Properties;
	using System.Diagnostics;

	#endregion NameSpaces

	[Serializable]
	public class ChoConfigurationCompositeChangedEventArgs : ChoConfigurationChangedEventArgs
	{
		/// <summary>
		/// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the configuration file, the section name, the old value, and the new value of the changes.</para>
		/// </summary>
		/// <param name="configurationFile"><para>The configuration file where the change occured.</para></param>
		/// <param name="sectionName"><para>The section name of the changes.</para></param>
        public ChoConfigurationCompositeChangedEventArgs(string sectionName)
            : base(sectionName, DateTime.MinValue)
		{
		}

		#region Object Overrides

		public override string ToString()
		{
			return String.Format("SectionName: {0}", SectionName);
		}

		#endregion Object Overrides
	}
}
