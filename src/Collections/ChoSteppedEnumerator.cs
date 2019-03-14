namespace Cinchoo.Core.Collections
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

	public class ChoSteppedEnumerator : ChoBaseEnumerator
	{
		#region Instance Data Members (Private)

		private readonly IEnumerable _enumerable;
		private readonly int _step = 1;

		private IEnumerator _enumerator;
		private int _currentIndex;

		#endregion Instance Data Members (Private)

		#region Constructors

        public ChoSteppedEnumerator(IEnumerable enumerable, int step)
		{
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");

			if (step <= 0)
				throw new ArgumentException("step must be positive.");

			_step = step;
            _enumerable = enumerable;

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

                try
                {
                    if (_currentIndex == 0)
                        return true;

                    if (_currentIndex % (_step + 1) == 0)
                        return true;
                }
                finally
                {
                    _currentIndex++;
                }
            }
        }

		public override void Reset()
		{
            _currentIndex = 0;
			_enumerator = _enumerable.GetEnumerator();
		}

		#endregion
    }
}
