namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoEqualityComparerAttribute : Attribute
    {
        #region Instance Data Members (Private)

        private readonly IChoEqualityComparer _equalityComparer;
        //private readonly IEqualityComparer<T> _equalityComparer;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoEqualityComparerAttribute(Type equalityComparerType)
        {
            if (equalityComparerType != null)
            {
                if (typeof(IChoEqualityComparer).IsAssignableFrom(equalityComparerType))
                    _equalityComparer = ChoObjectManagementFactory.CreateInstance(equalityComparerType) as IChoEqualityComparer;
            }
        }

        #endregion Constructors

        #region Instance Members (Public)

        internal bool IsEqualCompare<T>(T oldValue, T newValue)
        {
            Type objType = typeof(T);
            if (_equalityComparer != null && _equalityComparer.CanCompare(null, objType))
                return _equalityComparer.Equals(null, null, oldValue, newValue);
            else
                return Object.Equals(oldValue, newValue);

        }

        internal bool IsEqualCompare(object oldValue, object newValue)
        {
            Type objType = oldValue.GetType();

            if (_equalityComparer != null && _equalityComparer.CanCompare(null, objType))
                return _equalityComparer.Equals(null, null, oldValue, newValue);
            else
                return Object.Equals(oldValue, newValue);
        }

        #endregion Instance Members (Public)
    }
}
