using Microsoft.Maui.Controls;
using System;
using Temu_Catarig.Services;

namespace Temu_Catarig.Pages
{
    public partial class ForgotPasswordPage : ContentPage
    {
        private readonly AuthService _authService;

        public ForgotPasswordPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnResetClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EmailEntry.Text))
            {
                await DisplayAlert("Error", "Please enter your Gmail.", "OK");
                return;
            }

            var result = await _authService.ResetPassword(EmailEntry.Text.Trim());

            if (result.Success)
            {
                await DisplayAlert("Reset Sent", "Check your Gmail for the reset link.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage, "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
