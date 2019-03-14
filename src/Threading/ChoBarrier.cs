namespace Cinchoo.Core.Threading
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	#endregion NameSpaces

    public class ChoBarrier : ChoSyncDisposableObject
	{
		#region Instance Data Members (Private)

		protected Semaphore Semaphore;
		protected ChoReverseSemaphore RevSemaphore;

		#endregion Instance Data Members (Private)

		#region Constructors

		protected ChoBarrier(bool dummy)
		{
		}

		public ChoBarrier(int minimumCount, int maximumCount)
		{
			if (minimumCount < 0)
				throw new ArgumentException("MinimumCount must be >= 0.");
			if (maximumCount < 1)
				throw new ArgumentException("MaximumCount must be > 1.");
			if (minimumCount >= maximumCount)
				throw new ArgumentException("MinimumCount must be > MaximumCount.");

			Semaphore = new Semaphore(minimumCount, Int32.MaxValue);
			RevSemaphore = new ChoReverseSemaphore(maximumCount);
		}

		#endregion Constructors

		#region Instance Members (Public)

		public bool Wait()
		{
			return Wait(Timeout.Infinite);
		}

		public virtual bool Wait(int millisecondsTimeout)
		{
			throw new NotSupportedException();
		}

		#endregion Instance Members (Public)

		#region ChoSyncDisposableObject Overrides

		protected override void Dispose(bool finalize)
		{
			if (Semaphore != null)
			{
				Semaphore.Close();
				Semaphore = null;
			}
			if (RevSemaphore != null)
			{
				RevSemaphore.Close();
				RevSemaphore = null;
			}
		}

		#endregion ChoSyncDisposableObject Overrides

		#region Shared Members (Public)

		public static ChoBarrier LowBarrier(ChoBarrier barrier)
		{
			return new ChoLowBarrier(barrier);
		}

		public static ChoBarrier HighBarrier(ChoBarrier barrier)
		{
			return new ChoHighBarrier(barrier);
		}

		#endregion Shared Members (Public)

		#region ChoLowBarrier Class

		private class ChoLowBarrier : ChoBarrier
		{
			#region Instance Data Members (Private)

			private ChoBarrier _barrier;
			private bool _waitSuccess = false;

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoLowBarrier(ChoBarrier barrier) : base(false)
			{
				_barrier = barrier;
			}

			#endregion Constructors

			public override bool Wait(int millisecondsTimeout)
			{
				ChoGuard.NotDisposed(this);
				ChoGuard.NotDisposed(_barrier);

				bool retVal = _barrier.Semaphore.WaitOne(millisecondsTimeout);
				_waitSuccess = true;

				return retVal;
			}

			protected override void Dispose(bool finalize)
			{
				if (_waitSuccess)
					_barrier.RevSemaphore.Release();
			}
		}

		#endregion ChoLowBarrier Class

		#region ChoHighBarrier Class

		private class ChoHighBarrier : ChoBarrier
		{
			#region Instance Data Members (Private)

			private ChoBarrier _barrier;
			private bool _waitSuccess = false;

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoHighBarrier(ChoBarrier barrier)
				: base(false)
			{
				_barrier = barrier;
			}

			#endregion Constructors

			public override bool Wait(int millisecondsTimeout)
			{
				ChoGuard.NotDisposed(this);
				ChoGuard.NotDisposed(_barrier);

				bool retVal = _barrier.RevSemaphore.WaitOne(millisecondsTimeout);
				_waitSuccess = true;

				return retVal;
			}

			protected override void Dispose(bool finalize)
			{
				if (_waitSuccess)
					_barrier.Semaphore.Release();
			}
		}

		#endregion ChoHighBarrier Class
	}
}
