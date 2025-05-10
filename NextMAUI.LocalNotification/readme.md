







# NextMAUI

**NextMAUI** is a cross-platform mobile application framework built using .NET MAUI. It offers a robust and extensible notification system out-of-the-box, making it easy to integrate local notifications in your mobile applications.

## 🔧 Features

- Cross-platform notification support (Android, iOS)
- Event-driven architecture for notification tap and receive
- Easy integration with `MauiAppBuilder`
- Supports extra metadata (key-value) in notifications
- Built-in scheduling for future alerts

---

## Basic setup
1. Install NextMAUI.LocalNotification
2. Setup in MauiProgram.cs
3. Setup MainActivity and android.manifest (Android)
4. Setup info.plist (iOS)
5. Enjoy


---

## 🚀 Getting Started

### Prerequisites

- Minumum [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

*SOON*


## 🔔 How to Use the Notification System

### Step 1: Register Notification Service

In `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

#if ANDROID
    builder.UseAdvancedNotificationForAndroid(
        typeof(MainActivity),
        builder =>
        {
            // Customize notification builder
        });
#endif

#if IOS
    builder.UseAdvancedNotificationForIos();
#endif

    return builder.Build();
}
```

---

### Step 2: Create a Notification

Simple notification:

```csharp
var notification = INextNotificationService.Current.CreateSimpleNotification(
    title: "Hello!",
    content: "This is a simple notification"
);
```

Scheduled notification:

```csharp
var notification = INextNotificationService.Current.CreateSimpleNotification(
    title: "Reminder",
    content: "This is a scheduled notification",
    timer: DateTime.Now.AddMinutes(5),
    extraData: new Dictionary<string, object> {
        { "userId", 123 },
        { "event", "Meeting" }
    }
);
```

---

### Step 3: Show the Notification

```csharp
INextNotificationService.Current.ShowNotification(notification);
```

---

### Step 4: Handle Events

```csharp
INextNotificationService.Current.NotificationTapped += (sender, e) =>
{
    var data = e.NotificationOption?.ExtraDatas;
    Console.WriteLine($"Tapped notification: {data?["event"]}");
};

INextNotificationService.Current.NotificationReceived += (sender, e) =>
{
    Console.WriteLine($"Received: {e.NotificationOption?.Title}");
};
```

---

## 📘 API Overview

### `INextNotificationService`

```csharp
interface INextNotificationService
{
    event EventHandler<NotificationEventArgs> NotificationTapped;
    event EventHandler<NotificationEventArgs> NotificationReceived;
    void ShowNotification(INextNotificationOptions notificationElement);
    INextNotificationOptions CreateSimpleNotification(string title, string content, Dictionary<string, object> extraData = null);
    INextNotificationOptions CreateSimpleNotification(string title, string content, DateTime timer, Dictionary<string, object> extraData = null);
}
```

### `INextNotificationOptions`

```csharp
interface INextNotificationOptions
{
    NotificationType NotificationType { get; }
    string Title { get; }
    string Body { get; }
    DateTime? Timer { get; }
    Dictionary<string, object> ExtraDatas { get; }
}
```

### `NextNotificationType`

```csharp
enum NextNotificationType
{
    SimpleNotification = 1,
    SimpleNotificationWithTimer = 2
}
```

---

## 📄 License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## 🙌 Author

Created and maintained by [Emin Niftili](https://github.com/EminNiftili)
