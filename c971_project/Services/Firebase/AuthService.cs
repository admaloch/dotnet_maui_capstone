using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Diagnostics;
using c971_project;


namespace c971_project.Services.Firebase
{
    public class AuthService
    {
        private readonly FirebaseAuthClient _authClient;
        private bool _isLoggedIn;

        public AuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyDyvCzaynVSUc45mswyrZInbGhpoKksr_A",
                AuthDomain = "wgu-cloud-planner.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                new EmailProvider()
                }
            };
            _authClient = new FirebaseAuthClient(config);

            // Initialize login state
            _isLoggedIn = _authClient.User != null;

            // Debug initial state
            Debug.WriteLine($"AuthService initialized - User: {_authClient.User?.Uid ?? "null"}");
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
                _isLoggedIn = result != null;

                Debug.WriteLine($"Login successful - User: {result?.User?.Uid ?? "null"}");
                return _isLoggedIn;
            }
            catch (Exception ex)
            {
                _isLoggedIn = false;
                Debug.WriteLine($"AUTH LOGIN ERROR: {ex.Message}");
                throw; // Re-throw to let ViewModel handle it
            }
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var result = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
                _isLoggedIn = result != null;

                Debug.WriteLine($"Registration successful - User: {result?.User?.Uid ?? "null"}");
                return _isLoggedIn;
            }
            catch (Exception ex)
            {
                _isLoggedIn = false;
                Debug.WriteLine($"AUTH REGISTER ERROR: {ex.Message}");
                throw; // Re-throw to let ViewModel handle it
            }
        }

        public async Task DeleteUserAsync()
        {
            try
            {
                if (_authClient.User != null)
                {
                    // Delete the Firebase authentication user
                    await _authClient.User.DeleteAsync();
                    Debug.WriteLine("Firebase authentication user deleted successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting Firebase user: {ex.Message}");
                throw; // Re-throw to handle in ViewModel
            }
        }

        public void Logout()
        {
            try
            {
                var userIdBefore = _authClient.User?.Uid;
                var emailBefore = _authClient.User?.Info?.Email;

                _authClient.SignOut();

                // Force update internal state
                _isLoggedIn = false;

                // Clear any cached data
                ClearAuthCache();

                Debug.WriteLine($"Logout completed. User {userIdBefore} ({emailBefore}) signed out.");
                Debug.WriteLine($"After logout - IsLoggedIn: {IsLoggedIn}, User: {_authClient.User?.Uid ?? "null"}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logout error: {ex.Message}");
                // Even if there's an error, force logged out state
                _isLoggedIn = false;
                ClearAuthCache();
            }
        }

        private void ClearAuthCache()
        {
            try
            {
                // Remove any stored credentials or cached data
                if (Preferences.ContainsKey("user_id"))
                    Preferences.Remove("user_id");

                if (Preferences.ContainsKey("user_email"))
                    Preferences.Remove("user_email");

                if (Preferences.ContainsKey("firebase_token"))
                    Preferences.Remove("firebase_token");

                Debug.WriteLine("Auth cache cleared");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing auth cache: {ex.Message}");
            }
        }

        // Enhanced properties with additional checks
        public bool IsLoggedIn => _authClient.User != null && _isLoggedIn;
        public string CurrentUserId => IsLoggedIn ? _authClient.User.Uid : null;
        public string CurrentUserEmail => IsLoggedIn ? _authClient.User.Info.Email : null;

        // Method to verify auth state (useful for debugging)
        public string GetAuthState()
        {
            return $"Internal: {_isLoggedIn}, Firebase: {_authClient.User != null}, UserId: {_authClient.User?.Uid ?? "null"}";
        }
    }
}