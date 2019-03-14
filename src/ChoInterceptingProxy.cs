namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.Data;
    using System.Reflection;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Proxies;
    using System.Runtime.Remoting.Messaging;

    using eSquare.Core.Reflection;
    using eSquare.Core.Attributes;
    using eSquare.Core.Configuration;

    #endregion NameSpaces

    public class ChoInterceptingProxy : RealProxy, IRemotingTypeInfo, IDisposable
    {
        private string _typeName;

        public ChoInterceptingProxy(object subject)
            : base(subject.GetType())
        {
            //We have to attach our object to the proxy.  
            //This isn't an automagic thing, probably because it isn't always relevant.
            AttachServer((MarshalByRefObject)subject);
        }

        public void Dispose()
        {
            //For proper cleanup, let's detach our server. 
            //I am sure this is done in GC, but why risk it?
            DetachServer();
        }

        /// Here is the magic of our proxy.
        /// We create a new instance of the class, then get a transparent proxy of our server.
        /// Note that we are returning object, but this is actually of whatever type our obj
        /// parameter is.
        public static object Instance(object obj)
        {
            if (obj == null) return obj;
            
            return ChoType.IsInterceptableObject(obj.GetType()) ? new ChoInterceptingProxy(obj).GetTransparentProxy() : obj;
        }

        public object OwnerObject
        {
            get { return this.GetUnwrappedServer(); }
        }

        /// In the Invoke Method we do our interception work. 
        public override IMessage Invoke(IMessage msg)
        {
            ///the MethodCallMessageWrapper provides read/write access to the method call arguments. 
            MethodCallMessageWrapper methodCallMsg = new MethodCallMessageWrapper((IMethodCallMessage)msg);
            ///This is the reflected method base of the called method. 
            MethodInfo methodInfo = (MethodInfo)methodCallMsg.MethodBase;

            ///This is the object we are proxying. 
            MarshalByRefObject owner = GetUnwrappedServer();
            ///Some basic initializations for later use 
            IMethodReturnMessage returnMsg = null;
            if (owner != null)
            {
                ChoMemberInfo memberInfo = new ChoMemberInfo(owner, methodCallMsg);
                try
                {
                    lock (this)
                    {
                        if (owner is ChoInterceptableObject && memberInfo.DirtyOperation && !((ChoInterceptableObject)owner).Silent)
                            ((ChoInterceptableObject)owner).SetDirty(memberInfo.DirtyOperation);

                        if (owner is ChoInterceptableObject)
                            ((ChoInterceptableObject)owner).PreInvoke(methodCallMsg, memberInfo);

                        if (memberInfo.Info != null)
                        {
                            foreach (IChoBeforeMemberCallAttribute beforeMemberCallAttribute in ChoType.GetMemberAttributesByBaseType(memberInfo.Info,
                                typeof(IChoBeforeMemberCallAttribute)))
                            {
                                beforeMemberCallAttribute.Validate(memberInfo.Value, ((ChoInterceptableObject)owner).Silent);
                            }
                        }

                        //outVal = methodInfo.Invoke(owner, methodCallMsg.Args);
                        returnMsg = RemotingServices.ExecuteMessage(owner, methodCallMsg);

                        if (memberInfo.Info != null)
                        {
                            memberInfo.MethodMsg = returnMsg;
                            foreach (IChoAfterMemberCallAttribute afterMemberCallAttribute in ChoType.GetMemberAttributesByBaseType(memberInfo.Info,
                                typeof(IChoAfterMemberCallAttribute)))
                            {
                                afterMemberCallAttribute.Validate(memberInfo.Value, ((ChoInterceptableObject)owner).Silent);
                            }
                        }

                        if (owner is ChoInterceptableObject)
                            ((ChoInterceptableObject)owner).PostInvoke(methodCallMsg, memberInfo);

                        return returnMsg; // new ReturnMessage(outVal, methodCallMsg.Args, methodCallMsg.Args.Length, methodCallMsg.LogicalCallContext, methodCallMsg);
                    }
                }
                catch (TargetInvocationException ex)
                {
                    return new ReturnMessage(ex.InnerException, methodCallMsg);
                }
            }

            throw new NullReferenceException("Missing target object.");
        }


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
    }
}
