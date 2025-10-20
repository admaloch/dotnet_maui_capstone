using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Diagnostics;
using Microsoft.Maui.Storage;

namespace c971_project.Services.Firebase
{
    public class AuthService
    {
        private readonly FirebaseAuthClient _authClient;
        private bool _isLoggedIn;
        public event EventHandler AuthStateChanged;

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

            // Initialize auth state from SecureStorage
            InitializeAuthState();

            Debug.WriteLine($"AuthService initialized - User: {_authClient.User?.Uid ?? "null"}");
        }

        /// <summary>
        /// Checks SecureStorage for stored user ID to maintain login state across app restarts.
        /// </summary>
        public void InitializeAuthState()
        {
            try
            {
                var storedUserId = SecureStorage.GetAsync("user_id").GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(storedUserId) && _authClient.User == null)
                {
                    Debug.WriteLine($"Found stored user ID: {storedUserId}");
                    // Note: FirebaseAuthClient handles session persistence internally
                }

                _isLoggedIn = _authClient.User != null;

                AuthStateChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing auth state: {ex.Message}");
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
                _isLoggedIn = result != null;

                if (_isLoggedIn)
                {
                    await SecureStorage.SetAsync("user_id", result.User.Uid);
                    Debug.WriteLine($"Login successful - User: {result.User.Uid}");
                }

                AuthStateChanged?.Invoke(this, EventArgs.Empty);
                return _isLoggedIn;
            }
            catch (Exception ex)
            {
                _isLoggedIn = false;
                Debug.WriteLine($"AUTH LOGIN ERROR: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var result = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
                _isLoggedIn = result != null;

                if (_isLoggedIn)
                {
                    await SecureStorage.SetAsync("user_id", result.User.Uid);
                    AuthStateChanged?.Invoke(this, EventArgs.Empty);
                }

                return _isLoggedIn;
            }
            catch (Exception ex)
            {
                _isLoggedIn = false;
                Debug.WriteLine($"AUTH REGISTER ERROR: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteUserAsync()
        {
            try
            {
                if (_authClient.User != null)
                {
                    // Delete Firebase authentication user
                    await _authClient.User.DeleteAsync();

                    // Clear secure storage and cached auth data
                    SecureStorage.Remove("user_id");
                    SecureStorage.Remove("firebase_token");
                    ClearAuthCache();

                    // Update internal state
                    _isLoggedIn = false;
                    AuthStateChanged?.Invoke(this, EventArgs.Empty);

                    Debug.WriteLine("Firebase authentication user deleted successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting Firebase user: {ex.Message}");
                throw; // Let ViewModel handle the error if needed
            }
        }


        public void Logout()
        {
            try
            {
                var userIdBefore = _authClient.User?.Uid;

                _authClient.SignOut();
                _isLoggedIn = false;

                SecureStorage.Remove("user_id");
                ClearAuthCache();

                AuthStateChanged?.Invoke(this, EventArgs.Empty);

                Debug.WriteLine($"Logout successful - Previous user: {userIdBefore}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logout error: {ex.Message}");
                _isLoggedIn = false;
                SecureStorage.Remove("user_id");
                ClearAuthCache();
            }
        }

        private void ClearAuthCache()
        {
            try
            {
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

        // Determines if the user is logged in
        public bool IsAuthenticated() => _isLoggedIn && _authClient.User != null;

        public bool IsLoggedIn => _authClient.User != null && _isLoggedIn;
        public string CurrentUserId => IsLoggedIn ? _authClient.User.Uid : null;
        public string CurrentUserEmail => IsLoggedIn ? _authClient.User.Info.Email : null;
    }
}
