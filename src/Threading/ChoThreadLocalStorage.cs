namespace Cinchoo.Core.Threading
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading;
	using Cinchoo.Core.Shell;

	#endregion NameSpaces

	internal class ChoTLSState
	{
		private readonly object _padLock = new object();
		private object _target;

		public object Target
		{
			get { lock (_padLock) { return _target; } }
			internal set { lock (_padLock) { _target = value; } }
		}
	}

	public static class ChoThreadLocalStorage
	{
		private readonly static object _padLock = new object();

		[ThreadStatic]
		private static volatile object _target;

		public static object Target
		{
			get { return _target; }
		}

		public static void Register(object target)
		{
			_target = target;
		}
	}
}
