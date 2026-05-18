using Microsoft.Maui.Controls;
using System;
using System.Linq;
using Temu_Catarig.Services;

namespace Temu_Catarig.Pages
{
    public partial class CartPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;

        public CartPage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCart();
        }

        private async Task LoadCart()
        {
            if (!AuthService.IsLoggedIn)
            {
                EmptyCartLabel.IsVisible = true;
                CartCollection.IsVisible = false;
                CheckoutBar.IsVisible = false;
                return;
            }

            try
            {
                var items = await _firebaseService.GetCartItemsAsync(AuthService.UserId!);
                CartCollection.ItemsSource = items;

                bool hasItems = items != null && items.Count > 0;
                EmptyCartLabel.IsVisible = !hasItems;
                CartCollection.IsVisible = hasItems;
                CheckoutBar.IsVisible = hasItems;

                if (hasItems)
                {
                    double total = items!.Sum(i => i.Price * i.Quantity);
                    TotalCountLabel.Text = $"Total ({items.Count} items):";
                    TotalPriceLabel.Text = $"${total:F2}";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not load cart: " + ex.Message, "OK");
            }
        }

        private async void OnIncreaseQuantityClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Models.CartItem item)
            {
                item.Quantity++;
                await _firebaseService.AddToCartAsync(AuthService.UserId!, item);
                await LoadCart();
            }
        }

        private async void OnDecreaseQuantityClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Models.CartItem item)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    await _firebaseService.AddToCartAsync(AuthService.UserId!, item);
                }
                else
                {
                    bool answer = await DisplayAlert("Remove Item", "Do you want to remove this item from your cart?", "Yes", "No");
                    if (answer)
                    {
                        await _firebaseService.RemoveFromCartAsync(AuthService.UserId!, item.ProductId);
                    }
                }
                await LoadCart();
            }
        }

        private async void OnCheckoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("CheckoutPage");
        }

        private async void OnContinueShoppingClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LandingPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }
    }
}
