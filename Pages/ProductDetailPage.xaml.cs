using Microsoft.Maui.Controls;
using System;
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

            var cartItem = new Models.CartItem
            {
                ProductId = SelectedProduct.Id,
                Title = SelectedProduct.Title,
                Price = SelectedProduct.Price,
                ImageUrl = SelectedProduct.ImageUrl,
                Quantity = 1,
                SellerId = SelectedProduct.SellerId,
                SellerName = SelectedProduct.SellerName
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
