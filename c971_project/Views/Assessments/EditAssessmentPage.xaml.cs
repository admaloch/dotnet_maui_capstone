using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class EditAssessmentPage : ContentPage
    {
        public EditAssessmentPage(EditAssessmentViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
