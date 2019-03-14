#region NameSpaces

using System;
using Cinchoo.Core.APM;

#endregion NameSpaces

namespace Cinchoo.Core.Services
{    
    interface IChoExecutionService
    {
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue(Action func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
        Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T>(Action<T> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

        Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);

		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout, int maxNoOfRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);

		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout, int maxNoOfRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		//Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<TResult>(Func<TResult> func, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
        Cinchoo.Core.APM.IChoAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, Cinchoo.Core.APM.ChoAsyncCallback callback, object state);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry);
		Cinchoo.Core.APM.IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, Cinchoo.Core.APM.ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);
        
        void Start();
        void Stop();

		string Name { get; }
	}
}
