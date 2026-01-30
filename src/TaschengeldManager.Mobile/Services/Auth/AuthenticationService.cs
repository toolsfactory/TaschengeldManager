using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Storage;

namespace TaschengeldManager.Mobile.Services.Auth;

/// <summary>
/// Implementation of authentication service handling login, tokens, and biometric
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ITaschengeldApi _api;
    private readonly ISecureStorageService _storage;

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _tokenExpiry;
    private readonly SemaphoreSlim _tokenRefreshLock = new(1, 1);

    public UserDto? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null && !string.IsNullOrEmpty(_accessToken);
    public bool IsBiometricEnabled { get; private set; }

    public event EventHandler<AuthenticationStateChangedEventArgs>? AuthenticationStateChanged;

    public AuthenticationService(ITaschengeldApi api, ISecureStorageService storage)
    {
        _api = api;
        _storage = storage;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // Registration returns MFA setup info, not login tokens
        // User needs to set up MFA and then login separately
        return await _api.RegisterAsync(request, ct);
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _api.LoginAsync(request, ct);
            await HandleLoginResponseAsync(response);
            return LoginResult.Succeeded(CurrentUser!);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Check if MFA is required
            var content = ex.Content;
            if (content?.Contains("mfaToken") == true)
            {
                // Parse MFA required response
                var mfaResponse = System.Text.Json.JsonSerializer.Deserialize<MfaRequiredResponse>(content);
                if (mfaResponse != null)
                {
                    return LoginResult.MfaRequired(mfaResponse.MfaToken, mfaResponse.AvailableMethods ?? []);
                }
            }
            return LoginResult.Failed("Ungültige Anmeldedaten");
        }
        catch (Exception ex)
        {
            return LoginResult.Failed(ex.Message);
        }
    }

    public async Task<LoginResult> LoginChildAsync(ChildLoginRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _api.LoginChildAsync(request, ct);
            await HandleLoginResponseAsync(response);
            return LoginResult.Succeeded(CurrentUser!);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var content = ex.Content;
            if (content?.Contains("mfaToken") == true)
            {
                var mfaResponse = System.Text.Json.JsonSerializer.Deserialize<MfaRequiredResponse>(content);
                if (mfaResponse != null)
                {
                    return LoginResult.MfaRequired(mfaResponse.MfaToken, mfaResponse.AvailableMethods ?? []);
                }
            }
            return LoginResult.Failed("Ungültiger Familien-Code, Name oder PIN");
        }
        catch (Exception ex)
        {
            return LoginResult.Failed(ex.Message);
        }
    }

    public async Task<LoginResult> LoginWithBiometricAsync(CancellationToken ct = default)
    {
        try
        {
            var deviceId = await _storage.GetAsync(SecureStorageKeys.DeviceId);
            var biometricToken = await _storage.GetAsync(SecureStorageKeys.BiometricToken);

            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(biometricToken))
            {
                return LoginResult.Failed("Biometrie nicht konfiguriert");
            }

            var response = await _api.LoginBiometricAsync(new BiometricLoginRequest
            {
                DeviceId = deviceId,
                BiometricToken = biometricToken
            }, ct);

            await HandleLoginResponseAsync(response);
            return LoginResult.Succeeded(CurrentUser!);
        }
        catch (Exception ex)
        {
            return LoginResult.Failed(ex.Message);
        }
    }

    public async Task<LoginResponse> VerifyTotpAsync(string mfaToken, string code, CancellationToken ct = default)
    {
        var response = await _api.VerifyTotpAsync(new VerifyTotpRequest
        {
            MfaToken = mfaToken,
            Code = code
        }, ct);

        await HandleLoginResponseAsync(response);
        return response;
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            if (!string.IsNullOrEmpty(_refreshToken))
            {
                await _api.LogoutAsync(new LogoutRequest { RefreshToken = _refreshToken }, ct);
            }
        }
        catch
        {
            // Ignore logout API errors
        }
        finally
        {
            await ClearSessionAsync();
        }
    }

    public async Task<bool> EnableBiometricAsync(CancellationToken ct = default)
    {
        try
        {
            var deviceId = await GetOrCreateDeviceIdAsync();
            var deviceName = DeviceInfo.Current.Name ?? "Unknown Device";
            var platform = DeviceInfo.Current.Platform.ToString();

            var response = await _api.EnableBiometricAsync(new EnableBiometricRequest
            {
                DeviceId = deviceId,
                DeviceName = deviceName,
                Platform = platform,
                BiometryType = Core.Enums.BiometryType.Fingerprint // TODO: Detect actual type
            }, ct);

            await _storage.SetAsync(SecureStorageKeys.BiometricToken, response.BiometricToken);
            IsBiometricEnabled = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task DisableBiometricAsync(CancellationToken ct = default)
    {
        var deviceId = await _storage.GetAsync(SecureStorageKeys.DeviceId);
        if (!string.IsNullOrEmpty(deviceId))
        {
            try
            {
                await _api.DisableBiometricAsync(deviceId, ct);
            }
            catch
            {
                // Ignore API errors
            }
        }

        await _storage.RemoveAsync(SecureStorageKeys.BiometricToken);
        IsBiometricEnabled = false;
    }

    public async Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        var refreshToken = await _storage.GetAsync(SecureStorageKeys.RefreshToken);
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        try
        {
            var response = await _api.RefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            }, ct);

            await HandleLoginResponseAsync(response);
            return true;
        }
        catch
        {
            await ClearSessionAsync();
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        // If token is still valid, return it
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-1))
        {
            return _accessToken;
        }

        // Try to refresh the token
        await _tokenRefreshLock.WaitAsync(ct);
        try
        {
            // Double-check after acquiring lock
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-1))
            {
                return _accessToken;
            }

            var refreshToken = await _storage.GetAsync(SecureStorageKeys.RefreshToken);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var response = await _api.RefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            }, ct);

            await HandleLoginResponseAsync(response);
            return _accessToken;
        }
        catch
        {
            await ClearSessionAsync();
            return null;
        }
        finally
        {
            _tokenRefreshLock.Release();
        }
    }

    private async Task HandleLoginResponseAsync(LoginResponse response)
    {
        _accessToken = response.AccessToken;
        _refreshToken = response.RefreshToken;
        _tokenExpiry = response.ExpiresAt;
        CurrentUser = response.User;

        // Store refresh token securely
        await _storage.SetAsync(SecureStorageKeys.RefreshToken, response.RefreshToken);
        await _storage.SetAsync(SecureStorageKeys.UserId, response.User.Id.ToString());
        await _storage.SetAsync(SecureStorageKeys.UserRole, response.User.Role.ToString());

        // Check if biometric is enabled
        IsBiometricEnabled = await _storage.ContainsKeyAsync(SecureStorageKeys.BiometricToken);

        // Notify listeners
        AuthenticationStateChanged?.Invoke(this, new AuthenticationStateChangedEventArgs
        {
            IsAuthenticated = true,
            User = CurrentUser
        });
    }

    private async Task ClearSessionAsync()
    {
        _accessToken = null;
        _refreshToken = null;
        CurrentUser = null;

        await _storage.RemoveAsync(SecureStorageKeys.AccessToken);
        await _storage.RemoveAsync(SecureStorageKeys.RefreshToken);
        await _storage.RemoveAsync(SecureStorageKeys.UserId);
        await _storage.RemoveAsync(SecureStorageKeys.UserRole);
        // Keep DeviceId and BiometricToken for potential re-login

        AuthenticationStateChanged?.Invoke(this, new AuthenticationStateChangedEventArgs
        {
            IsAuthenticated = false,
            User = null
        });
    }

    private async Task<string> GetOrCreateDeviceIdAsync()
    {
        var deviceId = await _storage.GetAsync(SecureStorageKeys.DeviceId);
        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = Guid.NewGuid().ToString();
            await _storage.SetAsync(SecureStorageKeys.DeviceId, deviceId);
        }
        return deviceId;
    }
}

/// <summary>
/// Response when MFA is required
/// </summary>
internal record MfaRequiredResponse
{
    public string MfaToken { get; init; } = string.Empty;
    public string[]? AvailableMethods { get; init; }
    public Guid UserId { get; init; }
}
