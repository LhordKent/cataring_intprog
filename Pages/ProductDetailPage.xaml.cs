using Microsoft.Maui.Controls;
using System;
using System.Linq;
using Temu_Catarig.Services;

namespace Temu_Catarig.Pages
{
    [QueryProperty(nameof(SelectedProduct), "Product")]
    public partial class ProductDetailPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private readonly AuthService _authService;
        private Models.Product _selectedProduct;

        public Models.Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                BindingContext = _selectedProduct;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (_selectedProduct != null && AuthService.UserId == _selectedProduct.SellerId)
            {
                BottomActionBar.IsVisible = false;
            }
            else
            {
                BottomActionBar.IsVisible = true;
            }

            if (_selectedProduct != null)
            {
                _ = LoadReviews();
            }
        }

        private async Task LoadReviews()
        {
            try
            {
                var reviews = await _firebaseService.GetReviewsForProductAsync(_selectedProduct.Id);
                ReviewsCollection.ItemsSource = reviews;
                NoReviewsLabel.IsVisible = reviews == null || reviews.Count == 0;
            }
            catch (Exception)
            {
                // Silently fail or show minimal error
                NoReviewsLabel.Text = "Could not load reviews";
                NoReviewsLabel.IsVisible = true;
            }
        }

        public ProductDetailPage(FirebaseService firebaseService, AuthService authService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _authService = authService;
        }

        private async void OnAddToCartClicked(object sender, EventArgs e)
        {
            if (!AuthService.IsLoggedIn)
            {
                await DisplayAlert("Login Required", "Please login to add items to your cart.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            if (SelectedProduct == null) return;

            if (AuthService.UserId == SelectedProduct.SellerId)
            {
                await DisplayAlert("Action Denied", "You cannot add your own product to the cart.", "OK");
                return;
            }

            var latestProduct = await _firebaseService.GetProductByIdAsync(SelectedProduct.Id);
            if (latestProduct == null)
            {
                await DisplayAlert("Error", "This product is no longer available.", "OK");
                return;
            }

            if (latestProduct.Stock <= 0)
            {
                await DisplayAlert("Error", "This product is out of stock.", "OK");
                return;
            }

            var cartItems = await _firebaseService.GetCartItemsAsync(AuthService.UserId!);
            var existingCartItem = cartItems?.FirstOrDefault(item => item.ProductId == SelectedProduct.Id);
            int currentCartQty = existingCartItem?.Quantity ?? 0;

            if (currentCartQty + 1 > latestProduct.Stock)
            {
                await DisplayAlert("Error", $"Cannot add more items. Only {latestProduct.Stock} available in stock.", "OK");
                return;
            }

            var cartItem = new Models.CartItem
            {
                ProductId = latestProduct.Id,
                Title = latestProduct.Title,
                Price = latestProduct.Price,
                ImageUrl = latestProduct.ImageUrl,
                Quantity = currentCartQty + 1,
                SellerId = latestProduct.SellerId,
                SellerName = latestProduct.SellerName
            };

            await _firebaseService.AddToCartAsync(AuthService.UserId!, cartItem);
            await DisplayAlert("Success", "Product added to cart!", "OK");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
