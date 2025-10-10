using Firebase.Auth;
using Firebase.Auth.Providers;

namespace c971_project.Services.Firebase
{
    public class AuthService
    {
        private readonly FirebaseAuthClient _authClient;

        public AuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyA08iMZxu5NZWCquBPGqyNxBDWVNCg-1rQ", // From Firebase Console > Project Settings
                AuthDomain = "wgu-cloud-planner.firebaseapp.com", // From Firebase Console
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider() // Enable email/password auth
                }
            };

            _authClient = new FirebaseAuthClient(config);
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
                return result != null;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Login Error", ex.Message, "OK");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var result = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
                return result != null;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Registration Error", ex.Message, "OK");
                return false;
            }
        }

        public void Logout()
        {
            _authClient.SignOut();
        }

        public bool IsLoggedIn => _authClient.User != null;
        public string CurrentUserId => _authClient.User?.Uid;
        public string CurrentUserEmail => _authClient.User?.Info?.Email;
    }
}