using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using AccessControlAdapterSample.AdapterData;
using AccessControlAdapterSample.Adapter;

namespace AccessControlAdapterSample
{
	class HttpRequestHandlers
	{
		public HttpRequestHandlers(uint responsePageSize, string integrationId,
			IAccessControlAdapter accessControlAdapter, INotificationsManager notificationsManager)
		{
			_responsePageSize = responsePageSize;
			_accessControlAdapter = accessControlAdapter;
			_notificationsManager = notificationsManager;

			_infoData.IntegrationId = integrationId;

			InitializeRequestHandlers();
		}



		public ResponseData ProcessRequest(HttpListenerContext context, EventWaitHandle stopEventHandle)
		{
			var request = context.Request;

			var path = request.Url?.LocalPath;
			if (path == null)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, "ProcessRequest: got request with empty path.");
				return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = "Path is empty." };
			}

			var parameters = HttpUtility.ParseQueryString(request.Url?.Query ?? string.Empty);
			if (_requestHandlersMap.TryGetValue(path, out var requestHandler))
			{
				Logging.Logger.Log(NLog.LogLevel.Debug, $"ProcessRequest: handler for '{path}' was found.");
				return requestHandler(context, parameters, stopEventHandle);
			}

			string? foundHandlerPath = null;
			foreach (var handlerPath in _requestByIdHandlersMap.Keys)
			{
				if (!path.StartsWith(handlerPath))
					continue;

				if (path.IndexOf('/', handlerPath.Length) >= 0)
					continue;

				if (foundHandlerPath == null || foundHandlerPath.Length < handlerPath.Length)
				{
					foundHandlerPath = handlerPath;
					break;
				}
			}

			if (foundHandlerPath != null && _requestByIdHandlersMap.TryGetValue(foundHandlerPath, out var requestByIdHandler))
			{
				var id = GetObjectIdFromUrlPath(path);
				if (id != null)
				{
					Logging.Logger.Log(NLog.LogLevel.Debug, $"ProcessRequest: handler for '{path}' was found.");
					return requestByIdHandler(context, id, parameters, stopEventHandle);
				}
			}

			Logging.Logger.Log(NLog.LogLevel.Error, $"Resource at '{path}' was not found");

			return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"Resource at '{path}' was not found" };
		}



		void InitializeRequestHandlers()
		{
			_requestHandlersMap.Add("/info", OnProcessGetInfoRequest);

			if (!string.IsNullOrEmpty(_infoData.UsersUrl))
				_requestHandlersMap.Add(_infoData.UsersUrl, OnProcessGetUsersRequest);

			if (!string.IsNullOrEmpty(_infoData.ItemsUrl))
			{
				_requestHandlersMap.Add(_infoData.ItemsUrl, OnProcessGetItemsRequest);
				_requestByIdHandlersMap.Add($"{_infoData.ItemsUrl}/", OnProcessGetItemByIdRequest);
			}

			if (!string.IsNullOrEmpty(_infoData.ActionsUrl))
			{
				_requestHandlersMap.Add(_infoData.ActionsUrl, OnProcessGetActionsRequest);
				_requestByIdHandlersMap.Add($"{_infoData.ActionsUrl}/", OnProcessGetActionByIdRequest);
			}

			if (!string.IsNullOrEmpty(_infoData.ParametersUrl))
			{
				_requestHandlersMap.Add(_infoData.ParametersUrl, OnProcessGetParametersRequest);
				_requestByIdHandlersMap.Add($"{_infoData.ParametersUrl}/", OnProcessGetParameterByIdRequest);
			}

			if (!string.IsNullOrEmpty(_infoData.StatesUrl))
			{
				_requestHandlersMap.Add(_infoData.StatesUrl, OnProcessGetStatesRequest);
				_requestByIdHandlersMap.Add($"{_infoData.StatesUrl}/", OnProcessGetStateByIdRequest);
			}

			if (!string.IsNullOrEmpty(_infoData.EventsUrl))
			{
				_requestHandlersMap.Add(_infoData.EventsUrl, OnProcessGetEventsRequest);
				_requestByIdHandlersMap.Add($"{_infoData.EventsUrl}/", OnProcessGetEventByIdRequest);
			}

			if (!string.IsNullOrEmpty(_infoData.NotificationsUrl))
				_requestHandlersMap.Add(_infoData.NotificationsUrl, OnProcessGetNotificationsRequest);
		}

		uint GetPageNumValue(NameValueCollection parameters)
		{
			var pageNumStr = parameters.Get("page");
			if (pageNumStr == null)
				return 0;

			if (!uint.TryParse(pageNumStr, out var pageNum))
				return 0;

			return pageNum;
		}

		ResponseData OnProcessGetInfoRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var infoData = (InfoData)_infoData.Clone();
			infoData.UpdateTime(DateTime.UtcNow);

			return new ResponseData { Data = infoData };
		}

		ResponseData OnProcessGetUsersRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var pageNum = GetPageNumValue(parameters);
			var responseData = new ResponseData { Page = pageNum };

			var users = AccessControlDataCache.Instance.GetUsers();

			var offset = pageNum * _responsePageSize;
			var usersCount = GetMaxCount((uint)users.Count, offset, _responsePageSize);

			if (usersCount > 0)
				responseData.Data = users.GetRange((int)offset, (int)usersCount);

			if (users.Count > 1)
				responseData.NextPage = users.Count > offset + usersCount;

			return responseData;
		}

		ResponseData OnProcessGetItemsRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var pageNum = GetPageNumValue(parameters);
			var responseData = new ResponseData { Page = pageNum };

			var items = AccessControlDataCache.Instance.GetItems();

			var offset = pageNum * _responsePageSize;
			var itemsCount = GetMaxCount((uint)items.Count, offset, _responsePageSize);

			if (itemsCount > 0)
				responseData.Data = items.GetRange((int)offset, (int)itemsCount);

			if (items.Count > 1)
				responseData.NextPage = items.Count > offset + itemsCount;

			return responseData;
		}

		ResponseData OnProcessGetStatesRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var pageNum = GetPageNumValue(parameters);
			var responseData = new ResponseData { Page = pageNum };

			var states = AccessControlDataCache.Instance.GetItemsStates();

			var offset = pageNum * _responsePageSize;
			var statesCount = GetMaxCount((uint)states.Count, offset, _responsePageSize);

			if (statesCount > 0)
				responseData.Data = states.GetRange((int)offset, (int)statesCount);

			if (states.Count > 1)
				responseData.NextPage = states.Count > offset + statesCount;

			return responseData;
		}

		ResponseData OnProcessGetEventsRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var pageNum = GetPageNumValue(parameters);
			var responseData = new ResponseData { Page = pageNum };

			var events = AccessControlDataCache.Instance.GetEvents();

			var offset = pageNum * _responsePageSize;
			var eventsCount = GetMaxCount((uint)events.Count, offset, _responsePageSize);

			if (eventsCount > 0)
				responseData.Data = events.GetRange((int)offset, (int)eventsCount);

			if (events.Count > 1)
				responseData.NextPage = events.Count > offset + eventsCount;

			return responseData;
		}

		ResponseData OnProcessGetNotificationsRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var response = context.Response;
			response.ContentType = "text/event-stream; charset=utf-8";
			response.SendChunked = true;
			response.Headers.Add(HttpResponseHeader.Connection, "Keep-Alive");

			var notificationCookie = _notificationsManager.RegisterNotificationQueue(new Queue<NotificationData>());

			var emptyNotificationData = new NotificationData();
			var output = response.OutputStream;

			var keepAlive = GetKeepAliveValue(parameters, 20);
			var keepAliveTimer = new Stopwatch();

			keepAliveTimer.Start();

			_notificationsManager.PushAllItemsUpdatedNotification();

			WaitHandle[] wait = [_notificationsManager.NotificationReadyEvent, stopEventHandle];
			while (true)
			{
				var timeout = 0;
				if (_notificationsManager.IsNotificationQueueEmpty(notificationCookie) && keepAliveTimer.ElapsedMilliseconds < keepAlive)
					timeout = Math.Max((int)(keepAlive - keepAliveTimer.ElapsedMilliseconds), 0);

				if (WaitHandle.WaitAny(wait, timeout) == 1) // stopEventHandle
					return new ResponseData();

				var notificationData = _notificationsManager.DequeueNotification(notificationCookie);
				if (notificationData == null)
				{
					if (keepAliveTimer.ElapsedMilliseconds >= keepAlive)
					{
						notificationData = emptyNotificationData;

						Logging.Logger.Info($"Processing keep-alive notification; remote {context?.Request?.RemoteEndPoint}, keep-alive {keepAlive}");
					}
					else
						continue;
				}
				else
					Logging.Logger.Info($"Processing new notification; remote {context?.Request?.RemoteEndPoint}, keep-alive {keepAlive}");

				notificationData.UtcTime = DateTime.UtcNow.ToFileTimeUtc();

				keepAliveTimer.Restart();

				var serialized = notificationData.Serialize();
				var data = Encoding.UTF8.GetBytes(serialized);

				try
				{
					output.Write(data, 0, data.Length);
					output.Flush();
				}
				catch (HttpListenerException ex)
				{
					Logging.Logger.Error($"OnProcessGetNotificationsRequest. Unhandled exception was caught: {ex.Message}");
					break;
				}
			}

			_notificationsManager.UnregisterNotificationQueue(notificationCookie);

			return new ResponseData();
		}

		ResponseData OnProcessGetActionsRequest(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			var pageNum = GetPageNumValue(parameters);
			var responseData = new ResponseData { Page = pageNum };

			var actions = _accessControlAdapter.GetActions();

			var offset = pageNum * _responsePageSize;
			var actionsCount = GetMaxCount((uint)actions.Count, offset, _responsePageSize);

			if (actionsCount > 0)
				responseData.Data = actions.ToList().GetRange((int)offset, (int)actionsCount);

			if (actions.Count > 1)
				responseData.NextPage = actions.Count > offset + actionsCount;

			return responseData;
		}

		ResponseData OnProcessGetActionByIdRequest(HttpListenerContext context, string id, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			if (string.IsNullOrEmpty(id))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, "OnProcessGetActionByIdRequest: action id wasn't specified.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = "Action id wasn't specified." };
			}

			var actionData = _accessControlAdapter.GetAction(id);
			if (actionData == null)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Action with id = '{id}' wasn't found.");
				return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"Action with id = '{id}' wasn't found." };
			}

			var action = parameters.Get("action");
			if (action != null)
				return ExecuteSystemAction(actionData, action, GetActionParameters(parameters));

			return new ResponseData { Data = actionData };
		}

		ResponseData OnProcessGetParametersRequest(HttpListenerContext context, NameValueCollection requestParameters, EventWaitHandle stopEventHandle)
		{
			var pageNum = GetPageNumValue(requestParameters);
			var responseData = new ResponseData { Page = pageNum };

			var parameters = _accessControlAdapter.GetParameters();

			var offset = pageNum * _responsePageSize;
			var parametersCount = GetMaxCount((uint)parameters.Count, offset, _responsePageSize);

			if (parametersCount > 0)
				responseData.Data = parameters.ToList().GetRange((int)offset, (int)parametersCount);

			if (parameters.Count > 1)
				responseData.NextPage = parameters.Count > offset + parametersCount;

			return responseData;
		}

		ResponseData OnProcessGetParameterByIdRequest(HttpListenerContext context, string id, NameValueCollection requestParameters, EventWaitHandle stopEventHandle)
		{
			if (string.IsNullOrEmpty(id))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, "OnProcessGetParameterByIdRequest: parameter id wasn't specified.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = "Parameter id wasn't specified." };
			}

			var parameterData = _accessControlAdapter.GetParameter(id);
			if (parameterData == null)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Parameter with id = '{id}' wasn't found.");
				return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"Parameter with id = '{id}' wasn't found." };
			}

			Logging.Logger.Log(NLog.LogLevel.Debug, $"Get parameter with id = '{id}' success.");

			return new ResponseData { Data = parameterData };
		}

		ResponseData OnProcessGetItemByIdRequest(HttpListenerContext context, string id, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			if (string.IsNullOrEmpty(id))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, "OnProcessGetItemByIdRequest: item id wasn't specified.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = "Item id wasn't specified." };
			}

			var items = AccessControlDataCache.Instance.GetItems();
			var itemData = items.FirstOrDefault(i => i.Id == id);
			if (itemData == null)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Item with id = '{id}' wasn't found.");
				return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"Item with id = '{id}' wasn't found." };
			}

			var action = parameters.Get("action");
			if (action != null)
				return ExecuteItemAction(itemData, action, GetActionParameters(parameters));

			Logging.Logger.Log(NLog.LogLevel.Debug, $"Get item with id = '{id}' success.");

			return new ResponseData { Data = itemData };
		}

		ResponseData OnProcessGetStateByIdRequest(HttpListenerContext context, string id, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			if (string.IsNullOrEmpty(id))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, "OnProcessGetStateByIdRequest: state id wasn't specified.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = "State id wasn't specified." };
			}

			var states = AccessControlDataCache.Instance.GetItemsStates();
			foreach (var state in states)
			{
				if (state.Id == id)
				{
					Logging.Logger.Log(NLog.LogLevel.Debug, $"Get state with id = '{id}' success.");
					return new ResponseData { Data = state };
				}
			}

			Logging.Logger.Log(NLog.LogLevel.Error, $"State with id = '{id}' wasn't found.");

			return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"State with id = '{id}' wasn't found." };
		}

		ResponseData OnProcessGetEventByIdRequest(HttpListenerContext context, string id, NameValueCollection parameters, EventWaitHandle stopEventHandle)
		{
			if (string.IsNullOrEmpty(id))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, "OnProcessGetEventByIdRequest: event id wasn't specified.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = "Event id wasn't specified." };
			}

			var events = AccessControlDataCache.Instance.GetEvents();
			foreach (var e in events)
			{
				if (e.Id == id)
				{
					Logging.Logger.Log(NLog.LogLevel.Debug, $"Get event with id = '{id}' success.");
					return new ResponseData { Data = e };
				}
			}

			Logging.Logger.Log(NLog.LogLevel.Error, $"Event with id = '{id}' wasn't found.");

			return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"Event with id = '{id}' wasn't found." };
		}

		ResponseData ExecuteSystemAction(ActionData actionData, string action, Dictionary<string, string> actionParams)
		{
			Logging.Logger.Log(NLog.LogLevel.Debug, $"Try to '{action}' system action '{actionData.Title}'.");

			if (action != "trigger")
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Unsupported system action type: '{action}'.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = $"Unsupported system action type: '{action}'." };
			}

			if (actionData.Category != EntityData.SystemCategoryName)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Wrong system action category: '{actionData.Category}'. Category should be: '{EntityData.SystemCategoryName}'");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = $"Wrong system action category: '{actionData.Category}'. Category should be: '{EntityData.SystemCategoryName}'" };
			}

			if (!CheckActionParams(actionData, actionParams))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Wrong parameters specified for system action '{actionData.Title}'.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = $"Wrong parameters specified for system action '{actionData.Title}'." };
			}

			if (_accessControlAdapter.ExecuteSystemAction(actionData, actionParams, out var errorMessage))
				return new ResponseData { Error = (int)HttpStatusCode.OK, ErrorMessage = $"System action '{actionData.Title}' was triggered successfully." };

			return new ResponseData { Error = (int)HttpStatusCode.InternalServerError, ErrorMessage = errorMessage };
		}

		ResponseData ExecuteItemAction(ItemData itemData, string action, Dictionary<string, string> actionParams)
		{
			Logging.Logger.Log(NLog.LogLevel.Debug, $"Try to '{action}' item '{itemData.Title}'.");

			var category = EntityData.GetCategoryFromDataType(itemData.Type);

			var actionData = _accessControlAdapter.GetAction(action);
			if (actionData == null)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Can't find action: '{action}'.");
				return new ResponseData { Error = (int)HttpStatusCode.NotFound, ErrorMessage = $"Can't find action: '{action}'." };
			}

			if (actionData.Category != category)
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Item '{itemData.Title}' category: '{category}' differs from action category: '{actionData.Category}'.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = $"Item '{itemData.Title}' category: '{category}' differs from action category: '{actionData.Category}'." };
			}

			if (!CheckActionParams(actionData, actionParams))
			{
				Logging.Logger.Log(NLog.LogLevel.Error, $"Wrong parameters specified for action '{actionData.Title}'.");
				return new ResponseData { Error = (int)HttpStatusCode.BadRequest, ErrorMessage = $"Wrong parameters specified for action '{actionData.Title}'." };
			}

			if (_accessControlAdapter.ExecuteItemAction(actionData, itemData, actionParams, out var errorMessage))
			{
				var items = AccessControlDataCache.Instance.GetItems();
				var newItem = items.FirstOrDefault(i => i.Id == itemData.Id);
				if (newItem != null)
				{
					Logging.Logger.Log(NLog.LogLevel.Debug, $"'{action}' item '{itemData.Title}' was successfull.");
					return new ResponseData { Data = newItem };
				}

				Logging.Logger.Log(NLog.LogLevel.Error, $"Failed to update item '{itemData.Title}' data.");

				return new ResponseData { Error = (int)HttpStatusCode.InternalServerError, ErrorMessage = $"Failed to update item '{itemData.Title}' data." };
			}

			return new ResponseData { Error = (int)HttpStatusCode.InternalServerError, ErrorMessage = errorMessage };
		}



		static Dictionary<string, string> GetActionParameters(NameValueCollection parameters)
		{
			var actionParams = new Dictionary<string, string>();
			foreach (string key in parameters)
			{
				if (key == "action")
					continue;

				var value = parameters.Get(key);
				if (value != null)
					actionParams.Add(key, value);
			}

			return actionParams;
		}

		static bool CheckActionParams(ActionData actionData, Dictionary<string, string> actionParams)
		{
			if (actionData.Params.Count != actionParams.Count)
				return false;

			foreach (var key in actionData.Params)
				if (!actionParams.ContainsKey(key))
					return false;

			return true;
		}

		static uint GetMaxCount(uint itemCount, uint offset, uint count)
		{
			if (itemCount <= offset)
				return 0;

			if (itemCount <= offset + count)
				return itemCount - offset;

			return count;
		}

		static int GetKeepAliveValue(NameValueCollection parameters, int defaultValue, int minValue = 1, int maxValue = 60)
		{
			var keepAliveStr = parameters.Get("keepAlive");
			if (keepAliveStr == null)
				return defaultValue * 1000;

			if (!int.TryParse(keepAliveStr, out var keepAlive))
				return minValue;

			if (keepAlive < minValue)
				keepAlive = minValue;
			else if (keepAlive > maxValue)
				keepAlive = maxValue;

			return keepAlive * 1000;
		}


		static string? GetObjectIdFromUrlPath(string path)
		{
			var firstPosition = path.IndexOf('/');
			var lastPosition = path.LastIndexOf('/');

			if (firstPosition == lastPosition)
				return null;

			return path.Substring(lastPosition + 1);
		}



		delegate ResponseData HttpRequestHandler(HttpListenerContext context, NameValueCollection parameters, EventWaitHandle stopEventHandle);
		delegate ResponseData HttpRequestByIdHandler(HttpListenerContext context, string id, NameValueCollection parameters, EventWaitHandle stopEventHandle);

		readonly InfoData _infoData = new InfoData();
		readonly Dictionary<string, HttpRequestHandler> _requestHandlersMap = new Dictionary<string, HttpRequestHandler>();
		readonly Dictionary<string, HttpRequestByIdHandler> _requestByIdHandlersMap = new Dictionary<string, HttpRequestByIdHandler>();
		readonly IAccessControlAdapter _accessControlAdapter;
		readonly INotificationsManager _notificationsManager;

		readonly uint _responsePageSize;
	}
}
