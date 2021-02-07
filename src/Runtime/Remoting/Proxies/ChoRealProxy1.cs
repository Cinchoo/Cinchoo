namespace Cinchoo.Core.Runtime.Remoting
{
	#region NameSpaces

	using System;
	using System.Data;
	using System.Reflection;
	using System.Collections;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Proxies;
	using System.Runtime.Remoting.Messaging;

	using Cinchoo.Core.Factory;
	using Cinchoo.Core.Reflection;
	using Cinchoo.Core.Configuration;
	using System.Runtime.Remoting.Activation;
	using Cinchoo.Core.Services;
	using System.Diagnostics;
	using System.Runtime.Remoting.Services;
	using Cinchoo.Core.Properties;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

	#endregion NameSpaces

    internal class ChoRealProxyInstanceState
    {
        public object Target;
        public IMethodMessage MethodMessage;
        public Exception Exception;
        public bool IsInitialized;
    }

	public abstract class ChoRealProxy : RealProxy, IRemotingTypeInfo, IDisposable
    {
        #region Shared Data Members (Private)

        private readonly static object _padLock = new object();
        private readonly static Dictionary<Type, ChoRealProxyInstanceState> _instances = new Dictionary<Type, ChoRealProxyInstanceState>();

        #endregion Shared Data Members (Private)

        #region Instance Data Memebers (Private)

        private readonly Type _objType;
        private object _target;

		#endregion Instance Data Memebers (Private)

		#region Constructors

        public ChoRealProxy(Type type)
            : base(type)
        {
            _objType = type;
        }

		#endregion Constructors

		#region Instance Properties (Public)

		public object OwnerObject
		{
            get { return _target; }
		}

		#endregion Instance Properties (Public)

		public virtual object DoObjectInitialize(object target)
		{
			return target;
		}

		public virtual void PreMethodInvoke(object owner, MethodCallMessageWrapper methodCallMsg, ChoMemberInfo memberInfo)
		{
			MethodInfo methodInfo = (MethodInfo)methodCallMsg.MethodBase;

			ChoInterceptableObject interceptableObject = owner as ChoInterceptableObject;

			if (interceptableObject != null)
			{
				if (memberInfo.DirtyOperation && !interceptableObject.Silent)
				{
					if (interceptableObject.PreInvokeInternal(memberInfo))
					{
						if (interceptableObject.Initialized)
							interceptableObject.SetDirty(memberInfo.DirtyOperation);
						else
							interceptableObject.IsModified = true;
					}
					else
						return;
				}
			}

			if (memberInfo.Info != null)
			{
				if (memberInfo.DirtyOperation)
				{
					ChoValidation.Validate(memberInfo.Info, memberInfo.Value);
                    if (interceptableObject != null && interceptableObject.Initialized)
						ChoConfigurationObjectErrorManagerService.ResetObjectMemberError(interceptableObject, memberInfo.Name);
				}
			}
		}

		public virtual void PostMethodInvoke(object owner, MethodCallMessageWrapper methodCallMsg, ChoMemberInfo memberInfo)
		{
			MethodInfo methodInfo = (MethodInfo)methodCallMsg.MethodBase;

			ChoInterceptableObject interceptableObject = owner as ChoInterceptableObject;

			//if (memberInfo.Info != null)
			//{
			//    if (memberInfo.DirtyOperation)
			//    {
			//        ChoValidation.Validate(memberInfo.Info, memberInfo.Value);
			//        if (interceptableObject.Initialized)
			//            ChoConfigurationObjectErrorManagerService.ResetObjectMemberError(interceptableObject, memberInfo.Name);
			//    }
			//}

            if (interceptableObject != null)
            {
                interceptableObject.PostInvokeInternal(memberInfo);
            }
		}

        private ChoRealProxyInstanceState GetInstance(IConstructionCallMessage ctorCallMessage)
        {
            if (_instances.ContainsKey(ctorCallMessage.ActivationType))
                return _instances[ctorCallMessage.ActivationType];

            lock (_padLock)
            {
                if (!_instances.ContainsKey(ctorCallMessage.ActivationType))
                {
                    IConstructionReturnMessage retMsg = InitializeServerObject(ctorCallMessage);
                    IConstructionReturnMessage response = null;

                    if (retMsg.Exception == null)
                    {
                        _target = GetUnwrappedServer();
                        SetStubData(this, _target);
                        response = EnterpriseServicesHelper.CreateConstructionReturnMessage(ctorCallMessage, (MarshalByRefObject)this.GetTransparentProxy());
                    }
                    _instances.Add(ctorCallMessage.ActivationType, new ChoRealProxyInstanceState() { Target = _target, MethodMessage = response, Exception = retMsg.Exception, IsInitialized = false });
                }

                return _instances[ctorCallMessage.ActivationType];
            }
        }

		#region RealProxy Overrides

		/// In the Invoke Method we do our interception work. 
		public override IMessage Invoke(IMessage msg)
		{
			if (msg is IConstructionCallMessage)
			{
                ChoRealProxyInstanceState instance = GetInstance(msg as IConstructionCallMessage);
                if (instance.Exception != null)
                    throw instance.Exception;

                _target = instance.Target;
                return EnterpriseServicesHelper.CreateConstructionReturnMessage(msg as IConstructionCallMessage, (MarshalByRefObject)this.GetTransparentProxy());
            }
			else
			{
                IMethodMessage returnMsg = null;
                ChoRealProxyInstanceState instance = _instances[_target.GetType()];
                if (!instance.IsInitialized)
                {
                    DoObjectInitialize(_target);
                    instance.IsInitialized = true;
                }

				///the MethodCallMessageWrapper provides read/write access to the method call arguments. 
				MethodCallMessageWrapper methodCallMsg = new MethodCallMessageWrapper((IMethodCallMessage)msg);
				///This is the reflected method base of the called method. 
				MethodInfo methodInfo = (MethodInfo)methodCallMsg.MethodBase;

				///This is the object we are proxying. 
                MarshalByRefObject owner = _target as MarshalByRefObject;
				//MarshalByRefObject owner = _realObject as MarshalByRefObject;
				///Some basic initializations for later use 
				if (owner != null)
				{
					ChoMemberInfo memberInfo = new ChoMemberInfo(owner, _objType, methodCallMsg);
					if (!ChoType.IsValidObjectMember(memberInfo.Info))
						return RemotingServices.ExecuteMessage(owner, methodCallMsg);

					return ExecuteMethod(ref returnMsg, methodCallMsg, owner, memberInfo);

					//if (methodCallMsg.LogicalCallContext.GetData("CallerId") != null)
					//    return ExecuteMethod(ref returnMsg, methodCallMsg, owner, memberInfo);
					//else
					//{
					//    methodCallMsg.LogicalCallContext.SetData("CallerId", ChoRandom.NextRandom());
					//    IChoAsyncResult result = ChoQueuedExecutionService.Global.Enqueue(() =>
					//        {
					//            return ExecuteMethod(ref returnMsg, methodCallMsg, owner, memberInfo);
					//        });

					//    return (IMessage)result.EndInvoke();
					//}
				}
				else
					return new ReturnMessage(new NullReferenceException("Missing target object."), methodCallMsg);
			}
		}

		private ReturnMessage ExecuteMethod(ref IMethodMessage returnMsg, MethodCallMessageWrapper methodCallMsg, MarshalByRefObject owner, ChoMemberInfo memberInfo)
		{
			try
			{
				PreMethodInvoke(owner, methodCallMsg, memberInfo);

				returnMsg = RemotingServices.ExecuteMessage(owner, methodCallMsg);
				memberInfo.ReturnMessage = returnMsg as ReturnMessage;

				if (memberInfo.Info != null)
					memberInfo.MethodMsg = returnMsg;

				PostMethodInvoke(owner, methodCallMsg, memberInfo);

				return memberInfo.ReturnMessage;
			}
			catch (TargetInvocationException ex)
			{
				return new ReturnMessage(ex.InnerException, methodCallMsg);
			}
			catch (ChoFatalApplicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				//Trace.TraceError(ex.ToString());
				ChoInterceptableObject interceptableObject = owner as ChoInterceptableObject;
                if (interceptableObject != null && interceptableObject.Initialized)
				{
					if (interceptableObject.IsConfigObjectSilent)
					{
						ChoConfigurationObjectErrorManagerService.SetObjectMemberError(interceptableObject, memberInfo.Name,
							String.Format(Resources.ConfigConstructMsg, ChoString.ToString(memberInfo.Value), ex.Message));

						//PostMethodInvoke(owner, methodCallMsg, memberInfo);

						//return new ReturnMessage(null, methodCallMsg);
					}
				}
                string msg = "{0}: {1}".FormatString(memberInfo.Name, String.Format(Resources.ConfigConstructMsg, ChoString.ToString(memberInfo.Value), ex.Message));
                ChoValidationException ex1 = new ChoValidationException(msg);
                if (OnMemberError(owner, memberInfo, ref ex))
                    return new ReturnMessage(null, null, 0, null, methodCallMsg);
                else
                    return new ReturnMessage(ex1, methodCallMsg);
			}
		}

        protected internal virtual bool OnMemberError(MarshalByRefObject owner, ChoMemberInfo memberInfo, ref Exception ex)
        {
            return false;
        }

		#endregion RealProxy Overrides

		#region IRemotingTypeInfo Members

		public bool CanCastTo(Type fromType, object o)
		{
            if (o == null || fromType == null) return false;

			if (fromType.IsAssignableFrom(o.GetType()))
			{
				return true;
			}
			return false;
		}

        public string TypeName
        {
            get
            {
                return _objType.FullName;
            }
            set
            {

            }
        }

		#endregion
	
		#region IDisposable Members

		protected virtual void Dispose(bool finalize)
		{
		}

		public virtual void Dispose()
		{
			Dispose(false);
		}

		#endregion

		#region Finalizer

		~ChoRealProxy()
		{
			Dispose(true);
		}

		#endregion Finalizer
    }
}
