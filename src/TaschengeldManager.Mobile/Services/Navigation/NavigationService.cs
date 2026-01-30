using TaschengeldManager.Mobile.Services.Auth;

namespace TaschengeldManager.Mobile.Services.Navigation;

/// <summary>
/// Navigation service implementation using Shell navigation
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IAuthenticationService _authService;

    public NavigationService(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters != null)
        {
            await Shell.Current.GoToAsync(route, parameters);
        }
        else
        {
            await Shell.Current.GoToAsync(route);
        }
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task GoToRootAsync()
    {
        await Shell.Current.GoToAsync("//");
    }

    public async Task NavigateToLoginAsync()
    {
        // Clear navigation stack and go to login
        await Shell.Current.GoToAsync($"///{Routes.Login}");
    }

    public async Task NavigateToMainAsync()
    {
        var user = _authService.CurrentUser;
        if (user == null)
        {
            await NavigateToLoginAsync();
            return;
        }

        var route = user.Role switch
        {
            Core.Enums.UserRole.Parent => $"///{Routes.ParentDashboard}",
            Core.Enums.UserRole.Child => $"///{Routes.ChildDashboard}",
            Core.Enums.UserRole.Relative => $"///{Routes.RelativeDashboard}",
            _ => $"///{Routes.Login}"
        };

        await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToBiometricSetupAsync()
    {
        await Shell.Current.GoToAsync(Routes.BiometricSetup);
    }

    public async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await NavigateToLoginAsync();
    }
}
