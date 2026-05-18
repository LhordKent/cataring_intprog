using Microsoft.Maui.Controls;
using System;
using Temu_Catarig.Services;

namespace Temu_Catarig.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            var result = await _authService.LoginUser(email, password);
            if (result.Success)
            {
                if (AuthService.IsAdmin)
                {
                    await Shell.Current.GoToAsync("///AdminDashboardPage");
                }
                else
                {
                    await Shell.Current.GoToAsync("//LandingPage");
                }
            }
            else
            {
                await DisplayAlert("Login Failed", result.ErrorMessage, "OK");
            }
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }
    }
}
