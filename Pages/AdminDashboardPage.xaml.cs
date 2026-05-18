using Temu_Catarig.Services;
using Temu_Catarig.Models;

namespace Temu_Catarig.Pages;

public partial class AdminDashboardPage : ContentPage
{
    private readonly FirebaseService _firebaseService;
    private readonly AuthService _authService;

    public AdminDashboardPage(FirebaseService firebaseService, AuthService authService)
    {
        InitializeComponent();
        _firebaseService = firebaseService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPendingProducts();
    }

    private async Task LoadPendingProducts()
    {
        try
        {
            var pending = await _firebaseService.GetProductsAsync("Pending");
            PendingCollection.ItemsSource = pending;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load: " + ex.Message, "OK");
        }
    }

    private async void OnApproveClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            await _firebaseService.UpdateProductStatusAsync(product.Id, "Approved");
            await DisplayAlert("Success", "Product approved!", "OK");
            await LoadPendingProducts();
        }
    }

    private async void OnRejectClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            await _firebaseService.UpdateProductStatusAsync(product.Id, "Rejected");
            await DisplayAlert("Rejected", "Product rejected.", "OK");
            await LoadPendingProducts();
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        _authService.Logout();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
