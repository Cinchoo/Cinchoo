namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Diagnostics;
	using System.Collections.Generic;
    using System.Runtime.ConstrainedExecution;

	#endregion NameSpaces

    public abstract class ChoDisposableObject : CriticalFinalizerObject, IEquatable<ChoDisposableObject>, IChoDisposable
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

        void IChoDisposable.Dispose(bool finalize)
        {
            Dispose(finalize);
        }

		#endregion

        #region Object Overrrides

        public override bool Equals(object other)
        {
            if (!(other is ChoDisposableObject)) return false;

            return Equals((ChoDisposableObject)other);
        }

        public virtual bool Equals(ChoDisposableObject other)
        {
            return ChoObject.MemberwiseEquals<ChoDisposableObject>(this, other as ChoDisposableObject);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Object Overrides

        #region Operator Overloads

        public static bool operator ==(ChoDisposableObject a, ChoDisposableObject b)
        {
            return ChoObject.Equals<ChoDisposableObject>(a, b);
        }

        public static bool operator !=(ChoDisposableObject a, ChoDisposableObject b)
        {
            return !ChoObject.Equals<ChoDisposableObject>(a, b);
        }

        #endregion Operator Overloads
    }

    public abstract class ChoDisposableObject<T> : CriticalFinalizerObject, IEquatable<ChoDisposableObject<T>>, IChoDisposable
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

        void IChoDisposable.Dispose(bool finalize)
        {
            Dispose(finalize);
        }

        #endregion

        #region object overrrides

        public override bool Equals(object other)
        {
            if (other is ChoDisposableObject<T>)
                return Equals((ChoDisposableObject<T>)other);
            else
                return false;
        }

        public bool Equals(ChoDisposableObject<T> other)
        {
            return GetHashCode() == other.GetHashCode();
            //return ChoObject.MemberwiseEquals<ChoDisposableObject<T>>(this, other as ChoDisposableObject<T>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion object overrides

        #region Operator Overloads

        public static bool operator ==(ChoDisposableObject<T> a, ChoDisposableObject<T> b)
        {
            return ChoObject.Equals<ChoDisposableObject<T>>(a, b);
        }

        public static bool operator !=(ChoDisposableObject<T> a, ChoDisposableObject<T> b)
        {
            return !ChoObject.Equals<ChoDisposableObject<T>>(a, b);
        }

        #endregion Operator Overloads

    }
}
