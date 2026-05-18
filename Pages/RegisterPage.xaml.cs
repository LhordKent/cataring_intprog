using Microsoft.Maui.Controls;
using System;
using Temu_Catarig.Services;

namespace Temu_Catarig.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegisterPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            var result = await _authService.RegisterUser(email, password);
            if (result.Success)
            {
                _authService.Logout();
                await DisplayAlert("Success", "Account created successfully! Please log in.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await DisplayAlert("Registration Failed", result.ErrorMessage, "OK");
            }
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
