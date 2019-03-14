namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    public abstract class ChoComparableObject<T> : ChoEquatableObject<T>, IComparable, IComparable<T>
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is T)) return -1;

            return CompareTo((T)obj);
        }

        #endregion

        #region IComparable<T> Members

        public virtual int CompareTo(T other)
        {
            if (System.Object.ReferenceEquals(this, other))
                return 0;

            return ChoObject.Compare(this, other);
        }

        #endregion
    }
}
