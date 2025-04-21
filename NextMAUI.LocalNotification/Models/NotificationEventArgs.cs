namespace NextMAUI.LocalNotification.Models
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(INextNotificationOptions? option)
        {
            NotificationOption = option;
        }

        public INextNotificationOptions? NotificationOption { get; private set; }
    }
}
