namespace Cinchoo.Core.Collections
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

    public class ChoReverseEnumerator : ChoBaseEnumerator
    {
        #region Instance Data Members (Private)

        private readonly IEnumerable _enumerable;

        private IEnumerator _enumerator;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoReverseEnumerator(IEnumerable enumerable)
        {
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");
            _enumerable = enumerable.Cast<object>().Reverse();

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
            return _enumerator.MoveNext();
        }

        public override void Reset()
        {
            _enumerator = _enumerable.GetEnumerator();
        }

        #endregion
    }
}
