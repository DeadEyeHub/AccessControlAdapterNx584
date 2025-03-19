using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class ResponseData
	{
		[JsonProperty("page", Order = 1)]
		public uint? Page { get; set; }

		[JsonProperty("nextPage", Order = 2)]
		public bool? NextPage { get; set; }

		[JsonProperty("error", Order = 3)]
		public int? Error { get; set; }

		[JsonProperty("errorMessage", Order = 4)]
		public string? ErrorMessage { get; set; }

		[JsonProperty("data", Order = 5)]
		public object? Data { get; set; }
	}
}
