using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Shared;

/// <summary>
/// Base ViewModel for all dashboard pages with common functionality like logout
/// </summary>
public abstract partial class DashboardBaseViewModel : BaseViewModel
{
    protected readonly IAuthenticationService AuthService;
    protected readonly INavigationService NavigationService;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _userRole = string.Empty;

    protected DashboardBaseViewModel(
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        AuthService = authService;
        NavigationService = navigationService;
    }

    public virtual Task InitializeAsync()
    {
        var user = AuthService.CurrentUser;
        if (user != null)
        {
            UserName = user.Nickname ?? user.Email;
            UserRole = user.Role.ToString();
        }
        return Task.CompletedTask;
    }

    [RelayCommand]
    protected virtual async Task LogoutAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Ask for confirmation
            var confirm = await Shell.Current.DisplayAlert(
                "Abmelden",
                "Möchtest du dich wirklich abmelden?",
                "Ja, abmelden",
                "Abbrechen");

            if (confirm)
            {
                await NavigationService.LogoutAsync();
            }
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Abmelden: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    protected virtual async Task RefreshAsync()
    {
        // Override in derived classes to implement refresh logic
        await Task.CompletedTask;
    }
}
