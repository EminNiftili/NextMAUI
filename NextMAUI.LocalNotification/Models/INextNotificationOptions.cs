using NextMAUI.LocalNotification.Enums;

namespace NextMAUI.LocalNotification.Models
{
    public interface INextNotificationOptions
    {
        NextNotificationType NotificationType { get; }
        string Title { get; }
        string Body { get; }
        DateTime? Timer { get; }
        Dictionary<string, object> ExtraDatas { get; }
    }
}
