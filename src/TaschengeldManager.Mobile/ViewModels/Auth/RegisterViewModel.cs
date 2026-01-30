using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Auth;

/// <summary>
/// ViewModel for the registration page
/// </summary>
public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _nickname = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string? _passwordError;

    public RegisterViewModel(
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    private bool CanRegister() =>
        !string.IsNullOrWhiteSpace(Nickname) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(ConfirmPassword) &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        // Validate passwords match
        if (Password != ConfirmPassword)
        {
            PasswordError = "Passwörter stimmen nicht überein";
            return;
        }

        // Validate password length
        if (Password.Length < 8)
        {
            PasswordError = "Passwort muss mindestens 8 Zeichen lang sein";
            return;
        }

        PasswordError = null;

        try
        {
            IsBusy = true;
            ClearError();

            var request = new RegisterRequest
            {
                Email = Email.Trim(),
                Password = Password,
                Nickname = Nickname.Trim()
            };

            var response = await _authService.RegisterAsync(request);

            // Registration successful - show MFA setup info
            await Shell.Current.DisplayAlert(
                "Registrierung erfolgreich",
                "Dein Konto wurde erstellt. Bitte richte die Zwei-Faktor-Authentifizierung ein.",
                "OK");

            // Navigate to login
            await _navigationService.NavigateToLoginAsync();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            SetError("Diese E-Mail-Adresse ist bereits registriert");
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            SetError("Ungültige Eingabe. Bitte überprüfe deine Daten.");
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
    private async Task NavigateToLoginAsync()
    {
        await _navigationService.NavigateToLoginAsync();
    }
}
