# Story M001-S06: API-Client mit Refit

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **einen typsicheren API-Client mit Refit implementieren**, damit **ich die Backend-API einfach und wartbar ansprechen kann**.

## Akzeptanzkriterien

- [ ] Gegeben Refit-Interfaces, wenn sie definiert sind, dann generiert Refit automatisch die Implementierung
- [ ] Gegeben API-Aufrufe, wenn sie fehlschlagen, dann werden Fehler korrekt behandelt
- [ ] Gegeben der HttpClient, wenn Requests gesendet werden, dann wird der Authorization-Header gesetzt
- [ ] Gegeben Netzwerkfehler, wenn sie auftreten, dann wird eine Retry-Logik mit Polly angewendet

## Technische Hinweise

### API-Interface Definition
```csharp
public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<ApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request);

    [Post("/api/auth/register")]
    Task<ApiResponse<RegisterResponse>> RegisterAsync([Body] RegisterRequest request);

    [Post("/api/auth/refresh")]
    Task<ApiResponse<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest request);

    [Post("/api/auth/child-login")]
    Task<ApiResponse<LoginResponse>> ChildLoginAsync([Body] ChildLoginRequest request);
}

public interface IAccountApi
{
    [Get("/api/accounts/{accountId}")]
    Task<ApiResponse<AccountResponse>> GetAccountAsync(Guid accountId);

    [Get("/api/accounts/{accountId}/transactions")]
    Task<ApiResponse<List<TransactionResponse>>> GetTransactionsAsync(
        Guid accountId,
        [Query] int page = 1,
        [Query] int pageSize = 20);

    [Post("/api/accounts/{accountId}/transactions")]
    Task<ApiResponse<TransactionResponse>> CreateTransactionAsync(
        Guid accountId,
        [Body] CreateTransactionRequest request);
}
```

### DI-Registrierung mit Polly
```csharp
public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, string baseUrl)
    {
        // Retry-Policy definieren
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // HttpClient mit Authorization Handler
        services.AddTransient<AuthHeaderHandler>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddPolicyHandler(retryPolicy);

        services.AddRefitClient<IAccountApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddPolicyHandler(retryPolicy);

        return services;
    }
}
```

### Authorization Header Handler
```csharp
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthHeaderHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

### API Response Handling
```csharp
public static class ApiResponseExtensions
{
    public static async Task<T> HandleResponseAsync<T>(this Task<ApiResponse<T>> apiCall)
    {
        var response = await apiCall;

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return response.Content;
        }

        throw new ApiException(
            response.StatusCode,
            response.Error?.Content ?? "Ein Fehler ist aufgetreten");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-18 | API-Aufruf erfolgreich | Response deserialisiert |
| TC-M001-19 | API-Aufruf mit Auth-Header | Bearer Token im Header |
| TC-M001-20 | Netzwerkfehler | Retry wird ausgeführt |
| TC-M001-21 | 401 Unauthorized | Token-Refresh getriggert |

## Story Points
3

## Priorität
Hoch
