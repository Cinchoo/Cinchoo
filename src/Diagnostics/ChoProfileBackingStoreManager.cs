namespace Cinchoo.Core.Diagnostics
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Diagnostics;
	using Cinchoo.Core.Factory;
	using System.Xml;
	using System.Xml.Serialization;
	using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	public class ChoProfileBackStoreConfigSettings : ChoConfigurableObject
	{
		[XmlAttribute("name")]
		public string Name;
	}

	internal static class ChoProfileBackingStoreConfigManager
	{
		public static XmlNode Load(string name, ref string startActions, ref string stopActions)
		{
			return null;
		}
	}

	public static class ChoProfileBackingStoreManager
	{
		#region Shared Data Members (Private)

		private static readonly ChoNameableObject _padLock = new ChoNameableObject(typeof(ChoProfileBackingStoreManager));
		private static readonly Dictionary<string, IChoProfileBackingStore> _profileBackingStoreCache = new Dictionary<string, IChoProfileBackingStore>();
		private static readonly Lazy<IChoProfileBackingStore> _defaultProfileBackingStore = new Lazy<IChoProfileBackingStore>(() =>
		{
			IChoProfileBackingStore defaultProfileBackingStore = new ChoConsoleProfileBackingStore();
			defaultProfileBackingStore.Start();

			return defaultProfileBackingStore;
		});

		#endregion Shared Data Members (Private)

		#region Shared Members (Public)

		public static IChoProfileBackingStore GetProfileBackingStore(string name, string startActions, string stopActions)
		{
			if (_profileBackingStoreCache.ContainsKey(name))
				return _profileBackingStoreCache[name];

			lock (_padLock)
			{
				if (!_profileBackingStoreCache.ContainsKey(name))
				{
					try
					{
						XmlNode configNode = ChoProfileBackingStoreConfigManager.Load(name, ref startActions, ref stopActions);
						IChoProfileBackingStore profileBackingStore = null; //ChoObjectFactory.CreateInstance<IChoProfileBackingStore>(new object[] { name, startActions, stopActions, configNode });
						if (profileBackingStore == null)
						{
							Trace.TraceInformation("Failed to create '{0}' profile backingstore, using default one.", name);
							profileBackingStore = _defaultProfileBackingStore.Value;
						}

						_profileBackingStoreCache.Add(name, profileBackingStore);
					}
					catch (Exception ex)
					{
						Trace.TraceError("Failed to create '{0}' profile backingstore, using default one. {2}{1}", name, ex.ToString(), Environment.NewLine);
						_profileBackingStoreCache.Add(name, _defaultProfileBackingStore.Value);
					}
				}
			}

			return _profileBackingStoreCache[name];
		}

		#endregion Shared Members (Public)
	}
}
