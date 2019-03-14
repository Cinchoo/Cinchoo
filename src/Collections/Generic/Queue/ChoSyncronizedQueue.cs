namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	#endregion NameSpaces

	public partial class ChoQueue<T> : Queue<T>, ICloneable
	{
		private class ChoSyncronizedQueue<T1> : ChoQueue<T1>, ICloneable
		{
			#region Instance Data Members (Private)

			private readonly ChoQueue<T1> _queue;
			private new readonly object _syncRoot;

			#endregion Instance Data Members (Private)

			#region Constructors

			internal ChoSyncronizedQueue(ChoQueue<T1> queue)
				: this(queue, queue.SyncRoot)
			{
			}

			internal ChoSyncronizedQueue(ChoQueue<T1> queue, object syncObject)
			{
				this._queue = queue;
				this._syncRoot = syncObject;
			}

			#endregion Constructors

			#region Instance Properties (Public)

			public override object SyncRoot
			{
				get { return _syncRoot; }
			}

			public override int Count
			{
				get 
				{ 
					lock (_syncRoot) 
					{
						return _queue.Count; 
					} 
				}
			}

			public override bool IsSynchronized
			{
				get { return true; }
			}

			public override bool IsBlockingQueue
			{
				get { return _queue.IsBlockingQueue; }
			}

			#endregion Instance Properties (Public)

			#region Instance Members (Public)

			public override void Clear()
			{
				lock (_syncRoot)
				{
					_queue.Clear();
				}
			}

			public override bool Contains(T1 item)
			{
				lock (_syncRoot)
				{
					return _queue.Contains(item);
				}
			}

			public override void CopyTo(T1[] array, int arrayIndex)
			{
				lock (_syncRoot)
				{
					_queue.CopyTo(array, arrayIndex);
				}
			}

			public override T1 Dequeue(int millisecondsTimeout)
			{
				lock (_syncRoot)
				{
					while (_queue.Count == 0)
					{
						try
						{
							if (!Monitor.Wait(_syncRoot, millisecondsTimeout))
								throw new ChoQueueTimeoutException();
						}
						catch
						{
							Monitor.PulseAll(_syncRoot);
							throw;
						}
					}
					T1 item = _queue.Dequeue();
					Monitor.PulseAll(_syncRoot);
					return item;
				}
			}

			public override bool TryDequeue(out T1 item)
			{
				lock (_syncRoot)
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
			}

			public override void Enqueue(T1 item)
			{
				lock (_syncRoot)
				{
					_queue.Enqueue(item);
				}
			}

			public override IEnumerator<T1> GetEnumerator()
			{
				return new ChoSynchronizedEnumerator<T1>(_queue, _syncRoot);
			}

			public override T1 Peek()
			{
				lock (_syncRoot)
				{
					return _queue.Peek();
				}
			}

			public override T1[] ToArray()
			{
				lock (_syncRoot)
				{
					return _queue.ToArray();
				}
			}

			public override void TrimExcess()
			{
				lock (_syncRoot)
				{
					_queue.TrimExcess();
				}
			}

			#endregion Instance Members (Public)

			#region ICloneable Members

			public override object Clone()
			{
				lock (_syncRoot)
				{
					return _queue.Clone();
				}
			}

			#endregion
		}
	}
}
