using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using c971_project.Services.Firebase;
using c971_project.Models;

namespace c971_project.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isNotBusy = true;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            IsNotBusy = false;

            try
            {
                var success = await _authService.LoginAsync(Email, Password);
                if (success)
                {
                    // Navigate to main app
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            finally
            {
                IsNotBusy = true;
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            IsNotBusy = false;

            try
            {
                var success = await _authService.RegisterAsync(Email, Password);
                if (success)
                {
                    // CREATE STUDENT PROFILE AFTER SUCCESSFUL REGISTRATION
                    await CreateStudentProfile();
                    await Shell.Current.DisplayAlert("Success", "Account created! Please login.", "OK");
                }
            }
            finally
            {
                IsNotBusy = true;
            }
        }

        private async Task CreateStudentProfile()
        {
            var student = new Student
            {
                // Don't set Id here - it's set in constructor
                // We'll override it with the Firebase UID
            };

            // Override the generated ID with Firebase UID
            student.Id = _authService.CurrentUserId;

            student.Email = Email;
            student.Name = Email.Split('@')[0];
            student.StudentIdNumber = GenerateStudentId();
            student.Major = "Undeclared";
            student.Status = "Currently Enrolled";

            // TODO: Save to Firestore when we implement data service
            System.Diagnostics.Debug.WriteLine($"Student profile created for: {student.Name}");

            // For now, show a message
            await Shell.Current.DisplayAlert("Profile Created",
                $"Student profile created for {student.Name}. Please complete your details in the app.", "OK");
        }

        private string GenerateStudentId()
        {
            // Simple temporary ID generation
            return $"WGU{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}