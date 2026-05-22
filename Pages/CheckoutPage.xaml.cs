using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages;

public partial class CheckoutPage : ContentPage
{
    private readonly FirebaseService _firebaseService;
    private readonly AuthService _authService;
    private List<CartItem> _items;
    private double _totalAmount;

    public CheckoutPage(FirebaseService firebaseService, AuthService authService)
    {
        InitializeComponent();
        _firebaseService = firebaseService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCartDetails();
    }

    private async Task LoadCartDetails()
    {
        _items = await _firebaseService.GetCartItemsAsync(AuthService.UserId!);
        
        // Populate user details from profile
        var profile = await _firebaseService.GetUserProfileAsync(AuthService.UserId!);
        if (profile != null)
        {
            NameEntry.Text = profile.FullName;
            PhoneEntry.Text = profile.PhoneNumber;
            AddressEntry.Text = profile.ShippingAddress;
        }

        // Calculate unique sellers and total shipping
        var uniqueSellersCount = _items.Select(i => i.SellerId).Distinct().Count();
        double shippingFee = uniqueSellersCount * 80;
        double subtotal = _items.Sum(i => i.Price * i.Quantity);
        
        _totalAmount = subtotal + shippingFee;

        SubtotalLabel.Text = $"₱{subtotal:N2}";
        ShippingLabel.Text = $"₱{shippingFee:N2}";
        TotalLabel.Text = $"₱{_totalAmount:N2}";
    }

    private async void OnPlaceOrderClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || 
            string.IsNullOrWhiteSpace(PhoneEntry.Text) ||
            string.IsNullOrWhiteSpace(AddressEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all details.", "OK");
            return;
        }

        if (_items != null)
        {
            foreach (var item in _items)
            {
                var latestProduct = await _firebaseService.GetProductByIdAsync(item.ProductId);
                if (latestProduct == null)
                {
                    await DisplayAlert("Stock Issue", $"Product '{item.Title}' is no longer available.", "OK");
                    return;
                }
                if (latestProduct.Stock <= 0)
                {
                    await DisplayAlert("Stock Issue", $"Product '{item.Title}' is out of stock.", "OK");
                    return;
                }
                if (item.Quantity > latestProduct.Stock)
                {
                    await DisplayAlert("Stock Issue", $"Product '{item.Title}' does not have enough stock. Only {latestProduct.Stock} available.", "OK");
                    return;
                }
            }
        }

        string paymentMethod = "COD";
        if (GcashRadio.IsChecked) paymentMethod = "GCash";
        if (CardRadio.IsChecked) paymentMethod = "Card";

        try
        {
            // Group by ProductId to ensure each product gets its own order
            var itemsByProduct = _items.GroupBy(i => i.ProductId);
            var processedSellers = new HashSet<string>();

            foreach (var group in itemsByProduct)
            {
                var firstItem = group.First();
                double itemShipping = 0;

                // Only the first product processed for a seller carries the ₱80 fee
                if (!processedSellers.Contains(firstItem.SellerId))
                {
                    itemShipping = 80;
                    processedSellers.Add(firstItem.SellerId);
                }

                var order = new Order
                {
                    BuyerId = AuthService.UserId!,
                    ReceiverName = NameEntry.Text,
                    ReceiverPhone = PhoneEntry.Text,
                    ShippingAddress = AddressEntry.Text,
                    SellerId = firstItem.SellerId,
                    SellerName = firstItem.SellerName,
                    Items = group.ToList(), // This list will contain the same product (multiple quantities if any)
                    Subtotal = group.Sum(i => i.Price * i.Quantity),
                    ShippingFee = itemShipping,
                    TotalPrice = group.Sum(i => i.Price * i.Quantity) + itemShipping,
                    Status = "Pending",
                    PaymentMethod = paymentMethod,
                    OrderDate = DateTime.Now
                };

                await _firebaseService.PlaceOrderAsync(order);
            }

            await _firebaseService.ClearCartAsync(AuthService.UserId!);
            await DisplayAlert("Success", "Order placed successfully!", "OK");
            await Shell.Current.GoToAsync("//LandingPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to place order: " + ex.Message, "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
