# Story M002-S06: Automatischer Token-Refresh

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **nicht ständig neu eingeloggt werden müssen**, damit **ich die App nahtlos nutzen kann, ohne Unterbrechungen**.

## Akzeptanzkriterien

- [ ] Gegeben ein ablaufendes Token, wenn ein API-Aufruf gemacht wird, dann wird automatisch ein neues Token geholt
- [ ] Gegeben ein 401-Fehler, wenn er auftritt, dann wird versucht, das Token zu erneuern
- [ ] Gegeben ein ungültiges Refresh-Token, wenn der Refresh fehlschlägt, dann wird der Benutzer zum Login weitergeleitet
- [ ] Gegeben parallele API-Aufrufe, wenn sie gleichzeitig stattfinden, dann wird nur ein Refresh durchgeführt

## Technische Hinweise

### ITokenRefreshService Interface
```csharp
public interface ITokenRefreshService
{
    Task<bool> TryRefreshTokenAsync();
    event EventHandler? TokenRefreshFailed;
}
```

### TokenRefreshService Implementation
```csharp
public class TokenRefreshService : ITokenRefreshService
{
    private readonly IAuthApi _authApi;
    private readonly ITokenService _tokenService;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _isRefreshing;

    public event EventHandler? TokenRefreshFailed;

    public TokenRefreshService(IAuthApi authApi, ITokenService tokenService)
    {
        _authApi = authApi;
        _tokenService = tokenService;
    }

    public async Task<bool> TryRefreshTokenAsync()
    {
        // Nur ein Refresh zur gleichen Zeit
        await _refreshLock.WaitAsync();
        try
        {
            if (_isRefreshing)
                return false;

            _isRefreshing = true;

            var refreshToken = await _tokenService.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
            {
                OnTokenRefreshFailed();
                return false;
            }

            var response = await _authApi.RefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            });

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                await _tokenService.SaveTokensAsync(
                    response.Content.Token,
                    response.Content.RefreshToken);
                return true;
            }

            OnTokenRefreshFailed();
            return false;
        }
        catch
        {
            OnTokenRefreshFailed();
            return false;
        }
        finally
        {
            _isRefreshing = false;
            _refreshLock.Release();
        }
    }

    private void OnTokenRefreshFailed()
    {
        TokenRefreshFailed?.Invoke(this, EventArgs.Empty);
    }
}
```

### AuthenticatedHttpClientHandler
```csharp
public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly ITokenRefreshService _refreshService;
    private readonly INavigationService _navigationService;

    public AuthenticatedHttpClientHandler(
        ITokenService tokenService,
        ITokenRefreshService refreshService,
        INavigationService navigationService)
    {
        _tokenService = tokenService;
        _refreshService = refreshService;
        _navigationService = navigationService;

        _refreshService.TokenRefreshFailed += OnTokenRefreshFailed;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Token vor dem Request prüfen und ggf. erneuern
        if (await _tokenService.IsTokenExpiredAsync())
        {
            var refreshed = await _refreshService.TryRefreshTokenAsync();
            if (!refreshed)
            {
                // Zum Login navigieren
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        // Authorization Header setzen
        var token = await _tokenService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Bei 401 versuchen, Token zu erneuern und erneut zu senden
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await _refreshService.TryRefreshTokenAsync();
            if (refreshed)
            {
                // Neuen Token holen und Request wiederholen
                token = await _tokenService.GetAccessTokenAsync();
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // Request klonen (Original kann nicht wiederverwendet werden)
                var newRequest = await CloneRequestAsync(request);
                response = await base.SendAsync(newRequest, cancellationToken);
            }
        }

        return response;
    }

    private async void OnTokenRefreshFailed(object? sender, EventArgs e)
    {
        await _tokenService.ClearTokensAsync();
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await _navigationService.NavigateToAsync("//login");
        });
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(
        HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync();
            clone.Content = new StringContent(
                content,
                Encoding.UTF8,
                request.Content.Headers.ContentType?.MediaType ?? "application/json");
        }

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}
```

### Proaktiver Token-Refresh
```csharp
// In App.xaml.cs - Token vor Ablauf erneuern
public partial class App : Application
{
    private readonly ITokenService _tokenService;
    private readonly ITokenRefreshService _refreshService;
    private Timer? _refreshTimer;

    protected override async void OnStart()
    {
        base.OnStart();

        // Timer für proaktiven Refresh (5 Minuten vor Ablauf)
        StartRefreshTimer();
    }

    private async void StartRefreshTimer()
    {
        var expiration = await _tokenService.GetTokenExpirationAsync();
        if (expiration == null) return;

        var refreshTime = expiration.Value.AddMinutes(-5) - DateTime.UtcNow;
        if (refreshTime <= TimeSpan.Zero)
        {
            await _refreshService.TryRefreshTokenAsync();
            return;
        }

        _refreshTimer = new Timer(async _ =>
        {
            await _refreshService.TryRefreshTokenAsync();
            StartRefreshTimer(); // Nächsten Timer starten
        }, null, refreshTime, Timeout.InfiniteTimeSpan);
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-24 | Token läuft ab, API-Aufruf | Automatischer Refresh |
| TC-M002-25 | 401 bei API-Aufruf | Token wird erneuert, Retry |
| TC-M002-26 | Refresh-Token ungültig | Redirect zum Login |
| TC-M002-27 | Parallele API-Aufrufe | Nur ein Refresh |

## Story Points
2

## Priorität
Hoch
