using TaschengeldManager.Mobile.ViewModels.Child;

namespace TaschengeldManager.Mobile.Views.Pages.Child;

public partial class ChildAddExpensePage : ContentPage
{
    public ChildAddExpensePage(ChildAddExpenseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
