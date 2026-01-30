using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Relative;

public partial class RelativeGiftsViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<GiftItem> _gifts = [];

    [ObservableProperty]
    private decimal _totalGiftsAmount;

    [ObservableProperty]
    private int _giftsCount;

    [ObservableProperty]
    private bool _isRefreshing;

    public RelativeGiftsViewModel(
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
            // var gifts = await _api.GetMyGiftsAsync();
            // Gifts = new ObservableCollection<GiftItem>(gifts.Select(...));

            TotalGiftsAmount = Gifts.Sum(g => g.Amount);
            GiftsCount = Gifts.Count;
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
}

public class GiftItem
{
    public Guid Id { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Message { get; set; }

    public string RecipientInitials => string.IsNullOrEmpty(RecipientName) ? "?" : RecipientName[0].ToString().ToUpper();
}
