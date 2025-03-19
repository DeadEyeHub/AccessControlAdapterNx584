using System.Net;
using Newtonsoft.Json;
using System.Text;
using AccessControlAdapterSample.AdapterData;
using AccessControlAdapterSample.Adapter;
using AccessControlAdapterSample.Notifications;

namespace AccessControlAdapterSample;

public class AccessControlAdapterService : BackgroundService
{
	public AccessControlAdapterService(ILogger<AccessControlAdapterService> logger)
	{
		_logger = logger;
		_notificationsManager = new NotificationsManager();
	}



	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var serviceSettings = ReadServiceSettings();
		if (serviceSettings == null)
			return;

		ConfigureLogger(serviceSettings);

		var accessControlAddress = new UriBuilder("http", serviceSettings.AccessControlHost, serviceSettings.AccessControlPort).Uri;
		var accessControlAdapter = new AccessControlAdapter(accessControlAddress, _notificationsManager);

		_httpRequestHandlers =
			new HttpRequestHandlers(serviceSettings.ResponsePageSize, serviceSettings.AccessControlAdapterIntegrationId,
				accessControlAdapter, _notificationsManager);

		var accessControlDataUpdater = new AccessControlDataUpdater(serviceSettings.AccessControlPollTimeout, accessControlAdapter, _notificationsManager);

		var httpServer = new HttpServer();

		httpServer.ProcessRequest += (httpListenerContext, stopEventHandle) =>
		{
			OnProcessRequest(httpListenerContext, stopEventHandle);
		};
		httpServer.Start(serviceSettings.AccessControlAdapterHttpPort, serviceSettings.AccessControlAdapterHttpsPort, serviceSettings.UseHttpsServer);

		await Task.Delay(Timeout.Infinite, stoppingToken)
			.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

		httpServer.Dispose();

		accessControlDataUpdater.Dispose();
	}



	void OnProcessRequest(HttpListenerContext httpListenerContext, EventWaitHandle stopEventHandle)
	{
		if (_httpRequestHandlers == null)
		{
			Logging.Logger.Log(NLog.LogLevel.Fatal, "OnProcessRequest: _httpRequestHandlers wasn't initialized");
			return;
		}

		var request = httpListenerContext.Request;

		var responseData = new ResponseData { Error = (int)HttpStatusCode.MethodNotAllowed, ErrorMessage = $"Request method {request.HttpMethod} is not allowed" };
		if (request.HttpMethod == "GET")
			responseData = _httpRequestHandlers.ProcessRequest(httpListenerContext, stopEventHandle);
		else
			Logging.Logger.Log(NLog.LogLevel.Error, $"Request method {request.HttpMethod} is not allowed");

		var statusCode = HttpStatusCode.OK;
		if (responseData.Error != null)
		{
			if (typeof(HttpStatusCode).IsEnumDefined(responseData.Error.Value))
				statusCode = (HttpStatusCode)responseData.Error.Value;
			else
				statusCode = HttpStatusCode.InternalServerError;
		}

		var response = httpListenerContext.Response;
		response.StatusCode = (int)statusCode;

		if (response.OutputStream.CanWrite)
		{
			var serializedString = JsonConvert.SerializeObject(responseData);
			var body = Encoding.UTF8.GetBytes(serializedString);

			response.ContentType = "application/json; charset=utf-8";

			response.ContentLength64 = body.Length;
			response.OutputStream.Write(body, 0, body.Length);
			response.OutputStream.Flush();
		}

		response.Close();
	}

	ServiceSettings? ReadServiceSettings()
	{
		try
		{
			var settingsFilePath = AppContext.BaseDirectory + @"\AccessControlAdapterSettings.json";
			if (File.Exists(settingsFilePath))
			{
				var data = File.ReadAllText(settingsFilePath);
				var settings = JsonConvert.DeserializeObject<ServiceSettings>(data);
				if (settings != null)
					return settings;
			}
		}
		catch (Exception ex)
		{
			_logger.Log(LogLevel.Error, $"Failed to read AccessControlAdapter service settings. Exception was caught: '{ex.Message}'");
		}

		return null;
	}

	void ConfigureLogger(ServiceSettings serviceSettings)
	{
		var logLevel = NLog.LogLevel.Info;
		try
		{
			if (!string.IsNullOrEmpty(serviceSettings.LogLevel))
				logLevel = NLog.LogLevel.FromString(serviceSettings.LogLevel);
		}
		catch
		{
			// ignore
		}

		Logging.ConfigureLogger(logLevel, serviceSettings.LogFile);
	}



	readonly ILogger<AccessControlAdapterService> _logger;
	readonly INotificationsManager _notificationsManager;

	HttpRequestHandlers? _httpRequestHandlers;
}
