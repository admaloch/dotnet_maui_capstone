using c971_project.ViewModels;

namespace c971_project.Views;
public partial class AddCoursePage : ContentPage
{
    public AddCoursePage(AddCourseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}