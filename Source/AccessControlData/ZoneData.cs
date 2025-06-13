using Newtonsoft.Json;

namespace AccessControlAdapterSample.AccessControlData
{
    class ZoneData
    {
        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("state")]
        public bool State { get; set; }

        [JsonProperty("bypassed")]
        public bool Bypassed { get; set; }

        [JsonProperty("condition_flags")]
        public HashSet<string>? ConditionFlags { get; set; }

        [JsonProperty("type_flags", Required = Required.Always)]
        public HashSet<string>? TypeFlags { get; set; } = [];
    }
}