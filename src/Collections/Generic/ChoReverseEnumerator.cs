namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

    public class ChoReverseEnumerator<T> : ChoBaseEnumerator<T>
    {
        #region Instance Data Members (Private)

        private readonly IEnumerable<T> _enumerable;

        private IEnumerator<T> _enumerator;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoReverseEnumerator(IEnumerable<T> enumerable)
        {
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");
            _enumerable = enumerable.Reverse();

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
            return _enumerator.MoveNext();
        }

        public override void Reset()
        {
            _enumerator = _enumerable.GetEnumerator();
        }

        #endregion
    }
}
