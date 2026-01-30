using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Auth;

/// <summary>
/// ViewModel for MFA/TOTP verification page
/// </summary>
[QueryProperty(nameof(MfaToken), "mfaToken")]
public partial class MfaVerifyViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _mfaToken = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(VerifyCommand))]
    private string _code = string.Empty;

    public MfaVerifyViewModel(
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    private bool CanVerify() =>
        !string.IsNullOrWhiteSpace(Code) &&
        Code.Length == 6 &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanVerify))]
    private async Task VerifyAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var response = await _authService.VerifyTotpAsync(MfaToken, Code);

            // Success - navigate to main
            Code = string.Empty;
            await _navigationService.NavigateToMainAsync();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            SetError("Ungültiger Code. Bitte versuche es erneut.");
            Code = string.Empty;
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone)
        {
            SetError("Der Code ist abgelaufen. Bitte melde dich erneut an.");
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
    private async Task CancelAsync()
    {
        await _navigationService.NavigateToLoginAsync();
    }
}
