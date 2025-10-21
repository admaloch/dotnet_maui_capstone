using c971_project.Services.Firebase;
using c971_project.Views;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using c971_project.Core.Services;

namespace c971_project
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell(IAuthService authService)
        {
            _authService = authService;
            InitializeComponent();

            FlyoutBehavior = FlyoutBehavior.Disabled;

            // Register all routes (for navigation within the app)
            RegisterRoutes();

            // Subscribe to auth state changes
            _authService.AuthStateChanged += OnAuthStateChanged;

            // Set initial page based on auth state
            NavigateToInitialPage();
        }

        private void RegisterRoutes()
        {
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

        private void NavigateToInitialPage()
        {
            // Delay slightly to ensure Shell is fully loaded before navigating
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(50), () =>
            {
                if (_authService.IsAuthenticated())
                {
                    // Absolute route clears back stack
                    GoToAsync("//HomePage");
                    Debug.WriteLine("AppShell: User authenticated, navigating to HomePage.");
                }
                else
                {
                    GoToAsync("//LoginPage");
                    Debug.WriteLine("AppShell: User not authenticated, navigating to LoginPage.");
                }
            });
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            // When login/logout happens, switch pages immediately
            Dispatcher.Dispatch(() =>
            {
                if (_authService.IsAuthenticated())
                {
                    GoToAsync("//HomePage");
                    Debug.WriteLine("AuthStateChanged: Navigating to HomePage.");
                }
                else
                {
                    GoToAsync("//LoginPage");
                    Debug.WriteLine("AuthStateChanged: Navigating to LoginPage.");
                }
            });
        }

        protected override void OnDisappearing()
        {
            // Unsubscribe to avoid memory leaks
            _authService.AuthStateChanged -= OnAuthStateChanged;
            base.OnDisappearing();
        }
    }
}
