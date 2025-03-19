using Newtonsoft.Json;
using System.Reflection;

namespace AccessControlAdapterSample.AdapterData
{
	class InfoData : ICloneable
	{
		public InfoData()
		{
			var asm = Assembly.GetExecutingAssembly().GetName();

			Title = asm.Name ?? string.Empty;
			Version = asm.Version?.ToString() ?? "1.0.0.0";

			var supportsList = new List<string>();

			// Support items
			supportsList.Add("doors");
			supportsList.Add("partitions");
			supportsList.Add("zones");
			supportsList.Add("outputs");
			ItemsUrl = "/items";

			// Support users
			supportsList.Add("users");
			UsersUrl = "/users";

			// Support notifications
			supportsList.Add("notifications");
			NotificationsUrl = "/notifications";

			// Support events
			supportsList.Add("events");
			EventsUrl = "/events";

			// Support actions
			supportsList.Add("actions");
			ActionsUrl = "/actions";

			// Support parameters
			supportsList.Add("parameters");
			ParametersUrl = "/parameters";

			// Support states
			supportsList.Add("states");
			StatesUrl = "/states";

			Supports = supportsList.ToArray();

			UpdateTime(DateTime.UtcNow);
		}



		#region ICloneable

		public object Clone()
		{
			var infoData = new InfoData();
			infoData.Title = (string)Title.Clone();
			infoData.Version = (string)Version.Clone();
			infoData.Supports = (string[])Supports.Clone();
			infoData.UtcTime = UtcTime;
			infoData.UtcTimeString = (string)UtcTimeString.Clone();
			infoData.UsersUrl = (string)UsersUrl.Clone();
			infoData.NotificationsUrl = (string)NotificationsUrl.Clone();
			infoData.EventsUrl = (string)EventsUrl.Clone();
			infoData.ActionsUrl = (string)ActionsUrl.Clone();
			infoData.ParametersUrl = (string)ParametersUrl.Clone();
			infoData.ItemsUrl = (string)ItemsUrl.Clone();
			infoData.StatesUrl = (string)StatesUrl.Clone();
			infoData.IntegrationId = (string)IntegrationId.Clone();

			return infoData;
		}

		#endregion



		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("supports")]
		public string[] Supports { get; set; }

		[JsonProperty("utcTime")]
		public long UtcTime { get; set; }

		[JsonProperty("utcTimeString")]
		public string UtcTimeString { get; set; } = string.Empty;

		[JsonProperty("usersUrl")]
		public string UsersUrl { get; set; }

		[JsonProperty("notificationsUrl")]
		public string NotificationsUrl { get; set; }

		[JsonProperty("eventsUrl")]
		public string EventsUrl { get; set; }

		[JsonProperty("actionsUrl")]
		public string ActionsUrl { get; set; }

		[JsonProperty("parametersUrl")]
		public string ParametersUrl { get; set; }

		[JsonProperty("itemsUrl")]
		public string ItemsUrl { get; set; }

		[JsonProperty("statesUrl")]
		public string StatesUrl { get; set; }

		[JsonProperty("integrationId")]
		public string IntegrationId { get; set; } = string.Empty;



		public void UpdateTime(DateTime utcNow)
		{
			UtcTime = utcNow.ToFileTimeUtc();
			UtcTimeString = utcNow.ToString("yyyy.MM.dd hh:mm:ss.fff");
		}
	}
}
