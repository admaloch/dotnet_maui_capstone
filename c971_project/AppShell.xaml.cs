using c971_project.Views;
using Microsoft.Maui.Controls;

namespace c971_project
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EditStudentPage), typeof(EditStudentPage));

            Routing.RegisterRoute(nameof(TermPage), typeof(TermPage));
            Routing.RegisterRoute(nameof(AddTermPage), typeof(AddTermPage));
            Routing.RegisterRoute(nameof(EditTermPage), typeof(EditTermPage));

            Routing.RegisterRoute(nameof(CoursePage), typeof(CoursePage));
            Routing.RegisterRoute(nameof(AddCoursePage), typeof(AddCoursePage));
            Routing.RegisterRoute(nameof(EditCoursePage), typeof(EditCoursePage));

            Routing.RegisterRoute(nameof(AddAssessmentPage), typeof(AddAssessmentPage));

            Routing.RegisterRoute(nameof(AddNotePage), typeof(AddNotePage));

        }
    }


}
