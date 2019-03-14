namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    #endregion NameSpaces

    public class ChoWeakReference : ChoWeakReference<object>
    {
        #region Constructors

        /// <summary>        
        /// Initializes a new instance of the Minimal.ChoWeakReference{T} class, referencing        
        /// the specified object.        
        /// </summary>        
        /// <param name="target">The object to reference.</param>        
        public ChoWeakReference(object target)
            : base(target)
        {
        }

        /// <summary>        
        /// Initializes a new instance of the ChoWeakReference{T} class, referencing 
        /// the specified object and using the specified resurrection tracking. 
        /// </summary>        
        /// <param name="target">An object to track.</param> 
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If true, the object is tracked 
        /// after finalization; if false, the object is only tracked until finalization.</param> 
        public ChoWeakReference(object target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

        protected ChoWeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors
    }

    /// <summary> 
    /// Represents a weak reference, which references an object while still allowing   
    /// that object to be reclaimed by garbage collection.    
    /// </summary>    
    /// <typeparam name="T">The type of the object that is referenced.</typeparam>    
    [Serializable]
    public class ChoWeakReference<T>
        : WeakReference where T : class
    {
        #region Instance Data Members (Private)

        [NonSerialized]
        [XmlIgnore]
        private int _id = Int32.MinValue;

        #endregion Instance Data Members (Private)

        #region Constructors

        /// <summary>        
        /// Initializes a new instance of the Minimal.ChoWeakReference{T} class, referencing        
        /// the specified object.        
        /// </summary>        
        /// <param name="target">The object to reference.</param>        
        public ChoWeakReference(T target)
            : base(target)
        {
            _id = target.GetHashCode();
        }

        /// <summary>        
        /// Initializes a new instance of the ChoWeakReference{T} class, referencing 
        /// the specified object and using the specified resurrection tracking. 
        /// </summary>        
        /// <param name="target">An object to track.</param> 
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If true, the object is tracked 
        /// after finalization; if false, the object is only tracked until finalization.</param> 
        public ChoWeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
            _id = target.GetHashCode();
        }

        protected ChoWeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors

        #region Instance Properties (Public)

        /// <summary> 
        /// Gets or sets the object (the target) referenced by the current ChoWeakReference{T} 
        /// object. 
        /// </summary> 
        public new T Target
        {
            get
            {
                return (T)base.Target;
            }
            set
            {
                base.Target = value;
                _id = value.GetHashCode();
            }
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Public)

        public bool Equals(ChoWeakReference<T> target)
        {
            if (target == null) return false;

            if (!target.IsAlive) return false;
            if (Object.ReferenceEquals(this, target)) return true;

			return IsAlive && target.GetHashCode() == GetHashCode();
		}

        #endregion Instance Members (Public)

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ChoWeakReference<T>)) return false;

            ChoWeakReference<T> target = obj as ChoWeakReference<T>;

            //if (!target.IsAlive) return false;
            if (Object.ReferenceEquals(this, target)) return true;

            return IsAlive && target.GetHashCode() == GetHashCode();
        }

        public override string ToString()
        {
            if (IsAlive)
            {
                object target = Target;
                if (target != null) return target.ToString();
            }

            return base.ToString();
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion Object Overrides

        #region Operator Overloads

        /// <summary> 
        /// Casts an object of the type T to a weak reference 
        /// of T. 
        /// </summary> 
        public static implicit operator ChoWeakReference<T>(T target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            return new ChoWeakReference<T>(target);
        }

        /// <summary> 
        /// Casts a weak reference to an object of the type the 
        /// reference represents. 
        /// </summary> 
        public static implicit operator T(ChoWeakReference<T> reference)
        {
            return reference != null ? reference.Target : null;
        }

        public static bool operator ==(ChoWeakReference<T> p1, ChoWeakReference<T> p2)
        {
            if (Object.ReferenceEquals(p1, p2)) return true;

            // If one is null, but not both, return false.
            if (((object)p1 == null) || ((object)p1 == null))
                return false;

            return p1.Equals(p2);
        }

        public static bool operator !=(ChoWeakReference<T> p1, ChoWeakReference<T> p2)
        {
            return !(p1 == p2);
        }

        #endregion Operator Overloads

        #region Instance Properties (Private)

        private int Id
        {
            get
            {
                if (_id == Int32.MinValue)
                {
                    object target = Target;
                    if (target != null)
                        System.Threading.Interlocked.CompareExchange(ref _id, target.GetHashCode(), Int32.MinValue);
                }
                return _id;
            }
        }

        #endregion Instance Properties (Private)
    }
}
