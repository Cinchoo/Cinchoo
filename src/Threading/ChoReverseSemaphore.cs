namespace Cinchoo.Core.Threading
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	#endregion NameSpaces

	public sealed class ChoReverseSemaphore : ChoDisposableObject
	{
		#region Instance Data Members (Private)

		private int _count;
		private readonly int _maxCount;
		private AutoResetEvent _event = new AutoResetEvent(false);

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoReverseSemaphore(int count)
		{
			_maxCount = count;
		}

		#endregion Constructors

		#region Instance Members (Public)

		public void Release()
		{
			ChoGuard.NotDisposed(this);

			if (Interlocked.Decrement(ref _count) == _maxCount - 1)
				_event.Set();
		}

		public bool WaitOne()
		{
			return WaitOne(Timeout.Infinite);
		}

		public bool WaitOne(int milliSecondsInTimeout)
		{
			return WaitOne(milliSecondsInTimeout, false);
		}

		public bool WaitOne(TimeSpan timeout)
		{
			return WaitOne(timeout, false);
		}

		public bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			ChoGuard.NotDisposed(this);

			if (Interlocked.Increment(ref _count) == _maxCount)
				return _event.WaitOne(millisecondsTimeout, exitContext);

			return true;
		}

		public bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			ChoGuard.NotDisposed(this);

			if (Interlocked.Increment(ref _count) == _maxCount)
				return _event.WaitOne(timeout, exitContext);

			return true;
		}

		#endregion Instance Members (Public)

		protected override void Dispose(bool finalize)
		{
			if (_event != null)
			{
				_event.Close();
				_event = null;
			}
		}
	}
}
