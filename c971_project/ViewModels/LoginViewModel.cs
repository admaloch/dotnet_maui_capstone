using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using c971_project.Services.Firebase;
using c971_project.Models;

namespace c971_project.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isNotBusy = true;

        public LoginViewModel(AuthService authService, IFirestoreDataService firestoreDataService)
        {
            _authService = authService;
            _firestoreDataService = firestoreDataService;
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

                    // AUTO-LOGIN AND NAVIGATE TO HOME
                    await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Registration Error", ex.Message, "OK");
            }
            finally
            {
                IsNotBusy = true;
            }
        }

        private async Task CreateStudentProfile()
        {
            try
            {
                var student = new Student();

                // Override the generated ID with Firebase UID
                student.Id = _authService.CurrentUserId;

                student.Email = Email;
                student.Name = Email.Split('@')[0];
                student.StudentIdNumber = GenerateStudentId();
                student.Major = "Undeclared";
                student.Status = "Currently Enrolled";

                // SAVE TO FIRESTORE
                await _firestoreDataService.SaveStudentAsync(student);

                // Removed the alert here - let Register handle the success message
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Failed to create student profile: {ex.Message}", "OK");
                throw; // Re-throw so Register knows it failed
            }
        }

        private string GenerateStudentId()
        {
            // Simple temporary ID generation
            return $"WGU{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}