namespace Cinchoo.Core.Win32
{
	#region NameSpaces

	using System;
    using Microsoft.Win32;

    #endregion NameSpaces

    public class ChoRegistryKey : IDisposable
	{
		#region Instance Data Members (Private)

		private readonly string _registryKey;
		private readonly bool _createKeyIfNotFound;
		private readonly RegistryKey _registryHive;
		private readonly string _registrySubKeyName;
		private RegistryKey _registrySubKey;
        private bool _silent = false;

		#endregion Instance Data Members (Private)

		#region Constructors

        public ChoRegistryKey(string registryKey, bool createKeyIfNotFound = false, bool silent = false)
		{
			ChoGuard.ArgumentNotNullOrEmpty(registryKey, "RegistryKey");

			_createKeyIfNotFound = createKeyIfNotFound;
			_registryKey = registryKey;
            _silent = silent;

			string[] nameParts = registryKey.Split('\\');
			if (nameParts.Length < 2)
				throw new ArgumentException("Invalid registry key.");

			switch (nameParts[0].ToUpper())
			{
				case "HKEY_CLASSES_ROOT":
				case "HKCR":
					_registryHive = Registry.ClassesRoot;
					break;
				case "HKEY_CURRENT_USER":
				case "HKCU":
					_registryHive = Registry.CurrentUser;
					break;
				case "HKEY_LOCAL_MACHINE":
				case "HKLM":
					_registryHive = Registry.LocalMachine;
					break;
				case "HKEY_USERS":
					_registryHive = Registry.Users;
					break;
				case "HKEY_CURRENT_CONFIG":
					_registryHive = Registry.CurrentConfig;
					break;
				default:
					throw new ArgumentException("The registry hive '" + nameParts[0] + "' is not supported", "value");
			}

			_registrySubKeyName = String.Join("\\", nameParts, 1, nameParts.Length - 1);
		}

		public string[] GetValueNames()
		{
			return RegistrySubKey.GetValueNames();
		}

		public object GetValue(string name)
		{
			return RegistrySubKey.GetValue(name);
		}

        public object GetValue(string name, RegistryValueOptions options)
        {
            return RegistrySubKey.GetValue(name, null, options);
        }

		public void SetValue(string name, object value)
		{
			RegistrySubKey.SetValue(name, value);
		}

		public void SetValue(string name, object value, RegistryValueKind registryValueKind)
		{
			RegistrySubKey.SetValue(name, value, registryValueKind);
		}

		public void DeleteValue(string name)
		{
			RegistrySubKey.DeleteValue(name);
		}

		#endregion Constructors

		#region Instance Properties (Private)

		public RegistryKey RegistryHive
		{
			get { return _registryHive; }
		}

		public string SubKey
		{
			get { return _registrySubKeyName; }
		}

		public RegistryKey RegistrySubKey
		{
			get
			{
				if (_registrySubKey == null)
				{
					lock (this)
					{
                        if (_registrySubKey == null)
                        {
                            if (_createKeyIfNotFound)
                            {
                                _registrySubKey = RegistryHive.OpenOrCreateSubKey(SubKey);
                                if (_registrySubKey == null && !_silent)
                                    throw new ChoApplicationException("RegistryKey '{0}' can't be opened / created.".FormatString(_registryKey));
                            }
                            _registrySubKey = RegistryHive.OpenSubKey(SubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                            if (_registrySubKey == null && !_silent)
                                throw new ChoApplicationException("RegistryKey '{0}' not found".FormatString(_registryKey));
                        }

					}
				}
				return _registrySubKey;
			}
		}

        /// <summary>
        /// Set/Remove the running executable to run at system startup
        /// </summary>
        /// <param name="appName">A name to represent the application name in the registry</param>
        /// <param name="appLocation">Full path of the application to be set.</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public bool SetValue(string name, string value, bool remove)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name is missing.");

            if (!remove && String.IsNullOrEmpty(value))
                throw new ArgumentException("value is missing.");

            if (RegistrySubKey == null)
                return false;

            if (remove)
                RegistrySubKey.DeleteValue(name, false);
            else
                RegistrySubKey.SetValue(name, value);

            return true;
        }

		#endregion Instance Properties (Private)

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
