# Story M013-S03: Retry-Mechanismus

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **Benutzer** moechte ich **dass fehlgeschlagene API-Aufrufe automatisch wiederholt werden**, damit **kurzzeitige Netzwerkprobleme mich nicht behindern**.

## Akzeptanzkriterien

- [ ] Gegeben ein fehlgeschlagener API-Aufruf (Timeout, Netzwerkfehler), wenn er auftritt, dann wird automatisch ein Retry versucht
- [ ] Gegeben ein Retry-Versuch, wenn er stattfindet, dann wartet das System mit exponentieller Verzoegerung
- [ ] Gegeben maximal 3 Retry-Versuche, wenn alle fehlschlagen, dann wird der endgueltige Fehler angezeigt
- [ ] Gegeben ein HTTP-Fehler 4xx, wenn er auftritt, dann wird kein Retry durchgefuehrt (Client-Fehler)
- [ ] Gegeben ein HTTP-Fehler 5xx, wenn er auftritt, dann wird ein Retry durchgefuehrt (Server-Fehler)

## Technische Implementierung

### Polly-Konfiguration

```csharp
// NuGet Package: Microsoft.Extensions.Http.Polly

// MauiProgram.cs
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    // HTTP Client mit Retry-Policy
    builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://api.taschengeld.app");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

    return builder.Build();
}

private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 5xx, 408, NetworkException
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests) // 429
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, 4s, 8s
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                var logger = context.GetLogger();
                logger.LogWarning(
                    "Retry {RetryAttempt} nach {Delay}s wegen {Reason}",
                    retryAttempt,
                    timespan.TotalSeconds,
                    outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
            });
}

private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (result, timespan) =>
            {
                // Circuit geöffnet - zu viele Fehler
                WeakReferenceMessenger.Default.Send(new CircuitBreakerOpenMessage());
            },
            onReset: () =>
            {
                // Circuit wieder geschlossen
                WeakReferenceMessenger.Default.Send(new CircuitBreakerResetMessage());
            });
}
```

### API Client mit Retry-Awareness

```csharp
public interface IApiClient
{
    Task<T> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default);
    Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default);
    Task DeleteAsync(string endpoint, CancellationToken ct = default);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly IToastService _toastService;

    public ApiClient(
        HttpClient httpClient,
        ILogger<ApiClient> logger,
        IToastService toastService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _toastService = toastService;
    }

    public async Task<T> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint, ct);
            return await HandleResponseAsync<T>(response);
        }
        catch (BrokenCircuitException)
        {
            await _toastService.ShowErrorAsync("Server voruebergehend nicht erreichbar. Bitte warte einen Moment.");
            throw;
        }
        catch (HttpRequestException ex) when (ex.InnerException is TaskCanceledException)
        {
            await _toastService.ShowErrorAsync("Die Anfrage hat zu lange gedauert. Bitte versuche es erneut.");
            throw;
        }
    }

    private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions)!;
        }

        // Keine Retry bei Client-Fehlern
        if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApiException(response.StatusCode, error);
        }

        // Server-Fehler werden von Polly gehandelt
        response.EnsureSuccessStatusCode();
        return default!;
    }
}
```

### Manuelles Retry mit UI

```csharp
// Fuer Szenarien wo der Benutzer manuell retry ausloesen soll
public partial class RetryableViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isRetrying;

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        HasError = false;

        try
        {
            Data = await _apiClient.GetAsync<DataDto>("/api/data");
        }
        catch (Exception ex) when (ex is HttpRequestException or BrokenCircuitException)
        {
            HasError = true;
            ErrorMessage = "Laden fehlgeschlagen. Tippe zum Erneut versuchen.";
        }
    }

    [RelayCommand]
    private async Task RetryAsync()
    {
        IsRetrying = true;
        await LoadDataAsync();
        IsRetrying = false;
    }
}
```

### XAML: Retry-Button
```xml
<Grid>
    <!-- Normaler Inhalt -->
    <CollectionView ItemsSource="{Binding Data}"
                    IsVisible="{Binding HasError, Converter={StaticResource InverseBool}}">
        <!-- ... -->
    </CollectionView>

    <!-- Fehler-Zustand -->
    <VerticalStackLayout IsVisible="{Binding HasError}"
                         VerticalOptions="Center"
                         HorizontalOptions="Center"
                         Spacing="16">
        <Image Source="error_icon.png" WidthRequest="64" HeightRequest="64"/>
        <Label Text="{Binding ErrorMessage}"
               HorizontalTextAlignment="Center"/>
        <Button Text="Erneut versuchen"
                Command="{Binding RetryCommand}"
                IsEnabled="{Binding IsRetrying, Converter={StaticResource InverseBool}}">
            <Button.Triggers>
                <DataTrigger TargetType="Button"
                             Binding="{Binding IsRetrying}"
                             Value="True">
                    <Setter Property="Text" Value="Wird geladen..."/>
                </DataTrigger>
            </Button.Triggers>
        </Button>
    </VerticalStackLayout>
</Grid>
```

## Retry-Strategie

| Fehler-Typ | Retry | Delay |
|------------|-------|-------|
| Timeout (408) | Ja | Exponential |
| Server Error (5xx) | Ja | Exponential |
| Too Many Requests (429) | Ja | Exponential |
| Network Error | Ja | Exponential |
| Client Error (4xx) | Nein | - |
| Circuit Open | Nein | Warten bis Reset |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Timeout bei API-Aufruf | 3 Retry-Versuche mit Delay |
| TC-002 | Server 500 Fehler | Retry wird durchgefuehrt |
| TC-003 | Client 400 Fehler | Kein Retry, sofortiger Fehler |
| TC-004 | 5 Fehler hintereinander | Circuit Breaker oeffnet |
| TC-005 | Nach 30s | Circuit Breaker schliesst |
| TC-006 | Erfolg nach 2. Retry | Daten werden angezeigt |

## Story Points

2

## Prioritaet

Hoch
