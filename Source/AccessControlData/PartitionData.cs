using Newtonsoft.Json;

namespace AccessControlAdapterSample.AccessControlData
{
	class PartitionData
	{
        [JsonProperty("number")]
        public int Number { get; set; }
        
        [JsonProperty("condition_flags")]
        public HashSet<string>? ConditionFlags { get; set; }

        [JsonProperty("armed")]
        public bool Armed { get; set; }

        [JsonProperty("last_user")]
        public int user { get; set; }

    }
}
