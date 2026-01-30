using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Relative;

public partial class RelativeDashboardViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<ConnectedFamilyItem> _families = [];

    [ObservableProperty]
    private ObservableCollection<RecentGiftItem> _recentGifts = [];

    [ObservableProperty]
    private bool _isRefreshing;

    public RelativeDashboardViewModel(
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
            // var families = await _api.GetConnectedFamiliesAsync();
            // var gifts = await _api.GetRecentGiftsAsync();
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
    private async Task SendGiftAsync()
    {
        await _navigationService.NavigateToAsync(Routes.RelativeSendGift);
    }

    [RelayCommand]
    private async Task ShowAllGiftsAsync()
    {
        await _navigationService.NavigateToAsync(Routes.RelativeGifts);
    }
}

public class ConnectedFamilyItem
{
    public Guid Id { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public int ChildrenCount { get; set; }
}

public class RecentGiftItem
{
    public Guid Id { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
