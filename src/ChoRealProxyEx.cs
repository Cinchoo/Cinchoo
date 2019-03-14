namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Runtime.Remoting.Proxies;
	using System.Runtime.Remoting.Messaging;
	using System.Runtime.Remoting.Activation;
	using System.Reflection.Emit;
	using System.Reflection;
	using System.Runtime.Remoting.Contexts;

	#endregion NameSpaces

	//public class InterceptionProxy : RealProxy
	//{
	//    public InterceptionProxy(Type classToProxy)
	//        : base(classToProxy)
	//    {
	//    }

	//    public override IMessage Invoke(IMessage msg)
	//    {
	//        IConstructionCallMessage ctorMsg = msg as IConstructionCallMessage;

	//        if (ctorMsg != null)
	//        {
	//            return this.InitializeServerObject(ctorMsg);
	//        }
	//        else
	//        {
	//            throw new NotSupportedException();
	//        }
	//    }
	//}
	public class InterceptionProxy : RealProxy
	{
		delegate Cinchoo.RuntimeMethodHandle GetMethodHandle(IMethodCallMessage message);

		delegate object DynamicMethodDelegate(object target, object[] args);

		static GetMethodHandle getMethodHandle = CreateGetMethodHandle();

		[ThreadStatic]
		static Dictionary<Cinchoo.RuntimeMethodHandle, DynamicMethodDelegate> cache;

		object target;

		public InterceptionProxy(Type classToProxy) //, object target)
			: base(classToProxy)
		{
			//this.target = Activator.CreateInstance(classToProxy);
		}

		static GetMethodHandle CreateGetMethodHandle()
		{
			Type messageType = Type.GetType("System.Runtime.Remoting.Messaging.Message");

			DynamicMethod dm = new DynamicMethod("", typeof(Cinchoo.RuntimeMethodHandle),
				new Type[] { typeof(IMethodCallMessage) }, typeof(InterceptionProxy), true);

			ILGenerator il = dm.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, messageType);
			il.Emit(OpCodes.Ldfld, messageType.GetField("_methodDesc",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
			il.Emit(OpCodes.Newobj, typeof(Cinchoo.RuntimeMethodHandle).GetConstructor(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				null, new Type[] { typeof(IntPtr) }, null));

			il.Emit(OpCodes.Ret);

			return (GetMethodHandle)dm.CreateDelegate(typeof(GetMethodHandle));
		}

		static DynamicMethodDelegate CreateDynamicMethod(MethodInfo method)
		{
			ParameterInfo[] pi = method.GetParameters();

			DynamicMethod dm = new DynamicMethod("", typeof(object),
				new Type[] { typeof(object), typeof(object[]) },
				typeof(InterceptionProxy), true);

			ILGenerator il = dm.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);

			for (int i = 0; i < pi.Length; i++)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, i);

				Type parameterType = pi[i].ParameterType;
				if (parameterType.IsByRef)
				{
					parameterType = parameterType.GetElementType();
					if (parameterType.IsValueType)
					{
						il.Emit(OpCodes.Ldelem_Ref);
						il.Emit(OpCodes.Unbox, parameterType);
					}
					else
					{
						il.Emit(OpCodes.Ldelema, parameterType);
					}
				}
				else
				{
					il.Emit(OpCodes.Ldelem_Ref);

					if (parameterType.IsValueType)
					{
						il.Emit(OpCodes.Unbox, parameterType);
						il.Emit(OpCodes.Ldobj, parameterType);
					}
				}
			}

			if ((method.IsAbstract || method.IsVirtual)
				&& !method.IsFinal && !method.DeclaringType.IsSealed)
			{
				il.Emit(OpCodes.Callvirt, method);
			}
			else
			{
				il.Emit(OpCodes.Call, method);
			}

			if (method.ReturnType == typeof(void))
			{
				il.Emit(OpCodes.Ldnull);
			}
			else if (method.ReturnType.IsValueType)
			{
				il.Emit(OpCodes.Box, method.ReturnType);
			}
			il.Emit(OpCodes.Ret);

			return (DynamicMethodDelegate)dm.CreateDelegate(typeof(DynamicMethodDelegate));
		}

		IMethodReturnMessage InvokeMethod(IMethodCallMessage callMsg)
		{
			DynamicMethodDelegate method;
			if (cache == null)
			{
				cache = new Dictionary<Cinchoo.RuntimeMethodHandle, DynamicMethodDelegate>();
			}
			if (!cache.TryGetValue(getMethodHandle(callMsg), out method))
			{
				method = CreateDynamicMethod((MethodInfo)callMsg.MethodBase);
				cache.Add(new Cinchoo.RuntimeMethodHandle(callMsg.MethodBase.MethodHandle.Value), method);
			}

			object ret;
			object[] args = callMsg.Args;
			try
			{
				ret = method(target, args);
			}
			catch (Exception ex)
			{
				return new ReturnMessage(ex, callMsg);
			}
			return new ReturnMessage(ret, args, args.Length, null, callMsg);
		}
		// other methods omitted
		public override IMessage Invoke(IMessage msg)
		{
			IConstructionCallMessage ctorMsg = msg as IConstructionCallMessage;

			if (ctorMsg != null)
			{
				return this.InitializeServerObject(ctorMsg);
			}
			else
			{
				throw new NotSupportedException();
			}
		}
	}
	public class InterceptionProxyAttribute : ProxyAttribute
	{
		public override MarshalByRefObject CreateInstance(Type serverType)
		{
			RealProxy proxy = new InterceptionProxy(serverType);
			return (MarshalByRefObject)proxy.GetTransparentProxy();
		}
	}
}
