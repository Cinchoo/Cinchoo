namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
using Microsoft.Win32;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
	public class ChoMemberRegistryInfoAttribute : Attribute
	{
		#region Instance Properties

		public RegistryValueKind RegistryValueKind
		{
			get;
			private set;
		}

		#endregion Instance Properties

		#region Constructors

		public ChoMemberRegistryInfoAttribute(RegistryValueKind registryValueKind)
		{
			RegistryValueKind = registryValueKind;
		}

		#endregion Constructors
	}
}
