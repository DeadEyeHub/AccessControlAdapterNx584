using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class ActionData : EntityData, ICloneable
	{
		public ActionData() : base(DataType.Action)
		{
		}



		[JsonProperty("title", Order = 2)]
		public string? Title { get; set; }

		[JsonProperty("localizationId", Order = 3)]
		public string? LocalizationId { get; set; }

		[JsonProperty("category", Order = 4)]
		public string? Category { get; set; }

		[JsonProperty("params", Order = 5)]
		public List<string> Params { get; set; } = new List<string>();



		public object Clone()
		{
			var result = new ActionData();

			result.Id = Id?.Clone() as string;
			result.UtcTime = UtcTime;
			result.Title = Title?.Clone() as string;
			result.LocalizationId = LocalizationId?.Clone() as string;
			result.Category = Category?.Clone() as string;
			result.Params = new List<string>(Params);

			return result;
		}



		protected override void ResetEqualProperties(EntityData otherEntityData)
		{
			var otherItem = (ActionData)otherEntityData;

			if (Title == otherItem.Title)
			{
				Title = null;
				otherItem.Title = null;
			}

			if (LocalizationId == otherItem.LocalizationId)
			{
				LocalizationId = null;
				otherItem.LocalizationId = null;
			}

			if (Category == otherItem.Category)
			{
				Category = null;
				otherItem.Category = null;
			}

			var onlyThisParams = Params.Except(otherItem.Params).ToList();
			var onlyOtherParams = otherItem.Params.Except(Params).ToList();

			if (onlyThisParams.Count != Params.Count)
				Params = new List<string>(onlyThisParams);

			if (onlyOtherParams.Count != otherItem.Params.Count)
				otherItem.Params = new List<string>(onlyOtherParams);
		}
	}
}
