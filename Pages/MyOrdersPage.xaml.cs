using Microsoft.Maui.Controls;
using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages
{
    public partial class MyOrdersPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;

        public MyOrdersPage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                var orders = await _firebaseService.GetUserOrdersAsync(AuthService.UserId!);
                OrdersCollection.ItemsSource = orders;
                NoOrdersLabel.IsVisible = orders == null || orders.Count == 0;
                OrdersCollection.IsVisible = orders != null && orders.Count > 0;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load orders: " + ex.Message, "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnReviewClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Order order)
            {
                if (order.Items.Count == 0) return;

                string ratingStr = await DisplayActionSheet("Rate this product", "Cancel", null, "5 Stars", "4 Stars", "3 Stars", "2 Stars", "1 Star");
                if (ratingStr == "Cancel" || string.IsNullOrEmpty(ratingStr)) return;

                double rating = double.Parse(ratingStr.Split(' ')[0]);

                string comment = await DisplayPromptAsync("Product Review", "What do you think about the product?", "Submit", "Cancel");
                if (comment == null) return;

                var review = new Review
                {
                    ProductId = order.Items[0].ProductId,
                    BuyerId = AuthService.UserId!,
                    BuyerName = AuthService.UserDisplayName ?? "Customer",
                    Rating = rating,
                    Comment = comment,
                    Timestamp = DateTime.Now
                };

                try
                {
                    await _firebaseService.SubmitReviewAsync(review);
                    await _firebaseService.UpdateOrderReviewStatusAsync(order.Id, true);
                    await DisplayAlert("Success", "Thank you for your review!", "OK");
                    await LoadOrders();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to submit review: " + ex.Message, "OK");
                }
            }
        }
    }
}
