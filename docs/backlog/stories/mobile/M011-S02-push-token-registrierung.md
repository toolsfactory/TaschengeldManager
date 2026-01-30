# Story M011-S02: Push-Token Registrierung

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Benutzer** moechte ich **dass mein Geraet beim Backend fuer Push-Benachrichtigungen registriert wird**, damit **ich Benachrichtigungen auf diesem Geraet empfangen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein erfolgreicher Login, wenn ein FCM-Token vorhanden ist, dann wird er beim Backend registriert
- [ ] Gegeben ein aendernder Token, wenn er sich aendert, dann wird der neue Token beim Backend aktualisiert
- [ ] Gegeben ein Logout, wenn der Benutzer sich abmeldet, dann wird der Token beim Backend entfernt
- [ ] Gegeben mehrere Geraete, wenn der Benutzer sich auf verschiedenen Geraeten anmeldet, dann kann er auf allen Benachrichtigungen empfangen

## API-Endpunkte

### Token registrieren
```
POST /api/push/register
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "token": "fcm-device-token",
  "platform": "android",
  "deviceId": "unique-device-identifier"
}

Response 201:
{
  "id": "guid",
  "token": "fcm-device-token",
  "platform": "android",
  "registeredAt": "2026-01-20T10:00:00Z"
}

Response 400:
{
  "error": "Invalid token format"
}
```

### Token entfernen (Logout)
```
DELETE /api/push/unregister
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "deviceId": "unique-device-identifier"
}

Response 204 No Content
```

### Token aktualisieren
```
PUT /api/push/token
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "oldToken": "old-fcm-token",
  "newToken": "new-fcm-token",
  "deviceId": "unique-device-identifier"
}

Response 200:
{
  "token": "new-fcm-token",
  "updatedAt": "2026-01-20T11:00:00Z"
}
```

## Technische Notizen

- Device-ID: `DeviceInfo.Current.Platform` + `Preferences.Get("DeviceId", Guid.NewGuid().ToString())`
- Token wird in SecureStorage gespeichert
- Service: `IPushTokenService`

## Implementierungshinweise

```csharp
public interface IPushTokenService
{
    Task RegisterTokenAsync();
    Task UnregisterTokenAsync();
    Task UpdateTokenAsync(string newToken);
}

public class PushTokenService : IPushTokenService
{
    private readonly IPushNotificationService _pushService;
    private readonly IApiClient _apiClient;
    private readonly IAuthService _authService;

    private string DeviceId
    {
        get
        {
            var id = Preferences.Get("DeviceId", string.Empty);
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
                Preferences.Set("DeviceId", id);
            }
            return id;
        }
    }

    public PushTokenService(
        IPushNotificationService pushService,
        IApiClient apiClient,
        IAuthService authService)
    {
        _pushService = pushService;
        _apiClient = apiClient;
        _authService = authService;

        // Auf Token-Aenderungen reagieren
        WeakReferenceMessenger.Default.Register<PushTokenChangedMessage>(this, async (r, m) =>
        {
            if (_authService.IsAuthenticated)
            {
                await UpdateTokenAsync(m.NewToken);
            }
        });
    }

    public async Task RegisterTokenAsync()
    {
        if (!_authService.IsAuthenticated) return;

        var token = await _pushService.GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return;

        var request = new RegisterPushTokenRequest
        {
            Token = token,
            Platform = DeviceInfo.Platform.ToString().ToLower(),
            DeviceId = DeviceId
        };

        await _apiClient.PostAsync("/api/push/register", request);

        // Token lokal speichern
        await SecureStorage.SetAsync("PushToken", token);
    }

    public async Task UnregisterTokenAsync()
    {
        var request = new UnregisterPushTokenRequest
        {
            DeviceId = DeviceId
        };

        try
        {
            await _apiClient.DeleteAsync("/api/push/unregister", request);
        }
        catch
        {
            // Ignorieren bei Logout-Fehlern
        }

        SecureStorage.Remove("PushToken");
    }

    public async Task UpdateTokenAsync(string newToken)
    {
        var oldToken = await SecureStorage.GetAsync("PushToken");
        if (oldToken == newToken) return;

        var request = new UpdatePushTokenRequest
        {
            OldToken = oldToken ?? string.Empty,
            NewToken = newToken,
            DeviceId = DeviceId
        };

        await _apiClient.PutAsync("/api/push/token", request);
        await SecureStorage.SetAsync("PushToken", newToken);
    }
}

// Integration in AuthService
public class AuthService : IAuthService
{
    private readonly IPushTokenService _pushTokenService;

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        // ... Login-Logik ...

        if (result.IsSuccess)
        {
            // Push-Token registrieren nach erfolgreichem Login
            await _pushTokenService.RegisterTokenAsync();
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        // Push-Token entfernen vor Logout
        await _pushTokenService.UnregisterTokenAsync();

        // ... Logout-Logik ...
    }
}
```

### Backend: Push-Token Tabelle
```csharp
public class PushToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // "android", "ios"
    public string DeviceId { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public DateTime? LastUsedAt { get; set; }

    public User User { get; set; } = null!;
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Login mit gueltigem Token | Token wird registriert |
| TC-002 | Logout | Token wird entfernt |
| TC-003 | Token aendert sich | Backend wird aktualisiert |
| TC-004 | Login auf 2. Geraet | Beide Tokens registriert |
| TC-005 | Kein FCM-Token | Keine Registrierung (graceful) |
| TC-006 | Backend nicht erreichbar | Fehler wird geloggt, App funktioniert |

## Story Points

2

## Prioritaet

Hoch
