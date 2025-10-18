using c971_project.Views;
using Microsoft.Maui.Controls;

namespace c971_project
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
            Routing.RegisterRoute(nameof(ReportsPage), typeof(ReportsPage));


            Routing.RegisterRoute(nameof(EditStudentPage), typeof(EditStudentPage));

            Routing.RegisterRoute(nameof(TermPage), typeof(TermPage));
            Routing.RegisterRoute(nameof(AddTermPage), typeof(AddTermPage));
            Routing.RegisterRoute(nameof(EditTermPage), typeof(EditTermPage));

            Routing.RegisterRoute(nameof(CoursePage), typeof(CoursePage));
            Routing.RegisterRoute(nameof(AddCoursePage), typeof(AddCoursePage));
            Routing.RegisterRoute(nameof(EditCoursePage), typeof(EditCoursePage));

            Routing.RegisterRoute(nameof(AssessmentPage), typeof(AssessmentPage));
            Routing.RegisterRoute(nameof(AddAssessmentPage), typeof(AddAssessmentPage));
            Routing.RegisterRoute(nameof(EditAssessmentPage), typeof(EditAssessmentPage));

            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
            Routing.RegisterRoute(nameof(AddNotePage), typeof(AddNotePage));
            Routing.RegisterRoute(nameof(EditNotePage), typeof(EditNotePage));

            Routing.RegisterRoute(nameof(InstructorPage), typeof(InstructorPage));
        }
    }


}
