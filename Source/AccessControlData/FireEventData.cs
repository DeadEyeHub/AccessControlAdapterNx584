using Newtonsoft.Json;

namespace AccessControlAdapterSample.AccessControlData
{
	class FireEventData
	{
		[JsonProperty("id")]
		public string? EventId { get; set; }

		[JsonProperty("utcTime")]
		public long UtcTime { get; set; }

		[JsonProperty("cardHolderId")]
		public string? CardHolderId { get; set; }

		[JsonProperty("objectType")]
		public string? ObjectType { get; set; }

		[JsonProperty("objectId")]
		public string? ObjectId { get; set; }
	}
}
