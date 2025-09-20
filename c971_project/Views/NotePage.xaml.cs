using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class NotePage : ContentPage
    {
        public NotePage(NoteViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}