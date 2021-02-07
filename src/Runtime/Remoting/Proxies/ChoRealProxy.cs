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

    #endregion NameSpaces

    public class ChoRealProxy : RealProxy, IRemotingTypeInfo, IDisposable
    {
        #region Instance Data Memebers (Private)

        private MarshalByRefObject _target;
        private object _realObject;
        private string _typeName;
        private Type _objType;
        private bool _isInit = false;

        #endregion Instance Data Memebers (Private)

        #region Constructors

        public ChoRealProxy(MarshalByRefObject target, Type type)
            : base(type)
        {
            _realObject = _target = target;
            _objType = type;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public object OwnerObject
        {
            get { return this.GetUnwrappedServer(); }
        }

        #endregion Instance Properties (Public)

        private readonly static object _padLock = new object();
        private readonly static ChoDictionaryService<Type, IMethodMessage> _ctorMsgs = new ChoDictionaryService<Type, IMethodMessage>("d1");
        private readonly static ChoDictionaryService<Type, object> _instanceMsgs = new ChoDictionaryService<Type, object>("d2");

        public virtual object DoObjectInitialize(object target)
        {
            return target;
        }

        public virtual bool PreMethodInvoke(object owner, MethodCallMessageWrapper methodCallMsg, ChoMemberInfo memberInfo)
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
                        return false;
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
            return true;
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
                interceptableObject.PostInvokeInternal(memberInfo);
        }

        public virtual object GetInstance(IConstructionCallMessage ctorCallMessage, ref IMethodMessage returnMsg)
        {
            if (_ctorMsgs.ContainsKey(ctorCallMessage.ActivationType))
            {
                returnMsg = _ctorMsgs.GetValue(ctorCallMessage.ActivationType);
                return _instanceMsgs.GetValue(ctorCallMessage.ActivationType);
            }

            if (!Monitor.TryEnter(_padLock, 1000))
                return null;

            try
            {
                if (!_ctorMsgs.ContainsKey(ctorCallMessage.ActivationType))
                {
                    RealProxy defaultProxy = RemotingServices.GetRealProxy(_target);
                    var x = defaultProxy.InitializeServerObject(ctorCallMessage);
                    IConstructionReturnMessage response = EnterpriseServicesHelper.CreateConstructionReturnMessage(ctorCallMessage, (MarshalByRefObject)this.GetTransparentProxy());

                    _ctorMsgs.SetValue(ctorCallMessage.ActivationType, response);
                    _instanceMsgs.SetValue(ctorCallMessage.ActivationType, _target);

                    //_ctorMsgs.SetValue(ctorCallMessage.ActivationType, InitializeServerObject(ctorCallMessage));
                    //_instanceMsgs.SetValue(ctorCallMessage.ActivationType, DoObjectInitialize(GetUnwrappedServer()));
                }
            }
            finally
            {
                Monitor.Exit(_padLock);
                //DoObjectInitialize(_target);
            }

            returnMsg = _ctorMsgs.GetValue(ctorCallMessage.ActivationType);
            return _instanceMsgs.GetValue(ctorCallMessage.ActivationType);
        }

        #region RealProxy Overrides

        /// In the Invoke Method we do our interception work. 
        public override IMessage Invoke(IMessage msg)
        {
            IMethodMessage returnMsg = null;
            if (msg is IConstructionCallMessage)
            {
                _realObject = GetInstance(msg as IConstructionCallMessage, ref returnMsg);
                _objType = ((IConstructionCallMessage)msg).ActivationType;
                //SetStubData(this, _realObject);
            }
            else
            {
                if (!_isInit)
                {
                    DoObjectInitialize(_target);
                    _isInit = true;
                }

                ///the MethodCallMessageWrapper provides read/write access to the method call arguments. 
                MethodCallMessageWrapper methodCallMsg = new MethodCallMessageWrapper((IMethodCallMessage)msg);
                ///This is the reflected method base of the called method. 
                MethodInfo methodInfo = (MethodInfo)methodCallMsg.MethodBase;

                ///This is the object we are proxying. 
                MarshalByRefObject owner = _target;
                //MarshalByRefObject owner = _realObject as MarshalByRefObject;
                ///Some basic initializations for later use 
                if (owner != null)
                {
                    ChoMemberInfo memberInfo = new ChoMemberInfo(owner, _objType, methodCallMsg);
                    if (!ChoType.IsValidObjectMember(memberInfo.Info))
                        //|| ChoType.GetAttribute<ChoPropertyInfoAttribute>(memberInfo.Info, false) == null)
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

            return returnMsg;
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

                DoPostMethodInvoke(methodCallMsg, owner, memberInfo);
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception excp)
            {
                Exception ex = excp;
                if (ex is TargetInvocationException && ex.InnerException != null)
                    ex = ex.InnerException;

                memberInfo.Exception = ex;

                ChoInterceptableObject interceptableObject = owner as ChoInterceptableObject;
                if (interceptableObject != null && interceptableObject.Initialized)
                {
                    if (interceptableObject.IsConfigObjectSilent)
                    {
                        ChoConfigurationObjectErrorManagerService.SetObjectMemberError(interceptableObject, memberInfo.Name,
                            String.Format(Resources.ConfigConstructMsg, ChoString.ToString(memberInfo.Value), ex.Message));
                    }
                }

                string msg = "{0}: {1}".FormatString(memberInfo.Name, String.Format(Resources.ConfigConstructMsg, ChoString.ToString(memberInfo.Value), ex.Message));
                memberInfo.Exception = new ChoValidationException(msg, ex);

                DoPostMethodInvoke(methodCallMsg, owner, memberInfo);
            }

            return memberInfo.ReturnMessage;
            //try
            //{
            //    PreMethodInvoke(owner, methodCallMsg, memberInfo);

            //    returnMsg = RemotingServices.ExecuteMessage(owner, methodCallMsg);
            //    memberInfo.ReturnMessage = returnMsg as ReturnMessage;

            //    if (memberInfo.Info != null)
            //        memberInfo.MethodMsg = returnMsg;

            //    PostMethodInvoke(owner, methodCallMsg, memberInfo);

            //    return memberInfo.ReturnMessage;
            //}
            //catch (TargetInvocationException ex)
            //{
            //    return new ReturnMessage(ex.InnerException, methodCallMsg);
            //}
            //catch (ChoFatalApplicationException)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    //Trace.TraceError(ex.ToString());
            //    ChoInterceptableObject interceptableObject = owner as ChoInterceptableObject;
            //    if (interceptableObject != null && interceptableObject.Initialized)
            //    {
            //        if (interceptableObject.IsConfigObjectSilent)
            //        {
            //            ChoConfigurationObjectErrorManagerService.SetObjectMemberError(interceptableObject, memberInfo.Name,
            //                String.Format(Resources.ConfigConstructMsg, ChoString.ToString(memberInfo.Value), ex.Message));

            //            //PostMethodInvoke(owner, methodCallMsg, memberInfo);

            //            //return new ReturnMessage(null, methodCallMsg);
            //        }
            //    }
            //    string msg = "{0}: {1}".FormatString(memberInfo.Name, String.Format(Resources.ConfigConstructMsg, ChoString.ToString(memberInfo.Value), ex.Message));
            //    ChoValidationException ex1 = new ChoValidationException(msg);
            //    if (OnMemberError(owner, memberInfo, ref ex))
            //    {
            //        return new ReturnMessage(null, null, 0, null, methodCallMsg);
            //    }
            //    else
            //        return new ReturnMessage(ex1, methodCallMsg);
            //}
        }

        private void DoPostMethodInvoke(MethodCallMessageWrapper methodCallMsg, MarshalByRefObject owner, ChoMemberInfo memberInfo)
        {
            try
            {
                PostMethodInvoke(owner, methodCallMsg, memberInfo);
            }
            catch { }

            if (memberInfo.Exception == null)
            {
                if (memberInfo.ReturnMessage == null)
                    memberInfo.ReturnMessage = new ReturnMessage(null, null, 0, null, methodCallMsg);
            }
            else
                memberInfo.ReturnMessage = new ReturnMessage(memberInfo.Exception, methodCallMsg);
        }

        //protected internal virtual bool OnMemberError(MarshalByRefObject owner, ChoMemberInfo memberInfo, ref Exception ex)
        //{
        //    return true;
        //}

        #endregion RealProxy Overrides

        #region IRemotingTypeInfo Members

        public bool CanCastTo(Type fromType, object o)
        {
            if (fromType.IsAssignableFrom(o.GetType()))
            {
                return true;
            }
            return false;
        }

        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
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
