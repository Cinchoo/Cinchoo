namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Data;
    using System.Reflection;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Proxies;
    using System.Runtime.Remoting.Messaging;

    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Runtime.Remoting;

    #endregion NameSpaces

	[Serializable]
	public abstract class ChoInterceptableObject : ContextBoundObject, IDisposable
	{
		#region Constructors
		
		static ChoInterceptableObject()
		{
            //ChoAppDomain.Initialize();
		}

		#endregion Constructors

		[ChoHiddenMember]
        [NonSerialized]
		private readonly object _padLock = new object();

		[ChoHiddenMember]
        [NonSerialized]
        private bool _silent = false;

		[ChoHiddenMember]
        internal bool Silent
        {
            get { return _silent; }
        }

        [ChoHiddenMember]
        internal bool IsConfigObjectSilent
        {
            get;
            set;
        }

		[ChoHiddenMember]
        [NonSerialized]
        private bool _initialized = false;

		[ChoHiddenMember]
        internal bool Initialized
        {
			get { lock (_padLock) { return _initialized; } }
			set { lock (_padLock) { _initialized = value; } }
        }

		[ChoHiddenMember]
        [NonSerialized]
        private bool _isDirty;

		[ChoHiddenMember]
        internal bool Dirty
        {
			get { return _isDirty; }
        }

        [ChoHiddenMember]
        internal bool IsModified
        {
            get;
            set;
        }

		[ChoHiddenMember]
		internal bool PreInvokeInternal(ChoMemberInfo memberInfo)
		{
			return PreInvoke(memberInfo);
		}

		[ChoHiddenMember]
		protected virtual bool PreInvoke(ChoMemberInfo memberInfo)
        {
			return false;
        }

		[ChoHiddenMember]
		internal void PostInvokeInternal(ChoMemberInfo memberInfo)
		{
			PostInvoke(memberInfo);
		}

		[ChoHiddenMember]
		protected virtual void PostInvoke(ChoMemberInfo memberInfo)
        {
        }

		[ChoHiddenMember]
		internal void SetSilent(bool silent)
        {
            _silent = silent;
        }

		[ChoHiddenMember]
		internal void SetDirty(bool dirty)
        {
			_isDirty = dirty;
        }

		[ChoHiddenMember]
		internal void SetInitialized(bool initialized)
        {
			_initialized = initialized;
        }

		[ChoHiddenMember]
        [NonSerialized]
        private bool _readOnly;

		[ChoHiddenMember]
		internal void SetReadOnly(bool flag)
		{
			_readOnly = flag;
		}

		[ChoHiddenMember]
		internal bool IsReadOnly()
		{
			return _readOnly;
		}

		[ChoHiddenMember]
        internal object OwnerObject
        {
            get
            {
                return null;
            }
        }

		[ChoHiddenMember]
		internal static ChoInterceptableObject Silentable(ChoInterceptableObject interceptableObject)
        {
            if (interceptableObject == null)
                throw new ArgumentNullException("InterceptableObject");

            return new ChoSilentableObject(interceptableObject);
        }

        public void Init()
        {
        }

        private class ChoSilentableObject : ChoInterceptableObject, IDisposable
        {
            #region Instance Data Members

            private ChoInterceptableObject _interceptableObject;

            #endregion Instance Data Members

            #region Constructors

            internal ChoSilentableObject(ChoInterceptableObject interceptableObject)
            {
                if (interceptableObject == null)
                    throw new NullReferenceException("interceptableObject");

                _interceptableObject = interceptableObject;
                _interceptableObject.SetSilent(true);
            }

            #endregion Constructors

            #region IDisposable Members

            public override void Dispose()
            {
				if (_interceptableObject != null)
				{
                    //_interceptableObject.SetSilent(false);
					_interceptableObject.Dispose();
					_interceptableObject = null;
				}
            }

            #endregion
        }

        #region IDisposable Members

		[ChoHiddenMember]
		protected virtual void Dispose(bool finalize)
        {
        }

		[ChoHiddenMember]
		public virtual void Dispose()
        {
            Dispose(false);
        }

        #endregion

        #region Finalizer

		[ChoHiddenMember]
		~ChoInterceptableObject()
        {
            Dispose(true);
        }

        #endregion Finalizer
    }
}
