using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages
{
    public partial class ManageProductsPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;

        public ManageProductsPage(FirebaseService firebaseService)
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
                var allProducts = await _firebaseService.GetProductsAsync();
                var sellerProducts = allProducts.Where(p => p.SellerId == AuthService.UserId).ToList();
                ProductsList.ItemsSource = sellerProducts;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load products: " + ex.Message, "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            var product = (sender as View).GestureRecognizers.OfType<TapGestureRecognizer>().First().CommandParameter as Product;
            if (product != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Product", product }
                };
                await Shell.Current.GoToAsync("AddEditProductPage", navigationParameter);
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var product = (sender as View).GestureRecognizers.OfType<TapGestureRecognizer>().First().CommandParameter as Product;
            if (product != null)
            {
                bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete {product.Title}?", "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        await _firebaseService.DeleteProductAsync(product.Id);
                        await LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Delete failed: " + ex.Message, "OK");
                    }
                }
            }
        }
    }
}