# Seller Product Management Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Enable sellers to manage their products (add, edit, delete) with improved UI labeling and an admin approval workflow.

**Architecture:** 
1. Update `ProfilePage` to include "Manage Products".
2. Create `ManageProductsPage` for listing/deleting/editing.
3. Refactor `AddEditProductPage` to support "Add" (Pending status + Approval prompt) and "Edit" modes.
4. Update navigation and DI registrations.

**Tech Stack:** .NET MAUI, C#, FirebaseService

---

### Task 1: Registration and Routing

**Files:**
- Modify: `AppShell.xaml.cs`
- Modify: `MauiProgram.cs`

- [ ] **Step 1: Register ManageProductsPage route in AppShell.xaml.cs**
Add `Routing.RegisterRoute("ManageProductsPage", typeof(ManageProductsPage));` to the constructor.

- [ ] **Step 2: Register ManageProductsPage in MauiProgram.cs**
Add `builder.Services.AddTransient<ManageProductsPage>();` in the `CreateMauiApp` method.

- [ ] **Step 3: Commit**
```bash
git add AppShell.xaml.cs MauiProgram.cs
git commit -m "chore: register ManageProductsPage route and service"
```

---

### Task 2: Profile Page Updates

**Files:**
- Modify: `Pages/ProfilePage.xaml`
- Modify: `Pages/ProfilePage.xaml.cs`

- [ ] **Step 1: Add "Manage Products" menu item in Pages/ProfilePage.xaml**
Add the following after the "Add New Product" Grid:
```xml
<BoxView HeightRequest="1" Color="#F0F0F0" Margin="15,0"/>
<Grid Padding="15" ColumnDefinitions="Auto, *, Auto">
    <Label Grid.Column="0" Text="📝" VerticalOptions="Center"/>
    <Label Grid.Column="1" Text="Manage Products" TextColor="Black" VerticalOptions="Center" Margin="15,0,0,0"/>
    <Label Grid.Column="2" Text="›" TextColor="Gray" FontSize="20" VerticalOptions="Center"/>
    <Grid.GestureRecognizers>
        <TapGestureRecognizer Tapped="OnManageProductsClicked"/>
    </Grid.GestureRecognizers>
</Grid>
```

- [ ] **Step 2: Implement OnManageProductsClicked in Pages/ProfilePage.xaml.cs**
```csharp
private async void OnManageProductsClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync("ManageProductsPage");
}
```

- [ ] **Step 3: Commit**
```bash
git add Pages/ProfilePage.xaml Pages/ProfilePage.xaml.cs
git commit -m "feat: add Manage Products option to Profile page"
```

---

### Task 3: Add/Edit Product Page Enhancements

**Files:**
- Modify: `Pages/AddEditProductPage.xaml`
- Modify: `Pages/AddEditProductPage.xaml.cs`

- [ ] **Step 1: Update UI for dynamic labeling in Pages/AddEditProductPage.xaml**
Add `x:Name="PageTitle"` to the header Label and `x:Name="SubmitButton"` to the save Button.
```xml
<!-- In Header -->
<Label x:Name="PageTitle" Text="Add Product" TextColor="Black" FontSize="18" FontAttributes="Bold" VerticalOptions="Center"/>

<!-- In Form -->
<Button x:Name="SubmitButton" Text="ADD PRODUCT" Clicked="OnSaveClicked" ... />
```

- [ ] **Step 2: Add Product property and Mode logic in Pages/AddEditProductPage.xaml.cs**
Add a `Product` property and update logic to handle edit mode.
```csharp
[QueryProperty(nameof(Product), "Product")]
public partial class AddEditProductPage : ContentPage
{
    private Product _product;
    public Product Product 
    { 
        get => _product; 
        set 
        { 
            _product = value; 
            LoadProductData(); 
        } 
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

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // ... validation ...
        try {
            var productToSave = _product ?? new Product();
            productToSave.Title = NameEntry.Text;
            productToSave.Price = double.Parse(PriceEntry.Text);
            productToSave.Stock = int.Parse(StockEntry.Text ?? "0");
            productToSave.Category = CategoryEntry.Text ?? "General";
            productToSave.Description = DescriptionEditor.Text ?? "";
            productToSave.ImageUrl = _imageUrl;
            productToSave.SellerId = AuthService.UserId ?? "Unknown";
            productToSave.SellerName = AuthService.UserDisplayName ?? "Seller";

            if (_product == null) {
                productToSave.Status = "Pending";
                await _firebaseService.AddProductAsync(productToSave);
                await DisplayAlert("Success", "Product submitted! Please wait for admin's approval.", "OK");
            } else {
                await _firebaseService.UpdateProductAsync(productToSave);
                await DisplayAlert("Success", "Product updated successfully!", "OK");
            }
            await Shell.Current.GoToAsync("..");
        } catch (Exception ex) { ... }
    }
}
```

- [ ] **Step 3: Commit**
```bash
git add Pages/AddEditProductPage.xaml Pages/AddEditProductPage.xaml.cs
git commit -m "feat: update AddEditProductPage with dynamic labeling and approval prompt"
```

---

### Task 4: Implement Manage Products Page

**Files:**
- Create: `Pages/ManageProductsPage.xaml`
- Create: `Pages/ManageProductsPage.xaml.cs`

- [ ] **Step 1: Create Pages/ManageProductsPage.xaml**
Implement a list of products with Edit/Delete buttons.
```xml
<CollectionView x:Name="ProductsList">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Frame Margin="10,5" Padding="10">
                <Grid ColumnDefinitions="Auto, *, Auto">
                    <Image Grid.Column="0" Source="{Binding ImageUrl}" WidthRequest="60" HeightRequest="60" Aspect="AspectFill"/>
                    <VerticalStackLayout Grid.Column="1" Margin="10,0">
                        <Label Text="{Binding Title}" FontAttributes="Bold"/>
                        <Label Text="{Binding PriceFormatted}"/>
                        <Label Text="{Binding Status}" FontSize="12" TextColor="Gray"/>
                    </VerticalStackLayout>
                    <HorizontalStackLayout Grid.Column="2" Spacing="10">
                        <Button Text="Edit" Clicked="OnEditClicked" CommandParameter="{Binding .}"/>
                        <Button Text="Delete" TextColor="Red" Clicked="OnDeleteClicked" CommandParameter="{Binding .}"/>
                    </HorizontalStackLayout>
                </Grid>
            </Frame>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

- [ ] **Step 2: Implement Pages/ManageProductsPage.xaml.cs**
Load seller products and handle Edit/Delete.
```csharp
public partial class ManageProductsPage : ContentPage
{
    private readonly FirebaseService _firebaseService;
    public ManageProductsPage(FirebaseService firebaseService) {
        InitializeComponent();
        _firebaseService = firebaseService;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();
        var all = await _firebaseService.GetProductsAsync();
        ProductsList.ItemsSource = all.Where(p => p.SellerId == AuthService.UserId).ToList();
    }

    private async void OnEditClicked(object sender, EventArgs e) {
        var product = (sender as Button).CommandParameter as Product;
        var navigationParameter = new Dictionary<string, object> { { "Product", product } };
        await Shell.Current.GoToAsync("AddEditProductPage", navigationParameter);
    }

    private async void OnDeleteClicked(object sender, EventArgs e) {
        var product = (sender as Button).CommandParameter as Product;
        bool confirm = await DisplayAlert("Confirm", "Delete this product?", "Yes", "No");
        if (confirm) {
            await _firebaseService.DeleteProductAsync(product.Id);
            OnAppearing();
        }
    }
}
```

- [ ] **Step 3: Commit**
```bash
git add Pages/ManageProductsPage.xaml Pages/ManageProductsPage.xaml.cs
git commit -m "feat: implement ManageProductsPage for sellers"
```
