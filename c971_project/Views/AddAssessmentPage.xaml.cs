using c971_project.ViewModels;

namespace c971_project.Views;
public partial class AddAssessmentPage : ContentPage
{
    public AddAssessmentPage(AddAssessmentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}