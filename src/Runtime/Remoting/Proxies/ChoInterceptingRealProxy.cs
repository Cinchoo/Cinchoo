namespace Cinchoo.Core.Runtime.Remoting.Proxies
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Proxies;
    using System.Runtime.Remoting.Messaging;

    #endregion NameSpaces

    public class ChoInterceptingRealProxy : RealProxy, IRemotingTypeInfo, IDisposable
    {
        #region Instance Data Members (Private)

        private string _typeName;

        #endregion

        #region Constructors

        public ChoInterceptingRealProxy(object target)
            : base(target.GetType())
        {
            _typeName = target.GetType().FullName;
            AttachServer((MarshalByRefObject)target);
        }

        #endregion

        #region IRemotingTypeInfo Members

        ///<summary>
        ///Checks whether the proxy that represents the specified object type can be cast to the type represented by the <see cref="T:System.Runtime.Remoting.IRemotingTypeInfo"></see> interface.
        ///</summary>
        ///
        ///<returns>
        ///true if cast will succeed; otherwise, false.
        ///</returns>
        ///
        ///<param name="fromType">The type to cast to. </param>
        ///<param name="o">The object for which to check casting. </param>
        ///<exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission. </exception>
        public bool CanCastTo(Type fromType, object o)
        {
            return fromType.IsAssignableFrom(o.GetType());
        }

        ///<summary>
        ///Gets or sets the fully qualified type name of the server object in a <see cref="T:System.Runtime.Remoting.ObjRef"></see>.
        ///</summary>
        ///
        ///<value>
        ///The fully qualified type name of the server object in a <see cref="T:System.Runtime.Remoting.ObjRef"></see>.
        ///</value>
        ///
        ///<exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure" /></PermissionSet>
        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        #endregion

        #region RealProxy Members

        /// <summary>
        /// Executes a method call represented by the <paramref name="msg"/>
        /// parameter. The CLR will call this method when a method is called
        /// on the TransparentProxy. This method runs the invocation through
        /// the call handler pipeline and finally sends it down to the
        /// target object, and then back through the pipeline. 
        /// </summary>
        /// <param name="msg">An <see cref="IMessage"/> object that contains the information
        /// about the method call.</param>
        /// <returns>An <see cref="RemotingMethodReturn"/> object contains the
        /// information about the target method's return value.</returns>
        public override IMessage Invoke(IMessage msg)
        {
            ///the MethodCallMessageWrapper provides read/write access to the method call arguments. 
            MethodCallMessageWrapper mc = new MethodCallMessageWrapper((IMethodCallMessage)msg);

            ///This is the reflected method base of the called method. 
            MethodInfo mi = (MethodInfo)mc.MethodBase;

            ///This is the object we are proxying. 
            MarshalByRefObject owner = GetUnwrappedServer();

            IMessage retval = null;
            object outVal = null; 

            /// The caller is expecting to get something out of the method invocation.  So we need to construct a return message
            /// This is pretty simple, pass back out most of the stuff that was passed into us, and hand it the outVal as well.
            retval = new ReturnMessage(outVal, mc.Args, mc.Args.Length, mc.LogicalCallContext, mc);
            return retval;
        }

        #endregion RealProxy Members

        #region IDisposable Members

        public void Dispose()
        {
            DetachServer();
        }

        #endregion
    }
}
