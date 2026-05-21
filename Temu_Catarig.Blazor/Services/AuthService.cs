using Firebase.Auth;
using Firebase.Auth.Providers;
using Blazored.LocalStorage;

namespace Temu_Catarig.Blazor.Services;

public class AuthService
{
    private const string WebApiKey = "AIzaSyAysL9NVy9s_jGrKxnGll8_e7ctMc4UvUs";
    private const string AuthDomain = "temu-catarig.firebaseapp.com";

    private readonly FirebaseAuthClient _authClient;
    private readonly ILocalStorageService _localStorage;

    public string? UserToken { get; private set; }
    public string? UserId { get; private set; }
    public bool IsAdmin { get; private set; }

    public AuthService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        
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

    public async Task InitializeAsync()
    {
        UserId = await _localStorage.GetItemAsync<string>("userId");
        UserToken = await _localStorage.GetItemAsync<string>("userToken");
        IsAdmin = await _localStorage.GetItemAsync<bool>("isAdmin");
    }

    public async Task<string> GetFreshTokenAsync()
    {
        if (_authClient.User != null)
        {
            UserToken = await _authClient.User.GetIdTokenAsync();
            await _localStorage.SetItemAsync("userToken", UserToken);
            return UserToken;
        }
        return UserToken ?? string.Empty;
    }

    public string? UserEmail => _authClient.User?.Info?.Email;
    public string? UserDisplayName => _authClient.User?.Info?.DisplayName;

    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

    public async Task<(bool Success, string ErrorMessage)> RegisterUser(string email, string password)
    {
        try
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            UserToken = await userCredential.User.GetIdTokenAsync();
            UserId = userCredential.User.Uid;
            
            await SaveAuthState();
            
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, GetFriendlyErrorMessage(ex.Message));
        }
    }

    public async Task<(bool Success, string ErrorMessage)> LoginUser(string email, string password)
    {
        // Admin hardcoded login
        if (email?.Trim().ToLower() == "johnadmin@gmail.com" && password == "123123")
        {
            IsAdmin = true;
            UserId = "ADMIN_ID";
            UserToken = "ADMIN_TOKEN";
            await SaveAuthState();
            return (true, string.Empty);
        }

        try
        {
            var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            UserToken = await userCredential.User.GetIdTokenAsync();
            UserId = userCredential.User.Uid;
            IsAdmin = false;
            
            await SaveAuthState();
            
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, GetFriendlyErrorMessage(ex.Message));
        }
    }

    private async Task SaveAuthState()
    {
        await _localStorage.SetItemAsync("userId", UserId);
        await _localStorage.SetItemAsync("userToken", UserToken);
        await _localStorage.SetItemAsync("isAdmin", IsAdmin);
    }

    public async Task Logout()
    {
        if (!IsAdmin)
        {
            _authClient.SignOut();
        }
        UserToken = null;
        UserId = null;
        IsAdmin = false;
        await _localStorage.ClearAsync();
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
        
        return technicalMessage;
    }
}
