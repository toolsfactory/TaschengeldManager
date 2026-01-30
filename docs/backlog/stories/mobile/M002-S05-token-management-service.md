# Story M002-S05: Token-Management Service

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **einen Token-Management Service implementieren**, damit **JWT-Tokens sicher gespeichert, abgerufen und verwaltet werden können**.

## Akzeptanzkriterien

- [ ] Gegeben ein Token, wenn es gespeichert wird, dann ist es im Secure Storage abgelegt
- [ ] Gegeben der TokenService, wenn GetAccessToken aufgerufen wird, dann wird das aktuelle Token zurückgegeben
- [ ] Gegeben ein Token, wenn es abläuft, dann kann das Ablaufdatum geprüft werden
- [ ] Gegeben ein Logout, wenn er durchgeführt wird, dann werden alle Tokens gelöscht

## Technische Hinweise

### ITokenService Interface
```csharp
public interface ITokenService
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SaveTokensAsync(string accessToken, string refreshToken);
    Task ClearTokensAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsTokenExpiredAsync();
    Task<DateTime?> GetTokenExpirationAsync();
    Task<UserClaims?> GetUserClaimsAsync();
}

public record UserClaims(
    string UserId,
    string Email,
    string Role,
    string? FamilyId);
```

### TokenService Implementation
```csharp
public class TokenService : ITokenService
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    private string? _cachedAccessToken;
    private DateTime? _cachedExpiration;

    public async Task<string?> GetAccessTokenAsync()
    {
        if (_cachedAccessToken != null && !await IsTokenExpiredAsync())
        {
            return _cachedAccessToken;
        }

        try
        {
            _cachedAccessToken = await SecureStorage.Default.GetAsync(AccessTokenKey);
            return _cachedAccessToken;
        }
        catch (Exception)
        {
            // SecureStorage nicht verfügbar (z.B. im Simulator)
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(RefreshTokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        try
        {
            await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
            await SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);

            _cachedAccessToken = accessToken;
            _cachedExpiration = GetExpirationFromToken(accessToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving tokens: {ex.Message}");
            throw;
        }
    }

    public async Task ClearTokensAsync()
    {
        try
        {
            SecureStorage.Default.Remove(AccessTokenKey);
            SecureStorage.Default.Remove(RefreshTokenKey);

            _cachedAccessToken = null;
            _cachedExpiration = null;
        }
        catch
        {
            // Ignore errors during logout
        }

        await Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetAccessTokenAsync();
        return !string.IsNullOrEmpty(token) && !await IsTokenExpiredAsync();
    }

    public Task<bool> IsTokenExpiredAsync()
    {
        if (_cachedExpiration == null && _cachedAccessToken != null)
        {
            _cachedExpiration = GetExpirationFromToken(_cachedAccessToken);
        }

        var isExpired = _cachedExpiration == null ||
                        _cachedExpiration <= DateTime.UtcNow;

        return Task.FromResult(isExpired);
    }

    public Task<DateTime?> GetTokenExpirationAsync()
    {
        if (_cachedExpiration == null && _cachedAccessToken != null)
        {
            _cachedExpiration = GetExpirationFromToken(_cachedAccessToken);
        }

        return Task.FromResult(_cachedExpiration);
    }

    public async Task<UserClaims?> GetUserClaimsAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var payload = ParseJwtPayload(token);

            return new UserClaims(
                payload.GetValueOrDefault("sub", ""),
                payload.GetValueOrDefault("email", ""),
                payload.GetValueOrDefault("role", ""),
                payload.GetValueOrDefault("family_id"));
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetExpirationFromToken(string token)
    {
        try
        {
            var payload = ParseJwtPayload(token);
            if (payload.TryGetValue("exp", out var expValue) &&
                long.TryParse(expValue, out var exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }
        }
        catch
        {
            // Token parsing failed
        }

        return null;
    }

    private static Dictionary<string, string> ParseJwtPayload(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid JWT format");

        var payload = parts[1];

        // Base64 padding
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var json = Encoding.UTF8.GetString(
            Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/')));

        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)?
            .ToDictionary(k => k.Key, v => v.Value.ToString()) ??
            new Dictionary<string, string>();
    }
}
```

### DI-Registrierung
```csharp
// In MauiProgram.cs
builder.Services.AddSingleton<ITokenService, TokenService>();
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-19 | Token speichern | Im SecureStorage abgelegt |
| TC-M002-20 | Token abrufen | Korrekter Token zurückgegeben |
| TC-M002-21 | Token-Ablauf prüfen | Ablaufdatum korrekt |
| TC-M002-22 | Tokens löschen | Beide Tokens entfernt |
| TC-M002-23 | Claims aus Token extrahieren | UserId, Role korrekt |

## Story Points
3

## Priorität
Hoch
