using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using c971_project.Services.Firebase;
using c971_project.Models;
using System.Diagnostics;


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
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Login Failed", "Invalid email or password", "OK");
                }
            }
            catch (Exception ex)
            {
                // Show detailed error
                await Shell.Current.DisplayAlert("Login Error",
                    $"Failed to login: {ex.Message}\n\nStack: {ex.StackTrace}", "OK");
                Debug.WriteLine($"LOGIN ERROR: {ex}");
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
                    await CreateStudentProfile();
                    await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Registration Failed", "Failed to create account", "OK");
                }
            }
            catch (Exception ex)
            {
                // Show detailed error
                await Shell.Current.DisplayAlert("Registration Error",
                    $"Failed to register: {ex.Message}\n\nStack: {ex.StackTrace}", "OK");
                Debug.WriteLine($"REGISTRATION ERROR: {ex}");
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