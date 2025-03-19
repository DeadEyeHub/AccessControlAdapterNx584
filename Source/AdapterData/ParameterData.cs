using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class ParameterData : EntityData
	{
		public ParameterData() : base(DataType.Parameter)
		{
		}



		[JsonProperty("title", Order = 2)]
		public string Title { get; set; } = string.Empty;

		[JsonProperty("description", Order = 3)]
		public string Description { get; set; } = string.Empty;

		[JsonProperty("required", Order = 4)]
		public bool Required { get; set; }

		[JsonProperty("type", Order = 5)]
		public string ParamType { get; set; } = string.Empty;



		protected override void ResetEqualProperties(EntityData otherEntityData)
		{
			var otherItem = (ParameterData)otherEntityData;
		}
	}
}
