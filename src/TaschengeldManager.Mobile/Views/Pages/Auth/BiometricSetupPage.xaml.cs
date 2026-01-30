using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.Views.Pages.Auth;

public partial class BiometricSetupPage : ContentPage
{
    public BiometricSetupPage(BiometricSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
