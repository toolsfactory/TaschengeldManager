using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

public partial class ParentDashboardViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _welcomeMessage = "Willkommen!";

    [ObservableProperty]
    private bool _hasFamily;

    [ObservableProperty]
    private string _familyName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ChildSummary> _children = [];

    [ObservableProperty]
    private ObservableCollection<RequestSummary> _pendingRequests = [];

    [ObservableProperty]
    private int _pendingRequestsCount;

    [ObservableProperty]
    private bool _isRefreshing;

    public ParentDashboardViewModel(
        ITaschengeldApi api,
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _api = api;
        _authService = authService;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        var user = _authService.CurrentUser;
        if (user != null)
        {
            WelcomeMessage = $"Willkommen, {user.Nickname ?? user.Email}!";
        }

        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadDataAsync();
        IsRefreshing = false;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;

            // TODO: Load actual data from API
            // var family = await _api.GetMyFamilyAsync();
            // if (family != null) { ... }

            // Placeholder data
            HasFamily = false;
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Laden: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateFamilyAsync()
    {
        // TODO: Navigate to create family page
        await Shell.Current.DisplayAlert("Info", "Familie erstellen wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task AddChildAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ParentAddChild);
    }

    [RelayCommand]
    private async Task ChildSelectedAsync(ChildSummary? child)
    {
        if (child == null) return;
        await _navigationService.NavigateToAsync(Routes.ParentAccountDetail, new Dictionary<string, object>
        {
            { "childId", child.Id }
        });
    }
}

public class ChildSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Initials => string.IsNullOrEmpty(Name) ? "?" : Name[0].ToString().ToUpper();
}

public class RequestSummary
{
    public Guid Id { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
