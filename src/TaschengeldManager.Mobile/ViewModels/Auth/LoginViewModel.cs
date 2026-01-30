using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.Services.Storage;

namespace TaschengeldManager.Mobile.ViewModels.Auth;

/// <summary>
/// ViewModel for the parent login page
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IBiometricService _biometricService;
    private readonly INavigationService _navigationService;
    private readonly ISecureStorageService _storageService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBiometricAvailable;

    [ObservableProperty]
    private string? _mfaToken;

    public LoginViewModel(
        IAuthenticationService authService,
        IBiometricService biometricService,
        INavigationService navigationService,
        ISecureStorageService storageService)
    {
        _authService = authService;
        _biometricService = biometricService;
        _navigationService = navigationService;
        _storageService = storageService;
    }

    public async Task InitializeAsync()
    {
        // Check if biometric login is available (device support + enabled in app)
        var availability = await _biometricService.GetAvailabilityAsync();
        IsBiometricAvailable = availability == BiometricAvailability.Available
                               && _authService.IsBiometricEnabled;

        // Try to restore session
        if (await _authService.TryRestoreSessionAsync())
        {
            await _navigationService.NavigateToMainAsync();
        }
    }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var request = new LoginRequest
            {
                Email = Email.Trim(),
                Password = Password
            };

            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                Password = string.Empty;
                await _navigationService.NavigateToMainAsync();
            }
            else if (result.RequiresMfa)
            {
                MfaToken = result.MfaToken;
                await Shell.Current.GoToAsync($"{Routes.MfaVerify}?mfaToken={MfaToken}");
            }
            else
            {
                SetError(result.ErrorMessage ?? "Anmeldung fehlgeschlagen");
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
    private async Task LoginWithBiometricAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            // First, prompt for device biometric authentication
            var biometricResult = await _biometricService.AuthenticateAsync(
                "Bestätige deine Identität mit deinem Fingerabdruck.");

            if (!biometricResult.Success)
            {
                if (biometricResult.Status == BiometricAuthStatus.Cancelled)
                {
                    // User cancelled, don't show error
                    return;
                }
                SetError(biometricResult.ErrorMessage ?? "Biometrische Authentifizierung fehlgeschlagen");
                return;
            }

            // Device biometric succeeded, now authenticate with the API
            var result = await _authService.LoginWithBiometricAsync();

            if (result.Success)
            {
                await _navigationService.NavigateToMainAsync();
            }
            else
            {
                SetError(result.ErrorMessage ?? "Biometrische Anmeldung fehlgeschlagen");
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
    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync(Routes.Register);
    }

    [RelayCommand]
    private async Task NavigateToChildLoginAsync()
    {
        await Shell.Current.GoToAsync(Routes.ChildLogin);
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        // TODO: Implement forgot password flow
        await Shell.Current.DisplayAlert(
            "Passwort vergessen",
            "Diese Funktion wird noch implementiert.",
            "OK");
    }
}
