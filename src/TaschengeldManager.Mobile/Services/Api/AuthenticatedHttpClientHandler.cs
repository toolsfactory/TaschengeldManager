using System.Net;
using System.Net.Http.Headers;
using TaschengeldManager.Mobile.Services.Auth;

namespace TaschengeldManager.Mobile.Services.Api;

/// <summary>
/// HTTP handler that automatically adds authorization header and handles token refresh
/// </summary>
public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly IAuthenticationService _authService;

    public AuthenticatedHttpClientHandler(IAuthenticationService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the access token
        var token = await _authService.GetAccessTokenAsync(cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // If we get 401, the token might have expired - the AuthService handles refresh
        // through its GetAccessTokenAsync method
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Try to get a fresh token (this will trigger refresh if needed)
            token = await _authService.GetAccessTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                // Retry the request with the new token
                var newRequest = await CloneHttpRequestMessageAsync(request);
                newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await base.SendAsync(newRequest, cancellationToken);
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        // Copy headers
        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content
        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
