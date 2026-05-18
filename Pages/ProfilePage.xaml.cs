using Microsoft.Maui.Controls;
using System;
using Temu_Catarig.Models;
using Temu_Catarig.Services;

namespace Temu_Catarig.Pages
{
    public partial class ProfilePage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private readonly AuthService _authService;
        private UserProfile _currentUserProfile;

        public ProfilePage(FirebaseService firebaseService, AuthService authService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserProfile();
        }

        private async System.Threading.Tasks.Task LoadUserProfile()
        {
            try
            {
                _currentUserProfile = await _firebaseService.GetUserProfileAsync(AuthService.UserId);
                if (_currentUserProfile != null)
                {
                    UserNameLabel.Text = _currentUserProfile.FullName ?? "User";
                }
                else
                {
                    UserNameLabel.Text = "User";
                }
            }
            catch (Exception)
            {
                UserNameLabel.Text = "User";
            }
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("EditProfilePage");
        }

        private async void OnOrdersClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MyOrdersPage");
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LandingPage");
        }

        private async void OnCartClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("CartPage");
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async void OnContactUsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ContactUsPage");
        }

        private async void OnFulfillmentClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("OrderFulfillmentPage");
        }

        private async void OnAddNewProductClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUserProfile?.FullName))
            {
                await DisplayAlert("Profile Incomplete", "Please complete your profile name first.", "OK");
                await Shell.Current.GoToAsync("EditProfilePage");
                return;
            }
            await Shell.Current.GoToAsync("AddEditProductPage");
        }

        private async void OnManageProductsClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUserProfile?.FullName))
            {
                await DisplayAlert("Profile Incomplete", "Please complete your profile name first.", "OK");
                await Shell.Current.GoToAsync("EditProfilePage");
                return;
            }
            await Shell.Current.GoToAsync("ManageProductsPage");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            _authService.Logout();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
