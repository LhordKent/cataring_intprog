using Firebase.Auth;
using Firebase.Auth.Providers;

namespace Temu_Catarig.Services;

public class AuthService
{
    private const string WebApiKey = "AIzaSyAysL9NVy9s_jGrKxnGll8_e7ctMc4UvUs";
    private const string AuthDomain = "temu-catarig.firebaseapp.com";

    private static readonly FirebaseAuthClient _authClient;

    public static string? UserToken { get; private set; }
    public static string? UserId { get; private set; }
    public static bool IsAdmin { get; private set; }

    public static async Task<string> GetFreshTokenAsync()
    {
        if (_authClient.User != null)
        {
            UserToken = await _authClient.User.GetIdTokenAsync();
            return UserToken;
        }
        return string.Empty;
    }

    public static string? UserEmail => _authClient.User?.Info?.Email;
    public static string? UserDisplayName => _authClient.User?.Info?.DisplayName;

    public static bool IsLoggedIn => _authClient.User != null;

    static AuthService()
    {
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

    public AuthService() { }

    public async Task InitializeAsync()
    {
        if (_authClient.User != null)
        {
            try
            {
                UserToken = await _authClient.User.GetIdTokenAsync();
                UserId = _authClient.User.Uid;
            }
            catch
            {
                Logout();
            }
        }
    }

    public async Task<(bool Success, string ErrorMessage)> RegisterUser(string email, string password)
    {
        try
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            UserToken = await userCredential.User.GetIdTokenAsync();
            UserId = userCredential.User.Uid;
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
            UserId = "ADMIN_ID"; // Placeholder for admin
            UserToken = "ADMIN_TOKEN";
            return (true, string.Empty);
        }

        try
        {
            var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            UserToken = await userCredential.User.GetIdTokenAsync();
            UserId = userCredential.User.Uid;
            IsAdmin = false;
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
            await _authClient.ResetEmailPasswordAsync(email);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, GetFriendlyErrorMessage(ex.Message));
        }
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
        
        return technicalMessage; // Return technical message for better debugging
    }

    public void Logout()
    {
        if (!IsAdmin)
        {
            _authClient.SignOut();
        }
        UserToken = null;
        UserId = null;
        IsAdmin = false;
    }
}
