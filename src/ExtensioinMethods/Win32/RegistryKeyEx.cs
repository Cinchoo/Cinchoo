namespace Microsoft.Win32
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
using Microsoft.Win32;

	#endregion NameSpaces

	public static class RegistryKeyEx
	{
		public static RegistryKey OpenOrCreateSubKey(this RegistryKey registryKey, string subKey)
		{
			if (registryKey == null)
				throw new ArgumentException("Missing registrykey.");

			RegistryKey registrySubKey = registryKey.OpenSubKey(subKey, true);
			if (registrySubKey == null)
				registryKey.CreateSubKey(subKey);

			return registryKey.OpenSubKey(subKey, true);
		}
	}
}
