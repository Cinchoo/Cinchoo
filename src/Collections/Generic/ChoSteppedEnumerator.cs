namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

	public class ChoSteppedEnumerator<T> : ChoBaseEnumerator<T>
	{
		#region Instance Data Members (Private)

		private readonly IEnumerable<T> _enumerable;
		private readonly int _step = 1;

		private IEnumerator<T> _enumerator;
		private int _currentIndex = -1;

		#endregion Instance Data Members (Private)

		#region Constructors

        public ChoSteppedEnumerator(IEnumerable<T> enumerable, int step)
		{
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");

			if (step <= 0)
				throw new ArgumentException("step must be positive.");

			_step = step;
            _enumerable = enumerable;

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

                _currentIndex++;
                if (_currentIndex % (_step + 1) == 0)
                    return true;
            }
		}

		public override void Reset()
		{
            _currentIndex = -1;
			_enumerator = _enumerable.GetEnumerator();
		}

		#endregion
    }
}
