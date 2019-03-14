namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	public interface IChoExceptionHandledObject
	{
		bool HandleException(Exception ex, ref bool isDirty);
	}
}
