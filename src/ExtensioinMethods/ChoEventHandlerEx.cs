namespace System
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
using System.Reflection;
	using System.Diagnostics;

	#endregion NameSpaces

	public static class EventHandlerEx
	{
		public static void Raise(this EventHandler eventHandler, object sender, EventArgs e)
		{
			EventHandler lEventHandler = eventHandler;
			if (lEventHandler != null)
				lEventHandler(sender, e);
		}

		public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
		{
			EventHandler<T> lEventHandler = eventHandler;
			if (lEventHandler != null)
				lEventHandler(sender, e);
		}

		//public static void LoadHandlers<T>(this EventHandler<T> eventHandler, MethodInfo[] methodInfos, object sender) where T : EventArgs
		//{
		//    foreach (MethodInfo methodInfo in methodInfos)
		//    {
		//        try
		//        {
		//            if (methodInfo.IsStatic)
		//                eventHandler += methodInfo.CreateDelegate<EventHandler<T>>();
		//            else
		//                eventHandler += methodInfo.CreateDelegate<EventHandler<T>>(sender);
		//        }
		//        catch (Exception ex)
		//        {
		//            Trace.TraceError(ex.ToString());
		//        }
		//    }
		//}

		public static void LoadHandlers<T>(ref EventHandler<T> eventHandler, MethodInfo[] methodInfos, object sender) where T : EventArgs
		{
			foreach (MethodInfo methodInfo in methodInfos)
			{
				try
				{
					if (methodInfo.IsStatic)
						eventHandler += methodInfo.CreateDelegate<EventHandler<T>>();
					else
						eventHandler += methodInfo.CreateDelegate<EventHandler<T>>(sender);
				}
				catch (Exception ex)
				{
					Trace.TraceError(ex.ToString());
				}
			}
		}
    }
}
