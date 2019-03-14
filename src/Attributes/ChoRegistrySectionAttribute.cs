namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;

	#endregion NameSpaces

	public class ChoRegistrySectionAttribute1 : Attribute
	{
		#region Instance Data Members (Private)

		private string _registryKey;

		#endregion Instance Data Members (Private)

		#region Instance Properties (Public)

		public string RegistryKey
		{
			get { return _registryKey; }
		}

		#endregion Instance Properties (Private)

		#region Constructors

		public ChoRegistrySectionAttribute1(string registryKey)
		{
			ChoGuard.ArgumentNotNullOrEmpty(registryKey, "RegistryKey");

			_registryKey = registryKey;
		}

		#endregion Constructors

		#region Shared Members (Public)

		public static Type GetType(string registryKey, out ChoRegistrySectionAttribute1 registrySectionAttribute)
		{
			registrySectionAttribute = null;

			Type[] types = ChoType.GetTypes(typeof(ChoRegistrySectionAttribute1));
			if (types == null || types.Length == 0)
				return null;

			foreach (Type type in types)
			{
				if (type == null)
					continue;

				ChoRegistrySectionAttribute1 lRegistrySectionAttribute = ChoType.GetAttribute(type, typeof(ChoRegistrySectionAttribute1)) as ChoRegistrySectionAttribute1;
				if (lRegistrySectionAttribute == null)
					continue;

				if (String.Compare(lRegistrySectionAttribute.RegistryKey, registryKey, true) == 0)
				{
					registrySectionAttribute = lRegistrySectionAttribute;
					return type;
				}
			}

			return null;
		}

		#endregion Shared Members (Public)
	}
}
