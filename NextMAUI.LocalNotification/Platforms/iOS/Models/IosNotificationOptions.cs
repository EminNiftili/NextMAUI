using Foundation;
using NextMAUI.LocalNotification.Enums;
using NextMAUI.LocalNotification.Models;

namespace NextMAUI.LocalNotification.Platforms.iOS.Models
{
    [Preserve(AllMembers = true)]
    public class IosNotificationOptions : NSObject, INextNotificationOptions
    {
        public NextNotificationType NotificationType { get; internal set; }
        public string Title { get; internal set; }
        public string Body { get; internal set; }
        public int MessageId { get; internal set; }
        public DateTime? Timer { get; internal set; }
        public Dictionary<string, object>? ExtraDatas { get; internal set; }

        public NSObject ToNSObject()
        {
            var dict = new NSMutableDictionary();

            dict.SetValueForKey(new NSString(NotificationType.ToString()), new NSString(nameof(NotificationType)));
            dict.SetValueForKey(new NSString(Title ?? ""), new NSString(nameof(Title)));
            dict.SetValueForKey(new NSString(Body ?? ""), new NSString(nameof(Body)));
            dict.SetValueForKey(NSNumber.FromInt32(MessageId), new NSString(nameof(MessageId)));

            if (Timer.HasValue)
            {
                var dateString = Timer.Value.ToString("yyyy-MM-dd'T'HH:mm:ss");
                dict.SetValueForKey(new NSString(dateString), new NSString(nameof(Timer)));
            }
            else
            {
                dict.SetValueForKey(NSNull.Null, new NSString(nameof(Timer)));
            }

            if (ExtraDatas != null && ExtraDatas.Count != 0)
            {
                var extraDataDict = new NSMutableDictionary<NSString, NSObject>();
                foreach (var data in ExtraDatas)
                {
                    extraDataDict.SetValueForKey(NSObject.FromObject(data.Value), new NSString(data.Key));
                }

                dict.SetValueForKey(extraDataDict, new NSString(nameof(ExtraDatas)));
            }
            else
            {
                dict.SetValueForKey(NSNull.Null, new NSString(nameof(Timer)));
            }
            return dict;
        }

        public static IosNotificationOptions FromNSObject(NSObject nsObject)
        {
            var dict = nsObject as NSDictionary;
            if (dict == null)
                return null;

            var options = new IosNotificationOptions();

            if (dict[nameof(NotificationType)] is NSString typeStr &&
                Enum.TryParse<NextNotificationType>(typeStr.ToString(), out var notificationType))
            {
                options.NotificationType = notificationType;
            }

            if (dict[nameof(Title)] is NSString title)
                options.Title = title.ToString();

            if (dict[nameof(Body)] is NSString body)
                options.Body = body.ToString();


            if (dict[nameof(MessageId)] is NSNumber messageId)
                options.MessageId = messageId.Int32Value;

            if (dict[nameof(Timer)] is NSString timer)
            {
                string dateString = timer.ToString();
                options.Timer = DateTime.Parse(dateString);
            }

            if (dict[nameof(ExtraDatas)] is NSMutableDictionary<NSString, NSObject> extraDatas)
            {
                var extraDatasDict = new Dictionary<string, object>();
                foreach (var extraData in extraDatas.Keys)
                {
                    extraDatasDict.Add(extraData.ToString(), extraDatas[extraData.ToString()]);
                }
                options.ExtraDatas = extraDatasDict;
            }

            return options;
        }
    }
}
