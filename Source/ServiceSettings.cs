using Newtonsoft.Json;

namespace AccessControlAdapterSample
{
	class ServiceSettings
	{
		[JsonProperty("logLevel")]
		public string LogLevel { get; set; } = string.Empty;

		[JsonProperty("logFile")]
		public string LogFile { get; set; } = string.Empty;

		[JsonProperty("accessControlHost")]
		public string AccessControlHost { get; set; } = string.Empty;

		[JsonProperty("accessControlPort")]
		public int AccessControlPort { get; set; }

		[JsonProperty("accessControlAdapterHttpPort")]
		public int AccessControlAdapterHttpPort { get; set; }

		[JsonProperty("accessControlAdapterHttpsPort")]
		public int AccessControlAdapterHttpsPort { get; set; }

		[JsonProperty("useHttpsServer")]
		public bool UseHttpsServer { get; set; }

		[JsonProperty("accessControlAdapterIntegrationId")]
		public string AccessControlAdapterIntegrationId { get; set; } = string.Empty;

		[JsonProperty("accessControlPollTimeout")]
		public int AccessControlPollTimeout { get; set; }

		[JsonProperty("responsePageSize")]
		public uint ResponsePageSize { get; set; }
	}
}
