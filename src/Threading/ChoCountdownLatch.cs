namespace Cinchoo.Core.Threading
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
using System.Threading;

	#endregion NameSpaces

	public sealed class ChoCountdownLatch : ChoDisposableObject
	{
		#region Instance Data Members (Private)

		private int _remain;
		private EventWaitHandle _event;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoCountdownLatch(int count)
		{
			_remain = count;
			_event = new ManualResetEvent(false);
		}

		#endregion Constructors

		#region Instance Members (Public)

		public void Signal()
		{
			// The last thread to signal also sets the event.
			if (Interlocked.Decrement(ref _remain) == 0)
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

			return _event.WaitOne(millisecondsTimeout, exitContext);
		}

		public bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			ChoGuard.NotDisposed(this);

			return _event.WaitOne(timeout, exitContext);
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
