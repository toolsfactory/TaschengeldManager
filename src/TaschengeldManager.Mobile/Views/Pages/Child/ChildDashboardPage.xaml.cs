using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Child;

namespace TaschengeldManager.Mobile.Views.Pages.Child;

public partial class ChildDashboardPage : ContentPage
{
    private readonly INavigationService _navigationService;
    private readonly ChildDashboardViewModel _viewModel;

    public ChildDashboardPage(ChildDashboardViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _navigationService = navigationService;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlertAsync(
            "Abmelden",
            "Möchtest du dich wirklich abmelden?",
            "Ja, abmelden",
            "Abbrechen");

        if (confirm)
        {
            await _navigationService.LogoutAsync();
        }
    }
}
