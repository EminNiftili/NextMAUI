using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using NextMAUI.LocalNotification.Enums;
using NextMAUI.LocalNotification.Models;
using NextMAUI.LocalNotification.Platforms.Android.Models;
using NextMAUI.LocalNotification.Services;

namespace NextMAUI.LocalNotification.Platforms.Android.Services
{
    public class AndroidNotificationService : INotificationService
    {
        internal static Type NotificationIntentActivity;
        internal static Func<NotificationCompat.Builder, NotificationCompat.Builder> NotificationBuilder;
        private const string channelId = "default";
        private const string channelName = "Default";
        private const string channelDescription = "The default channel for notifications.";

        public const string NotificationKey = "local_android_notification";
        public const string NotificationValue = "local_android_notifaction_value_";

        private bool channelInitialized = false;
        private int messageId = 0;
        private int pendingIntentId = 0;

        NotificationManagerCompat compatManager;

        public event EventHandler<NotificationEventArgs> NotificationTapped;
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public AndroidNotificationService()
        {
            CreateNotificationChannel();
            compatManager = NotificationManagerCompat.From(Platform.AppContext);
        }

        public INextNotificationOptions CreateSimpleNotification(string title, string content, Dictionary<string, object> extraData = null)
        {
            var options = new AndroidNotificationOptions
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Body = content,
                NotificationType = NotificationType.SimpleNotification,
                ExtraDatas = extraData
            };

            return options;
        }
        public INextNotificationOptions CreateSimpleNotification(string title, string content, DateTime date, Dictionary<string, object> extraData = null)
        {
            var options = new AndroidNotificationOptions
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Body = content,
                NotificationType = NotificationType.SimpleNotificationWithTimer,
                ExtraDatas = extraData
            };

            return options;
        }

        public void ShowNotification(INextNotificationOptions options)
        {
            var androidOptions = options as AndroidNotificationOptions;
            if (androidOptions is null)
            {
                return;
            }
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.CancelCurrent;

            Intent intent;
            PendingIntent pendingIntent;

            if (androidOptions.NotificationType is NotificationType.SimpleNotificationWithTimer)
            {
                intent = new Intent(Platform.AppContext, typeof(string)); //AlarmHandler
                intent = CreateIntentByNotificationOption(androidOptions, intent);
                pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
                long triggerTime = GetNotifyTime(androidOptions.Timer.Value);
                AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
                alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
                return;
            }
            else
            {
                intent = new Intent(Platform.AppContext, NotificationIntentActivity); //MainActivity
                intent = CreateIntentByNotificationOption(androidOptions, intent);
                pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
            }
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, channelId);

            builder = NotificationBuilder.Invoke(builder);
            builder = builder.SetContentIntent(pendingIntent);

            if (androidOptions.NotificationType is NotificationType.SimpleNotification or NotificationType.SimpleNotificationWithTimer)
            {
                builder = builder.SetContentTitle(androidOptions.Title)
                                 .SetContentText(androidOptions.Body);
            }

            Notification notification = builder.Build();

            Show(notification, androidOptions);
        }
        public void CheckAndPrepareNotificationIntent(Intent intent)
        {
            if (intent == null || !intent.HasExtra(NotificationKey))
            {
                return;
            }
            var option = GetNotificationFromIntent(intent);
            NotificationTapped?.Invoke(this, new NotificationEventArgs(option));
        }

        private void CreateNotificationChannel()
        {
            // Create the notification channel, but only on API 26+.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);

#pragma warning disable CA1416 // Validate platform compatibility
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
#pragma warning restore CA1416 // Validate platform compatibility

                // Register the channel
                NotificationManager manager = (NotificationManager)Platform.AppContext.GetSystemService(Context.NotificationService);
                manager.CreateNotificationChannel(channel);
                channelInitialized = true;
            }
        }

        private long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }

        private void Show(Notification notification, AndroidNotificationOptions options)
        {
            NotificationReceived?.Invoke(this, new NotificationEventArgs(options));
            compatManager.Notify(messageId++, notification);
        }

        private AndroidNotificationOptions GetNotificationFromIntent(Intent intent)
        {
            var option = new AndroidNotificationOptions();
            option.Id = intent.GetStringExtra(NotificationKey);
            option.NotificationType = (NotificationType)intent.GetIntExtra($"{NotificationValue}{nameof(option.NotificationType)}", 0);
            if (intent.HasExtra($"{NotificationValue}{nameof(option.Title)}"))
            {
                option.Title = intent.GetStringExtra($"{NotificationValue}{nameof(option.Title)}");
            }
            if (intent.HasExtra($"{NotificationValue}{nameof(option.Body)}"))
            {
                option.Body = intent.GetStringExtra($"{NotificationValue}{nameof(option.Body)}");
            }
            if (intent.HasExtra($"{NotificationValue}{nameof(option.Timer)}"))
            {
                var timerAsString = intent.GetStringExtra($"{NotificationValue}{nameof(option.Timer)}");
                option.Timer = DateTime.Parse(timerAsString);
            }
            if (intent.HasExtra($"{NotificationValue}{nameof(option.ExtraDatas)}"))
            {
                var json = intent.GetStringExtra($"{NotificationValue}{nameof(option.ExtraDatas)}");
                
                option.ExtraDatas = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            }

            return option;
        }

        private Intent CreateIntentByNotificationOption(AndroidNotificationOptions option, Intent intent)
        {
            intent = intent.PutExtra(NotificationKey, option.Id);
            intent = intent.PutExtra($"{NotificationValue}{nameof(option.NotificationType)}", (int)option.NotificationType);
            if (option.NotificationType == NotificationType.SimpleNotification)
            {
                intent = intent.PutExtra($"{NotificationValue}{nameof(option.Title)}", option.Title);
                intent = intent.PutExtra($"{NotificationValue}{nameof(option.Body)}", option.Body);
            }
            if (option.NotificationType == NotificationType.SimpleNotificationWithTimer)
            {
                intent.PutExtra($"{NotificationValue}{nameof(option.Timer)}", option.Timer.ToString());
            }
            if (option.ExtraDatas != null && option.ExtraDatas.Count != 0)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(option.ExtraDatas);
                intent.PutExtra($"{NotificationValue}{nameof(option.ExtraDatas)}", json);
            }

            return intent;
        }
    }
}
