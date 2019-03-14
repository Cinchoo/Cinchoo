namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;

    using Cinchoo.Core.Services;

	#endregion

	[Serializable]
	public sealed class ChoNullSection : ChoConfigSection
	{
		#region Constructors

		public ChoNullSection(Type configObjectType)
			: base(configObjectType, null, null, null)
		{
		}


		public ChoNullSection(Type configObjectType, string filePath)
			: base(configObjectType, null, null, null)
		{
			//REDO:
			//ConfigPath = filePath;
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
			get { return new ChoNullConfigStorage(); }
		}

		#endregion ChoConfigSection Overrides
	}
}
