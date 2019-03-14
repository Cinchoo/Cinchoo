namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	[Serializable]
	public class ChoConfigurationODBCChangedEventArgs : ChoConfigurationChangedEventArgs
	{
		#region Instance Data Members (Public)

		public readonly string ConnectionString;
		public readonly string LastWriteTimeTableName;
		public readonly string LastWriteTimeColumnName;

		#endregion Instance Data Members (Public)

		#region Constructors

		/// <summary>
		/// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the configuration file, the section name, the old value, and the new value of the changes.</para>
		/// </summary>
		/// <param name="configurationFile"><para>The configuration file where the change occured.</para></param>
		/// <param name="sectionName"><para>The section name of the changes.</para></param>
		public ChoConfigurationODBCChangedEventArgs(string sectionName, string connectionString, string lastWriteTimeTableName, string lastWriteTimeColumnName)
			: base(sectionName, DateTime.MinValue)
		{
			ConnectionString = connectionString;
			LastWriteTimeTableName = lastWriteTimeTableName;
			LastWriteTimeColumnName = lastWriteTimeColumnName;
		}

		#endregion Constructors

		#region Object Overrides

		public override string ToString()
		{
			return String.Format("SectionName: {0}, Type: {1}, ConnectionString: {2}, LastWriteTimeTableName: {3}, LastWriteTimeColumnName: {4}",
				this.SectionName, typeof(ChoConfigurationODBCChangedEventArgs).FullName, ConnectionString, LastWriteTimeTableName, LastWriteTimeColumnName);
		}

		#endregion Object Overrides
	}
}
