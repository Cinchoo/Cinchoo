namespace Cinchoo.Core.Factory
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	public static class ChoObjectFactory
	{
		public static T CreateInstance<T>() where T : class
		{
			return (T)ChoType.CreateInstanceWithReflectionPermission(typeof(T));
		}

		public static T CreateInstance<T>(params object[] args) where T : class
		{
			return (T)ChoType.CreateInstanceWithReflectionPermission(typeof(T), args);
		}
	}
}
