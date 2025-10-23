using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class CoursePage : ContentPage
    {
        public CoursePage(CourseViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}