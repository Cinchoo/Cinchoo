namespace Cinchoo.Core.Collections
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoConstraintEnumerator : ChoBaseEnumerator
    {
        #region Instance Data Members (Private)

        private readonly Predicate<object> _match;
        private readonly IEnumerable _enumerable;
        private IEnumerator _enumerator;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoConstraintEnumerator(IEnumerable enumerable, Predicate<object> match)
        {
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");

            _enumerable = enumerable;
            _match = match;

            Reset();
        }

        #endregion Constructors

        #region IEnumerator Members

        public override object Current
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
