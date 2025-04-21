using NextMAUI.LocalNotification.Models;

namespace NextMAUI.LocalNotification.Services
{
    public interface INotificationService
    {
        public static INotificationService Current { get; protected set; }
        event EventHandler<NotificationEventArgs> NotificationTapped;
        event EventHandler<NotificationEventArgs> NotificationReceived;
        void ShowNotification(INextNotificationOptions notificationElement);
        INextNotificationOptions CreateSimpleNotification(string title, string content, Dictionary<string, object> extraData = null);
        INextNotificationOptions CreateSimpleNotification(string title, string content, DateTime timer, Dictionary<string, object> extraData = null);
    }
}
