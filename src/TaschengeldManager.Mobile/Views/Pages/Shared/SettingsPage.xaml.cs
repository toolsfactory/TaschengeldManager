using TaschengeldManager.Mobile.ViewModels.Shared;

namespace TaschengeldManager.Mobile.Views.Pages.Shared;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SettingsViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }

    private async void OnBiometricToggled(object sender, ToggledEventArgs e)
    {
        if (BindingContext is SettingsViewModel vm)
        {
            await vm.ToggleBiometricAsync(e.Value);
        }
    }
}
