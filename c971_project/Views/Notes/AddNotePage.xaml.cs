using c971_project.ViewModels;

namespace c971_project.Views;
public partial class AddNotePage : ContentPage
{
    public AddNotePage(AddNoteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}