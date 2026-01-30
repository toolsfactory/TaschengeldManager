using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

/// <summary>
/// ViewModel for displaying all children's accounts (M005-S01)
/// </summary>
public partial class ParentChildAccountsViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<ChildAccountItem> _children = [];

    [ObservableProperty]
    private decimal _totalBalance;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _hasChildren;

    [ObservableProperty]
    private bool _showEmptyState;

    private Guid? _familyId;

    public ParentChildAccountsViewModel(
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
        await LoadFamilyIdAsync();
        await LoadDataAsync();
    }

    private async Task LoadFamilyIdAsync()
    {
        try
        {
            var families = await _api.GetMyFamiliesAsync();
            if (families.Count > 0)
            {
                _familyId = families[0].Id;
            }
        }
        catch (Exception ex)
        {
            SetError($"Familie konnte nicht geladen werden: {ex.Message}");
        }
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
        if (_familyId == null)
        {
            ShowEmptyState = true;
            HasChildren = false;
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var children = await _api.GetChildrenAsync(_familyId.Value);

            Children.Clear();
            TotalBalance = 0;

            foreach (var child in children)
            {
                var item = new ChildAccountItem
                {
                    ChildId = child.Id,
                    AccountId = child.AccountId,
                    Name = child.Nickname,
                    Balance = child.Balance ?? 0,
                    Initials = GetInitials(child.Nickname),
                    LastActivityText = "Keine Aktivität" // TODO: Fetch from transactions
                };

                Children.Add(item);
                TotalBalance += item.Balance;
            }

            HasChildren = Children.Count > 0;
            ShowEmptyState = !HasChildren;
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Laden der Konten: {ex.Message}");
            ShowEmptyState = true;
            HasChildren = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ChildSelectedAsync(ChildAccountItem? child)
    {
        if (child == null) return;

        await _navigationService.NavigateToAsync(Routes.ParentAccountDetail, new Dictionary<string, object>
        {
            { "childId", child.ChildId },
            { "accountId", child.AccountId ?? Guid.Empty }
        });
    }

    [RelayCommand]
    private async Task AddChildAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ParentAddChild);
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "?";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();

        return name[0].ToString().ToUpper();
    }
}

/// <summary>
/// Display model for a child account in the overview list
/// </summary>
public class ChildAccountItem
{
    public Guid ChildId { get; set; }
    public Guid? AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Initials { get; set; } = string.Empty;
    public string LastActivityText { get; set; } = string.Empty;

    public string FormattedBalance => Balance.ToString("C", new System.Globalization.CultureInfo("de-DE"));
}
