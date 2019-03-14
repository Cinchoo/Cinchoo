namespace Cinchoo.Core.Ini
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cinchoo.Core.Services;
	using System.IO;

	#endregion NameSpaces

	public class ChoIniDocumentService : ChoSyncDisposableObject
	{
		#region Instance Data Members (Private)

		private readonly ChoIniDocument _iniDocument;
		private readonly ChoQueuedMsgService<string> _queuedMsgService;
		private readonly ChoTimerService<string> _timerService;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoIniDocumentService(string name, ChoIniDocument iniDocument)
		{
			ChoGuard.ArgumentNotNull(name, "Name");
			ChoGuard.ArgumentNotNull(iniDocument, "IniDocument");

			_iniDocument = iniDocument;
			_queuedMsgService = new ChoQueuedMsgService<string>(name, ChoStandardQueuedMsgObject<string>.QuitMsg, false, false,
				QueueMessageHandler);
			_timerService = new ChoTimerService<string>(String.Format("{0}_Timer", _iniDocument.GetHashCode()),
				new ChoTimerService<string>.ChoTimerServiceCallback(OnTimerServiceCallback), null, 1000, 5000, false);
		}

		#endregion Constructors

		#region ChoSyncMsgQProcessor Overrides

		private void QueueMessageHandler(IChoQueuedMsgServiceObject<string> msgObject)
		{
			if (msgObject == null || !ChoGuard.IsArgumentNotNullOrEmpty(msgObject.State))
				return;

			lock (_iniDocument.SyncRoot)
			{
				File.WriteAllText(_iniDocument.Path, msgObject.State);
			}
		}

		private void OnTimerServiceCallback(string state)
		{
			if (!_iniDocument.Dirty)
				return;

			_iniDocument.Dirty = false;

			if (_queuedMsgService != null)
				_queuedMsgService.Enqueue(ChoStandardQueuedMsgObject<string>.New(_iniDocument.ToString()));
		}

		#endregion ChoSyncMsgQProcessor Overrides

		#region ChoQueuedMsgService Overrides

		internal void Start()
		{
			if (_timerService != null)
				_timerService.Start();
			if (_queuedMsgService != null)
				_queuedMsgService.Start();
		}

		internal void Stop()
		{
			lock (_iniDocument.SyncRoot)
			{
				OnTimerServiceCallback(null);
				if (_timerService != null)
					_timerService.Stop();
				if (_queuedMsgService != null)
					_queuedMsgService.Stop();
			}
		}

		#endregion ChoQueuedMsgService Overrides

		#region Instance Properties (Public)

		public ChoIniDocument IniDocument
		{
			get { return _iniDocument; }
		}

		#endregion Instance Properties (Public)

		protected override void Dispose(bool finalize)
		{
			_iniDocument.Dispose();
			_queuedMsgService.Dispose();
		}
	}
}
