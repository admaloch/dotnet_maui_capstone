using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class EditNotePage : ContentPage
    {
        public EditNotePage(EditNoteViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}