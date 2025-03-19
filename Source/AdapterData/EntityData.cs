using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	abstract class EntityData
	{
		public EntityData(DataType itemType)
		{
			Type = itemType;
			Id = $"{itemType.ToString().ToLower()}-{Guid.NewGuid()}";
			UtcTime = DateTime.UtcNow.ToFileTimeUtc();
		}



		public enum DataType
		{
			Unknown = -1,
			User = 1,
			Door = 0,
			Zone = 2,
			Partition = 3,
			Output = 4,
			Event = 5,
			Action = 6,
			Parameter = 7,
			State = 8,
		}

		[JsonIgnore]
		public DataType Type;

		[JsonProperty("id", Order = 1)]
		public string? Id { get; set; }

		[JsonProperty("utcTime", Order = 10)]
		public long? UtcTime { get; set; }



		public void RemoveEqualProperties(EntityData otherEntityData)
		{
			if (Id == otherEntityData.Id)
			{
				Id = null;
				otherEntityData.Id = null;
			}

			if (UtcTime == otherEntityData.UtcTime)
			{
				UtcTime = null;
				otherEntityData.UtcTime = null;
			}

			ResetEqualProperties(otherEntityData);
		}



		protected abstract void ResetEqualProperties(EntityData otherEntityData);



		public static string GetCategoryFromDataType(DataType type)
		{
			switch (type)
			{
				case DataType.Door:
					return DoorCategoryName;
				case DataType.Partition:
					return PartitionCategoryName;
				case DataType.Zone:
					return ZoneCategoryName;
				case DataType.Output:
					return OutputCategoryName;
				case DataType.User:
					return UserCategoryName;
			}

			return string.Empty;
		}



		public static readonly string DoorCategoryName = "door";
		public static readonly string SystemCategoryName = "system";
		public static readonly string PartitionCategoryName = "partition";
		public static readonly string ZoneCategoryName = "zone";
		public static readonly string OutputCategoryName = "output";
		public static readonly string UserCategoryName = "user";
	}
}
