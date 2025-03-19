using AccessControlAdapterSample.Adapter;

namespace AccessControlAdapterSample
{
	sealed class AccessControlDataUpdater : IDisposable
	{
		public AccessControlDataUpdater(int pollTimeout, IAccessControlAdapter accessControlAdapter, INotificationsManager notificationsManager)
		{
			_accessControlAdapter = accessControlAdapter;
			_notificationsManager = notificationsManager;
			_pollTimeout = pollTimeout;

			RequestAccessControlData();

			StartAccessControlPollThread();
		}

		public void Dispose()
		{
			StopExternalDataParseThread();

			_accessControlPollThreadTerminationEvent.Dispose();
		}


		void StartAccessControlPollThread()
		{
			if (_accessControlPollThread != null)
				return;

			_accessControlPollThreadTerminationEvent.Reset();
			_accessControlPollThread = new Thread(AccessControlPollThreadProc);
			_accessControlPollThread.Start();
		}

		void StopExternalDataParseThread()
		{
			if (_accessControlPollThread == null)
				return;

			_accessControlPollThreadTerminationEvent.Set();
			_accessControlPollThread.Join();
			_accessControlPollThread = null;
		}

		void AccessControlPollThreadProc()
		{
			do
			{
				try
				{
					RequestAccessControlData();
				}
				catch (Exception ex)
				{
					Logging.Logger.Error($"AccessControlAdapter.AccessControlPollThreadProc unhandled exception was caught: {ex}.");
				}
			} while (!_accessControlPollThreadTerminationEvent.WaitOne(_pollTimeout));
		}

		void RequestAccessControlData()
		{
			AccessControlDataCache.Instance.SetItems(_accessControlAdapter.GetItems(), _notificationsManager);

			AccessControlDataCache.Instance.UsersMap = _accessControlAdapter.GetUsers();

			AccessControlDataCache.Instance.EventsMap = _accessControlAdapter.GetEvents();

			_accessControlAdapter.FireAccessControlEvents();
		}



		Thread? _accessControlPollThread;
		readonly ManualResetEvent _accessControlPollThreadTerminationEvent = new ManualResetEvent(false);
		readonly IAccessControlAdapter _accessControlAdapter;
		readonly INotificationsManager _notificationsManager;
		readonly int _pollTimeout; // Access control poll frequency in ms
	}
}
