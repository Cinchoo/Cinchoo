namespace Cinchoo.Core.Collections
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    #endregion NameSpaces

    public abstract class ChoBaseEnumerator : ChoSyncDisposableObject, IEnumerator, IEnumerable
    {
        #region Shared Data Members (Private)

        protected static readonly Predicate<object> DefaultMatch = item => true;

        #endregion Shared Data Members (Private)

        #region IEnumerator Members (Public)

        public abstract object Current
        {
            get;
        }

        public abstract bool MoveNext();
        public abstract void Reset();

        #endregion IEnumerator Members (Public)

        #region ChoSyncDisposableObject Overrides

        protected override void Dispose(bool finalize)
        {
            Reset();
        }

        #endregion ChoSyncDisposableObject Overrides

        #region Instance Members (Public)

        public IEnumerable Values
        {
            get
            {
                Reset();
                while (MoveNext())
                {
                    yield return Current;
                }
            }
        }

        #endregion Instance Members (Public)

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
