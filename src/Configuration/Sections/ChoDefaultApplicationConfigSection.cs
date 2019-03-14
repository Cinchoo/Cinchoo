namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	[Serializable]
	internal sealed class ChoDefaultApplicationConfigSection : ChoConfigSection
	{
		#region Constructors

		public ChoDefaultApplicationConfigSection(Type configObjectType)
			: base(configObjectType, null, null, null)
		{
		}

		#endregion Constructors

		#region IChoConfigSection Members

		public override object this[string key]
		{
			get { return null; }
		}

        public override bool HasConfigMemberDefined(string key)
        {
            return false;
        }

		public override bool HasConfigSectionDefined
		{
			get { return false; }
		}

		public override void Persist(string configSectionFullPath, ChoDictionaryService<string, object> stateInfo)
		{
		}

		public override object PersistableState
		{
			get { return null; }
		}

		#endregion

		#region ChoConfigSection Overrides

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoApplicationExeConfigStorage(); }
		}

		#endregion ChoConfigSection Overrides
	}
}
