using c971_project.Models;
using c971_project.Services.Firebase;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using c971_project.Views;


namespace c971_project.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;


        public LoginViewModel(IAuthService authService, IFirestoreDataService firestoreDataService)
        {
            _authService = authService;
            _firestoreDataService = firestoreDataService;
#if DEBUG
            // Only pre-fill in debug mode
            email = "admaloch91@gmail.com";
            password = "pass12345";
#endif
        }

        [RelayCommand]
        private async Task Login()
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
                    "We had trouble logging you in. Check your internet connection and try again", "OK");
                Debug.WriteLine($"LOGIN ERROR: {ex}");
            }
            finally { IsBusy = false; }

        }

        [RelayCommand]
        private async Task OnRegisterPageAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(RegisterPage));
            }
            finally { IsBusy = false; }
        }
    }
}