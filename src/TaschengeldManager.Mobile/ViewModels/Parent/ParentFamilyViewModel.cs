using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Feedback;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

public partial class ParentFamilyViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    private Guid? _familyId;

    [ObservableProperty]
    private bool _hasFamily;

    [ObservableProperty]
    private bool _hasNoFamily;

    [ObservableProperty]
    private string _familyName = string.Empty;

    [ObservableProperty]
    private string _familyCode = string.Empty;

    [ObservableProperty]
    private ObservableCollection<FamilyMemberItem> _children = [];

    [ObservableProperty]
    private ObservableCollection<FamilyMemberItem> _relatives = [];

    [ObservableProperty]
    private bool _isRefreshing;

    public ParentFamilyViewModel(
        ITaschengeldApi api,
        IAuthenticationService authService,
        INavigationService navigationService,
        IToastService toastService)
    {
        _api = api;
        _authService = authService;
        _navigationService = navigationService;
        _toastService = toastService;
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
            ClearError();

            var families = await _api.GetMyFamiliesAsync();

            if (families.Count > 0)
            {
                var family = families[0];
                _familyId = family.Id;
                FamilyName = family.Name;
                FamilyCode = family.FamilyCode ?? "N/A";
                HasFamily = true;
                HasNoFamily = false;

                // Load children
                var children = await _api.GetChildrenAsync(family.Id);
                Children.Clear();
                foreach (var child in children)
                {
                    Children.Add(new FamilyMemberItem
                    {
                        Id = child.Id,
                        Nickname = child.Nickname,
                        Balance = child.Balance ?? 0,
                        AccountId = child.AccountId
                    });
                }

                // Load family members (relatives)
                var members = await _api.GetFamilyMembersAsync(family.Id);
                Relatives.Clear();
                foreach (var member in members.Where(m => m.Role == Core.Enums.UserRole.Relative))
                {
                    Relatives.Add(new FamilyMemberItem
                    {
                        Id = member.Id,
                        Nickname = member.Nickname,
                        Email = member.Email ?? string.Empty
                    });
                }
            }
            else
            {
                HasFamily = false;
                HasNoFamily = true;
                _familyId = null;
            }
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
        var familyName = await Shell.Current.DisplayPromptAsync(
            "Familie erstellen",
            "Wie soll deine Familie heissen?",
            "Erstellen",
            "Abbrechen",
            "z.B. Familie Mueller");

        if (!string.IsNullOrWhiteSpace(familyName))
        {
            try
            {
                IsBusy = true;
                var request = new CreateFamilyRequest { Name = familyName };
                var family = await _api.CreateFamilyAsync(request);
                await _toastService.ShowSuccessAsync($"Familie '{family.Name}' erstellt!");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Fehler: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task CopyFamilyCodeAsync()
    {
        if (!string.IsNullOrEmpty(FamilyCode))
        {
            await Clipboard.Default.SetTextAsync(FamilyCode);
            await _toastService.ShowSuccessAsync("Familien-Code kopiert!");
        }
    }

    [RelayCommand]
    private async Task AddChildAsync()
    {
        if (_familyId == null)
        {
            await _toastService.ShowWarningAsync("Bitte erstelle zuerst eine Familie.");
            return;
        }

        await _navigationService.NavigateToAsync(Routes.ParentAddChild, new Dictionary<string, object>
        {
            { "familyId", _familyId.Value }
        });
    }

    [RelayCommand]
    private async Task ChildSelectedAsync(FamilyMemberItem? child)
    {
        if (child == null) return;

        if (child.AccountId.HasValue)
        {
            await _navigationService.NavigateToAsync(Routes.ParentAccountDetail, new Dictionary<string, object>
            {
                { "childId", child.Id },
                { "accountId", child.AccountId.Value }
            });
        }
    }

    [RelayCommand]
    private async Task InviteRelativeAsync()
    {
        if (_familyId == null)
        {
            await _toastService.ShowWarningAsync("Bitte erstelle zuerst eine Familie.");
            return;
        }

        await _navigationService.NavigateToAsync(Routes.ParentInvite, new Dictionary<string, object>
        {
            { "familyId", _familyId.Value }
        });
    }

    [RelayCommand]
    private async Task RemoveChildAsync(FamilyMemberItem? child)
    {
        if (child == null || _familyId == null) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Kind entfernen",
            $"Moechtest du '{child.Nickname}' wirklich aus der Familie entfernen?\n\nDas Konto und alle Transaktionen werden geloescht. Diese Aktion kann nicht rueckgaengig gemacht werden!",
            "Ja, entfernen",
            "Abbrechen");

        if (!confirmed) return;

        try
        {
            IsBusy = true;
            await _api.RemoveChildAsync(_familyId.Value, child.Id);
            await _toastService.ShowSuccessAsync($"'{child.Nickname}' wurde aus der Familie entfernt.");
            Children.Remove(child);
        }
        catch (Exception ex)
        {
            await _toastService.ShowErrorAsync($"Fehler: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public class FamilyMemberItem
{
    public Guid Id { get; set; }
    public Guid? AccountId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Initials => string.IsNullOrEmpty(Nickname) ? "?" : Nickname[0].ToString().ToUpper();
    public string FormattedBalance => Balance.ToString("C", new System.Globalization.CultureInfo("de-DE"));
}
