using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class EditCoursePage : ContentPage
    {
        public EditCoursePage(EditCourseViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
