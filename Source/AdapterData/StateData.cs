using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class StateData : EntityData
	{
		public StateData() : base(DataType.State)
		{
		}



		[JsonProperty("title", Order = 2)]
		public string? Title { get; set; }

		[JsonProperty("localizationId", Order = 3)]
		public string? LocalizationId { get; set; }

		[JsonProperty("category", Order = 4)]
		public string? Category { get; set; }



		protected override void ResetEqualProperties(EntityData otherEntityData)
		{
			var otherState = (StateData)otherEntityData;

			if (Title == otherState.Title)
			{
				Title = null;
				otherState.Title = null;
			}

			if (LocalizationId == otherState.LocalizationId)
			{
				LocalizationId = null;
				otherState.LocalizationId = null;
			}

			if (Category == otherState.Category)
			{
				Category = null;
				otherState.Category = null;
			}
		}
	}
}
