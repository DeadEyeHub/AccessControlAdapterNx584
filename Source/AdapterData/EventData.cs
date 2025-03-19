using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class EventData : EntityData
	{
		public EventData() : base(DataType.Event)
		{
		}



		[JsonProperty("description", Order = 2)]
		public string? Description { get; set; }

		[JsonProperty("category", Order = 3)]
		public string? Category { get; set; }



		protected override void ResetEqualProperties(EntityData otherEntityData)
		{
			var otherEvent = (EventData)otherEntityData;

			if (Description == otherEvent.Description)
			{
				Description = null;
				otherEvent.Description = null;
			}

			if (Category == otherEvent.Description)
			{
				Category = null;
				otherEvent.Category = null;
			}
		}
	}
}
