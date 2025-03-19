using AccessControlAdapterSample.AdapterData;

namespace AccessControlAdapterSample.Adapter
{
	interface IAccessControlAdapter
	{
		public Dictionary<string, ItemData> GetItems();
		public Dictionary<string, UserData> GetUsers();
		public Dictionary<string, EventData> GetEvents();
		public Dictionary<string, ActionData> GetActions();
		public Dictionary<string, ParameterData> GetParameters();

		public ActionData? GetAction(string id);
		public ParameterData? GetParameter(string id);

		public void FireAccessControlEvents();

		public bool ExecuteSystemAction(ActionData actionData, Dictionary<string, string> actionParams, out string errorMessage);
		public bool ExecuteItemAction(ActionData actionData, ItemData itemData, Dictionary<string, string> actionParams, out string errorMessage);
	}
}
