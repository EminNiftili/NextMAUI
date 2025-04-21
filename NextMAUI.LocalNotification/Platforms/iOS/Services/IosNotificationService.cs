using Foundation;
using NextMAUI.LocalNotification.Models;
using NextMAUI.LocalNotification.Platforms.iOS.Models;
using NextMAUI.LocalNotification.Services;
using UserNotifications;

namespace NextMAUI.LocalNotification.Platforms.iOS.Services
{

    [Preserve(AllMembers = true)]
    public class IosNotificationService : NSObject, INotificationService
    {
        internal static IosNotificationService Instance => INotificationService.Current as IosNotificationService;

        private int messageId = 0;

        public IosNotificationService()
        {
            INotificationService.Current = this;
            UNUserNotificationCenter.Current.Delegate = new NotificationReceiver();
        }

        public event EventHandler<NotificationEventArgs> NotificationTapped;
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public INextNotificationOptions CreateSimpleNotification(string title, string content, Dictionary<string, object> extraData = null)
        {
            var option = new IosNotificationOptions
            {
                Body = content,
                Title = title,
                MessageId = messageId++,
                NotificationType = Enums.NotificationType.SimpleNotification,
                ExtraDatas = extraData,
            };
            return option;
        }
        public INextNotificationOptions CreateSimpleNotification(string title, string content, DateTime timer, Dictionary<string, object> extraData = null)
        {
            var option = new IosNotificationOptions
            {
                Body = content,
                Title = title,
                MessageId = messageId++,
                NotificationType = Enums.NotificationType.SimpleNotification,
                Timer = timer,
                ExtraDatas = extraData,
            };
            return option;
        }

        public void ShowNotification(INextNotificationOptions options)
        {
            var iosOptions = options as IosNotificationOptions;
            if (iosOptions is null)
            {
                return;
            }

            var content = new UNMutableNotificationContent()
            {
                Badge = 1,
                Subtitle = "",
                UserInfo = NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { iosOptions.ToNSObject() },
                    new NSObject[] { new NSString("option") })
            };

            if (iosOptions.NotificationType is Enums.NotificationType.SimpleNotification or Enums.NotificationType.SimpleNotificationWithTimer)
            {
                content.Title = iosOptions.Title;
                content.Body = iosOptions.Body;
            }

            UNNotificationTrigger trigger;
            if (iosOptions.NotificationType is Enums.NotificationType.SimpleNotificationWithTimer)
                trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponents(iosOptions.Timer.Value), false);
            else
                trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);

            var request = UNNotificationRequest.FromIdentifier(iosOptions.MessageId.ToString(), content, trigger);

            ProceedNotificationReceived(new NotificationEventArgs(options));


            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                    throw new Exception($"Failed to schedule notification: {err}");
            });
        }

        private NSDateComponents GetNSDateComponents(DateTime dateTime)
        {
            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }

        internal void ProceedNotificationTapped(NotificationEventArgs eventArgs)
        {
            NotificationTapped?.Invoke(this, eventArgs);
        }
        internal void ProceedNotificationReceived(NotificationEventArgs eventArgs)
        {
            NotificationReceived?.Invoke(this, eventArgs);
        }
    }
}
