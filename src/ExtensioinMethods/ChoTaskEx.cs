using Cinchoo.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cinchoo.Core
{
    public static class ChoTaskEx
    {
        public static Task<TNewResult> ContinueWithContext<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, TNewResult> continuation)
        {
            Contract.Requires<ArgumentNullException>(task != null);
            Contract.Requires<ArgumentNullException>(continuation != null);

            // See: System.Runtime.CompilerServices.AsyncMethodBuilderCore.GetCompletionAction()
            ExecutionContext executionContext = ExecutionContext.Capture();
            // See: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create()
            // See: System.Runtime.CompilerServices.AsyncMethodBuilderCore.MoveNextRunner.Run()
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;
            return task.ContinueWith(t =>
                    new Func<TNewResult>(() => continuation(t)).InvokeWith(synchronizationContext, executionContext))
                .Unwrap();
        }

        public static Task<TNewResult> ContinueWithContext<TNewResult>(this Task task, Func<Task, TNewResult> continuation)
        {
            Contract.Requires<ArgumentNullException>(task != null);
            Contract.Requires<ArgumentNullException>(continuation != null);

            // See: System.Runtime.CompilerServices.AsyncMethodBuilderCore.GetCompletionAction()
            ExecutionContext executionContext = ExecutionContext.Capture();
            // See: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create()
            // See: System.Runtime.CompilerServices.AsyncMethodBuilderCore.MoveNextRunner.Run()
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;
            return task.ContinueWith(t =>
                    new Func<TNewResult>(() => continuation(t)).InvokeWith(synchronizationContext, executionContext))
                .Unwrap();
        }

        public static Task ContinueWithContext<TResult>(this Task<TResult> task, Action<Task<TResult>> continuation)
        {
            Contract.Requires<ArgumentNullException>(task != null);
            Contract.Requires<ArgumentNullException>(continuation != null);

            return task.ContinueWithContext(new Func<Task<TResult>, object>(t =>
            {
                continuation(t);
                return null;
            }));
        }

        public static Task ContinueWithContext(this Task task, Action<Task> continuation)
        {
            Contract.Requires<ArgumentNullException>(task != null);
            Contract.Requires<ArgumentNullException>(continuation != null);

            return task.ContinueWithContext(new Func<Task, object>(t =>
            {
                continuation(t);
                return null;
            }));
        }

        public static void LogExceptions(this Task task)
        {
            task.ContinueWith(t =>
            {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                    ChoTrace.Error(exception);
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
