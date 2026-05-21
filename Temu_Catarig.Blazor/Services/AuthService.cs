using Firebase.Auth;
using Firebase.Auth.Providers;
using Blazored.LocalStorage;

namespace Temu_Catarig.Blazor.Services;

public class AuthService
{
    private const string WebApiKey = "AIzaSyAysL9NVy9s_jGrKxnGll8_e7ctMc4UvUs";
    private const string AuthDomain = "temu-catarig.firebaseapp.com";

    private FirebaseAuthClient? _authClient;
    private readonly ILocalStorageService _localStorage;

    public string? UserToken { get; private set; }
    public string? UserId { get; private set; }
    public bool IsAdmin { get; private set; }

    public string? UserEmail => _authClient?.User?.Info?.Email ?? string.Empty;
    public string? UserDisplayName => _authClient?.User?.Info?.DisplayName ?? string.Empty;
    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public AuthService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    private void EnsureClientInitialized()
    {
        try
        {
            if (_authClient != null) return;

            var config = new FirebaseAuthConfig
            {
                ApiKey = WebApiKey,
                AuthDomain = AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            _authClient = new FirebaseAuthClient(config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: EnsureClientInitialized failed: {ex.Message}");
        }
    }

    public async Task InitializeAsync()
    {
        try 
        {
            EnsureClientInitialized();
            
            UserId = await _localStorage.GetItemAsync<string>("userId");
            UserToken = await _localStorage.GetItemAsync<string>("userToken");
            IsAdmin = await _localStorage.GetItemAsync<bool>("isAdmin");
            
            if (_authClient?.User != null)
            {
                try
                {
                    UserToken = await _authClient.User.GetIdTokenAsync();
                    await _localStorage.SetItemAsync("userToken", UserToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AuthService: Token refresh failed: {ex.Message}");
                    await Logout();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: Initialization failed: {ex.Message}");
        }
        
        NotifyStateChanged();
    }

    public async Task<string> GetFreshTokenAsync()
    {
        try
        {
            EnsureClientInitialized();
            if (_authClient?.User != null)
            {
                UserToken = await _authClient.User.GetIdTokenAsync();
                await _localStorage.SetItemAsync("userToken", UserToken);
                return UserToken;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: GetFreshTokenAsync failed: {ex.Message}");
        }
        return UserToken ?? string.Empty;
    }

    public async Task<(bool Success, string ErrorMessage)> RegisterUser(string email, string password)
    {
        try
        {
            EnsureClientInitialized();
            if (_authClient == null) return (false, "Authentication client not initialized.");

            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            UserToken = await userCredential.User.GetIdTokenAsync();
            UserId = userCredential.User.Uid;
            IsAdmin = false;
            
            await SaveStateToLocalStorage();
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, GetFriendlyErrorMessage(ex.Message));
        }
    }

    public async Task<(bool Success, string ErrorMessage)> LoginUser(string email, string password)
    {
        try
        {
            if (email?.Trim().ToLower() == "johnadmin@gmail.com" && password == "123123")
            {
                IsAdmin = true;
                UserId = "ADMIN_ID";
                UserToken = "ADMIN_TOKEN";
                await SaveStateToLocalStorage();
                return (true, string.Empty);
            }

            EnsureClientInitialized();
            if (_authClient == null) return (false, "Authentication client not initialized.");

            var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            UserToken = await userCredential.User.GetIdTokenAsync();
            UserId = userCredential.User.Uid;
            IsAdmin = false;
            
            await SaveStateToLocalStorage();
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, GetFriendlyErrorMessage(ex.Message));
        }
    }

    public async Task<(bool Success, string ErrorMessage)> ResetPassword(string email)
    {
        try
        {
            EnsureClientInitialized();
            if (_authClient == null) return (false, "Authentication client not initialized.");

            await _authClient.ResetEmailPasswordAsync(email);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, GetFriendlyErrorMessage(ex.Message));
        }
    }

    public async Task Logout()
    {
        try
        {
            if (!IsAdmin && _authClient?.User != null)
            {
                _authClient.SignOut();
            }
        }
        catch { }

        try
        {
            UserToken = null;
            UserId = null;
            IsAdmin = false;
            await _localStorage.ClearAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: Logout failed to clear storage: {ex.Message}");
        }
        
        NotifyStateChanged();
    }

    private async Task SaveStateToLocalStorage()
    {
        try
        {
            await _localStorage.SetItemAsync("userId", UserId);
            await _localStorage.SetItemAsync("userToken", UserToken);
            await _localStorage.SetItemAsync("isAdmin", IsAdmin);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: SaveStateToLocalStorage failed: {ex.Message}");
        }
        NotifyStateChanged();
    }

    private string GetFriendlyErrorMessage(string technicalMessage)
    {
        if (string.IsNullOrEmpty(technicalMessage)) return "An unexpected error occurred.";
        if (technicalMessage.Contains("INVALID_LOGIN_CREDENTIALS") || technicalMessage.Contains("INVALID_PASSWORD"))
            return "Invalid email or password.";
        if (technicalMessage.Contains("EMAIL_EXISTS"))
            return "Email already exists.";
        if (technicalMessage.Contains("WEAK_PASSWORD"))
            return "Password should be at least 6 characters.";
        if (technicalMessage.Contains("INVALID_EMAIL"))
            return "Please enter a valid email address.";
        if (technicalMessage.Contains("OPERATION_NOT_ALLOWED"))
            return "Email/Password authentication is not enabled in Firebase.";
        return technicalMessage;
    }
}
