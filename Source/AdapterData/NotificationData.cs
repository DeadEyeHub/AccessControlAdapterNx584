using Newtonsoft.Json;
using System.Text;

namespace AccessControlAdapterSample.AdapterData
{
	class NotificationData
	{
		public class ItemUpdatedData
		{
			[JsonProperty("id", Order = 1)]
			public string? Id = null;

			[JsonProperty("category", Order = 2)]
			public string? Category = null;

			[JsonProperty("old", Order = 3)]
			public object? OldData = null;

			[JsonProperty("current", Order = 4)]
			public object? CurrentData = null;

			[JsonProperty("new", Order = 5)]
			public object? NewData = null;
		}

		public class EventData
		{
			[JsonProperty("id")]
			public string? Id = null;

			[JsonProperty("eventDescriptionId")]
			public string? EventDescriptionId = null;

			[JsonProperty("utcTime")]
			public long UtcTime = -1;

			[JsonProperty("objectId")]
			public string? ObjectId = null;

			[JsonProperty("objectType")]
			public string? ObjectType = null;

			[JsonProperty("actorId")]
			public string? ActorId = null;

			[JsonProperty("actorType")]
			public string? ActorType = null;

			[JsonProperty("eventText")]
			public string? EventText = null;

			[JsonProperty("cards")]
			public List<string> Cards = null;
		}

		[JsonProperty("updatedUsers")]
		public List<ItemUpdatedData>? UpdatedUsers = null;

		[JsonProperty("updatedEvents")]
		public List<ItemUpdatedData>? UpdatedEvents = null;

		[JsonProperty("updatedActions")]
		public List<ItemUpdatedData>? UpdatedActions = null;

		[JsonProperty("updatedParameters")]
		public List<ItemUpdatedData>? UpdatedParameters = null;

		[JsonProperty("updatedItems")]
		public List<ItemUpdatedData>? UpdatedItems = null;

		[JsonProperty("events")]
		public List<EventData>? Events = null;

		[JsonProperty("utcTime")]
		public long UtcTime { get; set; }

		public List<ItemUpdatedData>? GetItemUpdatedDataList(EntityData.DataType itemType)
		{
			switch (itemType)
			{
				case EntityData.DataType.Door:
				case EntityData.DataType.Zone:
				case EntityData.DataType.Partition:
				case EntityData.DataType.Output:
					return UpdatedItems ?? (UpdatedItems = new List<ItemUpdatedData>());

				case EntityData.DataType.User:
					return UpdatedUsers ?? (UpdatedUsers = new List<ItemUpdatedData>());

				case EntityData.DataType.Event:
					return UpdatedEvents ?? (UpdatedEvents = new List<ItemUpdatedData>());

				case EntityData.DataType.Action:
					return UpdatedActions ?? (UpdatedActions = new List<ItemUpdatedData>());

				case EntityData.DataType.Parameter:
					return UpdatedParameters ?? (UpdatedParameters = new List<ItemUpdatedData>());
			}

			return null;
		}

		public string Serialize()
		{
			// NOTE: serialize into server event data stream items
			// NOTE: see https://html.spec.whatwg.org/multipage/server-sent-events.html
			var serialized = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var lines = serialized.Split('\n');
			if (lines.Length == 0)
				return string.Empty;

			var builder = new StringBuilder();

			builder.Append("event:notify\n");
			foreach (var line in lines)
			{
				builder.Append("data:");
				builder.Append(line);
				builder.Append("\n");
			}
			builder.Append("\n");

			return builder.ToString();
		}
	}
}
