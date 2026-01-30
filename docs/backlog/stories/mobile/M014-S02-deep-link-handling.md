# Story M014-S02: Deep-Link Handling

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **Benutzer** moechte ich **Links anklicken koennen, die mich direkt zu einer bestimmten Stelle in der App bringen**, damit **ich z.B. eine Einladung oder Benachrichtigung schnell oeffnen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Einladungs-Link, wenn der Benutzer ihn anklickt, dann wird er zur Einladungs-Annahme Seite geleitet
- [ ] Gegeben ein Geldanfrage-Link, wenn der Benutzer ihn anklickt, dann wird er zur Anfrage-Detail Seite geleitet
- [ ] Gegeben ein Deep-Link, wenn der Benutzer nicht eingeloggt ist, dann wird er erst zum Login geleitet und danach zum Ziel
- [ ] Gegeben ein ungueltiger Deep-Link, wenn er geoeffnet wird, dann wird eine Fehlermeldung angezeigt

## Deep-Link Schema

```
taschengeld://invite/{token}           -> Einladung annehmen
taschengeld://request/{requestId}      -> Geldanfrage anzeigen
taschengeld://transaction/{txId}       -> Transaktion anzeigen
taschengeld://gift/{giftId}            -> Geschenk anzeigen
taschengeld://email-verify/{token}     -> Email verifizieren
```

## Technische Implementierung

### Android: Intent Filter (AndroidManifest.xml)

```xml
<activity android:name=".MainActivity"
          android:exported="true"
          android:launchMode="singleTask">

    <!-- Custom URL Scheme -->
    <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="taschengeld" />
    </intent-filter>

    <!-- HTTPS Deep Links (Android App Links) -->
    <intent-filter android:autoVerify="true">
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="https"
              android:host="taschengeld.app"
              android:pathPrefix="/app" />
    </intent-filter>

</activity>
```

### Deep-Link Service

```csharp
public interface IDeepLinkService
{
    Task HandleDeepLinkAsync(Uri uri);
}

public class DeepLinkService : IDeepLinkService
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    private Uri? _pendingDeepLink;

    public async Task HandleDeepLinkAsync(Uri uri)
    {
        // Wenn nicht eingeloggt, speichern und spaeter verarbeiten
        if (!_authService.IsAuthenticated)
        {
            _pendingDeepLink = uri;
            await _navigationService.NavigateToLoginAsync();
            return;
        }

        await ProcessDeepLinkAsync(uri);
    }

    public async Task ProcessPendingDeepLinkAsync()
    {
        if (_pendingDeepLink != null)
        {
            var uri = _pendingDeepLink;
            _pendingDeepLink = null;
            await ProcessDeepLinkAsync(uri);
        }
    }

    private async Task ProcessDeepLinkAsync(Uri uri)
    {
        try
        {
            var host = uri.Host;
            var path = uri.AbsolutePath.TrimStart('/');
            var segments = path.Split('/');

            switch (host)
            {
                case "invite":
                    await HandleInviteAsync(segments.FirstOrDefault());
                    break;

                case "request":
                    await HandleRequestAsync(segments.FirstOrDefault());
                    break;

                case "transaction":
                    await HandleTransactionAsync(segments.FirstOrDefault());
                    break;

                case "gift":
                    await HandleGiftAsync(segments.FirstOrDefault());
                    break;

                case "email-verify":
                    await HandleEmailVerifyAsync(segments.FirstOrDefault());
                    break;

                default:
                    await _toastService.ShowErrorAsync("Unbekannter Link");
                    break;
            }
        }
        catch (Exception ex)
        {
            await _toastService.ShowErrorAsync("Link konnte nicht verarbeitet werden");
        }
    }

    private async Task HandleInviteAsync(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            await _toastService.ShowErrorAsync("Ungueltiger Einladungslink");
            return;
        }

        await Shell.Current.GoToAsync($"//invite?token={token}");
    }

    private async Task HandleRequestAsync(string? requestId)
    {
        if (string.IsNullOrEmpty(requestId) || !Guid.TryParse(requestId, out _))
        {
            await _toastService.ShowErrorAsync("Ungueltige Anfrage-ID");
            return;
        }

        await Shell.Current.GoToAsync($"//requests/detail?id={requestId}");
    }

    private async Task HandleTransactionAsync(string? transactionId)
    {
        if (string.IsNullOrEmpty(transactionId) || !Guid.TryParse(transactionId, out _))
        {
            await _toastService.ShowErrorAsync("Ungueltige Transaktions-ID");
            return;
        }

        await Shell.Current.GoToAsync($"//transactions/detail?id={transactionId}");
    }

    private async Task HandleGiftAsync(string? giftId)
    {
        if (string.IsNullOrEmpty(giftId) || !Guid.TryParse(giftId, out _))
        {
            await _toastService.ShowErrorAsync("Ungueltige Geschenk-ID");
            return;
        }

        await Shell.Current.GoToAsync($"//gifts/detail?id={giftId}");
    }

    private async Task HandleEmailVerifyAsync(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            await _toastService.ShowErrorAsync("Ungueltiger Verifizierungslink");
            return;
        }

        await Shell.Current.GoToAsync($"//email-verify?token={token}");
    }
}
```

### App.xaml.cs Integration

```csharp
public partial class App : Application
{
    private readonly IDeepLinkService _deepLinkService;

    public App(IDeepLinkService deepLinkService)
    {
        InitializeComponent();
        _deepLinkService = deepLinkService;
    }

    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _deepLinkService.HandleDeepLinkAsync(uri);
        });
    }
}

// Nach erfolgreichem Login
public class AuthService : IAuthService
{
    private readonly IDeepLinkService _deepLinkService;

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        // ... Login Logik ...

        if (result.IsSuccess)
        {
            // Pending Deep-Link verarbeiten
            await _deepLinkService.ProcessPendingDeepLinkAsync();
        }

        return result;
    }
}
```

### MainActivity.cs (Android)

```csharp
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    LaunchMode = LaunchMode.SingleTask)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "taschengeld")]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);

        if (intent?.Data != null)
        {
            var uri = new Uri(intent.Data.ToString()!);
            Platform.OnResume(this);

            // Deep-Link an App weiterleiten
            ((App)Application.Current!).OnAppLinkRequestReceived(uri);
        }
    }
}
```

## Deep-Link Beispiele

| Link | Aktion |
|------|--------|
| `taschengeld://invite/abc123` | Einladung mit Token annehmen |
| `taschengeld://request/guid-here` | Geldanfrage oeffnen |
| `https://taschengeld.app/app/invite/abc123` | Web-Link zu Einladung |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Einladungs-Link (eingeloggt) | Navigation zur Einladungsseite |
| TC-002 | Einladungs-Link (nicht eingeloggt) | Login -> dann Einladungsseite |
| TC-003 | Anfrage-Link | Navigation zur Anfrage-Detail |
| TC-004 | Ungueltiger Link | Fehlermeldung |
| TC-005 | App geschlossen, Link geoeffnet | App startet und navigiert |
| TC-006 | App im Hintergrund, Link geoeffnet | App kommt hoch und navigiert |

## Story Points

3

## Prioritaet

Mittel
