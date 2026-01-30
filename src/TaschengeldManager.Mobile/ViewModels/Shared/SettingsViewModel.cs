using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Shared;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IBiometricService _biometricService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    private string _userInitials = "?";

    [ObservableProperty]
    private string _roleDisplayName = string.Empty;

    [ObservableProperty]
    private Color _roleColor = Colors.Gray;

    [ObservableProperty]
    private bool _isBiometricEnabled;

    [ObservableProperty]
    private bool _canToggleBiometric;

    [ObservableProperty]
    private bool _isParentOrRelative;

    [ObservableProperty]
    private bool _isChild;

    [ObservableProperty]
    private int _activeSessionsCount = 1;

    [ObservableProperty]
    private bool _isDarkModeEnabled;

    [ObservableProperty]
    private string _appVersion = "1.0.0";

    public SettingsViewModel(
        IAuthenticationService authService,
        IBiometricService biometricService,
        INavigationService navigationService)
    {
        _authService = authService;
        _biometricService = biometricService;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        var user = _authService.CurrentUser;
        if (user != null)
        {
            UserName = user.Nickname ?? "Benutzer";
            UserEmail = user.Email;
            UserInitials = string.IsNullOrEmpty(UserName) ? "?" : UserName[0].ToString().ToUpper();

            IsParentOrRelative = user.Role is UserRole.Parent or UserRole.Relative;
            IsChild = user.Role == UserRole.Child;

            RoleDisplayName = user.Role switch
            {
                UserRole.Parent => "Elternteil",
                UserRole.Child => "Kind",
                UserRole.Relative => "Verwandter",
                _ => "Benutzer"
            };

            RoleColor = user.Role switch
            {
                UserRole.Parent => Color.FromArgb("#4CAF50"),
                UserRole.Child => Color.FromArgb("#2196F3"),
                UserRole.Relative => Color.FromArgb("#9C27B0"),
                _ => Colors.Gray
            };
        }

        // Check biometric availability
        var availability = await _biometricService.GetAvailabilityAsync();
        CanToggleBiometric = availability == BiometricAvailability.Available;
        IsBiometricEnabled = _authService.IsBiometricEnabled;

        // Get app version
        AppVersion = AppInfo.Current.VersionString;
    }

    public async Task ToggleBiometricAsync(bool enable)
    {
        if (enable == _authService.IsBiometricEnabled) return;

        try
        {
            IsBusy = true;

            if (enable)
            {
                // First authenticate with biometrics
                var result = await _biometricService.AuthenticateAsync(
                    "Bestätige deine Identität, um die biometrische Anmeldung zu aktivieren.");

                if (!result.Success)
                {
                    IsBiometricEnabled = false;
                    if (result.Status != BiometricAuthStatus.Cancelled)
                    {
                        await Shell.Current.DisplayAlert("Fehler", result.ErrorMessage ?? "Authentifizierung fehlgeschlagen", "OK");
                    }
                    return;
                }

                var success = await _authService.EnableBiometricAsync();
                if (success)
                {
                    IsBiometricEnabled = true;
                    await Shell.Current.DisplayAlert("Erfolg", "Biometrische Anmeldung aktiviert", "OK");
                }
                else
                {
                    IsBiometricEnabled = false;
                    await Shell.Current.DisplayAlert("Fehler", "Aktivierung fehlgeschlagen", "OK");
                }
            }
            else
            {
                await _authService.DisableBiometricAsync();
                IsBiometricEnabled = false;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        // TODO: Navigate to change password page
        await Shell.Current.DisplayAlert("Info", "Passwort ändern wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task ChangePinAsync()
    {
        // TODO: Navigate to change PIN page
        await Shell.Current.DisplayAlert("Info", "PIN ändern wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task ManageSessionsAsync()
    {
        // TODO: Navigate to sessions management page
        await Shell.Current.DisplayAlert("Info", "Sitzungsverwaltung wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task OpenPrivacyAsync()
    {
        // TODO: Open privacy policy URL
        await Browser.Default.OpenAsync("https://example.com/privacy", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task OpenTermsAsync()
    {
        // TODO: Open terms URL
        await Browser.Default.OpenAsync("https://example.com/terms", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Abmelden",
            "Möchtest du dich wirklich abmelden?",
            "Ja, abmelden",
            "Abbrechen");

        if (confirm)
        {
            await _navigationService.LogoutAsync();
        }
    }
}
