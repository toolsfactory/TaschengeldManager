using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Auth;

/// <summary>
/// ViewModel for the child login page
/// </summary>
public partial class ChildLoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _familyCode = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _nickname = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _pin = string.Empty;

    public ChildLoginViewModel(
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(FamilyCode) &&
        !string.IsNullOrWhiteSpace(Nickname) &&
        !string.IsNullOrWhiteSpace(Pin) &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var request = new ChildLoginRequest
            {
                FamilyCode = FamilyCode.Trim().ToUpperInvariant(),
                Nickname = Nickname.Trim(),
                Pin = Pin
            };

            var result = await _authService.LoginChildAsync(request);

            if (result.Success)
            {
                Pin = string.Empty;
                await _navigationService.NavigateToMainAsync();
            }
            else if (result.RequiresMfa)
            {
                await Shell.Current.GoToAsync($"{Routes.MfaVerify}?mfaToken={result.MfaToken}");
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
    private async Task NavigateToParentLoginAsync()
    {
        await _navigationService.NavigateToLoginAsync();
    }
}
