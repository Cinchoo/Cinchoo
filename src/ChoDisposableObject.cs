namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Diagnostics;
	using System.Collections.Generic;

	#endregion NameSpaces

	public abstract class ChoDisposableObject : ChoEquatableObject<ChoDisposableObject>, IChoDisposable
	{
		#region Instance Data Memebers (Private)

		[ChoIgnoreMemberFormatter]
		[ChoHiddenMember]
		private readonly StackTrace _objectCreationStackTrace;

		#endregion
		
		#region Instance Data Members (Protected)

		[ChoIgnoreMemberFormatter]
		[ChoHiddenMember]
		private bool _disposed = false;

		#endregion Instance Data Members (Protected)

		#region Constructors

		public ChoDisposableObject()
		{
			_objectCreationStackTrace = new StackTrace(1, true);
		}

		#endregion Constructors

		#region IChoDisposable Members

		[ChoIgnoreMemberFormatter]
		public bool IsDisposed
		{
			get { return _disposed; }
			set { _disposed = value; }
		}

		protected abstract void Dispose(bool finalize);

		#endregion

		#region IDisposable Members

		public virtual void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			ChoObjectDisposar.Dispose(this, false);
		}

		#endregion

		#region Destructor

		~ChoDisposableObject()
		{
			ChoObjectDisposar.Dispose(this, true);
		}

		#endregion Destructor

		#region IChoDisposable Members

		[ChoIgnoreMemberFormatter]
		public StackTrace ObjectCreationStackTrace
		{
			get { return _objectCreationStackTrace; }
		}

		#endregion

        void IChoDisposable.Dispose(bool finalize)
        {
            Dispose(finalize);
        }

    }

    public abstract class ChoDisposableObject<T> : ChoEquatableObject<T>, IChoDisposable
    {
        #region Instance Data Memebers (Private)

        [ChoIgnoreMemberFormatter]
        [ChoHiddenMember]
        private readonly StackTrace _objectCreationStackTrace;

        #endregion

        #region Instance Data Members (Protected)

        [ChoIgnoreMemberFormatter]
        [ChoHiddenMember]
        private bool _disposed = false;

        #endregion Instance Data Members (Protected)

        #region Constructors

        public ChoDisposableObject()
        {
            _objectCreationStackTrace = new StackTrace(1, true);
        }

        #endregion Constructors

        #region IChoDisposable Members

        [ChoIgnoreMemberFormatter]
        public bool IsDisposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }

        protected abstract void Dispose(bool finalize);

        #endregion

        #region IDisposable Members

        public virtual void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            ChoObjectDisposar.Dispose(this, false);
        }

        #endregion

        #region Destructor

        ~ChoDisposableObject()
        {
            ChoObjectDisposar.Dispose(this, true);
        }

        #endregion Destructor

        #region IChoDisposable Members

        [ChoIgnoreMemberFormatter]
        public StackTrace ObjectCreationStackTrace
        {
            get { return _objectCreationStackTrace; }
        }

        #endregion

        void IChoDisposable.Dispose(bool finalize)
        {
            Dispose(finalize);
        }

    }
}
