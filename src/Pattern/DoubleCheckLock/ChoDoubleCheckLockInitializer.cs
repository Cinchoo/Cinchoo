namespace Cinchoo.Core.Pattern
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	#endregion NameSpaces

	public interface IChoDoubleCheckLockObject<T>
	{
		object DoublCheckLockObject
		{
			get;
		}

		bool CanActionBePerformed(T key);
		void DoAction(T key, params object[] states);
	}

	public abstract class ChoDoubleCheckLockInitializer<T>
	{
		#region Instance Data Members (Private)

		private readonly object _dblCheckLock = new object();

		#endregion Instance Data Members (Private)

		#region Instance Members (Abstract)

		protected abstract bool CanActionBePerformed(T state);
		protected abstract void DoAction(T state, params object[] states);

		public void PerformAction(T key, params object[] states)
		{
			if (CanActionBePerformed(key))
				return;

			lock (_dblCheckLock)
			{
				if (CanActionBePerformed(key))
					return;

				DoAction(key, states);
			}
		}

		#endregion Instance Members (Abstract)

		#region Shared Members (Public)

		public static void PerformAction(IChoDoubleCheckLockObject<T> target, T key, params object[] states)
		{
			ChoGuard.ArgumentNotNull(target, "target");

			if (target.CanActionBePerformed(key))
				return;

			lock (target.DoublCheckLockObject)
			{
				if (target.CanActionBePerformed(key))
					return;

				target.DoAction(key, states);
			}
		}

		#endregion Shared Members (Public)

	}
}
