using NextMAUI.LocalNotification.Enums;
using NextMAUI.LocalNotification.Models;

namespace NextMAUI.LocalNotification.Platforms.Android.Models
{
    public class AndroidNotificationOptions : INextNotificationOptions
    {
        public string Id { get; internal set; }
        public NotificationType NotificationType { get; internal set; }
        public string Title { get; internal set; }
        public string Body { get; internal set; }
        public DateTime? Timer { get; internal set; }

        public Dictionary<string, object>? ExtraDatas { get; internal set; }
    }
}
