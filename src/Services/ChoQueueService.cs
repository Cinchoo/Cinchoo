namespace Cinchoo.Core.Services
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Cinchoo.Core.Collections;

	#endregion NameSpaces

	public interface IChoQueueServiceMsg<T>
	{
		T State
		{
			get;
		}

		bool IsShutdownMsg
		{
			get;
		}

		IChoQueueServiceMsg<T> ShutdownMsg
		{
			get;
		}
	}

	public abstract class ChoQueueServiceMsgBase<T> : IChoQueueServiceMsg<T>
	{
		#region Instance Data Members (Private)

		private readonly T _state;
		private readonly IEqualityComparer<T> _comparer;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoQueueServiceMsgBase(T state) : this(state, EqualityComparer<T>.Default)
		{
		}

		public ChoQueueServiceMsgBase(T state, IEqualityComparer<T> comparer)
		{
			_state = state;
			_comparer = comparer;
		}

		#endregion Constructors

		#region IChoQueueServiceMsg<T> Members

		public T State
		{
			get { return _state; }
		}

		public virtual bool IsShutdownMsg
		{
			get
			{
				return _comparer.Equals(_state, ShutdownMsg.State);
			}
		}

		public abstract IChoQueueServiceMsg<T> ShutdownMsg
		{
			get;
		}

		#endregion
	}

	public sealed class ChoSimpleQueueServiceMsg : ChoQueueServiceMsgBase<string>
	{
		#region Constants

		private const string SHUTDOWN_MSG = "${SHUTDOWN_MSG}";

		#endregion Constants

		#region Instance Data Members (Private)

		private readonly ChoSimpleQueueServiceMsg _shutdownMsg = new ChoSimpleQueueServiceMsg(SHUTDOWN_MSG);

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoSimpleQueueServiceMsg(string state) : this(state, EqualityComparer<string>.Default)
		{
		}

		public ChoSimpleQueueServiceMsg(string state, IEqualityComparer<string> comparer) : base(state, comparer)
		{
		}

		#endregion Constructors

		public override IChoQueueServiceMsg<string> ShutdownMsg
		{
			get { return _shutdownMsg; }
		}
	}

	public sealed class ChoQueueService<T> : ChoSyncDisposableObject where T : IChoQueueServiceMsg<T>
	{
		//#region Instance Data Members (Private)

		//private readonly Thread _queueProcessingThread;
		//private readonly object _padLock = new object();
		//private ChoQueue _queue = ChoQueue.BlockingQueue(new ChoQueue());
		//private ChoQMessageHandler _queueMessageHandler;
		////private AutoResetEvent _serviceStartedEvent = new AutoResetEvent(false);
		//private bool _stoppingServce = false;
		//private IChoQueuedMsgServiceObject<T> _shutdownMsg;

		//#endregion Instance Data Members (Private)

		public void Enqueue(T state)
		{
		}

		public bool TryEnqueue(T state)
		{
			return false;
		}

		protected override void Dispose(bool finalize)
		{
		}
	}

}
