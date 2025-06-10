using AccessControlAdapterSample.AccessControlData;
using AccessControlAdapterSample.AdapterData;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using LogLevel = NLog.LogLevel;

namespace AccessControlAdapterSample.Adapter
{
	class AccessControlAdapter : IAccessControlAdapter
	{
		public AccessControlAdapter(Uri accessControlAddress, INotificationsManager notificationsManager)
		{
			_accessControlAddress = accessControlAddress;
			_notificationsManager = notificationsManager;

			CreateActionsAndParameters();
		}



		#region IAccessControlDataConverter

		public Dictionary<string, ItemData> GetItems()
		{
			var itemsMap = new Dictionary<string, ItemData>();

			//var doors = RequestDataArray<DoorData>("/doors");
			//foreach (var door in doors)
			//	if (door.Id != null)
			//		itemsMap[door.Id] = CreateDoorItem(door);

			var zonesJson = RequestJson("/zones");
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<ZoneData>>>(zonesJson);
            List<ZoneData> zones = jsonObject["zones"];
            foreach (var zone in zones)
				itemsMap[zone.Number.ToString()] = CreateZoneItem(zone);

			//var partitions = RequestDataArray<PartitionData>("/partitions");
			//foreach (var partition in partitions)
			//	if (partition.Id != null)
			//		itemsMap[partition.Id] = CreatePartitionItem(partition);

			//var outputs = RequestDataArray<OutputData>("/outputs");
			//foreach (var output in outputs)
			//	if (output.Id != null)
			//		itemsMap[output.Id] = CreateOutputItem(output);

			return itemsMap;
		}

		public Dictionary<string, UserData> GetUsers()
		{
			//var cardHolders = RequestDataArray<CardHolderData>("/cardholders");

			var usersMap = new Dictionary<string, UserData>();
			//foreach (var cardHolder in cardHolders)
			//{
			//	if (cardHolder.Id == null)
			//		continue;

			//	var user = CreateUser(cardHolder);
			//	usersMap[cardHolder.Id] = user;
			//}

			return usersMap;
		}

		public Dictionary<string, EventData> GetEvents()
		{
			//var events = RequestDataArray<EventDescData>("/events");

			var eventsMap = new Dictionary<string, EventData>();
			//foreach (var e in events)
			//{
			//	if (e.Id == null)
			//		continue;

			//	var eventData = CreateEvent(e);
			//	eventsMap[e.Id] = eventData;
			//}

			return eventsMap;
		}

		public Dictionary<string, ActionData> GetActions()
		{
			return _actionsMap;
		}

		public Dictionary<string, ParameterData> GetParameters()
		{
			return _parametersMap;
		}

		public ActionData? GetAction(string id)
		{
			return _actionsMap.GetValueOrDefault(id);
		}

		public ParameterData? GetParameter(string id)
		{
			return _parametersMap.GetValueOrDefault(id);
		}

		public void FireAccessControlEvents()
		{
			//var firedEvents = RequestDataArray<FireEventData>("/fired_events");

			// Fire received events
			//foreach (var firedEvent in firedEvents)
			//{
			//	var eventDesc = AccessControlDataCache.Instance.GetEvent(firedEvent.EventId);
			//	if (eventDesc == null)
			//		continue; // Can't find event description with specified id

			//	var eventData = new NotificationData.EventData();

			//	eventData.Id = Guid.NewGuid().ToString();
			//	eventData.EventDescriptionId = firedEvent.EventId;
			//	eventData.EventText = eventDesc.Description;
			//	eventData.UtcTime = firedEvent.UtcTime;

			//	var item = AccessControlDataCache.Instance.GetItem(firedEvent.ObjectType, firedEvent.ObjectId);
			//	if (null != item)
			//	{
			//		eventData.ObjectId = item.Id;
			//		eventData.ObjectType = item.Category;
			//	}
			//	else
			//	{
			//		eventData.ObjectId = "";
			//		eventData.ObjectType = EntityData.SystemCategoryName;
			//	}

			//	var user = AccessControlDataCache.Instance.GetUser(firedEvent.CardHolderId);
			//	if (user != null)
			//	{
			//		eventData.ActorId = user.Id;
			//		eventData.ActorType = EntityData.UserCategoryName;
			//		eventData.Cards = new List<string>(user.Cards);
			//	}

			//	_notificationsManager.PushEventDataNotification(eventData);
			//}
		}

		public bool ExecuteSystemAction(ActionData actionData, Dictionary<string, string> actionParams, out string errorMessage)
		{
			//string requestPath;
			//if (actionData.Id == "systemAction")
			//	requestPath = "/system_action";
			//else
			//{
			//	errorMessage = $"No execution steps was specified for the system action '{actionData.Title}'.";
			//	Logging.Logger.Log(NLog.LogLevel.Error, errorMessage);

			//	return false;
			//}

			//var response = SendActionRequest(requestPath, actionParams);
			//if (response.StatusCode == HttpStatusCode.OK)
			//{
			//	errorMessage = "";
			//	Logging.Logger.Log(NLog.LogLevel.Debug, $"System action '{actionData.Title}' was triggered successfully.");

			//	return true;
			//}

			//errorMessage =
			//	$"Failed to trigger the system action '{actionData.Title}' with error code = {response.StatusCode}.";
			//Logging.Logger.Log(NLog.LogLevel.Error, errorMessage);

			errorMessage = "System actions are not supported in this adapter version.";
            return false;
		}

		public bool ExecuteItemAction(ActionData actionData, ItemData itemData, Dictionary<string, string> actionParams, out string errorMessage)
		{
            //string requestPath;
            //if (actionData.Id == "partitionArm")
            //	requestPath = $"/partition/{itemData.Id}/arm";
            //else if (actionData.Id == "partitionDisarm")
            //	requestPath = $"/partition/{itemData.Id}/disarm";
            //else if (actionData.Id == "doorLock")
            //	requestPath = $"/door/{itemData.Id}/lock";
            //else if (actionData.Id == "doorUnlock")
            //	requestPath = $"/door/{itemData.Id}/unlock";
            //else
            //{
            //	errorMessage = $"No execution steps was specified for the action '{actionData.Title}' for item '{itemData.Title}'.";
            //	Logging.Logger.Log(NLog.LogLevel.Error, errorMessage);
            //	return false;
            //}

            //var response = SendActionRequest(requestPath, actionParams);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //	using (var stream = response.Content.ReadAsStream())
            //	{
            //		var buffer = new byte[stream.Length];
            //		var bytesRead = stream.Read(buffer, 0, buffer.Length);
            //		if (bytesRead != buffer.Length)
            //		{
            //			errorMessage =
            //				$"Trigger the action '{actionData.Title}' for item '{itemData.Title}' response read error. Was read {bytesRead} instead of {buffer.Length} bytes.";
            //			Logging.Logger.Log(NLog.LogLevel.Error, errorMessage);

            //			return false;
            //		}

            //		var json = Encoding.UTF8.GetString(buffer);
            //		UpdateItemData(json, itemData);

            //		errorMessage = "";

            //		return true;
            //	}
            //}

            //errorMessage = $"Failed to trigger the action '{actionData.Title}' for item '{itemData.Title}' with error code = {response.StatusCode}.";
            //Logging.Logger.Log(NLog.LogLevel.Error, errorMessage);
            errorMessage = "System actions are not supported in this adapter version.";
            return false;
		}

		#endregion



		T[] RequestDataArray<T>(string requestPath)
		{
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Get, new Uri(_accessControlAddress, requestPath)))
				{
					using (var response = _httpClient.Send(message))
					{
						if (response.StatusCode == HttpStatusCode.OK)
						{
							using (var stream = response.Content.ReadAsStream())
							{
								var buffer = new byte[stream.Length];
								var bytesRead = stream.Read(buffer, 0, buffer.Length);
								if (bytesRead != buffer.Length)
									throw new Exception($"Failed to read data stream. Was read {bytesRead} instead of {buffer.Length} bytes.");

								var json = Encoding.UTF8.GetString(buffer);
								var dataList = JsonConvert.DeserializeObject<List<T>>(json);

                                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<ZoneData>>>(json);
                                List<ZoneData> zones = jsonObject["zones"];


                                return dataList?.ToArray() ?? [];
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logger.Log(LogLevel.Error, $"RequestDataArray exception was caught: '{ex.Message}'");
			}

			return [];
		}

        string RequestJson(string requestPath)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Get, new Uri(_accessControlAddress, requestPath)))
                {
                    using (var response = _httpClient.Send(message))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = response.Content.ReadAsStream())
                            {
                                var buffer = new byte[stream.Length];
                                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                                if (bytesRead != buffer.Length)
                                    throw new Exception($"Failed to read data stream. Was read {bytesRead} instead of {buffer.Length} bytes.");

                                var json = Encoding.UTF8.GetString(buffer);
								return json;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(LogLevel.Error, $"RequestDataArray exception was caught: '{ex.Message}'");
            }

            return string.Empty;
        }
        HttpResponseMessage SendActionRequest(string requestPath, Dictionary<string, string> actionParams)
		{
			try
			{
				var requestPathWithParameters = requestPath;
				if (actionParams.Any())
				{
					requestPathWithParameters += "?";
					foreach (var param in actionParams)
						requestPathWithParameters += $"{param.Key}={param.Value}";
				}

				using (var message = new HttpRequestMessage(HttpMethod.Post, new Uri(_accessControlAddress, requestPathWithParameters)))
				{
					return _httpClient.Send(message);
				}
			}
			catch (Exception ex)
			{
				Logging.Logger.Log(LogLevel.Error, $"RequestDataArray exception was caught: '{ex.Message}'");
			}

			return new HttpResponseMessage(HttpStatusCode.InternalServerError);
		}

		ItemData CreateDoorItem(DoorData door)
		{
			var item = new ItemData(EntityData.DataType.Door) { Title = door.Title, Id = door.Id };

			foreach (var state in PossibleDoorStates)
				item.States[state.Item1] = false;

			foreach (var state in door.States)
				item.States[state] = true;

			return item;
		}

		ItemData CreateZoneItem(ZoneData zone)
		{
			var item = new ItemData(EntityData.DataType.Zone) { Title = zone.Name, Id = zone.Number.ToString() };

			foreach (var state in PossibleZoneStates)
				item.States[state.Item1] = false;

			item.States["zoneBypassed"] = zone.Bypassed;
            item.States["zoneReady"] = !zone.State;
            return item;
		}

		ItemData CreatePartitionItem(PartitionData partition)
		{
			var item = new ItemData(EntityData.DataType.Partition) { Title = partition.Title, Id = partition.Id };

			foreach (var state in PossiblePartitionStates)
				item.States[state.Item1] = false;

			foreach (var state in partition.States)
				item.States[state] = true;

			return item;
		}

		ItemData CreateOutputItem(OutputData output)
		{
			var item = new ItemData(EntityData.DataType.Output) { Title = output.Title, Id = output.Id };

			foreach (var state in PossibleOutputStates)
				item.States[state.Item1] = false;

			foreach (var state in output.States)
				item.States[state] = true;

			return item;
		}

		UserData CreateUser(CardHolderData cardHolder)
		{
			return new UserData
			{
				Id = cardHolder.Id,
				FirstName = cardHolder.FirstName,
				LastName = cardHolder.LastName
			};
		}

		EventData CreateEvent(EventDescData e)
		{
			return new EventData
			{
				Id = e.Id,
				Description = e.Description,
				Category = e.Category
			};
		}

		void CreateActionsAndParameters()
		{
			var pinParam = new ParameterData
			{
				Id = "pin",
				Title = "PIN",
				Description = "PIN for a command",
				Required = true,
				ParamType = "password"
			};

			_parametersMap.Add(pinParam.Id, pinParam);

			// Partition actions
			var partitionArm = new ActionData
			{
				Id = "partitionArm",
				Title = "Arm",
				LocalizationId = "#PartitionActionArm",
				Category = EntityData.PartitionCategoryName,
				Params = ["pin"]
			};

			var partitionDisarm = new ActionData
			{
				Id = "partitionDisarm",
				Title = "Disarm",
				LocalizationId = "#PartitionActionDisarm",
				Category = EntityData.PartitionCategoryName,
				Params = ["pin"]
			};

			_actionsMap.Add(partitionArm.Id, partitionArm);
			_actionsMap.Add(partitionDisarm.Id, partitionDisarm);

			// Door actions
			var doorLock = new ActionData
			{
				Id = "doorLock",
				Title = "Lock",
				LocalizationId = "#DoorActionLock",
				Category = EntityData.DoorCategoryName
			};

			var doorUnlock = new ActionData
			{
				Id = "doorUnlock",
				Title = "Unlock",
				LocalizationId = "#DoorActionUnlock",
				Category = EntityData.DoorCategoryName
			};

			_actionsMap.Add(doorLock.Id, doorLock);
			_actionsMap.Add(doorUnlock.Id, doorUnlock);

			// System actions
			var systemAction = new ActionData
			{
				Id = "systemAction",
				Title = "System Action",
				Category = EntityData.SystemCategoryName
			};

			_actionsMap.Add(systemAction.Id, systemAction);
		}

		void UpdateItemData(string json, ItemData itemData)
		{
			if (itemData.Type == EntityData.DataType.Door)
			{
				var newDoorData = JsonConvert.DeserializeObject<DoorData>(json);
				if (newDoorData != null)
					AccessControlDataCache.Instance.UpdateItem(CreateDoorItem(newDoorData), _notificationsManager);
			}
			else if (itemData.Type == EntityData.DataType.Zone)
			{
				var newZoneData = JsonConvert.DeserializeObject<ZoneData>(json);
				if (newZoneData != null)
					AccessControlDataCache.Instance.UpdateItem(CreateZoneItem(newZoneData), _notificationsManager);
			}
			else if (itemData.Type == EntityData.DataType.Partition)
			{
				var newPartitionData = JsonConvert.DeserializeObject<PartitionData>(json);
				if (newPartitionData != null)
					AccessControlDataCache.Instance.UpdateItem(CreatePartitionItem(newPartitionData), _notificationsManager);
			}
			else if (itemData.Type == EntityData.DataType.Output)
			{
				var newOutputData = JsonConvert.DeserializeObject<OutputData>(json);
				if (newOutputData != null)
					AccessControlDataCache.Instance.UpdateItem(CreateOutputItem(newOutputData), _notificationsManager);
			}
			else if (itemData.Type == EntityData.DataType.User)
			{
				var newCardHolderData = JsonConvert.DeserializeObject<CardHolderData>(json);
				if (newCardHolderData != null)
					AccessControlDataCache.Instance.UpdateUser(CreateUser(newCardHolderData));
			}
		}



		readonly Uri _accessControlAddress;
		readonly INotificationsManager _notificationsManager;
		readonly Dictionary<string, ActionData> _actionsMap = new Dictionary<string, ActionData>();
		readonly Dictionary<string, ParameterData> _parametersMap = new Dictionary<string, ParameterData>();
		readonly HttpClient _httpClient = new HttpClient();



		static readonly Tuple<string, string>[] PossibleDoorStates =
		[
			new Tuple<string, string>("opened", "#DoorStatusOpen"),
			new Tuple<string, string>("closed", "#DoorStatusClosed"),
		];

		static readonly Tuple<string, string>[] PossibleZoneStates =
		[
			new Tuple<string, string>("zoneReady", "#StatusReady"),
			new Tuple<string, string>("zoneBypassed", "")
		];

		static readonly Tuple<string, string>[] PossiblePartitionStates =
		[
			new Tuple<string, string>("partitionArmed", "#StatusArmed"),
			new Tuple<string, string>("partitionDisarmed", ""),
			new Tuple<string, string>("partitionInAlarm", "#StatusAlarmed"),
			new Tuple<string, string>("partitionReady", "#StatusReady")
		];

		static readonly Tuple<string, string>[] PossibleOutputStates =
		[
		];
	}
}
