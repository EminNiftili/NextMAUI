using Foundation;
using NextMAUI.LocalNotification.Models;
using NextMAUI.LocalNotification.Platforms.iOS.Models;
using System.ComponentModel;
using UserNotifications;

namespace NextMAUI.LocalNotification.Platforms.iOS.Services
{
    [Preserve(AllMembers = true)]
    public class NotificationReceiver : UNUserNotificationCenterDelegate
    {
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            UNNotificationPresentationOptions? presentationOptions = null;
            if (OperatingSystem.IsIOSVersionAtLeast(14))
            {
                presentationOptions = UNNotificationPresentationOptions.Banner
                    | UNNotificationPresentationOptions.List
                    | UNNotificationPresentationOptions.Sound;

            }
            else
            {
                presentationOptions = UNNotificationPresentationOptions.Alert
                    | UNNotificationPresentationOptions.Sound;
            }

            completionHandler(presentationOptions.Value);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            if (response.IsDefaultAction)
                ProcessNotification(response.Notification);

            completionHandler();
        }

        void ProcessNotification(UNNotification notification)
        {
            IosNotificationOptions? option = null;
            if (notification.Request.Content.UserInfo.TryGetValue(new NSString("option"), out var nsObject))
            {
                option = IosNotificationOptions.FromNSObject(nsObject);
            }
            IosNotificationService.Instance?.ProceedNotificationTapped(new NotificationEventArgs(option));
        }
    }
}
