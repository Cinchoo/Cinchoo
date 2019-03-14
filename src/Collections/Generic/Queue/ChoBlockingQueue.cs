namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Cinchoo.Core.Threading;

	#endregion NameSpaces

	public partial class ChoQueue<T> : Queue<T>, ICloneable, IDisposable
	{
		private class ChoBlockingQueue<T1> : ChoQueue<T1>, ICloneable
		{
			#region Instance Data Members (Private)

			private readonly ChoQueue<T1> _queue;
			private readonly ChoBarrier _barrier;
			private readonly int _size;

			#endregion Instance Data Members (Private)

			#region Constructors

			internal ChoBlockingQueue(ChoQueue<T1> queue)
				: this(queue, Int32.MaxValue)
			{
			}

			internal ChoBlockingQueue(ChoQueue<T1> queue, int size)
			{
				this._queue = queue;
				this._size = size;
				this._barrier = new ChoBarrier(0, size);
			}

			#endregion Constructors

			#region Instance Properties (Public)

			public override int Size
			{
				get { return _size; }
			}

			public override object SyncRoot
			{
				get { return _syncRoot; }
			}

			public override int Count
			{
				get { return _queue.Count; }
			}

			public override bool IsSynchronized
			{
				get { return true; }
			}

			public override bool IsBlockingQueue
			{
				get { return true; }
			}

			#endregion Instance Properties (Public)

			#region Instance Members (Public)

			public override void Clear()
			{
				_queue.Clear();
			}

			public override bool Contains(T1 item)
			{
				return _queue.Contains(item);
			}

			public override void CopyTo(T1[] array, int arrayIndex)
			{
				_queue.CopyTo(array, arrayIndex);
			}

			public override T1 Dequeue(int millisecondsTimeout)
			{
				using (ChoBarrier lowBarrier = ChoBarrier.LowBarrier(_barrier))
				{
					if (!lowBarrier.Wait(millisecondsTimeout))
						throw new ChoQueueTimeoutException();

					T1 item = _queue.Dequeue();
					return item;
				}
			}

			public override bool TryDequeue(out T1 item)
			{
				if (_queue.Count == 0)
				{
					item = default(T1);
					return false;
				}
				else
				{
					item = _queue.Dequeue();
					return true;
				}
			}

			public override void Enqueue(T1 item, int millisecondsTimeout)
			{
				using (ChoBarrier highBarrier = ChoBarrier.HighBarrier(_barrier))
				{
					if (!highBarrier.Wait(millisecondsTimeout))
						throw new ChoQueueTimeoutException();

					_queue.Enqueue(item);
				}
			}

			public override IEnumerator<T1> GetEnumerator()
			{
				return _queue.GetEnumerator();
			}

			public override T1 Peek()
			{
				return _queue.Peek();
			}

			public override T1[] ToArray()
			{
				return _queue.ToArray();
			}

			public override void TrimExcess()
			{
				_queue.TrimExcess();
			}

			#endregion Instance Members (Public)

			#region ICloneable Members

			public override object Clone()
			{
				return _queue.Clone();
			}

			#endregion

			#region Instance Members (Protected)

			protected override void Dispose(bool finalize)
			{
				if (_barrier != null)
				    _barrier.Dispose();
			}

			#endregion Instance Members (Protected)
		}
	}
}
