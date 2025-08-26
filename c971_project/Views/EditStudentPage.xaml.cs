using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class EditStudentPage : ContentPage
    {
        public EditStudentPage(StudentViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;  // ViewModel is automatically injected
        }
    }
}