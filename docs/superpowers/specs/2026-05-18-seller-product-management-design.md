# Design Spec: Seller Product Management & UI Updates

## Problem Statement
Sellers need a way to manage their existing products (edit/delete) and need clearer feedback when adding a new product (labeling and approval prompt).

## Proposed Solution
Enhance the Seller Tools in the Profile Page and create a dedicated management interface for seller-owned products.

### 1. Profile Page (`ProfilePage.xaml`)
*   Add a new menu item in the "Seller Tools" section labeled **"Manage Products"**.
*   Icon: 📝 or ⚙️.
*   Action: Navigates to `ManageProductsPage`.

### 2. Manage Products Page (`ManageProductsPage.xaml`)
*   **Purpose**: Display a list of products owned by the current seller.
*   **Data Source**: `FirebaseService.GetProductsAsync()` filtered by `SellerId`.
*   **UI Features**:
    *   `CollectionView` listing products.
    *   Each item shows: Image, Title, Price, and Status (Pending/Approved).
    *   **Edit Action**: Navigates to `AddEditProductPage` with the product object.
    *   **Delete Action**: Confirmation dialog followed by `FirebaseService.DeleteProductAsync()`.

### 3. Add/Edit Product Page (`AddEditProductPage.xaml`)
*   **UI Updates**:
    *   Header title changes based on mode: "Add Product" or "Edit Product".
    *   Main button text changes: **"ADD PRODUCT"** for new, **"UPDATE PRODUCT"** for existing.
*   **Logic Updates**:
    *   **Add Mode**: Sets `Status = "Pending"`. Shows prompt: *"Product submitted! Please wait for admin's approval."* upon success.
    *   **Edit Mode**: Updates existing fields.

### 4. Service Updates (`FirebaseService.cs`)
*   Ensure `AddProductAsync` sets `Status = "Pending"`.
*   Ensure `UpdateProductAsync` is available for editing.
*   Ensure `DeleteProductAsync` works correctly.

## Success Criteria
*   Profile page has "Manage Products" button.
*   "Add New Product" shows "ADD PRODUCT" button.
*   Adding a product triggers the admin approval message.
*   Sellers can view their list of products.
*   Sellers can delete their own products.
*   Sellers can edit their own products.

## Testing Plan
1.  Navigate to Profile -> Add New Product. Verify button text is "ADD PRODUCT".
2.  Add a product and verify the "wait for admin approval" alert appears.
3.  Navigate to Profile -> Manage Products. Verify the new product appears in the list.
4.  Edit the product and verify changes are saved.
5.  Delete the product and verify it is removed from the list.
