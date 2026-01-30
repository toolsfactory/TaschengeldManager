using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace TaschengeldManager.Mobile.Services.Auth;

/// <summary>
/// Implementation of biometric service using Plugin.Fingerprint
/// </summary>
public class BiometricService : IBiometricService
{
    public async Task<BiometricAvailability> GetAvailabilityAsync()
    {
        try
        {
            var availability = await CrossFingerprint.Current.GetAvailabilityAsync();
            return availability switch
            {
                FingerprintAvailability.Available => BiometricAvailability.Available,
                FingerprintAvailability.NoFingerprint => BiometricAvailability.NotEnrolled,
                FingerprintAvailability.NoPermission => BiometricAvailability.PermissionDenied,
                FingerprintAvailability.NoSensor => BiometricAvailability.NoHardware,
                FingerprintAvailability.Denied => BiometricAvailability.PermissionDenied,
                _ => BiometricAvailability.Unknown
            };
        }
        catch
        {
            return BiometricAvailability.Unknown;
        }
    }

    public async Task<BiometricAuthResult> AuthenticateAsync(string reason, CancellationToken ct = default)
    {
        try
        {
            var config = new AuthenticationRequestConfiguration(
                "Biometrische Authentifizierung",
                reason)
            {
                CancelTitle = "Abbrechen",
                FallbackTitle = "PIN verwenden",
                AllowAlternativeAuthentication = true
            };

            var result = await CrossFingerprint.Current.AuthenticateAsync(config, ct);

            if (result.Authenticated)
            {
                return BiometricAuthResult.Succeeded();
            }

            var status = result.Status switch
            {
                FingerprintAuthenticationResultStatus.Canceled => BiometricAuthStatus.Cancelled,
                FingerprintAuthenticationResultStatus.TooManyAttempts => BiometricAuthStatus.TooManyAttempts,
                FingerprintAuthenticationResultStatus.NotAvailable => BiometricAuthStatus.NotAvailable,
                FingerprintAuthenticationResultStatus.Failed => BiometricAuthStatus.Failed,
                _ => BiometricAuthStatus.Unknown
            };

            return BiometricAuthResult.Failed(status, result.ErrorMessage);
        }
        catch (Exception ex)
        {
            return BiometricAuthResult.Failed(BiometricAuthStatus.Unknown, ex.Message);
        }
    }
}
