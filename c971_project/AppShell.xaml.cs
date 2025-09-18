using c971_project.Views;

namespace c971_project
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EditStudentPage), typeof(EditStudentPage));
            Routing.RegisterRoute(nameof(AddTermPage), typeof(AddTermPage));
            Routing.RegisterRoute(nameof(TermPage), typeof(TermPage));
            Routing.RegisterRoute(nameof(AddCoursePage), typeof(AddCoursePage));

        }
    }


}
