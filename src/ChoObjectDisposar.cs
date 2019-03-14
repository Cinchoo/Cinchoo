namespace Cinchoo.Core
{
	#region NameSpaces

    using System;
    using System.Diagnostics;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    public static class ChoObjectDisposar
	{
		#region Shared Members (Public)

		public static void Dispose(IDisposable target, bool finalize)
		{
			Dispose(target, finalize, null);
		}

		private static void Dispose(IDisposable target, bool finalize, Action<bool> disposeMethod)
		{
			ChoGuard.ArgumentNotNull(target, "Target");

			if (target is IChoSyncDisposable)
			{
				IChoSyncDisposable disposableObj = target as IChoSyncDisposable;
				if (disposableObj.DisposableLockObj != null)
				{
					lock (disposableObj.DisposableLockObj)
					{
						CheckNDisposeObject(finalize, disposableObj, disposeMethod);
					}
				}
				else
					CheckNDisposeObject(finalize, disposableObj, disposeMethod);
			}
			else if (target is IChoDisposable)
				CheckNDisposeObject(finalize, target as IChoDisposable, disposeMethod);
			else
				target.Dispose();
		}

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		[ChoStreamProfile("Following are the objects failed to dispose...", NameFromTypeFullName = typeof(ChoObjectDisposar), StartActions = "Roll")]
		private static void ReportMemoryLeak(IChoDisposable target)
		{
            return;
			ChoGuard.ArgumentNotNull(target, "Target");

			if (target.IsDisposed)
				return;

			ChoDisposableObjectAttribute disposableObjectAttribute = ChoType.GetAttribute<ChoDisposableObjectAttribute>(target.GetType());
			if (disposableObjectAttribute == null || !disposableObjectAttribute.ContainsUnmanagedResources)
				return;

			AppDomain currentDomain = AppDomain.CurrentDomain;
			if (!currentDomain.IsFinalizingForUnload() &&
			   !Environment.HasShutdownStarted)
			{
				ChoStringMsgBuilder msg = new ChoStringMsgBuilder(target.GetType().FullName);

				//Console.WriteLine("Object allocated at:");
				for (int index = 0; index < target.ObjectCreationStackTrace.FrameCount; ++index)
				{
					StackFrame frame = target.ObjectCreationStackTrace.GetFrame(index);
					msg.AppendLine(" {0}", frame.ToString());
				}

                //ChoProfile.DefaultContext.AppendLine(msg.ToString());
			}
		}

		private static void CheckNDisposeObject(bool finalize, IChoDisposable disposableObj, Action<bool> disposeMethod)
		{
			if (disposableObj.IsDisposed)
				return;

            bool isDisposeSuccess = true;
			if (disposeMethod == null)
			{
                disposableObj.Dispose(finalize);
                //if (ChoType.HasMethod(disposableObj, "Dispose", new object[] { finalize }))
                //    ChoType.InvokeMethod(disposableObj, "Dispose", new object[] { finalize });
                //else
                //    isDisposeSuccess = false;
			}
			else
				disposeMethod(finalize);

            if (!finalize && isDisposeSuccess)
				GC.SuppressFinalize(disposableObj);
			else
			{
				AppDomain currentDomain = AppDomain.CurrentDomain;
				if (!currentDomain.IsFinalizingForUnload() && !Environment.HasShutdownStarted)
					ReportMemoryLeak(disposableObj);
			}
		}

		#endregion Shared Members (Private)
	}
}
