namespace Cinchoo.Core.IO
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;

	using Cinchoo.Core.Properties;
	using System.Diagnostics;
    using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	[Serializable]
	public class ChoSourceCompositeChangedEventArgs : ChoSourceChangedEventArgs
	{
        internal ChoSourceCompositeChangedEventArgs(ChoConfigurationChangedEventArgs e)
            : base(e.SectionName, e.LastUpdatedTimeStamp)
        {
        }

		/// <summary>
		/// <para>Initialize a new instance of the <see cref="SourceChangingEventArgs"/> class with the Source file, the section name, the old value, and the new value of the changes.</para>
		/// </summary>
		/// <param name="SourceFile"><para>The Source file where the change occured.</para></param>
		/// <param name="name"><para>The section name of the changes.</para></param>
        public ChoSourceCompositeChangedEventArgs(string name)
            : base(name, DateTime.MinValue)
		{   
		}

		#region Object Overrides

		public override string ToString()
		{
			return String.Format("Name: {0}", Name);
		}

		#endregion Object Overrides
	}
}
