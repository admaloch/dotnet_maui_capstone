using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using c971_project.Services.Firebase;
using c971_project.Models;
using System.Diagnostics;
using c971_project.Views;


namespace c971_project.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string studentIdNumber;

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private Dictionary<string, string> _majorOptions = new();

        [ObservableProperty]
        private string _selectedMajorDisplayName = "Undeclared";

        // Computed property for the Picker
        public List<string> MajorDisplayNames => _majorOptions.Values.ToList();

        public RegisterViewModel(AuthService authService, IFirestoreDataService firestoreDataService)
        {
            _authService = authService;
            _firestoreDataService = firestoreDataService;
            _= LoadMajorsAsync();
        }

        private async Task LoadMajorsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                MajorOptions = await _firestoreDataService.GetMajorsAsync();
                OnPropertyChanged(nameof(MajorDisplayNames));
                // Set default selection
                if (MajorOptions.Count > 0)
                {
                    SelectedMajorDisplayName = MajorDisplayNames.First();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading majors: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to load majors list", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task OnLoginPageAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync("..");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var success = await _authService.RegisterAsync(Email, Password);
                if (success)
                {
                    await CreateStudentProfile();
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
                                  "We had trouble registering your account. Check your internet connection and try again", "OK");
                Debug.WriteLine($"REGISTRATION ERROR: {ex}");
            }
            finally { IsBusy = false; }

        }

        private async Task CreateStudentProfile()
        {
            var student = new Student();
            student.Id = _authService.CurrentUserId;
            student.Email = Email;
            student.StudentIdNumber = StudentIdNumber;
            student.Name = FullName;
            student.Major = SelectedMajorDisplayName; // Changed from SelectedMajor
            student.Status = "Currently Enrolled";

            await _firestoreDataService.SaveStudentAsync(student);
        }


    }
}