using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Auth;

/// <summary>
/// ViewModel for the biometric setup page shown after login
/// </summary>
public partial class BiometricSetupViewModel : BaseViewModel
{
    private readonly IBiometricService _biometricService;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private bool _isBiometricAvailable;

    [ObservableProperty]
    private string _biometricStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _showSetupOptions;

    public BiometricSetupViewModel(
        IBiometricService biometricService,
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _biometricService = biometricService;
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task CheckAvailabilityAsync()
    {
        var availability = await _biometricService.GetAvailabilityAsync();

        switch (availability)
        {
            case BiometricAvailability.Available:
                IsBiometricAvailable = true;
                ShowSetupOptions = true;
                BiometricStatusMessage = "Fingerabdruck verfügbar. Möchtest du ihn für die Anmeldung aktivieren?";
                break;
            case BiometricAvailability.NotEnrolled:
                IsBiometricAvailable = false;
                ShowSetupOptions = false;
                BiometricStatusMessage = "Kein Fingerabdruck registriert. Bitte richte zuerst einen Fingerabdruck in den Geräteeinstellungen ein.";
                break;
            case BiometricAvailability.NoHardware:
                IsBiometricAvailable = false;
                ShowSetupOptions = false;
                BiometricStatusMessage = "Dieses Gerät unterstützt keine biometrische Authentifizierung.";
                break;
            case BiometricAvailability.PermissionDenied:
                IsBiometricAvailable = false;
                ShowSetupOptions = false;
                BiometricStatusMessage = "Berechtigung für biometrische Authentifizierung wurde verweigert.";
                break;
            default:
                IsBiometricAvailable = false;
                ShowSetupOptions = false;
                BiometricStatusMessage = "Biometrische Authentifizierung ist derzeit nicht verfügbar.";
                break;
        }
    }

    [RelayCommand]
    private async Task EnableBiometricAsync()
    {
        if (IsBusy || !IsBiometricAvailable) return;

        try
        {
            IsBusy = true;
            ClearError();

            // First, authenticate the user with their biometrics
            var authResult = await _biometricService.AuthenticateAsync(
                "Bestätige deine Identität, um die biometrische Anmeldung zu aktivieren.");

            if (!authResult.Success)
            {
                if (authResult.Status == BiometricAuthStatus.Cancelled)
                {
                    // User cancelled, don't show error
                    return;
                }
                SetError(authResult.ErrorMessage ?? "Biometrische Authentifizierung fehlgeschlagen.");
                return;
            }

            // Now register with the backend
            var success = await _authService.EnableBiometricAsync();

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    "Erfolgreich",
                    "Biometrische Anmeldung wurde aktiviert. Du kannst dich ab jetzt mit deinem Fingerabdruck anmelden.",
                    "OK");
                await _navigationService.NavigateToMainAsync();
            }
            else
            {
                SetError("Aktivierung fehlgeschlagen. Bitte versuche es später erneut.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Fehler: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        await _navigationService.NavigateToMainAsync();
    }
}
