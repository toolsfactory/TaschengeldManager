using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.Views.Pages.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
