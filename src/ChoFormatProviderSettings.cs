namespace Cinchoo.Core
{
	#region NameSpaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;

	#endregion NameSpaces

	[Serializable]
	[ChoTypeFormatter("Format Providers")]
	[ChoConfigurationSection("cinchoo/formatProvidersSettings")]
	[XmlRoot("formatProviderSettings")]
	public class ChoFormatProviderSettings : IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlElement("formatProvider", typeof(ChoObjConfigurable))]
		[ChoIgnoreMemberFormatter]
		public ChoObjConfigurable[] FormatProviderTypes = new ChoObjConfigurable[0];

		#endregion

		#region Instance Data Members (Private)

		private ChoDictionary<string, IFormatProvider> _formatProviders = new ChoDictionary<string,IFormatProvider>();

		#endregion

		#region Instance Properties (Public)

		[XmlIgnore]
		[ChoIgnoreMemberFormatter]
		public IFormatProvider[] FormatProviders
		{
			get { return _formatProviders.ToValuesArray(); }
		}

		[XmlIgnore]
		[ChoMemberFormatter(ChoNull.NullString, Formatter = typeof(ChoArrayToStringFormatter))]
		internal string[] FormatProviderKeys
		{
			get { return _formatProviders.ToKeysArray(); }
		}

		#endregion

		#region Instance Members (Public)

		public bool TryGetValue(string formatterName, out IFormatProvider formatProvider)
		{
			return _formatProviders.TryGetValue(formatterName, out formatProvider);
		}

		#endregion Instance Members (Public)

		#region Shared Properties

		public static ChoFormatProviderSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoFormatProviderSettings>(); }
		}

		#endregion

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			if (!beforeFieldInit)
				ChoObjConfigurable.Load<IFormatProvider>(ChoPath.AddExtension(typeof(ChoFormatProviderSettings).FullName, ChoReservedFileExt.Log), ChoType.GetTypes<IFormatProvider>(),
					_formatProviders, FormatProviderTypes, ChoDefaultObjectKey.Name);
			//else
				//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoFormatProviderSettings).FullName, ChoReservedFileExt.Err));
			
			return false;
		}

		#endregion
	}
}
