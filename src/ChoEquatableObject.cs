namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    public abstract class ChoEquatableObject<T> : ChoNotifyPropertyChangedObject, IEquatable<T>
    {
        #region Object Overrrides

        public override bool Equals(object other)
        {
            if (!(other is ChoEquatableObject<T>)) return false;

            return Equals((T)other);
        }

        public virtual bool Equals(T other)
        {
            return ChoObject.MemberwiseEquals<ChoEquatableObject<T>>(this, other as ChoEquatableObject<T>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Object Overrides

        #region Operator Overloads

        public static bool operator ==(ChoEquatableObject<T> a, ChoEquatableObject<T> b)
        {
            return ChoObject.Equals<ChoEquatableObject<T>>(a, b);
        }

        public static bool operator !=(ChoEquatableObject<T> a, ChoEquatableObject<T> b)
        {
            return !ChoObject.Equals<ChoEquatableObject<T>>(a, b);
        }

        #endregion Operator Overloads
	}
}
