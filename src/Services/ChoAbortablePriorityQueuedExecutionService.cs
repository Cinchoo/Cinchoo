namespace Cinchoo.Core.Services
{
	#region NameSpaces

	using System;
	using System.Threading;
	
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Collections.Generic;

	#endregion NameSpaces

	[ChoAppDomainEventsRegisterableType]
	[ChoBufferProfile(ChoString.Empty, NameFromTypeFullName = typeof(ChoAbortablePriorityQueuedExecutionService))]
	public class ChoAbortablePriorityQueuedExecutionService : ChoAbortableExecutionServiceBase
	{
		#region Global QueuedExecutionService

		private static ChoAbortablePriorityQueuedExecutionService _globalAbortableQueuedExecutionService = new ChoAbortablePriorityQueuedExecutionService(ChoGlobalApplicationSettings.Me.ApplicationName, true);

		#endregion Global QueuedExecutionService

		#region Instance Data Members (Private)

		private readonly ChoPriorityQueuedMsgService<ChoExecutionServiceData> _queuedMsgService;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoAbortablePriorityQueuedExecutionService(string name)
			: this(name, true)
		{
		}

		public ChoAbortablePriorityQueuedExecutionService(string name, bool autoStart)
			: base(name)
		{
			_queuedMsgService = new ChoPriorityQueuedMsgService<ChoExecutionServiceData>("{0}_{1}".FormatString(name, typeof(ChoAbortablePriorityQueuedExecutionService).Name), 
				ChoStandardQueuedMsgObject<ChoExecutionServiceData>.QuitMsg, autoStart, true, QueueMessageHandler);
		}

		#endregion Constructors

		#region Enqueue Overloads

		public override IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			ChoGuard.ArgumentNotNull(func, "Function");
			CheckTimeoutArg(timeout);

			ChoExecutionServiceData data = new ChoExecutionServiceData(func, parameters, timeout, new ChoAbortableAsyncResult(callback, state), maxNoOfRetry, sleepBetweenRetry);
			_queuedMsgService.Enqueue(0, ChoStandardQueuedMsgObject<ChoExecutionServiceData>.New(data));
             
			return data.Result as IChoAbortableAsyncResult;
		}

		#endregion Enqueue Overloads

		#region Instance Members (Public)

		public override void Start()
		{
			ChoGuard.NotDisposed(this);
			_queuedMsgService.Start();
		}

		public override void Stop()
		{
			ChoGuard.NotDisposed(this);
			//Console.WriteLine(String.Format("Stopping {0} asynchronous execution service...", Name));
			_queuedMsgService.Dispose();
		}

		#endregion Instance Members (Public)

		#region Shared Members (Private)

		private void QueueMessageHandler(IChoQueuedMsgServiceObject<ChoExecutionServiceData> msgObject)
		{
			if (msgObject == null || !ChoGuard.IsArgumentNotNullOrEmpty(msgObject.State))
				return;

			ChoAbortableAsyncResult asyncResult = msgObject.State.Result as ChoAbortableAsyncResult;
			if (msgObject.State.Result != null)
			{
				if (msgObject.State.Result is ChoAbortableAsyncResult && ((ChoAbortableAsyncResult)msgObject.State.Result).AbortRequested)
				{
					asyncResult.SetAsAborted(true);
					return;
				}
			}
			WaitFor(msgObject.State.Func, msgObject.State.Parameters, msgObject.State.Timeout, msgObject.State.MaxNoOfRetry, msgObject.State.SleepBetweenRetry, msgObject.State);
		}

		private void WaitFor(Delegate func, object[] parameters, int timeout, int maxNoOfRetry, int sleepBetweenRetry, ChoExecutionServiceData data)
		{
			ChoGuard.ArgumentNotNull(func, "Function");

			ChoAbortableAsyncResult asyncResult = data.Result as ChoAbortableAsyncResult;

			ChoWaitFor.CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			object retValue = null;
			ChoList<Exception> aggExceptions = new ChoList<Exception>();
			Func<object> wrappedFunc = delegate
			{
				if (asyncResult.AbortRequested)
					asyncResult.SetAsAborted(true);
				
				asyncResult.ThreadToKill = Thread.CurrentThread;
				return func.DynamicInvoke(parameters);
			};

			while (true)
			{
				if (asyncResult.AbortRequested)
					asyncResult.SetAsAborted(true);

				try
				{
					if (timeout == Timeout.Infinite)
						retValue = wrappedFunc.Invoke();
					else
					{
						IAsyncResult result = wrappedFunc.BeginInvoke(null, null);
						Thread.Sleep(1000);
						using (result.AsyncWaitHandle)
						{
							if (!result.AsyncWaitHandle.WaitOne(timeout, true))
							{
								if (asyncResult.ThreadToKill != null && asyncResult.ThreadToKill.IsAlive)
								{
									asyncResult.ThreadToKill.AbortThread();
								}

								ChoWaitFor.RaiseTimeoutException(func, timeout);
							}
							else
							{
								try
								{
									retValue = wrappedFunc.EndInvoke(result);
								}
								catch (ThreadAbortException)
								{
									//Thread.ResetAbort();
									asyncResult.SetAsAborted(true);
									return;
								}
							}
						}
					}
					asyncResult.SetAsSuccess(retValue, true);
					return;
				}
				catch (ThreadAbortException)
				{
					Thread.ResetAbort();
					ChoProfile.WriteLine("Thread Aborted.");
					ChoProfile.WriteLine(data.ToString());
					asyncResult.SetAsAborted(true);
					break;
				}
				catch (ChoFatalApplicationException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (maxNoOfRetry != 0)
					{
						if (noOfRetry == maxNoOfRetry)
						{
							asyncResult.SetAsFailed(new ChoAggregateException(String.Format("The method failed to execute after {0} retries.", maxNoOfRetry), aggExceptions), true);
							return;
						}

						noOfRetry++;
						aggExceptions.Add(ex);

						Thread.Sleep((int)sleepBetweenRetry);
					}
					else
					{
						ChoProfile.WriteLine(ex.ToString());
						ChoProfile.WriteLine(data.ToString());
						asyncResult.SetAsFailed(ex.InnerException != null ? ex.InnerException : ex, true);
						return;
					}
				}
			}

			//if (asyncResult.AbortRequested)
			//    asyncResult.SetAsAborted(true);

			//Action wrappedFunc1 = delegate
			//{
			//    asyncResult.ThreadToKill = Thread.CurrentThread;

			//    while (true)
			//    {
			//        if (asyncResult.AbortRequested)
			//        {
			//            asyncResult.SetAsAborted(true);
			//            break;
			//        }

			//        try
			//        {
			//            if (timeout == Timeout.Infinite)
			//                retValue = func.DynamicInvoke(parameters);
			//            else
			//            {
			//                IAsyncResult result = wrappedFunc.BeginInvoke(null, null);
			//                Thread.Sleep(1000);
			//                using (result.AsyncWaitHandle)
			//                {
			//                    if (!result.AsyncWaitHandle.WaitOne(timeout, true))
			//                    {
			//                        try
			//                        {
			//                            if (asyncResult.ThreadToKill != null && asyncResult.ThreadToKill.IsAlive)
			//                                asyncResult.ThreadToKill.Abort();
			//                        }
			//                        catch (ThreadAbortException)
			//                        {
			//                            Thread.ResetAbort();
			//                        }

			//                        ChoWaitFor.RaiseTimeoutException(func, timeout);
			//                    }
			//                    else
			//                        retValue = wrappedFunc.EndInvoke(result);
			//                }
			//            }
			//            asyncResult.SetAsSuccess(retValue, true);
			//            return;
			//        }
			//        catch (ThreadAbortException)
			//        {
			//            Thread.ResetAbort();
			//            ChoProfile.WriteLine("Thread Aborted.");
			//            ChoProfile.WriteLine(data.ToString());
			//            asyncResult.SetAsAborted(true);
			//            break;
			//        }
			//        catch (Exception ex)
			//        {
			//            if (maxNoOfRetry != 0)
			//            {
			//                if (noOfRetry == maxNoOfRetry)
			//                {
			//                    asyncResult.SetAsFailed(new ChoAggregateException(String.Format("The method failed to execute after {0} retries.", maxNoOfRetry), aggExceptions), true);
			//                    return;
			//                }

			//                noOfRetry++;
			//                aggExceptions.Add(ex);

			//                Thread.Sleep((int)sleepBetweenRetry);
			//            }
			//            else
			//            {
			//                ChoProfile.WriteLine(ex.ToString());
			//                ChoProfile.WriteLine(data.ToString());
			//                asyncResult.SetAsFailed(ex.InnerException != null ? ex.InnerException : ex, true);
			//                return;
			//            }
			//        }
			//    }
			//};

			//IAsyncResult r = wrappedFunc1.BeginInvoke(null, null);
			//wrappedFunc1.EndInvoke(r);
		}
		private static void CheckTimeoutArg(int timeout)
		{
			if (timeout != -1 && timeout < 0)
				throw new ArgumentOutOfRangeException("Timeout");
		}

		#endregion Shared Members (Private)

		#region ChoSyncDisposableObject Overrides

		protected override void Dispose(bool finalize)
		{
			try
			{
				Stop();
			}
			finally
			{
				IsDisposed = true;
			}
		}

		#endregion ChoSyncDisposableObject Overrides

		#region Shared Properties (Public)

		public static ChoAbortablePriorityQueuedExecutionService Global
		{
			get { return _globalAbortableQueuedExecutionService; }
		}

		#endregion Shared Properties (Public)

		[ChoAppDomainUnloadMethod("Stopping global abortable queued execution service...")]
		private static void StopGlobalQueuedExecutionService()
		{
			if (_globalAbortableQueuedExecutionService != null)
			{
				_globalAbortableQueuedExecutionService.Dispose();
				_globalAbortableQueuedExecutionService = null;
			}
		}
	}
}
