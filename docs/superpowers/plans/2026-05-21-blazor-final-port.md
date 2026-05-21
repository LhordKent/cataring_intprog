# Temu_Catarig Blazor Full Port Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Complete the 1:1 logic port from MAUI to Blazor and implement the "Account Hub" UI design.

**Architecture:** Refactor existing services to include missing MAUI logic (Cloudinary, Stock deduction). Implement a centered "Account Hub" layout for the profile and seller tools.

**Tech Stack:** .NET 9, Blazor WASM, Tailwind CSS, Firebase, Blazored.LocalStorage, CloudinaryDotNet.

---

### Task 1: Unify Services (1:1 Logic Port)

**Files:**
- Modify: `Temu_Catarig.Blazor/Services/FirebaseService.cs`
- Modify: `Temu_Catarig.Blazor/Services/ImageService.cs`
- Modify: `Temu_Catarig.Blazor/Services/AuthService.cs`

- [ ] **Step 1: Port Stock Deduction to FirebaseService**
Ensure `PlaceOrderAsync` matches MAUI's logic of updating product stock.
```csharp
// Inside PlaceOrderAsync
await _client.Child("orders").PostAsync(order);
foreach (var item in order.Items) {
    var product = await GetProductByIdAsync(item.ProductId);
    if (product != null) {
        product.Stock -= item.Quantity;
        if (product.Stock < 0) product.Stock = 0;
        await UpdateProductAsync(product);
    }
}
```

- [ ] **Step 2: Port Cloudinary to ImageService**
Use the MAUI credentials and implement `IBrowserFile` support.
```csharp
public async Task<string> UploadImageAsync(IBrowserFile file) {
    using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5); // 5MB
    var uploadParams = new ImageUploadParams() {
        File = new FileDescription(file.Name, stream),
        Folder = "temu_products"
    };
    var result = await _cloudinary.UploadAsync(uploadParams);
    return result?.SecureUrl?.ToString() ?? string.Empty;
}
```

- [ ] **Step 3: Commit**
`git commit -m "feat: 1:1 service logic port from MAUI"`

### Task 2: Implement "Account Hub" UI

**Files:**
- Modify: `Temu_Catarig.Blazor/Pages/Profile.razor`
- Modify: `Temu_Catarig.Blazor/Layout/NavMenu.razor`

- [ ] **Step 1: Design Account Hub Layout**
Create the centered grid with tiles for Orders, Edit Profile, and Seller Tools.
- [ ] **Step 2: Update Navigation**
Align the Header navigation with the desktop design (Logo, Search center, Actions right).
- [ ] **Step 3: Commit**
`git commit -m "ui: implement centered account hub layout"`

### Task 3: Port Seller Management (Add/Edit Product)

**Files:**
- Create/Modify: `Temu_Catarig.Blazor/Pages/Admin/AddEditProduct.razor`
- Modify: `Temu_Catarig.Blazor/Pages/Admin/Dashboard.razor` (Manage Products)

- [ ] **Step 1: Build Add/Edit Form**
2-column layout. Left: Image upload with preview. Right: Product details.
- [ ] **Step 2: Implement Logic**
Use `ImageService` for Cloudinary uploads and `FirebaseService` for saving.
- [ ] **Step 3: Commit**
`git commit -m "feat: port product management with cloudinary uploads"`

### Task 4: Port Order Fulfillment & History

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Orders.razor` (My Orders)
- Create: `Temu_Catarig.Blazor/Pages/Admin/Fulfillment.razor`

- [ ] **Step 1: Implement "My Orders"**
1:1 port of the MAUI list logic.
- [ ] **Step 2: Implement "Order Fulfillment"**
1:1 port of the MAUI seller fulfillment logic.
- [ ] **Step 3: Commit**
`git commit -m "feat: port orders and fulfillment pages"`
