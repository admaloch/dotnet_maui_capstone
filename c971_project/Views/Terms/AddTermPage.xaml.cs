using c971_project.ViewModels;

namespace c971_project.Views;
public partial class AddTermPage : ContentPage
{
    public AddTermPage(AddTermViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}