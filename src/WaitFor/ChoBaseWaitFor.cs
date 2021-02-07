namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Cinchoo.Core.Collections.Generic;

	#endregion NameSpaces

	public abstract class ChoBaseWaitFor : IDisposable
	{
		#region Instance Data Members (Protected)

		protected readonly string FuncSignature;

		#endregion Instance Data Members (Protected)

		#region Constructors

        public ChoBaseWaitFor(Delegate func)
		{
			ChoGuard.ArgumentNotNull(func, "Function");

			FuncSignature = ChoDelegateEx.ToString(func);
		}

		#endregion Constructors

		#region Instance Members (Protected)

		protected void CheckParams(int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			if (timeout != Timeout.Infinite && timeout < 0)
				throw new ArgumentOutOfRangeException("Timeout");

			if (maxNoOfRetry < 0)
				throw new ArgumentNullException("MaxNoOfRetry should be >= 0.");

            if (maxNoOfRetry > 0 && sleepBetweenRetry <= 0)
				throw new ArgumentOutOfRangeException("SleepBetweenRetry should be > 0");
		}

		protected int HandleException(int maxNoOfRetry, int sleepBetweenRetry, int noOfRetry, ChoList<Exception> aggExceptions, Exception ex)
		{
			if (noOfRetry == maxNoOfRetry)
				throw new ChoAggregateException(String.Format("The method failed to execute with {0} of retries.", maxNoOfRetry), aggExceptions);

			noOfRetry++;
			aggExceptions.Add(ex);

			Thread.Sleep((int)sleepBetweenRetry);
			return noOfRetry;
		}

		#endregion Instance Members (Protected)

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
