# Design Spec: User Profile Management & Selling Constraints

## Problem Statement
Users need a way to manage their personal profile details (Name, Number, Address, Gmail). To ensure accountability, sellers must have a completed profile name before they are allowed to list or manage products.

## Proposed Solution
Create a profile editing interface and enforce a profile check on all seller-related actions.

### 1. Profile Page (`ProfilePage.xaml`)
*   **Account Settings**: Add a new menu item labeled **"Profile"**.
*   **Header Display**: Update the orange header to display the user's `FullName` from their `UserProfile`. Default to "User" if null.
*   **Seller Tools Logic**: 
    *   Intercept clicks on **"Add New Product"** and **"Manage Products"**.
    *   Check if `UserProfile.FullName` is null or empty.
    *   If empty: Show Alert *"Please complete your profile name first"* and navigate to `EditProfilePage`.

### 2. Edit Profile Page (`EditProfilePage.xaml`)
*   **Purpose**: Allow users to update their personal information.
*   **Fields**:
    *   **Name** (Entry)
    *   **Number** (Entry, Numeric)
    *   **Address** (Editor/Entry)
    *   **Gmail** (Entry, Read-only or editable)
*   **Action**: "SAVE CHANGES" button that calls `FirebaseService.SaveUserProfileAsync()`.

### 3. Data & Services
*   **Model**: Use `Models/UserProfile.cs`.
*   **Persistence**: `FirebaseService.GetUserProfileAsync()` and `SaveUserProfileAsync()`.
*   **State**: The `ProfilePage` should refresh data `OnAppearing`.

## Success Criteria
*   Users can navigate to "Profile" from the settings list.
*   Profile details (Name, Number, Address, Email) are saved to Firebase.
*   The Profile header shows the saved Name.
*   If a user tries to "Add New Product" without a name, they are redirected to the Profile edit page.
*   If a user tries to "Manage Products" without a name, they are redirected to the Profile edit page.

## Testing Plan
1.  Log in with a fresh account (no name).
2.  Tap "Add New Product". Verify alert appears and redirects to Edit Profile.
3.  Fill in the name "John Doe" and save.
4.  Verify the Profile header now says "John Doe".
5.  Tap "Add New Product" again. Verify it now goes to the product form.
