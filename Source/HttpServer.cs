using System.Net;

namespace AccessControlAdapterSample
{
	class HttpServer : IDisposable
	{
		public HttpServer()
		{
			_stop = new ManualResetEvent(false);
			_listener = new HttpListener();
			_listenerThread = new Thread(ProcessRequests);
		}

		public void Dispose()
		{
			Stop();

			_stop.Dispose();
			_listener.Close();
		}



		public event Action<HttpListenerContext, EventWaitHandle>? ProcessRequest;



		public void Start(int httpPort, int httpsPort, bool useHttps)
		{
			_listener.Prefixes.Add($"http://*:{httpPort}/");

			if (useHttps)
				_listener.Prefixes.Add($"https://*:{httpsPort}/");

			_listener.Start();
			_listenerThread.Start();
		}

		public void Stop()
		{
			_stop.Set();

			if (_listener.IsListening)
				_listener.Stop();

			if (_listenerThread.IsAlive)
				_listenerThread.Join();
		}

		private void ProcessRequests()
		{
			while (_listener.IsListening)
			{
				var context = _listener.BeginGetContext(OnContextReady, null);
				if (0 == WaitHandle.WaitAny([_stop, context.AsyncWaitHandle]))
					return;
			}
		}

		private void OnContextReady(IAsyncResult asyncResult)
		{
			try
			{
				var context = _listener.EndGetContext(asyncResult);
				ThreadPool.QueueUserWorkItem(thread =>
				{
					try
					{
						ProcessRequest?.Invoke(context, _stop);
					}
					catch
					{
						// ignored
					}
				});
			}
			catch
			{
				// ignored
			}
		}



		private readonly HttpListener _listener;
		private readonly Thread _listenerThread;
		private readonly ManualResetEvent _stop;
	}
}
