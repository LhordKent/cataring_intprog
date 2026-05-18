using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages
{
    public partial class EditProfilePage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private UserProfile _profile;

        public EditProfilePage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProfile();
        }

        private async Task LoadProfile()
        {
            try
            {
                _profile = await _firebaseService.GetUserProfileAsync(AuthService.UserId);
                
                NameEntry.Text = _profile.FullName;
                PhoneEntry.Text = _profile.PhoneNumber;
                AddressEditor.Text = _profile.ShippingAddress;
                EmailEntry.Text = string.IsNullOrEmpty(_profile.Email) ? AuthService.UserEmail : _profile.Email;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load profile: " + ex.Message, "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                await DisplayAlert("Error", "Name is required.", "OK");
                return;
            }

            try
            {
                _profile.FullName = NameEntry.Text;
                _profile.PhoneNumber = PhoneEntry.Text;
                _profile.ShippingAddress = AddressEditor.Text;
                _profile.Email = EmailEntry.Text;

                await _firebaseService.SaveUserProfileAsync(_profile);
                await DisplayAlert("Success", "Profile updated!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Save failed: " + ex.Message, "OK");
            }
        }
    }
}