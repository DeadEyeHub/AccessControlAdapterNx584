using AccessControlAdapterSample.AdapterData;

namespace AccessControlAdapterSample.Notifications
{
	class NotificationsManager : INotificationsManager
	{
		#region INotificationsManager

		public long RegisterNotificationQueue(Queue<NotificationData> queue)
		{
			lock (_notificationsLock)
			{
				var cookie = ++_notificationQueueCookie;
				_notificationQueueMap[cookie] = queue;
				return cookie;
			}
		}

		public bool UnregisterNotificationQueue(long cookie)
		{
			lock (_notificationsLock)
			{
				return _notificationQueueMap.Remove(cookie);
			}
		}

		public NotificationData? DequeueNotification(long cookie)
		{
			lock (_notificationsLock)
			{
				if (_notificationQueueMap.TryGetValue(cookie, out var notificationQueue))
				{
					if (notificationQueue?.Count > 0)
						return notificationQueue.Dequeue();

					_notificationReadyEvent.Reset();
				}
			}

			return null;
		}

		public bool IsNotificationQueueEmpty(long cookie)
		{
			lock (_notificationsLock)
			{
				if (_notificationQueueMap.TryGetValue(cookie, out var notificationQueue))
					return notificationQueue.Count == 0;
			}

			return true;
		}

		public bool PushItemUpdatedNotification(EntityData.DataType itemType, EntityData? oldItem, EntityData? newItem)
		{
			if (oldItem != null || newItem != null)
			{
				var notificationData = new NotificationData();
				if (AddChangedItemToNotificationData(itemType, oldItem, newItem, notificationData))
					EnqueueNotification(notificationData);

				return true;
			}

			return false;
		}

		public void PushAllItemsUpdatedNotification()
		{
			var notificationData = new NotificationData();

			var itemsList = AccessControlDataCache.Instance.GetItems();
			var count = 0;
			foreach (var item in itemsList)
			{
				if (AddCurrentItemToNotificationData(item, notificationData))
					++count;
			}

			if (count > 0)
				EnqueueNotification(notificationData);
		}

		public void PushEventDataNotification(NotificationData.EventData eventData)
		{
			EnqueueNotification(new NotificationData { Events = new List<NotificationData.EventData> { eventData } });
		}

		public ManualResetEvent NotificationReadyEvent => _notificationReadyEvent;

		#endregion



		void EnqueueNotification(NotificationData notificationData)
		{
			lock (_notificationsLock)
			{
				if (_notificationQueueMap.Count > 0)
				{
					foreach (var notificationQueue in _notificationQueueMap)
						notificationQueue.Value.Enqueue(notificationData);

					_notificationReadyEvent.Set();
				}
			}
		}

		bool AddChangedItemToNotificationData(EntityData.DataType itemType, EntityData? oldItem, EntityData? newItem, NotificationData notificationData)
		{
			var itemUpdatedDataList = notificationData.GetItemUpdatedDataList(itemType);
			if (itemUpdatedDataList == null)
				return false;

			if (newItem != null && oldItem != null)
			{
				if (newItem.Id != oldItem.Id)
				{
					Logging.Logger.Error($"ID of {itemType} has been changed \"{oldItem.Id}\" => \"{newItem.Id}\"; this should never happen");
					return false;
				}

				if (oldItem.Type != newItem.Type)
				{
					Logging.Logger.Error($"Type of {itemType} has been changed \"{oldItem.Type}\" => \"{newItem.Type}\"; this should never happen");
					return false;
				}
			}

			var changed = newItem != null && oldItem != null;
			var itemUpdatedData = new NotificationData.ItemUpdatedData()
			{
				Id = changed ? newItem.Id : null,
				Category = changed ? EntityData.GetCategoryFromDataType(itemType) : null,
				NewData = (newItem as ICloneable)?.Clone(),
				OldData = (oldItem as ICloneable)?.Clone()
			};

			if (itemUpdatedData.OldData != null && itemUpdatedData.NewData != null)
				((EntityData)itemUpdatedData.OldData).RemoveEqualProperties((EntityData)itemUpdatedData.NewData);

			itemUpdatedDataList.Add(itemUpdatedData);

			return true;
		}

		bool AddCurrentItemToNotificationData(ItemData currentItem, NotificationData notificationData)
		{
			var itemUpdatedDataList = notificationData.GetItemUpdatedDataList(currentItem.Type);
			if (itemUpdatedDataList == null)
				return false;

			var itemUpdatedData = new NotificationData.ItemUpdatedData()
			{
				CurrentData = (currentItem as ICloneable).Clone()
			};

			itemUpdatedDataList.Add(itemUpdatedData);

			return true;
		}


		
		readonly Dictionary<long, Queue<NotificationData>> _notificationQueueMap = new Dictionary<long, Queue<NotificationData>>();
		readonly object _notificationsLock = new object();
		readonly ManualResetEvent _notificationReadyEvent = new ManualResetEvent(false);
		long _notificationQueueCookie = 0;
	}
}
