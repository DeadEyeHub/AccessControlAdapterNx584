using Newtonsoft.Json;

namespace AccessControlAdapterSample.AdapterData
{
	class UserData : EntityData
	{
		public UserData() : base(DataType.User)
		{
		}

		[JsonProperty("firstName", Order = 2)]
		public string? FirstName { get; set; }

		[JsonProperty("lastName", Order = 3)]
		public string? LastName { get; set; }

		[JsonProperty("cards", Order = 4)]
		public string[] Cards { get; set; } = [];

		[JsonProperty("photoBase64", Order = 5)]
		public string? PhotoBase64 { get; set; }



		protected override void ResetEqualProperties(EntityData otherEntityData)
		{
			var otherUser = (UserData)otherEntityData;

			if (FirstName == otherUser.FirstName)
			{
				FirstName = null;
				otherUser.FirstName = null;
			}

			if (LastName == otherUser.LastName)
			{
				LastName = null;
				otherUser.LastName = null;
			}

			if (PhotoBase64 == otherUser.PhotoBase64)
			{
				PhotoBase64 = null;
				otherUser.PhotoBase64 = null;
			}

			var onlyThisCards = Cards.Except(otherUser.Cards).ToList();
			var onlyOtherCards = otherUser.Cards.Except(Cards).ToList();

			if (onlyThisCards.Count != Cards.Length)
				Cards = onlyThisCards.ToArray();

			if (onlyOtherCards.Count != otherUser.Cards.Length)
				otherUser.Cards = onlyOtherCards.ToArray();
		}
	}
}
