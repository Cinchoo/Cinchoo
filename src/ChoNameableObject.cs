namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
using System.Diagnostics;

	#endregion NameSpaces

	[DebuggerDisplay("Name={Name}")]
	public class ChoNameableObject : ChoFormattableObject
	{
		#region Instance Properties (Public)

		public string Name
		{
			get;
			private set;
		}

		#endregion Instance Properties (Public)

		#region Constructors

		public ChoNameableObject() : this(String.Format("ChoNamedObject_{0}", ChoRandom.NextRandom()))
		{
		}

		public ChoNameableObject(string name)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
			Name = name;
		}

		public ChoNameableObject(Type type)
		{
			ChoGuard.ArgumentNotNullOrEmpty(type, "Type");
			Name = type.FullName;
		}

		#endregion Constructors
	}
}
