using Microsoft.Maui.Controls;
using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages
{
    public partial class OrderFulfillmentPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;

        public OrderFulfillmentPage(FirebaseService firebaseService)
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
                var orders = await _firebaseService.GetSellerOrdersAsync(AuthService.UserId!);
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

        private async void OnFulfillClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Order order)
            {
                try
                {
                    string nextStatus = order.Status == "Pending" ? "Shipped" : "Delivered";
                    await _firebaseService.UpdateOrderStatusAsync(order.Id, nextStatus);
                    
                    string message = nextStatus == "Shipped" ? "Order marked as shipped!" : "Order marked as delivered!";
                    await DisplayAlert("Success", message, "OK");
                    await LoadOrders();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to update order: " + ex.Message, "OK");
                }
            }
        }
    }
}
