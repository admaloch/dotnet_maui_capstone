using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class InstructorPage : ContentPage
    {
        public InstructorPage(InstructorViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}