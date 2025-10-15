using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Diagnostics;
using c971_project;


namespace c971_project.Services.Firebase
{
    public class AuthService
    {
        private readonly FirebaseAuthClient _authClient;

        public AuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = FirebaseConstants.ApiKey,
                AuthDomain = FirebaseConstants.AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
            new EmailProvider()
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
                Debug.WriteLine($"AUTH LOGIN ERROR: {ex.Message}");
                throw; // Re-throw to let ViewModel handle it
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
                Debug.WriteLine($"AUTH REGISTER ERROR: {ex.Message}");
                throw; // Re-throw to let ViewModel handle it
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