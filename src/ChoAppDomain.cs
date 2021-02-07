namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Threading;
	using Cinchoo.Core.Collections.Generic;
	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Services;
	using Cinchoo.Core.ServiceProcess;
	using Cinchoo.Core.Shell;
    using Cinchoo.Core.Reflection;

	#endregion NameSpaces

	[ChoAppDomainEventsRegisterableType]
	public static class ChoAppDomain
	{
		#region Shared Data Members (Private)

		private static ChoDictionary<Type, MethodInfo[]> _onDomainLoadHandlers = ChoDictionary<Type, MethodInfo[]>.Synchronized(new ChoDictionary<Type, MethodInfo[]>());
		private static ChoDictionary<Type, MethodInfo[]> _onDomainUnloadHandlers = ChoDictionary<Type, MethodInfo[]>.Synchronized(new ChoDictionary<Type, MethodInfo[]>());

		#endregion Shared Data Members (Private)

		#region Constructors

		static ChoAppDomain()
		{
			//ChoApplication.Initialize();
            //ChoApplication.RaiseApplyFrxParamsOverrides(true);
            ChoAssembly.Initialize();
            RegisterAppDomainEvents();
            string envPath = ChoEnvironmentSettings.GetConfigFilePath();
			ChoGlobalApplicationSettings x = ChoGlobalApplicationSettings.Me;
			ChoApplication.Refresh();
            ChoTypesManager.Initialize();
			ChoAbortableQueuedExecutionService asyncExecutionService = ChoAbortableQueuedExecutionService.Global;
			_Load();
		}

		#endregion Constructors

		#region Shared Members (Public)

		public static void Refresh()
		{
			//ChoEnvironmentSettings.Refresh();
		}

		[ChoAppDomainLoadMethod("AppDomain Initialize....")]
		internal static void Initialize()
		{
			//ChoConsole.Initialize();
			//ChoCodeBase codeBase = ChoCodeBase.Me;
		}

		private readonly static object _padLock = new object();
		private static bool _shutdownCompleted = false;

		internal static void Exit()
		{
			if (_shutdownCompleted)
				return;

            if (!Monitor.TryEnter(_padLock, 1000))
                return;

            try
            {
                if (_shutdownCompleted)
                    return;

                //ChoQueuedExecutionService.Global.Enqueue(() =>
                //    {
                OnDomainUnload(null, null);
                //});
            }
            finally
            {
                Monitor.Exit(_padLock);
            }
		}

		public static void UnregisterMe(object target)
		{
			ChoGuard.ArgumentNotNull(target, "Target");
			UnregisterMe(target.GetType());
		}

		public static void UnregisterMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (!_onDomainUnloadHandlers.ContainsKey(type)) return;
			_onDomainUnloadHandlers.Remove(type);
		}

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		private static void _Load()
		{
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach (Type type in ChoType.GetTypes(typeof(ChoAppDomainEventsRegisterableTypeAttribute)))
			{
				#region Find and load domain load handlers

				methods.Clear();
				foreach (MethodInfo methodInfo in ChoType.GetMethods(type, typeof(ChoAppDomainLoadMethodAttribute)))
				{
					if (methodInfo == null) continue;
					if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(void) && methodInfo.IsStatic)
						methods.Add(methodInfo);
				}
				if (methods != null && methods.Count > 0)
					_onDomainLoadHandlers.Add(type, methods.ToArray());

				#endregion Find and load domain load handlers

				#region Find and load domain unload handlers

				methods.Clear();
				foreach (MethodInfo methodInfo in ChoType.GetMethods(type, typeof(ChoAppDomainUnloadMethodAttribute)))
				{
					if (methodInfo == null) continue;
					if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(void) && methodInfo.IsStatic)
						methods.Add(methodInfo);
				}
				if (methods != null && methods.Count > 0)
					_onDomainUnloadHandlers.Add(type, methods.ToArray());

				#endregion Find and load domain unload handlers
			}

			OnAssemblyLoad(null, null);
		}

		/// <summary>
		/// Register for ProcessExit and DomainUnload events on the AppDomain
		/// </summary>
		/// <remarks>
		/// <para>
		/// This needs to be in a separate method because the events make
		/// a LinkDemand for the ControlAppDomain SecurityPermission. Because
		/// this is a LinkDemand it is demanded at JIT time. Therefore we cannot
		/// catch the exception in the method itself, we have to catch it in the
		/// caller.
		/// </para>
		/// </remarks>
		private static void RegisterAppDomainEvents()
		{
			//AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(OnAssemblyLoad);

			// ProcessExit seems to be fired if we are part of the default domain
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

			// Otherwise DomainUnload is fired
			AppDomain.CurrentDomain.DomainUnload += new EventHandler(OnDomainUnload);
		}

		/// <summary>
		/// Called when the <see cref="AppDomain.DomainUnload"/> event fires
		/// </summary>
		/// <param name="sender">the <see cref="AppDomain"/> that is exiting</param>
		/// <param name="e">null</param>
		/// <remarks>
		/// <para>
		/// Called when the <see cref="AppDomain.DomainUnload"/> event fires.
		/// </para>
		/// <para>
		/// When the event is triggered the log4net system is <see cref="Shutdown()"/>.
		/// </para>
		/// </remarks>
		private static void OnDomainUnload(object sender, EventArgs e)
		{
            if (ChoFramework.ShutdownRequested)
                return;

            ChoFramework.ShutdownRequested = true;

            if (_shutdownCompleted)
                return;

            try
            {
                Thread.Sleep(1000);  //TODO: To be parameterized

                foreach (Type type in _onDomainUnloadHandlers.ToKeysArray())
                {
                    foreach (MethodInfo methodInfo in _onDomainUnloadHandlers[type])
                    {
                        if (ChoTraceSwitch.Switch.TraceVerbose)
                            Trace.WriteLine(ChoType.GetMemberAttribute<ChoAppDomainUnloadMethodAttribute>(methodInfo).Description);
                        //ChoTrace.Info(ChoType.GetMemberAttribute<ChoAppDomainUnloadMethodAttribute>(methodInfo).Description);

                        //using (ChoBufferProfileEx profile = new ChoBufferProfileEx(ChoType.GetMemberAttribute<ChoAppDomainUnloadMethodAttribute>(methodInfo).Description))
                        //{
                        try
                        {
                            ChoType.InvokeMethod(type, methodInfo.Name, null);
                        }
                        catch (Exception) // ex)
                        {
                            //profile.Append(ex);
                        }
                        //}
                    }
                }

                //if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
                //    ChoFramework.Shutdown();
            }
            finally
            {
                try
                {
                    ChoTrace.FlushAll();
                }
                catch { }
                _shutdownCompleted = true;
            }
		}

		/// <summary>
		/// Called when the <see cref="AppDomain.ProcessExit"/> event fires
		/// </summary>
		/// <param name="sender">the <see cref="AppDomain"/> that is exiting</param>
		/// <param name="e">null</param>
		/// <remarks>
		/// <para>
		/// Called when the <see cref="AppDomain.ProcessExit"/> event fires.
		/// </para>
		/// <para>
		/// When the event is triggered the log4net system is <see cref="Shutdown()"/>.
		/// </para>
		/// </remarks>
		private static void OnProcessExit(object sender, EventArgs e)
		{
			//OnDomainUnload(sender, e);
            Exit();
		}

		private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			foreach (Type type in _onDomainLoadHandlers.ToKeysArray())
			{
				foreach (MethodInfo methodInfo in _onDomainLoadHandlers[type])
				{
					using (ChoBufferProfileEx profile = new ChoBufferProfileEx(ChoType.GetMemberAttribute<ChoAppDomainLoadMethodAttribute>(methodInfo).Description))
					{
						try
						{
							ChoType.InvokeMethod(type, methodInfo.Name, null);
						}
						catch (Exception ex)
						{
							profile.Append(ex);
						}
					}
				}
			}
		}

		#endregion Shared Members (Private)
	}
}
