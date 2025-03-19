using Newtonsoft.Json;

namespace AccessControlAdapterSample.AccessControlData
{
	class OutputData
	{
		[JsonProperty("id")]
		public string? Id { get; set; }

		[JsonProperty("title")]
		public string? Title { get; set; }

		[JsonProperty("states")]
		public string[] States { get; set; } = [];
	}
}
