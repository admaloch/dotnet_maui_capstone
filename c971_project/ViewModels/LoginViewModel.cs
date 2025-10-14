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