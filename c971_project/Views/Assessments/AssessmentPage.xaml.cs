using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class AssessmentPage : ContentPage
    {
        public AssessmentPage(AssessmentViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}