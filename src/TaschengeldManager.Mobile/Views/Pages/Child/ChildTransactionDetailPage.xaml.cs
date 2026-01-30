using TaschengeldManager.Mobile.ViewModels.Child;

namespace TaschengeldManager.Mobile.Views.Pages.Child;

public partial class ChildTransactionDetailPage : ContentPage
{
    public ChildTransactionDetailPage(ChildTransactionDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
