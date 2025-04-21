using NextMAUI.LocalNotification.Enums;

namespace NextMAUI.LocalNotification.Models
{
    public interface INextNotificationOptions
    {
        NotificationType NotificationType { get; }
        string Title { get; }
        string Body { get; }
        DateTime? Timer { get; }
        Dictionary<string, object> ExtraDatas { get; }
    }
}
