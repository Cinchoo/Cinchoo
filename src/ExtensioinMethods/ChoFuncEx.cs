using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public static class ChoFuncEx
    {
        public static TResult InvokeWithContext<TResult>(this Func<TResult> function)
        {
            Contract.Requires<ArgumentNullException>(function != null);

            ExecutionContext executionContext = ExecutionContext.Capture();
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;

            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            try
            {
                if (synchronizationContext == null)
                {
                    TResult result = function.InvokeWith(executionContext);
                    taskCompletionSource.SetResult(result);
                }
                else
                {
                    // See: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create()
                    synchronizationContext.OperationStarted();
                    // See: System.Threading.Tasks.SynchronizationContextAwaitTaskContinuation.PostAction()
                    synchronizationContext.Post(_ =>
                    {
                        try
                        {
                            TResult result = function.InvokeWith(executionContext);
                            // See: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.NotifySynchronizationContextOfCompletion()
                            synchronizationContext.OperationCompleted();
                            taskCompletionSource.SetResult(result);
                        }
                        catch (Exception exception)
                        {
                            taskCompletionSource.SetException(exception);
                        }
                    }, null);
                }
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }

            taskCompletionSource.Task.Wait();
            return taskCompletionSource.Task.Result;
        }

        public static void InvokeWithContext(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            InvokeWithContext(new Func<object>(() =>
            {
                action();
                return null;
            }));
        }

        public static Task<TResult> InvokeWith<TResult>(this Func<TResult> function, SynchronizationContext synchronizationContext, ExecutionContext executionContext)
        {
            Contract.Requires<ArgumentNullException>(function != null);

            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            try
            {
                if (synchronizationContext == null)
                {
                    TResult result = function.InvokeWith(executionContext);
                    taskCompletionSource.SetResult(result);
                }
                else
                {
                    // See: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create()
                    synchronizationContext.OperationStarted();
                    // See: System.Threading.Tasks.SynchronizationContextAwaitTaskContinuation.PostAction()
                    synchronizationContext.Post(_ =>
                    {
                        try
                        {
                            TResult result = function.InvokeWith(executionContext);
                            // See: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.NotifySynchronizationContextOfCompletion()
                            synchronizationContext.OperationCompleted();
                            taskCompletionSource.SetResult(result);
                        }
                        catch (Exception exception)
                        {
                            taskCompletionSource.SetException(exception);
                        }
                    }, null);
                }
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }

            return taskCompletionSource.Task;
        }

        private static TResult InvokeWith<TResult>(this Func<TResult> function, ExecutionContext executionContext)
        {
            Contract.Requires<ArgumentNullException>(function != null);

            if (executionContext == null)
            {
                return function();
            }

            TResult result = default(TResult);
            // See: System.Runtime.CompilerServices.AsyncMethodBuilderCore.MoveNextRunner.Run()
            ExecutionContext.Run(executionContext, _ => result = function(), null);
            return result;
        }

        public static Task InvokeWith(this Action action, SynchronizationContext synchronizationContext, ExecutionContext executionContext)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return new Func<object>(() =>
            {
                action();
                return null;
            }).InvokeWith(synchronizationContext, executionContext);
        }
    }
}
