using Microsoft.Maui.Controls;
using System;
using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages
{
    public partial class LandingPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;

        public LandingPage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await _firebaseService.GetProductsAsync("Approved");
                ProductsCollection.ItemsSource = products;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not load products: " + ex.Message, "OK");
            }
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }

        private async void OnCartClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("CartPage");
        }

        private async void OnProductTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is Models.Product product)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Product", product }
                };
                await Shell.Current.GoToAsync("ProductDetailPage", parameters);
            }
        }
    }
}
