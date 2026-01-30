using TaschengeldManager.Mobile.ViewModels.Parent;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentAddChildPage : ContentPage
{
    public ParentAddChildPage(ParentAddChildViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
