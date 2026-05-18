# User Profile Management Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Allow users to manage their profile (Name, Number, Address, Email) and enforce a profile name check before selling products.

**Architecture:** 
1. Create `EditProfilePage` for data entry.
2. Update `ProfilePage` to display user name and provide navigation.
3. Add constraint logic in `ProfilePage` to redirect name-less users to the edit page.
4. Update navigation and DI registrations.

**Tech Stack:** .NET MAUI, C#, FirebaseService

---

### Task 1: Registration and Routing

**Files:**
- Modify: `AppShell.xaml.cs`
- Modify: `MauiProgram.cs`

- [ ] **Step 1: Register EditProfilePage route in AppShell.xaml.cs**
Add `Routing.RegisterRoute("EditProfilePage", typeof(EditProfilePage));` to the constructor.

- [ ] **Step 2: Register EditProfilePage in MauiProgram.cs**
Add `builder.Services.AddTransient<EditProfilePage>();` in the `CreateMauiApp` method.

- [ ] **Step 3: Commit**
```bash
git add AppShell.xaml.cs MauiProgram.cs
git commit -m "chore: register EditProfilePage route and service"
```

---

### Task 2: Profile Page UI & Header Updates

**Files:**
- Modify: `Pages/ProfilePage.xaml`
- Modify: `Pages/ProfilePage.xaml.cs`

- [ ] **Step 1: Update Header and add Profile option in Pages/ProfilePage.xaml**
Update the header Label to have `x:Name="UserNameLabel"` and add the "Profile" option under "Account Settings".
```xml
<!-- In Header -->
<Label x:Name="UserNameLabel" Text="User" TextColor="White" FontSize="20" FontAttributes="Bold" VerticalOptions="Center"/>

<!-- Under Account Settings Header -->
<Grid Padding="15" ColumnDefinitions="Auto, *, Auto">
    <Label Grid.Column="0" Text="👤" VerticalOptions="Center"/>
    <Label Grid.Column="1" Text="Profile" TextColor="Black" VerticalOptions="Center" Margin="15,0,0,0"/>
    <Label Grid.Column="2" Text="›" TextColor="Gray" FontSize="20" VerticalOptions="Center"/>
    <Grid.GestureRecognizers>
        <TapGestureRecognizer Tapped="OnProfileClicked"/>
    </Grid.GestureRecognizers>
</Grid>
<BoxView HeightRequest="1" Color="#F0F0F0" Margin="15,0"/>
```

- [ ] **Step 2: Update Pages/ProfilePage.xaml.cs to load name and handle clicks**
Implement `OnAppearing` to fetch the user profile and update the header. Add `OnProfileClicked`.
```csharp
private UserProfile _currentUserProfile;

protected override async void OnAppearing()
{
    base.OnAppearing();
    await LoadUserProfile();
}

private async Task LoadUserProfile()
{
    _currentUserProfile = await _firebaseService.GetUserProfileAsync(AuthService.UserId);
    UserNameLabel.Text = string.IsNullOrEmpty(_currentUserProfile?.FullName) ? "User" : _currentUserProfile.FullName;
}

private async void OnProfileClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync("EditProfilePage");
}
```

- [ ] **Step 3: Commit**
```bash
git add Pages/ProfilePage.xaml Pages/ProfilePage.xaml.cs
git commit -m "feat: update Profile page UI with header name and Profile option"
```

---

### Task 3: Implement Selling Constraints

**Files:**
- Modify: `Pages/ProfilePage.xaml.cs`

- [ ] **Step 1: Update OnAddNewProductClicked and OnManageProductsClicked with name check**
```csharp
private async void OnAddNewProductClicked(object sender, EventArgs e)
{
    if (string.IsNullOrEmpty(_currentUserProfile?.FullName))
    {
        await DisplayAlert("Profile Incomplete", "Please complete your profile name first.", "OK");
        await Shell.Current.GoToAsync("EditProfilePage");
        return;
    }
    await Shell.Current.GoToAsync("AddEditProductPage");
}

private async void OnManageProductsClicked(object sender, EventArgs e)
{
    if (string.IsNullOrEmpty(_currentUserProfile?.FullName))
    {
        await DisplayAlert("Profile Incomplete", "Please complete your profile name first.", "OK");
        await Shell.Current.GoToAsync("EditProfilePage");
        return;
    }
    await Shell.Current.GoToAsync("ManageProductsPage");
}
```

- [ ] **Step 2: Commit**
```bash
git add Pages/ProfilePage.xaml.cs
git commit -m "feat: enforce profile name check before selling"
```

---

### Task 4: Implement Edit Profile Page

**Files:**
- Create: `Pages/EditProfilePage.xaml`
- Create: `Pages/EditProfilePage.xaml.cs`

- [ ] **Step 1: Create Pages/EditProfilePage.xaml**
Implement the form with Name, Number, Address, and Email.
```xml
<VerticalStackLayout Padding="20" Spacing="15">
    <Entry x:Name="NameEntry" Placeholder="Full Name"/>
    <Entry x:Name="PhoneEntry" Placeholder="Phone Number" Keyboard="Numeric"/>
    <Editor x:Name="AddressEditor" Placeholder="Shipping Address" HeightRequest="100"/>
    <Entry x:Name="EmailEntry" Placeholder="Email" IsReadOnly="True" BackgroundColor="#F0F0F0"/>
    <Button Text="SAVE CHANGES" Clicked="OnSaveClicked" ... />
</VerticalStackLayout>
```

- [ ] **Step 2: Implement Pages/EditProfilePage.xaml.cs**
```csharp
public partial class EditProfilePage : ContentPage
{
    private readonly FirebaseService _firebaseService;
    private UserProfile _profile;

    public EditProfilePage(FirebaseService firebaseService) {
        InitializeComponent();
        _firebaseService = firebaseService;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();
        _profile = await _firebaseService.GetUserProfileAsync(AuthService.UserId);
        NameEntry.Text = _profile.FullName;
        PhoneEntry.Text = _profile.PhoneNumber;
        AddressEditor.Text = _profile.ShippingAddress;
        EmailEntry.Text = string.IsNullOrEmpty(_profile.Email) ? AuthService.UserEmail : _profile.Email;
    }

    private async void OnSaveClicked(object sender, EventArgs e) {
        if (string.IsNullOrEmpty(NameEntry.Text)) {
            await DisplayAlert("Error", "Name is required.", "OK");
            return;
        }
        _profile.FullName = NameEntry.Text;
        _profile.PhoneNumber = PhoneEntry.Text;
        _profile.ShippingAddress = AddressEditor.Text;
        _profile.Email = EmailEntry.Text;
        await _firebaseService.SaveUserProfileAsync(_profile);
        await DisplayAlert("Success", "Profile updated!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}
```

- [ ] **Step 3: Commit**
```bash
git add Pages/EditProfilePage.xaml Pages/EditProfilePage.xaml.cs
git commit -m "feat: implement EditProfilePage"
```
