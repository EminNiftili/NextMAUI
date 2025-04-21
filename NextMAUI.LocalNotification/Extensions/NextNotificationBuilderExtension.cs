using NextMAUI.LocalNotification.Services;
#if ANDROID
using NextMAUI.LocalNotification.Platforms.Android.Services;
#elif IOS
using NextMAUI.LocalNotification.Platforms.iOS.Services;
using UserNotifications;
#endif

namespace NextMAUI.LocalNotification.Extensions
{
    public static class NextNotificationBuilderExtension
    {
#if ANDROID
        public static MauiAppBuilder UseAdvancedNotificationForAndroid(this MauiAppBuilder builder, Type notificationIntentType, Action<AndroidX.Core.App.NotificationCompat.Builder> action)
        {
            var ns = new AndroidNotificationService();
            builder.Services.AddSingleton<INextNotificationService>(ns);
            AndroidNotificationService.NotificationIntentActivity = notificationIntentType;
            AndroidNotificationService.NotificationBuilder = (AndroidX.Core.App.NotificationCompat.Builder notBuilder) =>
            {
                action(notBuilder);
                return notBuilder;
            };
            return builder;
        }
#endif

#if IOS
        public static MauiAppBuilder UseAdvancedNotificationForIos(this MauiAppBuilder builder)
        {
            var ns = new IosNotificationService();
            builder.Services.AddSingleton<INextNotificationService>(ns);
            return builder;
        }
#endif
    }
}
