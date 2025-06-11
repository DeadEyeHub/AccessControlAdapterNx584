using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class ItemData : EntityData, ICloneable, IEquatable<ItemData>
	{
		public ItemData(DataType itemType) : base(itemType)
		{
			Category = GetCategoryNameFromDataType(itemType);
		}



		[JsonProperty("title", Order = 2)]
		public string? Title { get; set; }

		[JsonProperty("category", Order = 3)]
		public string? Category { get; set; }

		[JsonProperty("states", Order = 4)]
		public Dictionary<string, bool?> States { get; set; } = new Dictionary<string, bool?>();
        
		[JsonProperty("conditionFlags", Order = 5)]
        public string[] ConditionFlags { get; set; } = [];

        [JsonProperty("typeFlags", Order = 6)]
        public string[] TypeFlags { get; set; } = [];



        public object Clone()
		{
			var result = new ItemData(Type);

			result.Id = Id?.Clone() as string;
			result.UtcTime = UtcTime;
			result.Title = Title?.Clone() as string;
			result.Category = Category?.Clone() as string;
			result.States = new Dictionary<string, bool?>(States);

			return result;
		}

		public bool Equals(ItemData? other)
		{
			if (other == null)
				return false;

			if (Type != other.Type)
				return false;

			if (Id != other.Id)
				return false;

			if (States.Count != other.States.Count)
				return false;

			foreach (var pair in States)
			{
				if (!other.States.TryGetValue(pair.Key, out var otherValue))
					return false;
				
				if (pair.Value.HasValue != otherValue.HasValue)
					return false;

				if (pair.Value.HasValue && otherValue.HasValue && pair.Value.Value != otherValue.Value)
					return false;
			}

			if (!string.Equals(Title, other.Title))
				return false;

			if (!string.Equals(Category, other.Category))
				return false;

			return true;
		}



		protected override void ResetEqualProperties(EntityData otherEntityData)
		{
			var otherItem = (ItemData)otherEntityData;

			if (Title == otherItem.Title)
			{
				Title = null;
				otherItem.Title = null;
			}

			if (Category == otherItem.Category)
			{
				Category = null;
				otherItem.Category = null;
			}

			var onlyThisStates = States.Except(otherItem.States).ToList();
			var onlyOtherStates = otherItem.States.Except(States).ToList();

			if (onlyThisStates.Count != States.Count)
				States = new Dictionary<string, bool?>(onlyThisStates);

			if (onlyOtherStates.Count != otherItem.States.Count)
				otherItem.States = new Dictionary<string, bool?>(onlyOtherStates);
		}



		static string GetCategoryNameFromDataType(DataType itemType)
		{
			switch (itemType)
			{
				case DataType.Door:
					return "door";
				case DataType.Partition:
					return "partition";
				case DataType.Zone:
					return "zone";
				case DataType.Output:
					return "output";
			}

			return string.Empty;
		}

	}
}
