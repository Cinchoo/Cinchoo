namespace Cinchoo.Core.Pattern
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	public abstract class ChoSingletonObject : ContextBoundObject
	{
		#region Instance Data Members (Private)

		[ChoHiddenMember]
		private readonly ChoSingletonAttribute _singletonAttribute;

		#endregion Instance Data Members (Private)
	
		#region Constructors

		[ChoHiddenMember]
		public ChoSingletonObject()
		{
			_singletonAttribute = ChoType.GetAttribute(GetType(), typeof(ChoSingletonAttribute)) as ChoSingletonAttribute;
		}

		#endregion Constructors

		#region Instance Members (Public)

		[ChoHiddenMember]
		public void Log(string msg)
		{
			if (_singletonAttribute == null)
				return;
			_singletonAttribute.GetMe(GetType()).Log(msg);
		}

		[ChoHiddenMember]
		public void Log(bool condition, string msg)
		{
			if (_singletonAttribute == null)
				return;
			_singletonAttribute.GetMe(GetType()).Log(condition, msg);
		}

		#endregion Instance Members (Public)

		#region Object Overrides

		public override string ToString()
		{
			return ChoObject.ToString(this);
		}

		#endregion Object Overrides
	}
}
