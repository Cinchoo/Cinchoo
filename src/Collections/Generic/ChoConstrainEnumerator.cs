namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoConstraintEnumerator<T> : ChoBaseEnumerator<T>
    {
        #region Instance Data Members (Private)

        private readonly Predicate<T> _match;
        private readonly IEnumerable<T> _enumerable;
        private IEnumerator<T> _enumerator;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoConstraintEnumerator(IEnumerable<T> enumerable, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");

            _enumerable = enumerable;
            _match = match;

            Reset();
        }

        #endregion Constructors

        #region IEnumerator<T> Members

        public override T Current
        {
            get
            {
                ChoGuard.NotDisposed(this);
                return _enumerator.Current;
            }
        }

        public override bool MoveNext()
        {
            ChoGuard.NotDisposed(this);

            while (true)
            {
                if (!_enumerator.MoveNext())
                    return false;

                if (_match(_enumerator.Current))
                    return true;
            }
        }

        public override void Reset()
        {
            _enumerator = _enumerable.GetEnumerator();
        }

        #endregion
    }
}
