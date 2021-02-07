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
	[ChoTypeFormatter("Validation Managers")]
	[ChoConfigurationSection("cinchoo/validationManagerSettings", Defaultable=false)]
	[XmlRoot("validationManagerSettings")]
	public class ChoValidationManagerSettings //: IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlElement("validationManager", typeof(ChoObjConfigurable))]
		[ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable = false)]
        public ChoObjConfigurable[] ValidationManagerTypes = new ChoObjConfigurable[0];

		#endregion

		#region Instance Data Members (Private)

		private ChoDictionary<string, IChoValidationManager> _validationManagers;

		#endregion

		#region Shared Data Members (Private)

        private static string _buildInValidationManagerType = typeof(ChoValidationManager).SimpleQualifiedName();

		#endregion Shared Data Members (Private)

		#region Instance Properties (Public)

		[XmlIgnore]
		[ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable = false)]
        public IChoValidationManager[] ValidationManagers
		{
			get { return _validationManagers.ToValuesArray(); }
		}

		[XmlIgnore]
		[ChoMemberFormatter(ChoNull.NullString, Formatter = typeof(ChoArrayToStringFormatter))]
        [ChoPropertyInfo(Persistable = false)]
        public string[] ValidationManagerKeys
		{
			get { return _validationManagers.ToKeysArray(); }
		}

		#endregion

		#region Shared Properties

        private static readonly object _padLock = new object();
        private static ChoValidationManagerSettings _validationManagerSettings;
		public static ChoValidationManagerSettings Me
		{
			get 
            {
                if (_validationManagerSettings != null)
                    return _validationManagerSettings;

                lock (_padLock)
                {
                    if (_validationManagerSettings == null)
                    {
                        _validationManagerSettings = new ChoValidationManagerSettings();
                        _validationManagerSettings.Initialize(false, null);
                    }

                    return _validationManagerSettings;
                }

                //return ChoConfigurationManagementFactory.CreateInstance<ChoValidationManagerSettings>(); 
            }
		}

		private static ChoDictionary<string, IChoValidationManager> _defaultValidationManagers;
		public static ChoDictionary<string, IChoValidationManager> DefaultValidationManagers
		{
			get
			{
				if (_defaultValidationManagers == null)
				{
					//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoValidationManagerSettings).FullName, ChoReservedFileExt.Err));
					
					_defaultValidationManagers = new ChoDictionary<string, IChoValidationManager>();
					ChoObjConfigurable.Load<IChoValidationManager>(ChoPath.AddExtension(typeof(ChoValidationManagerSettings).FullName, ChoReservedFileExt.Log), 
						_buildInValidationManagerType, _defaultValidationManagers, null);
				}

				return _defaultValidationManagers;
			}
		}

		#endregion

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			//Create the default/built-in objects
			_validationManagers = new ChoDictionary<string, IChoValidationManager>(DefaultValidationManagers);

            //ChoObjConfigurable.Load<IChoValidationManager>(ChoType.GetLogFileName(typeof(ChoValidationManagerSettings)), ChoType.GetTypes(typeof(ChoValidationManagerAttribute)),
            //    _validationManagers, ValidationManagerTypes);

			return false;
		}

		#endregion

		public bool Validate(ChoValidationResults validationResults)
		{
			//Dummy validation routine to instruct Framework, not to do auto validation on this object
			return true;
		}
	}
}
