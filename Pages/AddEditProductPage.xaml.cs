using Microsoft.Maui.Controls;
using System;
using Temu_Catarig.Services;
using Temu_Catarig.Models;
using System.Collections.Generic;

namespace Temu_Catarig.Pages
{
    [QueryProperty(nameof(Product), "Product")]
    public partial class AddEditProductPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private readonly ImageService _imageService;
        private readonly AuthService _authService;
        private string? _imageUrl;
        private Product? _product;

        public Product? Product 
        { 
            get => _product; 
            set 
            { 
                _product = value; 
                LoadProductData(); 
            } 
        }

        public AddEditProductPage(FirebaseService firebaseService, ImageService imageService, AuthService authService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _imageService = imageService;
            _authService = authService;
        }

        private void LoadProductData()
        {
            if (_product != null)
            {
                PageTitle.Text = "Edit Product";
                SubmitButton.Text = "UPDATE PRODUCT";
                NameEntry.Text = _product.Title;
                PriceEntry.Text = _product.Price.ToString();
                StockEntry.Text = _product.Stock.ToString();
                CategoryEntry.Text = _product.Category;
                DescriptionEditor.Text = _product.Description;
                _imageUrl = _product.ImageUrl;
                ProductImage.Source = _imageUrl;
                ProductImage.IsVisible = true;
                UploadPlaceholder.IsVisible = false;
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnUploadImageTapped(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Product Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Show loading state
                    UploadPlaceholder.IsVisible = false;
                    ProductImage.IsVisible = true;
                    ProductImage.Source = ImageSource.FromStream(() => result.OpenReadAsync().Result);

                    // Upload to Cloudinary
                    using var stream = await result.OpenReadAsync();
                    _imageUrl = await _imageService.UploadImageAsync(stream, result.FileName);

                    if (string.IsNullOrEmpty(_imageUrl))
                    {
                        await DisplayAlert("Error", "Failed to upload image to Cloudinary.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Image selection failed: " + ex.Message, "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                await DisplayAlert("Error", "Please enter a product name.", "OK");
                return;
            }

            if (!double.TryParse(PriceEntry.Text, out double price) || price <= 0)
            {
                await DisplayAlert("Error", "Price must be a positive number.", "OK");
                return;
            }

            if (!int.TryParse(StockEntry.Text ?? "0", out int stock) || stock < 0)
            {
                await DisplayAlert("Error", "Stock must be a non-negative integer.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_imageUrl))
            {
                await DisplayAlert("Error", "Please upload a product image.", "OK");
                return;
            }

            try
            {
                var productToSave = _product ?? new Product();
                productToSave.Title = NameEntry.Text;
                productToSave.Price = price;
                productToSave.Stock = stock;
                productToSave.Category = CategoryEntry.Text ?? "General";
                productToSave.Description = DescriptionEditor.Text ?? "";
                productToSave.ImageUrl = _imageUrl;
                productToSave.SellerId = AuthService.UserId ?? "Unknown";
                productToSave.SellerName = AuthService.UserDisplayName ?? "Seller";

                if (_product == null)
                {
                    productToSave.Status = "Pending";
                    await _firebaseService.AddProductAsync(productToSave);
                    await DisplayAlert("Success", "Product submitted! Please wait for admin's approval.", "OK");
                }
                else
                {
                    await _firebaseService.UpdateProductAsync(productToSave);
                    await DisplayAlert("Success", "Product updated successfully!", "OK");
                }
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Save failed: " + ex.Message, "OK");
            }
        }
    }
}
