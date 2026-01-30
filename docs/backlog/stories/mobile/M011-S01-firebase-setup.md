# Story M011-S01: Firebase Cloud Messaging Setup

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Entwickler** moechte ich **Firebase Cloud Messaging (FCM) in der App integrieren**, damit **die App Push-Benachrichtigungen empfangen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Android-Geraet, wenn die App gestartet wird, dann wird FCM initialisiert
- [ ] Gegeben FCM initialisiert, wenn ein Token generiert wird, dann ist dieser fuer die Registrierung verfuegbar
- [ ] Gegeben eine Push-Berechtigung noetig (Android 13+), wenn die App gestartet wird, dann wird die Berechtigung angefragt
- [ ] Gegeben eine eingehende Push-Nachricht, wenn die App im Vordergrund ist, dann wird sie verarbeitet
- [ ] Gegeben eine eingehende Push-Nachricht, wenn die App im Hintergrund ist, dann wird sie als System-Notification angezeigt

## Technische Setup

### Firebase Console
1. Projekt in Firebase Console erstellen
2. Android-App hinzufuegen mit Package Name
3. `google-services.json` herunterladen
4. Server Key fuer Backend notieren

### NuGet Packages
```xml
<ItemGroup>
    <PackageReference Include="Plugin.Firebase" Version="2.0.x" />
    <PackageReference Include="Plugin.Firebase.CloudMessaging" Version="2.0.x" />
</ItemGroup>
```

## Implementierungshinweise

### google-services.json platzieren
```
/Platforms/Android/google-services.json
```

### AndroidManifest.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />

    <application
        android:allowBackup="true"
        android:icon="@mipmap/appicon"
        android:roundIcon="@mipmap/appicon_round"
        android:supportsRtl="true">

        <!-- Firebase Cloud Messaging -->
        <receiver
            android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver"
            android:exported="false" />
        <receiver
            android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver"
            android:exported="true"
            android:permission="com.google.android.c2dm.permission.SEND">
            <intent-filter>
                <action android:name="com.google.android.c2dm.intent.RECEIVE" />
                <action android:name="com.google.android.c2dm.intent.REGISTRATION" />
                <category android:name="${applicationId}" />
            </intent-filter>
        </receiver>

    </application>
</manifest>
```

### MauiProgram.cs
```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .RegisterFirebaseServices();

    // Push Notification Service registrieren
    builder.Services.AddSingleton<IPushNotificationService, FirebasePushService>();

    return builder.Build();
}

public static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
{
    builder.ConfigureLifecycleEvents(events =>
    {
#if ANDROID
        events.AddAndroid(android => android.OnCreate((activity, bundle) =>
        {
            Firebase.FirebaseApp.InitializeApp(activity);
        }));
#endif
    });

    return builder;
}
```

### Push Notification Service
```csharp
public interface IPushNotificationService
{
    Task<string?> GetTokenAsync();
    Task<bool> RequestPermissionAsync();
    event EventHandler<PushNotificationReceivedEventArgs> NotificationReceived;
}

public class FirebasePushService : IPushNotificationService
{
    public event EventHandler<PushNotificationReceivedEventArgs>? NotificationReceived;

    public FirebasePushService()
    {
        CrossFirebaseCloudMessaging.Current.NotificationReceived += OnNotificationReceived;
        CrossFirebaseCloudMessaging.Current.TokenChanged += OnTokenChanged;
    }

    public async Task<string?> GetTokenAsync()
    {
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        return await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
    }

    public async Task<bool> RequestPermissionAsync()
    {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            return status == PermissionStatus.Granted;
        }
#endif
        return true;
    }

    private void OnNotificationReceived(object? sender, FCMNotificationReceivedEventArgs e)
    {
        var notification = new PushNotificationReceivedEventArgs
        {
            Title = e.Notification.Title,
            Body = e.Notification.Body,
            Data = e.Notification.Data
        };

        NotificationReceived?.Invoke(this, notification);
    }

    private void OnTokenChanged(object? sender, FCMTokenChangedEventArgs e)
    {
        // Token hat sich geaendert - Backend informieren
        WeakReferenceMessenger.Default.Send(new PushTokenChangedMessage(e.Token));
    }
}

public class PushNotificationReceivedEventArgs : EventArgs
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public IDictionary<string, string>? Data { get; set; }
}
```

### App.xaml.cs - Permission Request
```csharp
public partial class App : Application
{
    public App(IPushNotificationService pushService)
    {
        InitializeComponent();
        MainPage = new AppShell();

        // Push-Berechtigung anfragen nach kurzem Delay
        Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(2000); // Warten bis App geladen
            await pushService.RequestPermissionAsync();
        });
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | App-Start auf Android | FCM wird initialisiert |
| TC-002 | Token abrufen | Gueltiger FCM-Token wird zurueckgegeben |
| TC-003 | Android 13+: Berechtigung | Dialog wird angezeigt |
| TC-004 | Push im Vordergrund | Event wird ausgeloest |
| TC-005 | Push im Hintergrund | System-Notification erscheint |
| TC-006 | Token aendert sich | TokenChanged Event wird ausgeloest |

## Story Points

3

## Prioritaet

Hoch
