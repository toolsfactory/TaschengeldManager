using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.Views.Pages.Auth;

public partial class ChildLoginPage : ContentPage
{
    public ChildLoginPage(ChildLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
