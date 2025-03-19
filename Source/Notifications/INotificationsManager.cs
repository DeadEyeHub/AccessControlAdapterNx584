using AccessControlAdapterSample.AdapterData;

namespace AccessControlAdapterSample
{
	interface INotificationsManager
	{
		long RegisterNotificationQueue(Queue<NotificationData> queue);
		bool UnregisterNotificationQueue(long cookie);

		NotificationData? DequeueNotification(long cookie);

		bool IsNotificationQueueEmpty(long cookie);

		bool PushItemUpdatedNotification(EntityData.DataType itemType, EntityData? oldItem, EntityData? newItem);
		void PushAllItemsUpdatedNotification();

		void PushEventDataNotification(NotificationData.EventData eventData);

		ManualResetEvent NotificationReadyEvent { get; }
	}
}
