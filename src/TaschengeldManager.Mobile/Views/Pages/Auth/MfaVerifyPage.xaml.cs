using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.Views.Pages.Auth;

public partial class MfaVerifyPage : ContentPage
{
    public MfaVerifyPage(MfaVerifyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
