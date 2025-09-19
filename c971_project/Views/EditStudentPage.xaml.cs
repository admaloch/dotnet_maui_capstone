using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class EditStudentPage : ContentPage
    {
        public EditStudentPage(EditStudentViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
