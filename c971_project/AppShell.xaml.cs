using c971_project.Services;
using c971_project.Services.Firebase;
using c971_project.Views;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace c971_project
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;

        public AppShell(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();

            // Register all routes
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

            // Initialize menu visibility
            UpdateMenuVisibility();

            // Subscribe to auth state changes
            _authService.AuthStateChanged += OnAuthStateChanged;

            AutoNavigateToHome();
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            UpdateMenuVisibility();
        }

        private void UpdateMenuVisibility()
        {
            var isAuthenticated = _authService.IsAuthenticated();

            // Show app menu items only when authenticated
            HomeItem.IsVisible = isAuthenticated;
            SearchItem.IsVisible = isAuthenticated;
            ReportsItem.IsVisible = isAuthenticated;

            // Show login only when NOT authenticated
            LoginItem.IsVisible = !isAuthenticated;

            // Force UI update
            OnPropertyChanged(nameof(CurrentItem));
        }

        private void AutoNavigateToHome()
        {
            if (_authService.IsAuthenticated())
            {
                // Small delay to ensure Shell is fully initialized
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    try
                    {
                        CurrentItem = HomeItem;
                        Debug.WriteLine("Auto-navigated to HomePage");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Auto-navigation error: {ex.Message}");
                    }
                });
            }
        }

        protected override void OnDisappearing()
        {
            // Clean up event subscription
            _authService.AuthStateChanged -= OnAuthStateChanged;
            base.OnDisappearing();
        }
    }
}